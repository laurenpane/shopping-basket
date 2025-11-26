using MediatR;

namespace ShoppingBasket.Core.Commands;


public record AddBasketItemCommand(
    Guid UserId,
    Guid ItemId,
    int Quantity,
    string ShippingCountryCode) : IRequest<Guid>;