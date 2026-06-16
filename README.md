# IpApiIo — Official .NET client for [ip-api.io](https://ip-api.io)

[![NuGet](https://img.shields.io/nuget/v/IpApiIo)](https://www.nuget.org/packages/IpApiIo)
[![test](https://github.com/ip-api-io/ipapi-dotnet/actions/workflows/test.yml/badge.svg)](https://github.com/ip-api-io/ipapi-dotnet/actions/workflows/test.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

The official .NET client for the [ip-api.io](https://ip-api.io) IP intelligence
platform. One client covers [IP geolocation](https://ip-api.io/what-is-my-ip),
[email validation](https://ip-api.io/email-validation) and [verification](https://ip-api.io/email-verification-api)
(syntax, MX, SMTP deliverability), [fraud detection](https://ip-api.io/fraud-detection-api)
and [risk scoring](https://ip-api.io/risk-score),
[VPN](https://ip-api.io/vpn-detection-api)/[proxy](https://ip-api.io/proxy-detection-api)/[Tor detection](https://ip-api.io/tor-detection),
[disposable email detection](https://ip-api.io/disposable-email-checker), [ASN lookup](https://ip-api.io/asn-lookup),
[WHOIS](https://ip-api.io/whois-lookup), [reverse DNS](https://ip-api.io/reverse-dns-lookup),
[MX records](https://ip-api.io/mx-record-lookup) and [domain age](https://ip-api.io/domain-age-checker).

Zero package dependencies — built on `HttpClient` and `System.Text.Json`, fully async.

## Install

```bash
dotnet add package IpApiIo
```

## Quickstart

```csharp
using IpApiIo;

using var client = new IpApiClient(apiKey: "YOUR_API_KEY"); // free key at https://ip-api.io

// Where is this IP, and is it risky?
var info = await client.LookupAsync("8.8.8.8");
Console.WriteLine(info.Location.Country);            // "United States"
Console.WriteLine(info.SuspiciousFactorsInfo.IsVpn); // False

var risk = await client.RiskScoreAsync("8.8.8.8");
Console.WriteLine($"{risk.Score} {risk.RiskLevel}"); // 0 low

var email = await client.ValidateEmailAsync("user@example.com");
Console.WriteLine(email.Reachable);                  // "yes"
```

An API key is required — the API rejects keyless requests with `401`. Sign up at
[ip-api.io](https://ip-api.io) for a free key.

## Documentation

Each guide documents the methods for one capability, with runnable examples and a link
to the matching ip-api.io product page:

- **[IP geolocation & bulk lookup](docs/ip-geolocation.md)** — `LookupAsync`, `LookupBatchAsync`
- **[Email validation & verification](docs/email-validation.md)** — `EmailInfoAsync`, `ValidateEmailAsync`, `ValidateEmailBatchAsync`
- **[Fraud detection & risk scoring](docs/fraud-risk-scoring.md)** — `RiskScoreAsync`, `EmailRiskScoreAsync`, `IpReputationAsync`
- **[VPN, proxy & Tor detection](docs/vpn-proxy-tor.md)** — `TorCheckAsync`, `SuspiciousFactorsInfo`
- **[ASN & DNS lookups](docs/asn-and-dns.md)** — `AsnAsync`, `WhoisAsync`, `ReverseDnsAsync`, `ForwardDnsAsync`, `MxRecordsAsync`
- **[Domain age checker](docs/domain-age.md)** — `DomainAgeAsync`, `DomainAgeBatchAsync`
- **[Errors, rate limits & usage](docs/error-handling.md)** — exception types, `RateLimitAsync`, `UsageSummaryAsync`

## Methods

Every method maps to one ip-api.io endpoint and its product page:

| Method | Endpoint | Product page |
|---|---|---|
| `LookupAsync()` / `LookupAsync(ip)` | `GET /api/v1/ip[/{ip}]` | [IP geolocation](https://ip-api.io/what-is-my-ip) |
| `LookupBatchAsync(ips)` | `POST /api/v1/ip/batch` (≤100 IPs) | [Bulk IP lookup](https://ip-api.io/bulk-ip-lookup) |
| `EmailInfoAsync(email)` | `GET /api/v1/email/{email}` | [Email validation](https://ip-api.io/email-validation) |
| `ValidateEmailAsync(email)` | `GET /api/v1/email/advanced/{email}` | [Advanced email validation](https://ip-api.io/advanced-email-validation) |
| `ValidateEmailBatchAsync(emails)` | `POST /api/v1/email/advanced/batch` (≤100) | [Email list cleaning](https://ip-api.io/email-list-cleaning) |
| `RiskScoreAsync()` / `RiskScoreAsync(ip)` | `GET /api/v1/risk-score[/{ip}]` | [Risk score](https://ip-api.io/risk-score) |
| `EmailRiskScoreAsync(email)` | `GET /api/v1/risk-score/email/{email}` | [Fraud detection](https://ip-api.io/fraud-detection-api) |
| `IpReputationAsync(ip)` | `GET /api/v1/ip-reputation/{ip}` | [IP reputation](https://ip-api.io/ip-reputation) |
| `TorCheckAsync(ip)` | `GET /api/v1/tor/{ip}` | [Tor detection](https://ip-api.io/tor-detection) |
| `AsnAsync(ip)` | `GET /api/v1/asn/{ip}` | [ASN lookup](https://ip-api.io/asn-lookup) |
| `WhoisAsync(domain)` | `GET /api/v1/dns/whois/{domain}` | [WHOIS lookup](https://ip-api.io/whois-lookup) |
| `ReverseDnsAsync(ip)` | `GET /api/v1/dns/reverse/{ip}` | [Reverse DNS](https://ip-api.io/reverse-dns-lookup) |
| `ForwardDnsAsync(hostname)` | `GET /api/v1/dns/forward/{hostname}` | — |
| `MxRecordsAsync(domain)` | `GET /api/v1/dns/mx/{domain}` | [MX record lookup](https://ip-api.io/mx-record-lookup) |
| `DomainAgeAsync(domain)` | `GET /api/v1/domain/age/{domain}` | [Domain age checker](https://ip-api.io/domain-age-checker) |
| `DomainAgeBatchAsync(domains)` | `POST /api/v1/domain/age/batch` | [Domain age checker](https://ip-api.io/domain-age-checker) |
| `RateLimitAsync()` | `GET /api/v1/ratelimit` | — |
| `UsageSummaryAsync()` | `GET /api/v1/usage/summary` | — |

All responses are fully typed (`IpInfo`, `RiskScore`, `AdvancedEmailValidation`, …).
Every method accepts an optional `CancellationToken`.

## Error handling

The client throws typed exceptions and **never retries** — `RateLimitException.Reset`
tells you when your quota renews:

```csharp
try
{
    await client.LookupAsync("8.8.8.8");
}
catch (RateLimitException e)
{
    Console.WriteLine($"limit={e.Limit} remaining={e.Remaining} resetsAt={e.Reset}");
}
catch (AuthenticationException)
{
    Console.WriteLine("invalid API key");
}
```

See [docs/error-handling.md](docs/error-handling.md) for the full exception taxonomy.

## Links

- Full tutorial: https://ip-api.io/docs/sdk/dotnet
- Website: https://ip-api.io
- API reference: https://ip-api.io/api-docs.html
- OpenAPI spec: https://ip-api.io/openapi.json
- Get a free API key: https://ip-api.io

---

`IpApiIo` is the official client for [ip-api.io](https://ip-api.io).
It is not affiliated with ip-api.com or ipapi.com.
