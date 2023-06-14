namespace Aptabase.Application.Billing;

public class Checkout
{
    public string Type { get; set; } = "checkouts";
    public string Id { get; set; } = "";
    public CheckoutAttributes Attributes { get; set; } = new CheckoutAttributes();
}

public class CheckoutAttributes
{
    public string Url { get; set; } = "";
    public long VariantId { get; set; }
    public ProductOptions ProductOptions { get; set; } = new ProductOptions();
    public CheckoutOptions CheckoutOptions { get; set; } = new CheckoutOptions();
    public CheckoutData CheckoutData { get; set; } = new CheckoutData();
}

public class ProductOptions
{
    public long[] EnabledVariants { get; set; } = new long[0];
}

public class CheckoutOptions
{
    public bool Embed { get; set; }
    public bool Dark { get; set; }
}

public class CheckoutData
{
    public string Email { get; set; } = "";
    public string Name { get; set; } = "";
}