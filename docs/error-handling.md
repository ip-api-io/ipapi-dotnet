# Errors, rate limits & usage

The client throws a typed exception for every HTTP failure and **never retries** — you
stay in control of back-off. It also exposes your current quota so you can throttle
before you hit a limit.

## Exception taxonomy

Every exception extends `IpApiException`, which carries `StatusCode` and the raw
response `Body`. Catch the specific subclass you care about:

| Exception | HTTP status | Meaning |
|---|---|---|
| `AuthenticationException` | 401, 403 | Missing or invalid API key |
| `RateLimitException` | 429 | Quota exhausted (see below) |
| `InvalidRequestException` | 400, 404, 422 | Malformed input or unknown resource |
| `ServerException` | 5xx | ip-api.io server-side failure |
| `IpApiException` | other | Base / fallback |

```csharp
using IpApiIo;

using var client = new IpApiClient(apiKey: "YOUR_API_KEY");

try
{
    var info = await client.LookupAsync("8.8.8.8");
    Console.WriteLine(info.Location.Country);
}
catch (RateLimitException e)
{
    Console.WriteLine($"quota hit — resets at {e.Reset}");
}
catch (AuthenticationException)
{
    Console.WriteLine("check your API key");
}
catch (InvalidRequestException e)
{
    Console.WriteLine($"bad request: {e.Message}");
}
catch (ServerException)
{
    Console.WriteLine("ip-api.io is having trouble, try later");
}
catch (IpApiException e)
{
    Console.WriteLine($"error {e.StatusCode}: {e.Message}");
}
```

Transport failures (DNS, connection, timeout) surface as `HttpRequestException` /
`TaskCanceledException`, not an `IpApiException`.

## Rate limits

On HTTP 429 the client throws `RateLimitException`, parsed from the `x-ratelimit-*`
response headers. Because the client never retries, **`Reset` tells you when to**:

```csharp
try
{
    await client.LookupAsync("8.8.8.8");
}
catch (RateLimitException e)
{
    Console.WriteLine(e.Limit);     // your quota for the window
    Console.WriteLine(e.Remaining); // requests left (0 here)
    Console.WriteLine(e.Reset);     // unix timestamp when quota renews
    // schedule a retry at e.Reset instead of hammering the API
}
```

## `RateLimitAsync()` — check quota proactively

Read your current limits without triggering a 429, so you can throttle in advance.

```csharp
var rl = await client.RateLimitAsync();

Console.WriteLine(rl.PlanName);
Console.WriteLine($"{rl.IpApi.Remaining} / {rl.IpApi.Limit}");
Console.WriteLine($"{rl.EmailApi.UsagePercent} % used");
Console.WriteLine(rl.NextRenewalDate);
```

`RateLimitInfo`: `PlanId`, `PlanName`, `IpApi` and `EmailApi`
(`ApiLimitInfo`: `Limit`, `Remaining`, `Used`, `UsagePercent`), `IntervalSeconds`,
`NextRenewalDate`, `Status`.

## `UsageSummaryAsync()` — account usage

Aggregate usage for the current period — handy for dashboards and internal alerts.

```csharp
var usage = await client.UsageSummaryAsync();

Console.WriteLine($"{usage.TotalRequests} {usage.SuccessfulRequests}");
Console.WriteLine($"{usage.RateLimitedRequests} {usage.QuotaConsumed}");
Console.WriteLine($"{usage.PeriodStart} -> {usage.PeriodEnd}");
```

`UsageSummary`: `ApiKey`, `ApiType`, `PeriodStart`, `PeriodEnd`, `TotalRequests`,
`SuccessfulRequests`, `RateLimitedRequests`, `QuotaConsumed`, `BatchOperations`,
`AvgRequestDurationMs`.

## See also

- [IP geolocation & bulk lookup](ip-geolocation.md) — the most common call
- API reference: https://ip-api.io/api-docs.html
- Get a free API key: https://ip-api.io
