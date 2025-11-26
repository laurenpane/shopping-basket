using FastEndpoints;
using MediatR;
using ShoppingBasket.Api.Requests;
using ShoppingBasket.Core.Dtos;
using ShoppingBasket.Core.Queries;

namespace ShoppingBasket.Api.Endpoints;

public class GetBasketSummaryEndpoint(
    IMediator mediator)
    : Endpoint<GetBasketSummaryRequest, BasketSummaryDto>
{
    public override void Configure()
    {
        Get("/baskets/{userId}/summary");
        Version(1);
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetBasketSummaryRequest req, CancellationToken ct)
    {
        var query = new GetBasketSummaryQuery(
            req.UserId,
            req.PageNumber,
            req.PageSize);

        var result = await mediator.Send(query, ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}