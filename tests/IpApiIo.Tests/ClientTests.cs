using System.Net;
using System.Text;
using Xunit;

namespace IpApiIo.Tests;

/// <summary>HttpMessageHandler returning a canned response and recording requests.</summary>
internal sealed class FakeHandler : HttpMessageHandler
{
    public List<(HttpRequestMessage Request, string? Body)> Requests { get; } = new();

    public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;

    public string ResponseBody { get; set; } = "{}";

    public Dictionary<string, string> ResponseHeaders { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? body = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);
        Requests.Add((request, body));

        var response = new HttpResponseMessage(Status)
        {
            Content = new StringContent(ResponseBody, Encoding.UTF8, "application/json"),
        };
        foreach (var (name, value) in ResponseHeaders)
        {
            response.Headers.TryAddWithoutValidation(name, value);
        }
        return response;
    }
}

public class ClientTests
{
    // IpInfoV1Dto example from https://ip-api.io/openapi.json
    private const string IpInfoFixture = """
        {
          "ip": "203.0.113.195",
          "isp": "Comcast Cable Communications",
          "asn": "AS7922",
          "suspicious_factors": {
            "is_proxy": false, "is_tor_node": false, "is_spam": false,
            "is_crawler": false, "is_datacenter": true, "is_vpn": false, "is_threat": false
          },
          "location": {
            "country": "United States", "country_code": "US", "city": "San Francisco",
            "latitude": 37.7749, "longitude": -122.4194, "zip": "94105",
            "timezone": "America/Los_Angeles", "local_time": "2023-06-21T14:30:00-07:00",
            "local_time_unix": 1687385400, "is_daylight_savings": true
          }
        }
        """;

    private static (IpApiClient Client, FakeHandler Handler) CreateClient(string? apiKey = null)
    {
        var handler = new FakeHandler();
        var client = new IpApiClient(apiKey: apiKey, httpClient: new HttpClient(handler));
        return (client, handler);
    }

    [Fact]
    public async Task Lookup_ParsesResponse_AndSendsUserAgent()
    {
        var (client, handler) = CreateClient();
        handler.ResponseBody = IpInfoFixture;

        var info = await client.LookupAsync("203.0.113.195");

        Assert.Equal("203.0.113.195", info.Ip);
        Assert.Equal("United States", info.Location.Country);
        Assert.True(info.SuspiciousFactorsInfo.IsDatacenter);
        Assert.Equal("AS7922", info.Asn);

        var request = handler.Requests.Single().Request;
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Equal("https://ip-api.io/api/v1/ip/203.0.113.195", request.RequestUri!.ToString());
        Assert.Equal($"ip-api-io-dotnet/{IpApiClient.Version}", request.Headers.UserAgent.ToString());
    }

    [Fact]
    public async Task ApiKey_SentAsQueryParam()
    {
        var (client, handler) = CreateClient(apiKey: "secret123");
        handler.ResponseBody = IpInfoFixture;

        await client.LookupAsync();

        Assert.Equal(
            "https://ip-api.io/api/v1/ip?api_key=secret123",
            handler.Requests.Single().Request.RequestUri!.ToString());
    }

    [Fact]
    public async Task EmailPath_IsUrlEncoded()
    {
        var (client, handler) = CreateClient();

        await client.ValidateEmailAsync("user+tag@example.com");

        Assert.Equal(
            "https://ip-api.io/api/v1/email/advanced/user%2Btag%40example.com",
            handler.Requests.Single().Request.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task BatchPost_SendsJsonBody()
    {
        var (client, handler) = CreateClient();
        handler.ResponseBody = """{"results": {}}""";

        await client.LookupBatchAsync(new[] { "8.8.8.8", "1.1.1.1" });

        var (request, body) = handler.Requests.Single();
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("https://ip-api.io/api/v1/ip/batch", request.RequestUri!.ToString());
        Assert.Equal("""{"ips":["8.8.8.8","1.1.1.1"]}""", body);
        Assert.Equal("application/json", request.Content!.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task BatchSize_IsValidated()
    {
        var (client, _) = CreateClient();

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.LookupBatchAsync(Array.Empty<string>()));
        await Assert.ThrowsAsync<ArgumentException>(
            () => client.LookupBatchAsync(Enumerable.Repeat("1.1.1.1", 101).ToArray()));
        await Assert.ThrowsAsync<ArgumentException>(
            () => client.ValidateEmailBatchAsync(Array.Empty<string>()));
    }

    [Fact]
    public async Task RateLimitException_ExposesHeaders()
    {
        var (client, handler) = CreateClient();
        handler.Status = HttpStatusCode.TooManyRequests;
        handler.ResponseBody = """{"message": "Rate limit exceeded"}""";
        handler.ResponseHeaders["x-ratelimit-limit"] = "1000";
        handler.ResponseHeaders["x-ratelimit-remaining"] = "0";
        handler.ResponseHeaders["x-ratelimit-reset"] = "1718200000";

        var error = await Assert.ThrowsAsync<RateLimitException>(() => client.LookupAsync("8.8.8.8"));

        Assert.Equal(429, error.StatusCode);
        Assert.Equal(1000, error.Limit);
        Assert.Equal(0, error.Remaining);
        Assert.Equal(1718200000, error.Reset);
        Assert.Contains("Rate limit exceeded", error.Message);
    }

    [Fact]
    public async Task AuthenticationException_On401()
    {
        var (client, handler) = CreateClient(apiKey: "bad");
        handler.Status = HttpStatusCode.Unauthorized;
        handler.ResponseBody = """{"error": "Invalid API key"}""";

        var error = await Assert.ThrowsAsync<AuthenticationException>(() => client.LookupAsync());
        Assert.Equal(401, error.StatusCode);
    }

    [Fact]
    public async Task InvalidRequestException_On400()
    {
        var (client, handler) = CreateClient();
        handler.Status = HttpStatusCode.BadRequest;
        handler.ResponseBody = """{"message": "Invalid IP address"}""";

        await Assert.ThrowsAsync<InvalidRequestException>(() => client.LookupAsync("not-an-ip"));
    }

    [Fact]
    public async Task ServerException_On500()
    {
        var (client, handler) = CreateClient();
        handler.Status = HttpStatusCode.InternalServerError;

        await Assert.ThrowsAsync<ServerException>(() => client.LookupAsync());
    }
}
