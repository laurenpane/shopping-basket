using MediatR;
using ShoppingBasket.Core.Dtos;

namespace ShoppingBasket.Core.Queries;

public record GetBasketSummaryQuery
    (Guid UserId, int PageNumber, int PageSize) : IRequest<BasketSummaryDto>;