namespace ShoppingBasket.Core.Entities;

public class DiscountCode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; }
    public decimal DiscountPercentage { get; set; }

    public DiscountCode() {}

    public DiscountCode(string code, decimal percentage)
    {
        Code = code;
        DiscountPercentage = percentage;
    }
}
