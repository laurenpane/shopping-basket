using FastEndpoints;
using MediatR;
using ShoppingBasket.Api.Requests;
using ShoppingBasket.Api.Responses;
using ShoppingBasket.Core.Commands;

namespace ShoppingBasket.Api.Endpoints;

public class AddBasketItemEndpoint 
    : Endpoint<AddBasketItemRequest, AddBasketItemResponse>
{
    private readonly IMediator _mediator;
    private readonly ILogger<AddBasketItemEndpoint> _logger;

    public AddBasketItemEndpoint(
        IMediator mediator,
        ILogger<AddBasketItemEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/basket/items");
        // Version(1);
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddBasketItemRequest req, CancellationToken ct)
    {
        var basketId = await _mediator.Send(
            new AddItemToBasketCommand(
                req.UserId,
                req.ItemId,
                req.Quantity,
                req.ShippingCountryCode),
            ct);

        _logger.LogInformation("Basket updated successfully {BasketId}", basketId);

        await Send.OkAsync(new AddBasketItemResponse(basketId), ct);
    }
}