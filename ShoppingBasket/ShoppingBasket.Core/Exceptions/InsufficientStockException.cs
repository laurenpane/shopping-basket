namespace ShoppingBasket.Core.Exceptions;

public class InsufficientStockException(Guid itemId) : Exception($"Insufficient stock for item: {itemId}");