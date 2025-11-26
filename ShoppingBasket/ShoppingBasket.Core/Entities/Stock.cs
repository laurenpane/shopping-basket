namespace ShoppingBasket.Core.Entities;

public class Stock
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ItemId { get; set; }
    public Item? Item { get; set; }
    public int QuantityAvailable { get; set; }

    public Stock() {}

    public Stock(Guid itemId, int quantityAvailable)
    {
        ItemId = itemId;
        QuantityAvailable = quantityAvailable;
    }
}