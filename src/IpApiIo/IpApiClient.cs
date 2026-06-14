using System.Text;
using System.Text.Json;

namespace IpApiIo;

/// <summary>
/// Client for the ip-api.io IP intelligence and email validation API.
/// An API key is required by the live API — get a free key at https://ip-api.io.
/// </summary>
public sealed class IpApiClient : IDisposable
{
    public const string Version = "1.0.0";

    /// <summary>Documented per-request limit for batch endpoints.</summary>
    public const int MaxBatchSize = 100;

    private const string UserAgent = "ip-api-io-dotnet/" + Version;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly string? _apiKey;
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;

    /// <param name="apiKey">API key, sent as the api_key query parameter. Get a free key at https://ip-api.io.</param>
    /// <param name="baseUrl">API origin, override for testing.</param>
    /// <param name="timeout">Per-request timeout (default 10 seconds).</param>
    /// <param name="httpClient">Optional pre-configured HttpClient (the caller keeps ownership).</param>
    public IpApiClient(
        string? apiKey = null,
        string baseUrl = "https://ip-api.io",
        TimeSpan? timeout = null,
        HttpClient? httpClient = null)
    {
        _apiKey = apiKey;
        _baseUrl = baseUrl.TrimEnd('/');
        _ownsHttpClient = httpClient is null;
        _httpClient = httpClient ?? new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(10) };
    }

    // -- IP intelligence ------------------------------------------------------

    /// <summary>Geolocation + threat intelligence for the caller's IP.</summary>
    public Task<IpInfo> LookupAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IpInfo>("/api/v1/ip", cancellationToken);

    /// <summary>Geolocation + threat intelligence for a specific IP.</summary>
    public Task<IpInfo> LookupAsync(string ip, CancellationToken cancellationToken = default) =>
        GetAsync<IpInfo>($"/api/v1/ip/{Uri.EscapeDataString(ip)}", cancellationToken);

    /// <summary>Look up to 100 IP addresses in a single request.</summary>
    public Task<BatchIpLookupResponse> LookupBatchAsync(
        IReadOnlyList<string> ips, CancellationToken cancellationToken = default)
    {
        CheckBatch(ips, nameof(ips));
        return PostAsync<BatchIpLookupResponse>(
            "/api/v1/ip/batch", new { ips }, cancellationToken);
    }

    public Task<Dictionary<string, JsonElement>> IpReputationAsync(
        string ip, CancellationToken cancellationToken = default) =>
        GetAsync<Dictionary<string, JsonElement>>(
            $"/api/v1/ip-reputation/{Uri.EscapeDataString(ip)}", cancellationToken);

    public Task<TorDetection> TorCheckAsync(string ip, CancellationToken cancellationToken = default) =>
        GetAsync<TorDetection>($"/api/v1/tor/{Uri.EscapeDataString(ip)}", cancellationToken);

    public Task<AsnLookup> AsnAsync(string ip, CancellationToken cancellationToken = default) =>
        GetAsync<AsnLookup>($"/api/v1/asn/{Uri.EscapeDataString(ip)}", cancellationToken);

    // -- Email validation -------------------------------------------------------

    /// <summary>Syntax, disposability and MX analysis of an email address.</summary>
    public Task<EmailInfo> EmailInfoAsync(string email, CancellationToken cancellationToken = default) =>
        GetAsync<EmailInfo>($"/api/v1/email/{Uri.EscapeDataString(email)}", cancellationToken);

    /// <summary>Advanced validation including SMTP deliverability checks.</summary>
    public Task<AdvancedEmailValidation> ValidateEmailAsync(
        string email, CancellationToken cancellationToken = default) =>
        GetAsync<AdvancedEmailValidation>(
            $"/api/v1/email/advanced/{Uri.EscapeDataString(email)}", cancellationToken);

    /// <summary>Advanced-validate up to 100 email addresses in a single request.</summary>
    public Task<BatchEmailValidationResponse> ValidateEmailBatchAsync(
        IReadOnlyList<string> emails, CancellationToken cancellationToken = default)
    {
        CheckBatch(emails, nameof(emails));
        return PostAsync<BatchEmailValidationResponse>(
            "/api/v1/email/advanced/batch", new { emails }, cancellationToken);
    }

    // -- Risk scoring -----------------------------------------------------------

    /// <summary>Fraud risk score for the caller's IP.</summary>
    public Task<RiskScore> RiskScoreAsync(CancellationToken cancellationToken = default) =>
        GetAsync<RiskScore>("/api/v1/risk-score", cancellationToken);

    /// <summary>Fraud risk score for a specific IP.</summary>
    public Task<RiskScore> RiskScoreAsync(string ip, CancellationToken cancellationToken = default) =>
        GetAsync<RiskScore>($"/api/v1/risk-score/{Uri.EscapeDataString(ip)}", cancellationToken);

    public Task<RiskScore> EmailRiskScoreAsync(string email, CancellationToken cancellationToken = default) =>
        GetAsync<RiskScore>(
            $"/api/v1/risk-score/email/{Uri.EscapeDataString(email)}", cancellationToken);

    // -- DNS & domains ----------------------------------------------------------

    public Task<Whois> WhoisAsync(string domain, CancellationToken cancellationToken = default) =>
        GetAsync<Whois>($"/api/v1/dns/whois/{Uri.EscapeDataString(domain)}", cancellationToken);

    public Task<ReverseDns> ReverseDnsAsync(string ip, CancellationToken cancellationToken = default) =>
        GetAsync<ReverseDns>($"/api/v1/dns/reverse/{Uri.EscapeDataString(ip)}", cancellationToken);

    public Task<ForwardDns> ForwardDnsAsync(string hostname, CancellationToken cancellationToken = default) =>
        GetAsync<ForwardDns>($"/api/v1/dns/forward/{Uri.EscapeDataString(hostname)}", cancellationToken);

    public Task<MxLookup> MxRecordsAsync(string domain, CancellationToken cancellationToken = default) =>
        GetAsync<MxLookup>($"/api/v1/dns/mx/{Uri.EscapeDataString(domain)}", cancellationToken);

    public Task<DomainAge> DomainAgeAsync(string domain, CancellationToken cancellationToken = default) =>
        GetAsync<DomainAge>($"/api/v1/domain/age/{Uri.EscapeDataString(domain)}", cancellationToken);

    public Task<BatchDomainAgeResponse> DomainAgeBatchAsync(
        IReadOnlyList<string> domains, CancellationToken cancellationToken = default)
    {
        if (domains.Count == 0)
        {
            throw new ArgumentException("domains must not be empty", nameof(domains));
        }
        return PostAsync<BatchDomainAgeResponse>(
            "/api/v1/domain/age/batch", new { domains }, cancellationToken);
    }

    // -- Account ----------------------------------------------------------------

    public Task<RateLimitInfo> RateLimitAsync(CancellationToken cancellationToken = default) =>
        GetAsync<RateLimitInfo>("/api/v1/ratelimit", cancellationToken);

    public Task<UsageSummary> UsageSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<UsageSummary>("/api/v1/usage/summary", cancellationToken);

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    // -- Internals ------------------------------------------------------------

    private static void CheckBatch(IReadOnlyList<string> items, string name)
    {
        if (items.Count == 0)
        {
            throw new ArgumentException($"{name} must not be empty", name);
        }
        if (items.Count > MaxBatchSize)
        {
            throw new ArgumentException($"{name} must contain at most {MaxBatchSize} items", name);
        }
    }

    private Task<T> GetAsync<T>(string path, CancellationToken cancellationToken) =>
        RequestAsync<T>(HttpMethod.Get, path, null, cancellationToken);

    private Task<T> PostAsync<T>(string path, object body, CancellationToken cancellationToken) =>
        RequestAsync<T>(HttpMethod.Post, path, body, cancellationToken);

    private async Task<T> RequestAsync<T>(
        HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        var url = _baseUrl + path;
        if (_apiKey is not null)
        {
            url += "?api_key=" + Uri.EscapeDataString(_apiKey);
        }

        using var request = new HttpRequestMessage(method, url);
        request.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        if (body is not null)
        {
            request.Content = new StringContent(
                JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw ErrorFrom(response, payload);
        }

        return JsonSerializer.Deserialize<T>(payload, JsonOptions)
            ?? throw new IpApiException("empty response from ip-api.io", (int)response.StatusCode, payload);
    }

    private static IpApiException ErrorFrom(HttpResponseMessage response, string body)
    {
        var status = (int)response.StatusCode;
        var message = "";
        try
        {
            using var parsed = JsonDocument.Parse(body);
            if (parsed.RootElement.ValueKind == JsonValueKind.Object)
            {
                if (parsed.RootElement.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String)
                {
                    message = m.GetString() ?? "";
                }
                else if (parsed.RootElement.TryGetProperty("error", out var e) && e.ValueKind == JsonValueKind.String)
                {
                    message = e.GetString() ?? "";
                }
            }
        }
        catch (JsonException)
        {
            message = body.Trim();
            if (message.Length > 200)
            {
                message = message[..200];
            }
        }
        if (message.Length == 0)
        {
            message = $"HTTP {status} from ip-api.io";
        }

        return status switch
        {
            401 or 403 => new AuthenticationException(message, status, body),
            429 => new RateLimitException(
                message,
                body,
                HeaderLong(response, "x-ratelimit-limit"),
                HeaderLong(response, "x-ratelimit-remaining"),
                HeaderLong(response, "x-ratelimit-reset")),
            400 or 404 or 422 => new InvalidRequestException(message, status, body),
            >= 500 => new ServerException(message, status, body),
            _ => new IpApiException(message, status, body),
        };
    }

    private static long? HeaderLong(HttpResponseMessage response, string name)
    {
        if (response.Headers.TryGetValues(name, out var values)
            && long.TryParse(values.FirstOrDefault(), out var parsed))
        {
            return parsed;
        }
        return null;
    }
}
