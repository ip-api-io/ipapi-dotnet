# IP geolocation & bulk lookup

Turn any IP address into geolocation, network and threat intelligence. A single
`LookupAsync` returns the country, city, coordinates, timezone, ISP and ASN of an IP,
plus the `SuspiciousFactorsInfo` flags used for fraud screening (proxy, VPN, Tor,
datacenter, spam, crawler, threat).

Powers the [IP geolocation API](https://ip-api.io/what-is-my-ip) and the
[bulk IP lookup](https://ip-api.io/bulk-ip-lookup) product.

## `LookupAsync(ip)` / `LookupAsync()` — geolocate one IP

Pass an IPv4/IPv6 address, or call the no-arg overload to geolocate the caller's own IP.

```csharp
using IpApiIo;

using var client = new IpApiClient(apiKey: "YOUR_API_KEY");

var info = await client.LookupAsync("8.8.8.8");

Console.WriteLine(info.Ip);                            // "8.8.8.8"
Console.WriteLine(info.Isp);                           // "Google LLC"
Console.WriteLine(info.Location.Country);              // "United States"
Console.WriteLine(info.Location.City);                 // "Mountain View"
Console.WriteLine($"{info.Location.Latitude}, {info.Location.Longitude}");
Console.WriteLine(info.Location.Timezone);             // "America/Los_Angeles"
Console.WriteLine(info.SuspiciousFactorsInfo.IsDatacenter); // True

// Geolocate the machine making the request
var me = await client.LookupAsync();
Console.WriteLine(me.Ip);
```

### Response (`IpInfo`)

| Property | Type | Description |
|---|---|---|
| `Ip` | `string` | The looked-up address |
| `Isp` | `string?` | Internet service provider |
| `Asn` | `string?` | Autonomous system the IP belongs to |
| `Location` | `IpLocation` | `Country`, `CountryCode`, `City`, `Latitude`, `Longitude`, `Zip`, `Timezone`, `LocalTime`, `LocalTimeUnix`, `IsDaylightSavings` |
| `SuspiciousFactorsInfo` | `SuspiciousFactors` | `IsProxy`, `IsVpn`, `IsTorNode`, `IsDatacenter`, `IsSpam`, `IsCrawler`, `IsThreat` |

> The `SuspiciousFactorsInfo` block is the fastest way to flag risky traffic in one call.
> For a single 0–100 score, see [Fraud detection & risk scoring](fraud-risk-scoring.md);
> for the individual checks, see [VPN, proxy & Tor detection](vpn-proxy-tor.md).

## `LookupBatchAsync(ips)` — geolocate up to 100 IPs

Look up to 100 addresses in one request — ideal for enriching logs, sign-up events or
historical data without a round trip per IP. Throws `ArgumentException` if the list is
empty or longer than 100.

```csharp
var batch = await client.LookupBatchAsync(new[] { "8.8.8.8", "1.1.1.1", "9.9.9.9" });

Console.WriteLine(batch.TotalProcessed);    // 3
Console.WriteLine(batch.SuccessfulLookups); // 3
Console.WriteLine(batch.FailedLookups);     // 0

foreach (var (ip, info) in batch.Results)
{
    Console.WriteLine($"{ip} {info.Location.Country} {info.SuspiciousFactorsInfo.IsVpn}");
}
```

### Response (`BatchIpLookupResponse`)

| Property | Type | Description |
|---|---|---|
| `Results` | `Dictionary<string, IpInfo>` | Map of IP → info |
| `TotalProcessed` | `int` | IPs received |
| `SuccessfulLookups` | `int` | IPs resolved |
| `FailedLookups` | `int` | IPs that could not be resolved |

## See also

- [Fraud detection & risk scoring](fraud-risk-scoring.md) — turn the flags into a score
- [VPN, proxy & Tor detection](vpn-proxy-tor.md) — the individual threat checks
- [ASN & DNS lookups](asn-and-dns.md) — network ownership for an IP
- Product pages: [IP geolocation](https://ip-api.io/what-is-my-ip) · [Bulk IP lookup](https://ip-api.io/bulk-ip-lookup)
- [Full tutorial on ip-api.io](https://ip-api.io/docs/sdk/dotnet/ip-geolocation)
