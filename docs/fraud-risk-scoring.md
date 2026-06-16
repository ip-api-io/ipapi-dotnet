# Fraud detection & risk scoring

Collapse every signal — geolocation, proxy/VPN/Tor flags, datacenter hosting,
disposable email, syntax — into a single 0–100 risk score you can act on at sign-up,
checkout or login. Or pull the raw [IP reputation](https://ip-api.io/ip-reputation)
record when you want to build your own rules.

Powers the [fraud detection API](https://ip-api.io/fraud-detection-api),
[risk score](https://ip-api.io/risk-score) and
[IP reputation](https://ip-api.io/ip-reputation) products.

## `RiskScoreAsync(ip)` / `RiskScoreAsync()` — score an IP

Returns a `Score` (0–100) and a human `RiskLevel`, plus the `Factors` that drove it.
The no-arg overload scores the caller's own IP.

```csharp
using IpApiIo;

using var client = new IpApiClient(apiKey: "YOUR_API_KEY");

var risk = await client.RiskScoreAsync("185.220.101.1");

Console.WriteLine(risk.Score);        // 88
Console.WriteLine(risk.RiskLevel);    // "high"
if (risk.Factors.IpFactorsInfo is not null)
{
    Console.WriteLine(risk.Factors.IpFactorsInfo.IsTorNode);
    Console.WriteLine(risk.Factors.IpFactorsInfo.IsDatacenter);
}

if (risk.Score >= 75)
{
    // block, or send to manual review / step-up auth
}
```

### Response (`RiskScore`)

| Property | Type | Description |
|---|---|---|
| `Score` | `double` | Risk score, 0 (safe) – 100 (high risk) |
| `RiskLevel` | `string` | Bucketed level, e.g. `"low"`, `"medium"`, `"high"` |
| `Ip` | `string?` | Scored IP (when applicable) |
| `Email` | `string?` | Scored email (when applicable) |
| `Factors` | `RiskScoreFactors` | `IpFactorsInfo` and/or `EmailFactorsInfo` (nullable) |

`IpFactors`: `IsProxy`, `IsVpn`, `IsTorNode`, `IsSpam`, `IsDatacenter`,
`RiskContribution`.
`EmailFactors`: `IsDisposable`, `IsValidSyntax`, `RiskContribution`.

## `EmailRiskScoreAsync(email)` — score an email

Same 0–100 scale, driven by email signals (disposable provider, invalid syntax).
Use it to grade leads or gate sign-ups by address quality.

```csharp
var risk = await client.EmailRiskScoreAsync("user@mailinator.com");

Console.WriteLine($"{risk.Score} {risk.RiskLevel}"); // 90 high
if (risk.Factors.EmailFactorsInfo is not null)
{
    Console.WriteLine(risk.Factors.EmailFactorsInfo.IsDisposable); // True
}
```

## `IpReputationAsync(ip)` — raw reputation record

Returns the underlying reputation data for an IP as a
`Dictionary<string, JsonElement>` — use it when you want the source signals rather than
a computed score.

```csharp
var reputation = await client.IpReputationAsync("185.220.101.1");
foreach (var (key, value) in reputation)
{
    Console.WriteLine($"{key}: {value}");
}
```

## See also

- [IP geolocation & bulk lookup](ip-geolocation.md) — `SuspiciousFactorsInfo` per IP
- [VPN, proxy & Tor detection](vpn-proxy-tor.md) — the individual checks behind the score
- [Email validation & verification](email-validation.md) — deliverability before scoring
- Product pages: [Fraud detection](https://ip-api.io/fraud-detection-api) · [Risk score](https://ip-api.io/risk-score) · [IP reputation](https://ip-api.io/ip-reputation)
- [Full tutorial on ip-api.io](https://ip-api.io/docs/sdk/dotnet/fraud-risk-scoring)
