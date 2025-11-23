namespace ShoppingBasket.Core.Entities;

public class BasketItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BasketId { get; set; }
    public Basket? Basket { get; set; }
    public Guid ItemId { get; set; }
    public Item? Item { get; set; }
    public int Quantity { get; set; }

    public BasketItem() {}

    public BasketItem(Guid basketId, Guid itemId, int quantity)
    {
        BasketId = basketId;
        ItemId = itemId;
        Quantity = quantity;
    }
}