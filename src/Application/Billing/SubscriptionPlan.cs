namespace Aptabase.Application.Billing;

public class SubscriptionPlan
{
    public string Name { get; }
    public int MonthlyPrice { get; }
    public int MonthlyEvents { get; }
    public long VariantId { get; }

    private SubscriptionPlan(string name, int monthlyEvents, int monthlyPrice, long variantId)
    {
        Name = name;
        MonthlyEvents = monthlyEvents;
        MonthlyPrice = monthlyPrice;
        VariantId = variantId;
    }

    public static readonly SubscriptionPlan AptabaseFree = new SubscriptionPlan("Free Plan", 20_000, 0, 0);
    public static readonly SubscriptionPlan Aptabase200k = new SubscriptionPlan("200k Plan", 200_000, 10, 85183);
    public static readonly SubscriptionPlan Aptabase1M = new SubscriptionPlan("1M Plan", 1_000_000, 20, 85184);
    public static readonly SubscriptionPlan Aptabase2M = new SubscriptionPlan("2M Plan", 2_000_000, 40, 85185);
    public static readonly SubscriptionPlan Aptabase5M = new SubscriptionPlan("5M Plan", 5_000_000, 75, 85187);
    public static readonly SubscriptionPlan Aptabase10M = new SubscriptionPlan("10M Plan", 10_000_000, 140, 85188);
    public static readonly SubscriptionPlan Aptabase20M = new SubscriptionPlan("20M Plan", 20_000_000, 240, 85190);
    public static readonly SubscriptionPlan Aptabase30M = new SubscriptionPlan("30M Plan", 30_000_000, 300, 85192);
    public static readonly SubscriptionPlan Aptabase50M = new SubscriptionPlan("50M Plan", 50_000_000, 450, 85194);

    public static readonly SubscriptionPlan[] All = new[]
    {
        Aptabase200k,
        Aptabase1M,
        Aptabase2M,
        Aptabase5M,
        Aptabase10M,
        Aptabase20M,
        Aptabase30M,
        Aptabase50M
    };
}
