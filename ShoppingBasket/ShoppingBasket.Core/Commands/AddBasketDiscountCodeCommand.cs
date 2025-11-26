using MediatR;
using ShoppingBasket.Core.Dtos;

namespace ShoppingBasket.Core.Commands;

public record AddBasketDiscountCodeCommand(
    Guid UserId, string DiscountCode) : IRequest<DiscountCodeResultDto>;