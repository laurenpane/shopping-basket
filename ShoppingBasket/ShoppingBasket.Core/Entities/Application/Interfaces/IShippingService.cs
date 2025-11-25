namespace ShoppingBasket.Core.Entities.Application.Interfaces;

public interface IShippingService
{
    Task<bool> IsValidCountryAsync(string code, CancellationToken ct);
}