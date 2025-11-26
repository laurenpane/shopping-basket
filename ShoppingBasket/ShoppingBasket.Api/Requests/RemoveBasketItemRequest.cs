using FastEndpoints;

namespace ShoppingBasket.Api.Requests;

public class RemoveBasketItemRequest
{
    [BindFrom("userId")] public Guid UserId { get; set; }
    [BindFrom("itemId")] public Guid BasketItemId { get; set; }
}