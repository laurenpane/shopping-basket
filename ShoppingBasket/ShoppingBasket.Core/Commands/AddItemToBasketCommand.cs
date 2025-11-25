using MediatR;

namespace ShoppingBasket.Core.Commands;


public record AddItemToBasketCommand(
    string UserId,
    Guid ItemId,
    int Quantity,
    string ShippingCountryCode) : IRequest<Guid>;