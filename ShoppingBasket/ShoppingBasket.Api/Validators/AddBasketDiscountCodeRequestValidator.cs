using FluentValidation;
using ShoppingBasket.Api.Requests;

namespace ShoppingBasket.Api.Validators;

public class AddBasketDiscountCodeRequestValidator : AbstractValidator<AddBasketDiscountCodeRequest> {
    
    public AddBasketDiscountCodeRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        
        // with more time, I would add validation here that the discount code exists in DiscountCodes
        // Like with StockService, I'd make a new service handling DiscountCode logic
    }
}