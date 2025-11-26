using FastEndpoints;
using MediatR;
using ShoppingBasket.Api.Requests;
using ShoppingBasket.Api.Responses;
using ShoppingBasket.Core.Commands;

namespace ShoppingBasket.Api.Endpoints;

public class RemoveBasketItemEndpoint(
    IMediator mediator)
    : Endpoint<RemoveBasketItemRequest, RemoveBasketItemResponse>
{
    public override void Configure()
    {
        Delete("/baskets/{userId}/items/{itemId}");
        // Version(1);
        AllowAnonymous();
    }

    public override async Task HandleAsync(RemoveBasketItemRequest req, CancellationToken ct)
    {
        await mediator.Send(
            new RemoveBasketItemCommand(
                req.UserId,
                req.BasketItemId),
            ct);

        await Send.NoContentAsync(ct);
    }
}