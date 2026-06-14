using System.Text.Json.Serialization;

namespace IpApiIo;

// Models mirror the schemas in https://ip-api.io/openapi.json.
// Unknown fields are tolerated for forward compatibility.

public sealed class SuspiciousFactors
{
    [JsonPropertyName("is_proxy")] public bool IsProxy { get; init; }
    [JsonPropertyName("is_tor_node")] public bool IsTorNode { get; init; }
    [JsonPropertyName("is_spam")] public bool IsSpam { get; init; }
    [JsonPropertyName("is_crawler")] public bool IsCrawler { get; init; }
    [JsonPropertyName("is_datacenter")] public bool IsDatacenter { get; init; }
    [JsonPropertyName("is_vpn")] public bool IsVpn { get; init; }
    [JsonPropertyName("is_threat")] public bool IsThreat { get; init; }
}

public sealed class IpLocation
{
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("country_code")] public string? CountryCode { get; init; }
    [JsonPropertyName("city")] public string? City { get; init; }
    [JsonPropertyName("latitude")] public double? Latitude { get; init; }
    [JsonPropertyName("longitude")] public double? Longitude { get; init; }
    [JsonPropertyName("zip")] public string? Zip { get; init; }
    [JsonPropertyName("timezone")] public string? Timezone { get; init; }
    [JsonPropertyName("local_time")] public string? LocalTime { get; init; }
    [JsonPropertyName("local_time_unix")] public long? LocalTimeUnix { get; init; }
    [JsonPropertyName("is_daylight_savings")] public bool? IsDaylightSavings { get; init; }
}

public sealed class IpInfo
{
    [JsonPropertyName("ip")] public string Ip { get; init; } = "";
    [JsonPropertyName("isp")] public string? Isp { get; init; }
    [JsonPropertyName("asn")] public string? Asn { get; init; }
    [JsonPropertyName("suspicious_factors")] public SuspiciousFactors SuspiciousFactorsInfo { get; init; } = new();
    [JsonPropertyName("location")] public IpLocation Location { get; init; } = new();
}

public sealed class BatchIpLookupResponse
{
    [JsonPropertyName("results")] public Dictionary<string, IpInfo> Results { get; init; } = new();
    [JsonPropertyName("total_processed")] public int TotalProcessed { get; init; }
    [JsonPropertyName("successful_lookups")] public int SuccessfulLookups { get; init; }
    [JsonPropertyName("failed_lookups")] public int FailedLookups { get; init; }
}

public sealed class MxRecord
{
    [JsonPropertyName("priority")] public int Priority { get; init; }
    [JsonPropertyName("hostname")] public string Hostname { get; init; } = "";
    [JsonPropertyName("ttl")] public long Ttl { get; init; }
}

public sealed class EmailSyntax
{
    [JsonPropertyName("domain")] public string? Domain { get; init; }
    [JsonPropertyName("username")] public string? Username { get; init; }
    [JsonPropertyName("is_valid")] public bool IsValid { get; init; }
    [JsonPropertyName("error_reasons")] public List<string> ErrorReasons { get; init; } = new();
}

public sealed class EmailInfo
{
    [JsonPropertyName("email")] public string Email { get; init; } = "";
    [JsonPropertyName("is_disposable")] public bool IsDisposable { get; init; }
    [JsonPropertyName("has_mx_records")] public bool HasMxRecords { get; init; }
    [JsonPropertyName("mx_records")] public List<MxRecord> MxRecords { get; init; } = new();
    [JsonPropertyName("syntax")] public EmailSyntax Syntax { get; init; } = new();
}

public sealed class AdvancedSyntax
{
    [JsonPropertyName("username")] public string Username { get; init; } = "";
    [JsonPropertyName("domain")] public string Domain { get; init; } = "";
    [JsonPropertyName("valid")] public bool Valid { get; init; }
}

public sealed class AdvancedSmtp
{
    [JsonPropertyName("host_exists")] public bool HostExists { get; init; }
    [JsonPropertyName("full_inbox")] public bool FullInbox { get; init; }
    [JsonPropertyName("catch_all")] public bool CatchAll { get; init; }
    [JsonPropertyName("deliverable")] public bool Deliverable { get; init; }
    [JsonPropertyName("disabled")] public bool Disabled { get; init; }
}

public sealed class AdvancedGravatar
{
    [JsonPropertyName("has_gravatar")] public bool HasGravatar { get; init; }
    [JsonPropertyName("gravatar_url")] public string GravatarUrl { get; init; } = "";
}

public sealed class AdvancedEmailValidation
{
    [JsonPropertyName("email")] public string Email { get; init; } = "";
    [JsonPropertyName("reachable")] public string Reachable { get; init; } = "";
    [JsonPropertyName("syntax")] public AdvancedSyntax Syntax { get; init; } = new();
    [JsonPropertyName("smtp")] public AdvancedSmtp? Smtp { get; init; }
    [JsonPropertyName("gravatar")] public AdvancedGravatar? Gravatar { get; init; }
    [JsonPropertyName("suggestion")] public string? Suggestion { get; init; }
    [JsonPropertyName("disposable")] public bool Disposable { get; init; }
    [JsonPropertyName("role_account")] public bool RoleAccount { get; init; }
    [JsonPropertyName("free")] public bool Free { get; init; }
    [JsonPropertyName("has_mx_records")] public bool HasMxRecords { get; init; }
}

public sealed class BatchEmailValidationResponse
{
    [JsonPropertyName("results")] public Dictionary<string, AdvancedEmailValidation> Results { get; init; } = new();
    [JsonPropertyName("totalProcessed")] public int TotalProcessed { get; init; }
    [JsonPropertyName("successfulValidations")] public int SuccessfulValidations { get; init; }
    [JsonPropertyName("failedValidations")] public int FailedValidations { get; init; }
}

public sealed class IpFactors
{
    [JsonPropertyName("is_proxy")] public bool IsProxy { get; init; }
    [JsonPropertyName("is_tor_node")] public bool IsTorNode { get; init; }
    [JsonPropertyName("is_spam")] public bool IsSpam { get; init; }
    [JsonPropertyName("is_vpn")] public bool IsVpn { get; init; }
    [JsonPropertyName("is_datacenter")] public bool IsDatacenter { get; init; }
    [JsonPropertyName("risk_contribution")] public double RiskContribution { get; init; }
}

public sealed class EmailFactors
{
    [JsonPropertyName("is_disposable")] public bool IsDisposable { get; init; }
    [JsonPropertyName("is_valid_syntax")] public bool IsValidSyntax { get; init; }
    [JsonPropertyName("risk_contribution")] public double RiskContribution { get; init; }
}

public sealed class RiskScoreFactors
{
    [JsonPropertyName("ip_factors")] public IpFactors? IpFactorsInfo { get; init; }
    [JsonPropertyName("email_factors")] public EmailFactors? EmailFactorsInfo { get; init; }
}

public sealed class RiskScore
{
    [JsonPropertyName("score")] public double Score { get; init; }
    [JsonPropertyName("risk_level")] public string RiskLevel { get; init; } = "";
    [JsonPropertyName("ip")] public string? Ip { get; init; }
    [JsonPropertyName("email")] public string? Email { get; init; }
    [JsonPropertyName("factors")] public RiskScoreFactors Factors { get; init; } = new();
}

public sealed class TorDetection
{
    [JsonPropertyName("ip")] public string Ip { get; init; } = "";
    [JsonPropertyName("is_tor")] public bool IsTor { get; init; }
    [JsonPropertyName("tor_node_count")] public int TorNodeCount { get; init; }
}

public sealed class AsnLookup
{
    [JsonPropertyName("ip")] public string Ip { get; init; } = "";
    [JsonPropertyName("asn")] public long? Asn { get; init; }
    [JsonPropertyName("organization")] public string? Organization { get; init; }
    [JsonPropertyName("network")] public string? Network { get; init; }
    [JsonPropertyName("is_datacenter")] public bool IsDatacenter { get; init; }
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("country_code")] public string? CountryCode { get; init; }
}

public sealed class DomainAge
{
    [JsonPropertyName("domain")] public string Domain { get; init; } = "";
    [JsonPropertyName("is_valid")] public bool IsValid { get; init; }
    [JsonPropertyName("registration_date")] public string? RegistrationDate { get; init; }
    [JsonPropertyName("age_in_years")] public int? AgeInYears { get; init; }
    [JsonPropertyName("age_in_days")] public long? AgeInDays { get; init; }
    [JsonPropertyName("error")] public string? Error { get; init; }
}

public sealed class BatchDomainAgeResponse
{
    [JsonPropertyName("results")] public Dictionary<string, DomainAge> Results { get; init; } = new();
}

public sealed class WhoisRegistrar
{
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("iana_id")] public string? IanaId { get; init; }
}

public sealed class WhoisStatus
{
    [JsonPropertyName("code")] public string Code { get; init; } = "";
    [JsonPropertyName("humanized")] public string Humanized { get; init; } = "";
}

public sealed class Whois
{
    [JsonPropertyName("domain")] public string Domain { get; init; } = "";
    [JsonPropertyName("registrar")] public WhoisRegistrar? Registrar { get; init; }
    [JsonPropertyName("registered_on")] public string? RegisteredOn { get; init; }
    [JsonPropertyName("expires_on")] public string? ExpiresOn { get; init; }
    [JsonPropertyName("updated_on")] public string? UpdatedOn { get; init; }
    [JsonPropertyName("name_servers")] public List<string> NameServers { get; init; } = new();
    [JsonPropertyName("status")] public List<WhoisStatus> Status { get; init; } = new();
    [JsonPropertyName("raw")] public string Raw { get; init; } = "";
    [JsonPropertyName("error")] public string? Error { get; init; }
}

public sealed class ReverseDns
{
    [JsonPropertyName("ip")] public string Ip { get; init; } = "";
    [JsonPropertyName("hostname")] public string? Hostname { get; init; }
    [JsonPropertyName("ptr_record")] public string? PtrRecord { get; init; }
    [JsonPropertyName("ttl")] public long? Ttl { get; init; }
}

public sealed class ForwardLookupRecord
{
    [JsonPropertyName("type")] public string Type { get; init; } = "";
    [JsonPropertyName("address")] public string Address { get; init; } = "";
    [JsonPropertyName("ttl")] public long Ttl { get; init; }
}

public sealed class ForwardDns
{
    [JsonPropertyName("hostname")] public string Hostname { get; init; } = "";
    [JsonPropertyName("addresses")] public List<ForwardLookupRecord> Addresses { get; init; } = new();
}

public sealed class MxLookup
{
    [JsonPropertyName("domain")] public string Domain { get; init; } = "";
    [JsonPropertyName("mx_records")] public List<MxRecord> MxRecords { get; init; } = new();
}

public sealed class ApiLimitInfo
{
    [JsonPropertyName("limit")] public long Limit { get; init; }
    [JsonPropertyName("remaining")] public long Remaining { get; init; }
    [JsonPropertyName("used")] public long Used { get; init; }
    [JsonPropertyName("usage_percent")] public double UsagePercent { get; init; }
}

public sealed class RateLimitInfo
{
    [JsonPropertyName("plan_id")] public string PlanId { get; init; } = "";
    [JsonPropertyName("plan_name")] public string? PlanName { get; init; }
    [JsonPropertyName("ip_api")] public ApiLimitInfo IpApi { get; init; } = new();
    [JsonPropertyName("email_api")] public ApiLimitInfo EmailApi { get; init; } = new();
    [JsonPropertyName("interval_seconds")] public long IntervalSeconds { get; init; }
    [JsonPropertyName("next_renewal_date")] public string? NextRenewalDate { get; init; }
    [JsonPropertyName("status")] public string? Status { get; init; }
}

public sealed class UsageSummary
{
    [JsonPropertyName("apiKey")] public string ApiKey { get; init; } = "";
    [JsonPropertyName("apiType")] public string ApiType { get; init; } = "";
    [JsonPropertyName("periodStart")] public string PeriodStart { get; init; } = "";
    [JsonPropertyName("periodEnd")] public string PeriodEnd { get; init; } = "";
    [JsonPropertyName("totalRequests")] public long TotalRequests { get; init; }
    [JsonPropertyName("successfulRequests")] public long SuccessfulRequests { get; init; }
    [JsonPropertyName("rateLimitedRequests")] public long RateLimitedRequests { get; init; }
    [JsonPropertyName("quotaConsumed")] public long QuotaConsumed { get; init; }
    [JsonPropertyName("batchOperations")] public long BatchOperations { get; init; }
    [JsonPropertyName("avgRequestDurationMs")] public double? AvgRequestDurationMs { get; init; }
}
