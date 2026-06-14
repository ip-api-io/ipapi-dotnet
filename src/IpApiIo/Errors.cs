namespace IpApiIo;

/// <summary>Base exception for all ip-api.io client failures.</summary>
public class IpApiException : Exception
{
    public int? StatusCode { get; }

    public string? Body { get; }

    public IpApiException(string message, int? statusCode = null, string? body = null)
        : base(message)
    {
        StatusCode = statusCode;
        Body = body;
    }
}

/// <summary>HTTP 401/403 — missing or invalid API key.</summary>
public sealed class AuthenticationException : IpApiException
{
    public AuthenticationException(string message, int statusCode, string? body)
        : base(message, statusCode, body)
    {
    }
}

/// <summary>
/// HTTP 429 — quota exhausted. Exposes the x-ratelimit-* response headers;
/// <see cref="Reset"/> is the unix timestamp when the quota renews.
/// The client never retries.
/// </summary>
public sealed class RateLimitException : IpApiException
{
    public long? Limit { get; }

    public long? Remaining { get; }

    public long? Reset { get; }

    public RateLimitException(string message, string? body, long? limit, long? remaining, long? reset)
        : base(message, 429, body)
    {
        Limit = limit;
        Remaining = remaining;
        Reset = reset;
    }
}

/// <summary>HTTP 400/404/422 — malformed input or unknown resource.</summary>
public sealed class InvalidRequestException : IpApiException
{
    public InvalidRequestException(string message, int statusCode, string? body)
        : base(message, statusCode, body)
    {
    }
}

/// <summary>HTTP 5xx — ip-api.io server-side failure.</summary>
public sealed class ServerException : IpApiException
{
    public ServerException(string message, int statusCode, string? body)
        : base(message, statusCode, body)
    {
    }
}
