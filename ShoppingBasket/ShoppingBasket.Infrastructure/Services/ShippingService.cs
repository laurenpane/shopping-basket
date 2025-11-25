using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Core.Entities.Application.Interfaces;
using ShoppingBasket.Infrastructure;
using ShoppingBasket.Infrastructure.Persistence;

namespace ShoppingBasket.Infrastructure.Services;

public class ShippingService(AppDbContext db) : IShippingService
{
    public Task<bool> IsValidCountryAsync(string code, CancellationToken ct)
        => db.ShippingCountries.AnyAsync(country => country.Code == code.ToUpper(), ct);
}