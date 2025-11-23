namespace ShoppingBasket.Core.Entities;

public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPercentage { get; set; }

    public Item() {}

    public Item(string name, decimal price, decimal discountPercentage = 0)
    {
        Name = name;
        Price = price;
        DiscountPercentage = discountPercentage;
    }
}