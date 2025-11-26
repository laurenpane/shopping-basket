namespace ShoppingBasket.Core.Entities;

public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public decimal RegularPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? DiscountPercentage { get; set; }

    public Item() {}

    public Item(string name, decimal price, decimal discountPercentage = 0, decimal? salePrice = null, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        RegularPrice = price;
        SalePrice = salePrice;
        DiscountPercentage = discountPercentage;
    }
}