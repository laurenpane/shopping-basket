using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Api.Middleware;
using ShoppingBasket.Core.Commands;
using ShoppingBasket.Core.Entities;
using ShoppingBasket.Core.Entities.Application.Interfaces;
using ShoppingBasket.Infrastructure.Handlers;
using ShoppingBasket.Infrastructure.Persistence;
using ShoppingBasket.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();
// builder.Services.SwaggerDocument(o =>
//     {
//         o.DocumentSettings = s =>
//         {
//             s.Version = "v1";
//             s.Title = "Shopping Basket API";
//         };
//     });

// to improve:
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<AddItemToBasketCommand>();
    cfg.RegisterServicesFromAssemblyContaining<AddItemToBasketHandler>();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("BasketDb"));

builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IStockService, StockService>();

var app = builder.Build();

// app.UseFastEndpoints(c =>
// {
//     c.Versioning.Prefix = "v";
// });
app.UseMiddleware<ExceptionMiddleware>();
app.UseFastEndpoints();
app.UseSwaggerGen();
SeedData(app);

app.Run();

static void SeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    db.Database.EnsureCreated();
    if (!db.Items.Any())
    {
        var dress = new Item("Blue Dress", 49.99m, id: Guid.Parse("77a3c917-8f3e-4834-ac8c-2edbe8878d48"));
        var jeans = new Item("Jeans", 59.99m);
        var shoes = new Item("Shoes", 89.99m);

        db.Items.AddRange(dress, jeans, shoes);
        db.SaveChanges();

        db.Stock.AddRange(
            new Stock(dress.Id, 10),
            new Stock(jeans.Id, 5),
            new Stock(shoes.Id, 8)
        );

        db.SaveChanges();
    }

    if (!db.ShippingCountries.Any())
    {
        db.ShippingCountries.AddRange(
            new ShippingCountry("UK", 4.99m),
            new ShippingCountry("US", 9.99m),
            new ShippingCountry("DE", 6.49m)
        );

        db.SaveChanges();
    }
}
