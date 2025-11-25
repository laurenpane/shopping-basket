using MediatR;

namespace ShoppingBasket.Core.Commands;

public record RemoveBasketItemCommand(
    Guid UserId,
    Guid BasketItemId) : IRequest;