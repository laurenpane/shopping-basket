namespace ShoppingBasket.Core.Dtos;

public record BasketItemDto(
    Guid BasketItemId,
    decimal RegularPrice,
    decimal? SalePrice,
    int Quantity
    );