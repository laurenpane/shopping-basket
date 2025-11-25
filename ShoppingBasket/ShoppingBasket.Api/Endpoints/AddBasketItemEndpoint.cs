using FastEndpoints;
using MediatR;
using ShoppingBasket.Api.Requests;
using ShoppingBasket.Api.Responses;
using ShoppingBasket.Core.Commands;

namespace ShoppingBasket.Api.Endpoints;

public class AddBasketItemEndpoint(
    IMediator mediator,
    ILogger<AddBasketItemEndpoint> logger)
    : Endpoint<AddBasketItemRequest, AddBasketItemResponse>
{
    public override void Configure()
    {
        Post("/baskets/{userId}/items");
        // Version(1);
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddBasketItemRequest req, CancellationToken ct)
    {
        var basketId = await mediator.Send(
            new AddBasketItemCommand(
                req.UserId,
                req.ItemId,
                req.Quantity,
                req.ShippingCountryCode),
            ct);

        await Send.OkAsync(new AddBasketItemResponse(basketId), ct);
    }
}