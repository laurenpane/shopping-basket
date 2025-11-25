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
    cfg.RegisterServicesFromAssemblyContaining<AddBasketItemCommand>();
    cfg.RegisterServicesFromAssemblyContaining<AddBasketItemHandler>();
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
    // Database seeding is done here in Program.cs as it was the simplest way within time constraints using InMemory.
    // For more advanced local development, had I used a database (e.g. Postgres), I would have added a script to create and seed a local instance via Docker
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    db.Database.EnsureCreated();
    if (!db.Items.Any())
    {
        // hard coded Guid here for manual testing purposes under time constraint (a valid item id is required for AddBasketItem).
        // Ideally there would be a filtered GET all endpoint called by the API user for the Items, which would then be passed into this request
        // when a BasketItem was added to a Basket.
        var dress = new Item("Dress", 49.99m, id: Guid.Parse("77a3c917-8f3e-4834-ac8c-2edbe8878d48"));
        var jeans = new Item("Low-Waisted Jeans", 59.99m, salePrice: 40, id: Guid.Parse("0d6fbf40-ed5e-4fdb-a6e3-15e5e1c94fa1"));
        var shoes = new Item("Leather Jacket", 89.99m);

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
        db.ShippingCountries.AddRange(
        new ShippingCountry("UK", 1.99m),
        new ShippingCountry("US", 5.99m),
        new ShippingCountry("AU", 7.49m)
    );
    
    if (!db.DiscountCodes.Any())
        db.DiscountCodes.Add(
        new DiscountCode("BLACKFRIDAY25", 0.25m));

    db.SaveChanges();
}
