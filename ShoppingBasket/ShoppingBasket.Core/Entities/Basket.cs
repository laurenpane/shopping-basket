namespace ShoppingBasket.Core.Entities;

public class Basket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public string ShippingCountry { get; set; }
    public List<BasketItem> Items { get; set; } = new();
    public string? DiscountCode { get; set; }
    
    public Basket() {}

    public Basket(string userId, string shippingCountry)
    {
        UserId = userId;
        ShippingCountry = shippingCountry;
    }
}