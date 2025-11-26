using FastEndpoints;

namespace ShoppingBasket.Api.Requests;

public class GetBasketSummaryRequest
{
    [BindFrom("userId")]
    public Guid UserId { get; set; }

    [QueryParam]
    public int PageNumber { get; set; } = 1;

    [QueryParam]
    public int PageSize { get; set; } = 10;
}