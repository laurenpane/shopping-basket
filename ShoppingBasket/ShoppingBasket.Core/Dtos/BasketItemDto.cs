namespace ShoppingBasket.Core.Dtos;

public record BasketItemDto(
    Guid ItemId,
    string Name,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
    );