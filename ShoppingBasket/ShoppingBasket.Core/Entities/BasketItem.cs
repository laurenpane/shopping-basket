namespace ShoppingBasket.Core.Entities;

public class BasketItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
    
    public Guid BasketId { get; private set; }
    
    public decimal Price { get; set; }
    // Although it seems like duplication, I added this here as it's important to store Price on BasketItem as well, as the Price on Item may change over time

    public BasketItem() {}

    public BasketItem(Guid basketId, Guid itemId, int quantity, decimal price)
    {
        BasketId = basketId;
        ItemId = itemId;
        Quantity = quantity;
        Price = price;
    }
    
    public void IncreaseQuantity(int amount) =>
        Quantity += amount;
}