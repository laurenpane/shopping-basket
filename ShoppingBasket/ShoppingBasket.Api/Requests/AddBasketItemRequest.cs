using FastEndpoints;

namespace ShoppingBasket.Api.Requests;

public class AddBasketItemRequest
{
    [BindFrom("userId")]
    public Guid UserId { get; set; }
    [BindFrom("itemId")]
    public Guid ItemId { get; set; }

    public required int Quantity { get; set; }
    public required string ShippingCountryCode { get; set; }
}