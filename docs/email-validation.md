# Email validation & verification

Check whether an email address is real, deliverable and safe to accept — before it
ever enters your database. The SDK exposes three levels: a fast syntax/MX/disposable
check, full SMTP verification, and a batch endpoint for cleaning whole lists.

Powers [email validation](https://ip-api.io/email-validation),
[advanced email validation](https://ip-api.io/advanced-email-validation),
[email verification](https://ip-api.io/email-verification-api),
[disposable email detection](https://ip-api.io/disposable-email-checker) and
[email list cleaning](https://ip-api.io/email-list-cleaning).

## `EmailInfoAsync(email)` — fast syntax, MX & disposable check

A lightweight check (no SMTP probe): validates syntax, confirms the domain has MX
records, and flags disposable/throwaway providers. Use it inline on sign-up forms.

```csharp
using IpApiIo;

using var client = new IpApiClient(apiKey: "YOUR_API_KEY");

var info = await client.EmailInfoAsync("user@example.com");

Console.WriteLine(info.Syntax.IsValid);   // True
Console.WriteLine(info.IsDisposable);     // False
Console.WriteLine(info.HasMxRecords);     // True
if (info.MxRecords.Count > 0)
{
    Console.WriteLine(info.MxRecords[0].Hostname);
}
```

### Response (`EmailInfo`)

| Property | Type | Description |
|---|---|---|
| `Email` | `string` | The address checked |
| `IsDisposable` | `bool` | Throwaway / temporary provider |
| `HasMxRecords` | `bool` | Domain can receive mail |
| `MxRecords` | `List<MxRecord>` | Each: `Priority`, `Hostname`, `Ttl` |
| `Syntax` | `EmailSyntax` | `IsValid`, `Domain`, `Username`, `ErrorReasons` |

## `ValidateEmailAsync(email)` — full SMTP deliverability

Advanced verification that connects to the mail server to confirm the mailbox is
deliverable, and adds role-account, free-provider, catch-all and Gravatar signals.
Use it before sending important mail or accepting a paying customer.

```csharp
var result = await client.ValidateEmailAsync("user@example.com");

Console.WriteLine(result.Reachable);      // "yes" | "no" | "unknown"
Console.WriteLine(result.Smtp?.Deliverable); // True
Console.WriteLine(result.Smtp?.CatchAll);    // False
Console.WriteLine(result.Disposable);     // False
Console.WriteLine(result.RoleAccount);    // False  (e.g. info@, support@)
Console.WriteLine(result.Free);           // False  (e.g. gmail.com)
Console.WriteLine(result.Suggestion);     // typo fix, e.g. "user@gmail.com"
```

### Response (`AdvancedEmailValidation`)

| Property | Type | Description |
|---|---|---|
| `Email` | `string` | The address checked |
| `Reachable` | `string` | `"yes"`, `"no"` or `"unknown"` |
| `Syntax` | `AdvancedSyntax` | `Username`, `Domain`, `Valid` |
| `Smtp` | `AdvancedSmtp?` | `HostExists`, `Deliverable`, `FullInbox`, `CatchAll`, `Disabled` |
| `Gravatar` | `AdvancedGravatar?` | `HasGravatar`, `GravatarUrl` |
| `Suggestion` | `string?` | Suggested correction for a likely typo |
| `Disposable` | `bool` | Throwaway provider |
| `RoleAccount` | `bool` | Role address (info@, sales@, …) |
| `Free` | `bool` | Free webmail provider |
| `HasMxRecords` | `bool` | Domain can receive mail |

## `ValidateEmailBatchAsync(emails)` — clean a list (≤100)

Advanced-validate up to 100 addresses in one request — the building block for
[email list cleaning](https://ip-api.io/email-list-cleaning). Throws
`ArgumentException` if the list is empty or longer than 100.

```csharp
var batch = await client.ValidateEmailBatchAsync(new[]
{
    "user@example.com",
    "fake@mailinator.com",
});

Console.WriteLine(batch.TotalProcessed);        // 2
Console.WriteLine(batch.SuccessfulValidations); // 2

foreach (var (email, result) in batch.Results)
{
    Console.WriteLine($"{email} {result.Reachable} {result.Disposable}");
}
```

### Response (`BatchEmailValidationResponse`)

| Property | Type | Description |
|---|---|---|
| `Results` | `Dictionary<string, AdvancedEmailValidation>` | Map of email → result |
| `TotalProcessed` | `int` | Emails received |
| `SuccessfulValidations` | `int` | Emails validated |
| `FailedValidations` | `int` | Emails that errored |

## See also

- [Fraud detection & risk scoring](fraud-risk-scoring.md) — `EmailRiskScoreAsync` for a 0–100 score
- [ASN & DNS lookups](asn-and-dns.md) — `MxRecordsAsync` to inspect a domain's mail servers
- Product pages: [Email validation](https://ip-api.io/email-validation) · [Advanced validation](https://ip-api.io/advanced-email-validation) · [Email verification API](https://ip-api.io/email-verification-api) · [Disposable email checker](https://ip-api.io/disposable-email-checker) · [Email list cleaning](https://ip-api.io/email-list-cleaning)
- [Full tutorial on ip-api.io](https://ip-api.io/docs/sdk/dotnet/email-validation)
