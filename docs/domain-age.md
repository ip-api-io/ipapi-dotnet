# Domain age checker

Newly registered domains are a strong fraud and spam signal. `DomainAgeAsync` returns
how long ago a domain was registered, derived from WHOIS data, so you can flag or block
domains created days ago.

Powers the [domain age checker](https://ip-api.io/domain-age-checker).

## `DomainAgeAsync(domain)` — age of one domain

```csharp
using IpApiIo;

using var client = new IpApiClient(apiKey: "YOUR_API_KEY");

var age = await client.DomainAgeAsync("example.com");

Console.WriteLine(age.IsValid);          // True
Console.WriteLine(age.RegistrationDate);  // "1995-08-14"
Console.WriteLine(age.AgeInYears);        // 30
Console.WriteLine(age.AgeInDays);         // 11000+

if (age.AgeInDays is < 30)
{
    // treat brand-new domains as higher risk
}
```

### Response (`DomainAge`)

| Property | Type | Description |
|---|---|---|
| `Domain` | `string` | The domain checked |
| `IsValid` | `bool` | Whether age could be determined |
| `RegistrationDate` | `string?` | First registration date |
| `AgeInYears` | `int?` | Age in whole years |
| `AgeInDays` | `long?` | Age in days |
| `Error` | `string?` | Reason when `IsValid` is false |

## `DomainAgeBatchAsync(domains)` — many domains at once

Check a list of domains in one request (non-empty; throws `ArgumentException` if empty).

```csharp
var batch = await client.DomainAgeBatchAsync(new[]
{
    "example.com",
    "brand-new-domain.xyz",
});

foreach (var (domain, age) in batch.Results)
{
    Console.WriteLine($"{domain} {age.AgeInDays}");
}
```

### Response (`BatchDomainAgeResponse`)
`Results` — a `Dictionary<string, DomainAge>` mapping each domain to its age.

## See also

- [ASN & DNS lookups](asn-and-dns.md) — `WhoisAsync` for the full registration record
- [Fraud detection & risk scoring](fraud-risk-scoring.md) — combine age with other signals
- Product page: [Domain age checker](https://ip-api.io/domain-age-checker)
- [Full tutorial on ip-api.io](https://ip-api.io/docs/sdk/dotnet/domain-age)
