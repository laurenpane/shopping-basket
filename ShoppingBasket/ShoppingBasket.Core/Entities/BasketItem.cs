namespace ShoppingBasket.Core.Entities;

public class BasketItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
    
    public Guid BasketId { get; private set; }
    
    public decimal RegularPrice { get; set; }
    public decimal? SalePrice { get; set; }
    // Although it seems like duplication of Item, I added these here as it's important to store prices on BasketItem as well
    // as the prices on an item may change over time but the BasketItem at point of checkout should be recorded

    public BasketItem() {}

    public BasketItem(Guid basketId, Guid itemId, int quantity, decimal regularPrice, decimal? salePrice)
    {
        BasketId = basketId;
        ItemId = itemId;
        Quantity = quantity;
        RegularPrice = regularPrice;
        SalePrice = salePrice;
    }
    
    public void IncreaseQuantity(int amount) =>
        Quantity += amount;
}