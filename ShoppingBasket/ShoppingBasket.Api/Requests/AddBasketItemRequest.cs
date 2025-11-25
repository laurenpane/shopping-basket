namespace ShoppingBasket.Api.Requests;

public record AddBasketItemRequest(
    string UserId,
    Guid ItemId,
    int Quantity,
    string ShippingCountryCode);