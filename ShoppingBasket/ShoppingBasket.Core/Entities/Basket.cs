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
    
    public BasketItem? AddBasketItem(Guid itemId, int quantity, decimal regularPrice, decimal? salePrice)
    {
        var existing = Items.FirstOrDefault(i => i.ItemId == itemId);

        if (existing is null)
        {
            var newItem = new BasketItem(Id, itemId, quantity, regularPrice, salePrice);
            Items.Add(newItem);
            return newItem;
        }
        existing.IncreaseQuantity(quantity);
        return null;
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
    
    public IEnumerable<BasketItem> GetSaleItems()
        => Items.Where(i => i.SalePrice.HasValue);
    
    public IEnumerable<BasketItem> GetNonSaleItems()
        => Items.Where(i => !i.SalePrice.HasValue);

    public decimal GetSubtotalNonSale() =>
        GetNonSaleItems().Sum(i => i.Quantity * i.RegularPrice);
    
    public decimal GetSubtotalSale() =>
        GetNonSaleItems().Sum(i => i.Quantity * i.SalePrice!.Value);

}