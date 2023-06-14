using System.Data;
using Aptabase.Application.Authentication;
using Aptabase.Application.Query;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Aptabase.Application.Billing;

public class BillingUsage
{
    public long Count { get; set; }
    public long Quota { get; set; }
}

public struct GenerateCheckoutUrlRequestParams
{
    public long VariantId { get; set; }
}

[ApiController, IsAuthenticated]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class BillingController : Controller
{
    private readonly IQueryClient _queryClient;
    private readonly IDbConnection _db;
    private readonly LemonSqueezyClient _lsClient;

    public BillingController(IDbConnection db, IQueryClient queryClient, LemonSqueezyClient lsClient)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _queryClient = queryClient ?? throw new ArgumentNullException(nameof(queryClient));
        _lsClient = lsClient ?? throw new ArgumentNullException(nameof(lsClient));
    }

    [HttpGet("/api/_billing/usage")]
    public async Task<IActionResult> CurrentUsage(CancellationToken cancellationToken)
    {
        var user = this.GetCurrentUser();
        var releaseAppIds = await _db.QueryAsync<string>(@"SELECT id FROM apps WHERE owner_id = @userId", new { userId = user.Id });
        var debugAppIds = releaseAppIds.Select(id => $"{id}_DEBUG");
        var appIds = releaseAppIds.Concat(debugAppIds);

        var (year, month) = (DateTime.UtcNow.Year, DateTime.UtcNow.Month);

        var (rows, _) = await _queryClient.QueryAsync<BillingUsage>(
            $@"SELECT countMerge(events) as Count
               FROM billing_usage_v1_mv
               WHERE app_id IN ('{string.Join("','", appIds)}')
               AND year = {year}
               AND month = {month}", cancellationToken);
        
        return Ok(new {
            Count = rows.FirstOrDefault()?.Count ?? 0,
            Quota = 20000
        });
    }

    [HttpGet("/api/_billing")]
    public async Task<IActionResult> SubscriptionPlans(CancellationToken cancellationToken)
    {
        var user = this.GetCurrentUser();
        var currentVariantId = await _db.QuerySingleOrDefaultAsync<long>(@"SELECT variant_id FROM billing WHERE owner_id = @userId", new { userId = user.Id });

        var plans = SubscriptionPlan.All;
        var current = currentVariantId == 0 
            ? SubscriptionPlan.AptabaseFree 
            : plans.FirstOrDefault(p => p.VariantId == currentVariantId);

        return Ok(new {
            current,
            plans
        });
    }

    [HttpPost("/api/_billing/checkout")]
    public async Task<IActionResult> GenerateCheckoutUrl([FromBody] GenerateCheckoutUrlRequestParams body, CancellationToken cancellationToken)
    {
        var user = this.GetCurrentUser();
        var url = await _lsClient.CreateCheckout(body.VariantId, user.Name, user.Email, cancellationToken);

        return Ok(new { url });
    }
}
