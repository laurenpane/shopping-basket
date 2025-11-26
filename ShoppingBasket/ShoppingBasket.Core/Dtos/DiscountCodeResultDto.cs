namespace ShoppingBasket.Core.Dtos;

public record DiscountCodeResultDto(
    Guid BasketId,
    string DiscountCode,
    decimal DiscountAmountApplied
);