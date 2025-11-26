namespace ShoppingBasket.Core.Dtos;

public record BasketSummaryDto(
    Guid BasketId,
    decimal Subtotal,
    decimal SubtotalWithVat,
    string ShippingCountryCode,
    decimal ShippingCost,
    decimal DiscountAmountApplied,
    string DiscountCodeApplied,
    decimal Total,
    int PageNumber,
    int PageSize,
    IEnumerable<BasketItemDto> Items);

