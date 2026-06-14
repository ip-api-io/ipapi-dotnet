// Live smoke test against https://ip-api.io.
// Usage: IPAPI_API_KEY=... dotnet run --project examples/Smoke
// The API requires a key; without IPAPI_API_KEY this program skips.
using IpApiIo;

var apiKey = Environment.GetEnvironmentVariable("IPAPI_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("SKIPPED: set IPAPI_API_KEY to run the live smoke test");
    return 0;
}

using var client = new IpApiClient(apiKey: apiKey);

var info = await client.LookupAsync("8.8.8.8");
if (info.Ip != "8.8.8.8")
{
    throw new InvalidOperationException($"unexpected response for {info.Ip}");
}
Console.WriteLine($"lookup(8.8.8.8): {info.Location.Country} / {info.Asn}");

var rateLimit = await client.RateLimitAsync();
Console.WriteLine($"rate_limit: plan={rateLimit.PlanId} ip_api remaining={rateLimit.IpApi.Remaining}");

Console.WriteLine("smoke OK");
return 0;
