using FastEndpoints;

namespace ShoppingBasket.Api.Requests;

public class AddBasketDiscountCodeRequest
{
    [BindFrom("userId")]
    public Guid UserId { get; set; }
    public required string DiscountCode { get; set; }
}