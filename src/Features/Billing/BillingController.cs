using System.Data;
using Aptabase.Features.Authentication;
using Aptabase.Features.Query;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Aptabase.Features.Billing;

public class BillingUsage
{
    public long Count { get; set; }
    public long Quota { get; set; }
}

[ApiController, IsAuthenticated]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class BillingController : Controller
{
    private readonly IQueryClient _queryClient;
    private readonly IDbConnection _db;
    private readonly LemonSqueezyClient _lsClient;

    public BillingController(IDbConnection db, LemonSqueezyClient lsClient, IQueryClient queryClient)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _lsClient = lsClient ?? throw new ArgumentNullException(nameof(lsClient));
        _queryClient = queryClient ?? throw new ArgumentNullException(nameof(queryClient));
    }

    [HttpGet("/api/_billing")]
    public async Task<IActionResult> BillingState(CancellationToken cancellationToken)
    {
        var user = this.GetCurrentUser();
        var releaseAppIds = await _db.QueryAsync<string>(@"SELECT id FROM apps WHERE owner_id = @userId", new { userId = user.Id });
        var debugAppIds = releaseAppIds.Select(id => $"{id}_DEBUG");
        var appIds = releaseAppIds.Concat(debugAppIds).ToArray();

        var usage = await _queryClient.NamedQuerySingleAsync<BillingUsage>("get_billing_usage", new {
            app_ids = appIds,
            year = DateTime.UtcNow.Year,
            month = DateTime.UtcNow.Month
        }, cancellationToken);
        
        return Ok(new {
            Count = usage?.Count ?? 0,
            Quota = 20000
        });
    }

    [HttpPost("/api/_billing/checkout")]
    public async Task<IActionResult> GenerateCheckoutUrl(CancellationToken cancellationToken)
    {
        var user = this.GetCurrentUser();
        var url = await _lsClient.CreateCheckout(user.Name, user.Email, cancellationToken);

        return Ok(new { url });
    }
}