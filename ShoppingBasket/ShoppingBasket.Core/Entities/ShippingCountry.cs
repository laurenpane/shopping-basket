namespace ShoppingBasket.Core.Entities;

public class ShippingCountry
{
    public string Code { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    private ShippingCountry() { }

    public ShippingCountry(string code, decimal price)
    {
        Code = code.ToUpper();
        Price = price;
    }
}
