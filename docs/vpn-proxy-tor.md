# VPN, proxy & Tor detection

Catch traffic that hides behind anonymizers. Every `LookupAsync` already returns the
`SuspiciousFactorsInfo` flags for proxy, VPN, Tor, datacenter, spam and crawler; the
dedicated `TorCheckAsync` adds live Tor exit-node confirmation.

Powers [VPN detection](https://ip-api.io/vpn-detection-api),
[proxy detection](https://ip-api.io/proxy-detection-api) and
[Tor detection](https://ip-api.io/tor-detection).

## `SuspiciousFactorsInfo` — flags on every lookup

No extra call needed: read the flags from a normal [`LookupAsync`](ip-geolocation.md).

```csharp
using IpApiIo;

using var client = new IpApiClient(apiKey: "YOUR_API_KEY");

var info = await client.LookupAsync("185.220.101.1");
var f = info.SuspiciousFactorsInfo;

Console.WriteLine(f.IsVpn);         // VPN service
Console.WriteLine(f.IsProxy);       // open / anonymizing proxy
Console.WriteLine(f.IsTorNode);     // Tor node
Console.WriteLine(f.IsDatacenter);  // hosting / datacenter IP (often a bot)
Console.WriteLine(f.IsSpam);        // known spam source
Console.WriteLine(f.IsCrawler);     // known crawler / bot
Console.WriteLine(f.IsThreat);      // listed on a threat feed

if (f.IsVpn || f.IsProxy || f.IsTorNode)
{
    // require step-up verification
}
```

### `SuspiciousFactors`

| Property | Type | Meaning |
|---|---|---|
| `IsProxy` | `bool` | Open or anonymizing proxy |
| `IsVpn` | `bool` | Commercial VPN endpoint |
| `IsTorNode` | `bool` | Part of the Tor network |
| `IsDatacenter` | `bool` | Hosting / datacenter range |
| `IsSpam` | `bool` | Known spam source |
| `IsCrawler` | `bool` | Known crawler / bot |
| `IsThreat` | `bool` | Listed on a threat feed |

## `TorCheckAsync(ip)` — confirm a Tor exit node

A dedicated check against the live Tor node list, with a count of matching nodes.

```csharp
var tor = await client.TorCheckAsync("185.220.101.1");

Console.WriteLine(tor.IsTor);        // True
Console.WriteLine(tor.TorNodeCount); // number of matching Tor nodes
```

### Response (`TorDetection`)

| Property | Type | Description |
|---|---|---|
| `Ip` | `string` | The checked IP |
| `IsTor` | `bool` | Whether the IP is a Tor node |
| `TorNodeCount` | `int` | Matching nodes for the IP |

> Want one number instead of individual flags? See
> [Fraud detection & risk scoring](fraud-risk-scoring.md) — `RiskScoreAsync` folds all of
> these signals into a 0–100 score.

## See also

- [IP geolocation & bulk lookup](ip-geolocation.md) — where `SuspiciousFactorsInfo` comes from
- [Fraud detection & risk scoring](fraud-risk-scoring.md) — combine the flags into a score
- Product pages: [VPN detection](https://ip-api.io/vpn-detection-api) · [Proxy detection](https://ip-api.io/proxy-detection-api) · [Tor detection](https://ip-api.io/tor-detection)
- [Full tutorial on ip-api.io](https://ip-api.io/docs/sdk/dotnet/vpn-proxy-tor)
