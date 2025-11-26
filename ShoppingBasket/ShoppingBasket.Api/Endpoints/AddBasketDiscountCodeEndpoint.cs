using FastEndpoints;
using MediatR;
using ShoppingBasket.Api.Requests;
using ShoppingBasket.Api.Responses;
using ShoppingBasket.Core.Commands;

namespace ShoppingBasket.Api.Endpoints;

public class AddBasketDiscountCodeEndpoint(
    IMediator mediator)
    : Endpoint<AddBasketDiscountCodeRequest, AddBasketDiscountCodeResponse>
{
    public override void Configure()
    {
        Post("/baskets/{userId}/discount-code");
        Version(1);
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddBasketDiscountCodeRequest request, CancellationToken ct)
    {
        var discountCodeResult = await mediator.Send(
            new AddBasketDiscountCodeCommand(
                request.UserId, request.DiscountCode),
            ct);

        await Send.OkAsync(new AddBasketDiscountCodeResponse(
            discountCodeResult.DiscountCode,
            discountCodeResult.DiscountAmountApplied
            ), ct);
    }
}