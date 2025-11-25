using ShoppingBasket.Api.Requests;
using ShoppingBasket.Core.Entities.Application.Interfaces;
using FluentValidation;

namespace ShoppingBasket.Api.Validators;

public class AddBasketItemRequestValidator : AbstractValidator<AddBasketItemRequest>
{
    public AddBasketItemRequestValidator(
        IShippingService shippingService,
        IStockService stockService)
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.ShippingCountryCode)
            .NotEmpty()
            .MustAsync(async (code, ct) =>
                await shippingService.IsValidCountryAsync(code, ct))
            .WithMessage("Invalid shipping country.");
    }
}
