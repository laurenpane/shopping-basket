namespace ShoppingBasket.Core.Entities;

public class BasketItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
    
    public Guid BasketId { get; private set; }

    public BasketItem() {}

    public BasketItem(Guid basketId, Guid itemId, int quantity)
    {
        BasketId = basketId;
        ItemId = itemId;
        Quantity = quantity;
    }
    
    public void IncreaseQuantity(int amount) =>
        Quantity += amount;
}