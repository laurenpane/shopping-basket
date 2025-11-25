namespace ShoppingBasket.Core.Entities;

public class Basket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public string ShippingCountryCode { get; set; }
    public List<BasketItem> Items { get; set; } = new();
    public string? DiscountCode { get; set; }
    
    public Basket() {}

    public Basket(string userId, string shippingCountryCode)
    {
        UserId = userId;
        ShippingCountryCode = shippingCountryCode.ToUpper();
    }
    
    public void AddBasketItem(Guid itemId, int quantity)
    {
        var existing = Items.FirstOrDefault(i => i.ItemId == itemId);

        if (existing is null)
            Items.Add(new BasketItem(Id, itemId, quantity));

        else
            existing.IncreaseQuantity(quantity);
    }
}