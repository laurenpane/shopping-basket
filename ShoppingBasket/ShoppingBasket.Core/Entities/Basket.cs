namespace ShoppingBasket.Core.Entities;

public class Basket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string ShippingCountryCode { get; set; }
    public List<BasketItem> Items { get; set; } = new();
    public string? DiscountCode { get; set; }
    public decimal DiscountAmountApplied { get; set; }

    public Basket() {}

    public Basket(Guid userId, string shippingCountryCode)
    {
        UserId = userId;
        ShippingCountryCode = shippingCountryCode.ToUpper();
        DiscountAmountApplied = 0m;
    }
    
    public void AddBasketItem(Guid itemId, int quantity, decimal itemPrice)
    {
        var existing = Items.FirstOrDefault(i => i.ItemId == itemId);

        if (existing is null)
            Items.Add(new BasketItem(Id, itemId, quantity, itemPrice));

        else
            existing.IncreaseQuantity(quantity);
    }

    public void AddDiscount(string code, decimal amount)
    {
        if (!string.IsNullOrEmpty(DiscountCode))
        {
            throw new InvalidOperationException($"Code {code} has already been applied to basket {Id}");
        }
        DiscountCode = code;
        DiscountAmountApplied = amount;
    }
}