# ASN & DNS lookups

Resolve the network and DNS layer behind an IP or domain: which autonomous system
owns an address, who registered a domain, what a host's PTR record is, and which mail
servers a domain uses.

Powers [ASN lookup](https://ip-api.io/asn-lookup),
[WHOIS lookup](https://ip-api.io/whois-lookup),
[reverse DNS](https://ip-api.io/reverse-dns-lookup) and
[MX record lookup](https://ip-api.io/mx-record-lookup).

## `AsnAsync(ip)` — autonomous system for an IP

Returns the ASN, owning organization, network range and country for an IP — and
whether it belongs to a datacenter.

```csharp
using IpApiIo;

using var client = new IpApiClient(apiKey: "YOUR_API_KEY");

var asn = await client.AsnAsync("8.8.8.8");

Console.WriteLine(asn.Asn);          // 15169
Console.WriteLine(asn.Organization); // "Google LLC"
Console.WriteLine(asn.Network);      // "8.8.8.0/24"
Console.WriteLine(asn.IsDatacenter); // True
Console.WriteLine(asn.CountryCode);  // "US"
```

### Response (`AsnLookup`)
`Ip`, `Asn`, `Organization`, `Network`, `IsDatacenter`, `Country`, `CountryCode`.

## `WhoisAsync(domain)` — domain registration

WHOIS record for a domain: registrar, registration/expiry/update dates, name servers,
status codes and the raw WHOIS text.

```csharp
var whois = await client.WhoisAsync("example.com");

Console.WriteLine(whois.Registrar?.Name);
Console.WriteLine(whois.RegisteredOn);   // "1995-08-14"
Console.WriteLine(whois.ExpiresOn);
Console.WriteLine(string.Join(", ", whois.NameServers));
```

### Response (`Whois`)
`Domain`, `Registrar` (`Name`, `Url`, `IanaId`), `RegisteredOn`, `ExpiresOn`,
`UpdatedOn`, `NameServers`, `Status` (`Code`, `Humanized`), `Raw`, `Error`.

## `ReverseDnsAsync(ip)` — PTR record for an IP

```csharp
var rdns = await client.ReverseDnsAsync("8.8.8.8");

Console.WriteLine(rdns.Hostname);   // "dns.google"
Console.WriteLine(rdns.PtrRecord);
```

### Response (`ReverseDns`)
`Ip`, `Hostname`, `PtrRecord`, `Ttl`.

## `ForwardDnsAsync(hostname)` — resolve a hostname to addresses

```csharp
var fdns = await client.ForwardDnsAsync("dns.google");

foreach (var record in fdns.Addresses)
{
    Console.WriteLine($"{record.Type} {record.Address} {record.Ttl}"); // "A" "8.8.8.8" 300
}
```

### Response (`ForwardDns`)
`Hostname`, `Addresses` (each `Type`, `Address`, `Ttl`).

## `MxRecordsAsync(domain)` — mail servers for a domain

```csharp
var mx = await client.MxRecordsAsync("example.com");

foreach (var record in mx.MxRecords)
{
    Console.WriteLine($"{record.Priority} {record.Hostname} {record.Ttl}");
}
```

### Response (`MxLookup`)
`Domain`, `MxRecords` (each `Priority`, `Hostname`, `Ttl`).

## See also

- [IP geolocation & bulk lookup](ip-geolocation.md) — geolocation for the same IP
- [Email validation & verification](email-validation.md) — MX records feed deliverability
- [Domain age checker](domain-age.md) — registration age from WHOIS data
- Product pages: [ASN lookup](https://ip-api.io/asn-lookup) · [WHOIS lookup](https://ip-api.io/whois-lookup) · [Reverse DNS](https://ip-api.io/reverse-dns-lookup) · [MX record lookup](https://ip-api.io/mx-record-lookup)
