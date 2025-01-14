using Aptabase.Data;
using ClickHouse.Client.Utility;
using Dapper;
using Microsoft.Extensions.Caching.Memory;

namespace Aptabase.Features.Ingestion;

public enum AppKeyStatus
{
    Valid,
    Missing,
    InvalidRegion,
    InvalidFormat,
    NotFound
}

public interface IIngestionValidator
{
    (bool, string) IsValidBody(EventBody? body);

    Task<(string, AppKeyStatus)> IsAppKeyValid(string appKey);
}

public class IngestionValidator : IIngestionValidator
{
    private readonly IMemoryCache _cache;
    private readonly IDbContext _db;
    private readonly EnvSettings _env;

    private readonly TimeSpan SuccessCacheDuration = TimeSpan.FromMinutes(30);
    private readonly TimeSpan FailureCacheDuration = TimeSpan.FromMinutes(5);

    public IngestionValidator(IMemoryCache cache, IDbContext db, EnvSettings env)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public async Task<(string, AppKeyStatus)> IsAppKeyValid(string appKey)
    {
        if (string.IsNullOrWhiteSpace(appKey))
            return (string.Empty, AppKeyStatus.Missing);

        var parts = appKey.Split('-');
        if (parts is null || parts.Length < 3 || parts[0] != "A")
            return (string.Empty, AppKeyStatus.InvalidFormat);

        if (parts[1] != _env.Region)
            return (string.Empty, AppKeyStatus.InvalidRegion);

        var cacheKey = $"APP-KEY-STATUS-{appKey}";
        if (_cache.TryGetValue(cacheKey, out string? cachedAppId))
            return string.IsNullOrWhiteSpace(cachedAppId) ? (string.Empty, AppKeyStatus.NotFound) : (cachedAppId, AppKeyStatus.Valid);

        var appId = await FindActiveByAppKey(appKey);
        if (string.IsNullOrEmpty(appId))
        {
            _cache.Set(cacheKey, string.Empty, FailureCacheDuration);
            return (string.Empty, AppKeyStatus.NotFound);
        }

        _cache.Set(cacheKey, appId, SuccessCacheDuration);
        return (appId, AppKeyStatus.Valid);
    }

    private async Task<string?> FindActiveByAppKey(string appKey)
    {
        return await _db.Connection.ExecuteScalarAsync<string>($"SELECT id FROM apps WHERE app_key = @appKey AND deleted_at IS NULL", new { appKey });
    }

    public (bool, string) IsValidBody(EventBody? body)
    {
        if (body is null)
            return (false, "Missing event body.");

        if (body.Timestamp > DateTime.UtcNow.AddMinutes(1))
            return (false, "Future events are not allowed.");

        if (body.Timestamp < DateTime.UtcNow.AddDays(-1) && !_env.IsDevelopment)
            return (false, "Event is too old.");

        if (body.Props is not null)
        {
            foreach (var prop in body.Props.RootElement.EnumerateObject())
            {
                if (string.IsNullOrWhiteSpace(prop.Name))
                    return (false, "Property key must not be empty.");

                if (prop.Name.Length > 40)
                    return (false, string.Format("Property key {0} must be less than or equal to 40 characters.", prop.Name));

                if (prop.Value.ToString().Length > 200)
                    return (false, string.Format("Property value must be less than or equal to 200 characters. Value was: {0}", prop.Value.ToString()));
            }
        }

        return (true, string.Empty);
    }
}