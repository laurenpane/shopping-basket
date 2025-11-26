using Microsoft.EntityFrameworkCore;
using Moq;
using ShoppingBasket.Core.Commands;
using ShoppingBasket.Core.Entities;
using ShoppingBasket.Core.Entities.Application.Interfaces;
using ShoppingBasket.Core.Exceptions;
using ShoppingBasket.Infrastructure.Handlers;
using ShoppingBasket.Infrastructure.Persistence;
using Xunit;

namespace ShoppingBasket.UnitTests.Handlers;

public class AddBasketItemHandlerTests : IDisposable
{
    private readonly Mock<IShippingService> _mockShippingService;
    private readonly Mock<IStockService> _mockStockService;
    private readonly AppDbContext _dbContext;
    private readonly AddBasketItemHandler _handler;

    public AddBasketItemHandlerTests()
    {
        _mockShippingService = new Mock<IShippingService>();
        _mockStockService = new Mock<IStockService>();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        
        _handler = new AddBasketItemHandler(_dbContext, _mockShippingService.Object, _mockStockService.Object);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
    
    [Fact]
    public async Task Handle_ItemDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new AddBasketItemCommand(
            UserId: Guid.NewGuid(),
            ItemId: Guid.NewGuid(),
            Quantity: 1,
            ShippingCountryCode: "GB"
        );

        _mockShippingService.Setup(s => s.IsValidCountryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidShippingCountryCode_ThrowsInvalidShippingCountryException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item { Id = itemId, Name = "Test Item", RegularPrice = 50.00m };
        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        var command = new AddBasketItemCommand(
            UserId: Guid.NewGuid(),
            ItemId: itemId,
            Quantity: 1,
            ShippingCountryCode: "INVALID"
        );

        _mockShippingService.Setup(s => s.IsValidCountryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidShippingCountryException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InsufficientStock_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item { Id = itemId, Name = "Test Item", RegularPrice = 10.00m };
        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        var command = new AddBasketItemCommand(
            UserId: Guid.NewGuid(),
            ItemId: itemId,
            Quantity: 1,
            ShippingCountryCode: "GB"
        );

        _mockShippingService.Setup(s => s.IsValidCountryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _mockStockService.Setup(s => s.ReserveStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException($"Insufficient stock for item {itemId}"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_BasketDoesNotExist_CreatesNewBasketAndAddsItem()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var item = new Item { Id = itemId, Name = "Test Item", RegularPrice = 10.00m };
        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        var command = new AddBasketItemCommand(
            UserId: userId,
            ItemId: itemId,
            Quantity: 2,
            ShippingCountryCode: "GB"
        );

        _mockShippingService.Setup(s => s.IsValidCountryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _mockStockService.Setup(s => s.ReserveStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Stock(itemId, 19));

        // Act
        var basketId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, basketId);
        
        var basket = await _dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == basketId);
        
        Assert.NotNull(basket);
        Assert.Equal(userId, basket.UserId);
        Assert.Equal("GB", basket.ShippingCountryCode);
        Assert.Single(basket.Items);
        Assert.Equal(itemId, basket.Items[0].ItemId);
        Assert.Equal(2, basket.Items[0].Quantity);
        Assert.Equal(10.00m, basket.Items[0].RegularPrice);
        
        _mockStockService.Verify(s => s.ReserveStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BasketExists_AddsNewItemToExistingBasket()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingItemId = Guid.NewGuid();
        var newItemId = Guid.NewGuid();
        
        var existingItem = new Item { Id = existingItemId, Name = "Existing Item", RegularPrice = 5.00m };
        var newItem = new Item { Id = newItemId, Name = "New Item", RegularPrice = 15.00m };
        _dbContext.Items.AddRange(existingItem, newItem);
        
        var basket = new Basket(userId, "GB");
        basket.AddBasketItem(existingItemId, 1, 5.00m, null);
        _dbContext.Baskets.Add(basket);
        await _dbContext.SaveChangesAsync();

        var command = new AddBasketItemCommand(
            UserId: userId,
            ItemId: newItemId,
            Quantity: 3,
            ShippingCountryCode: "GB"
        );

        _mockShippingService.Setup(s => s.IsValidCountryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _mockStockService.Setup(s => s.ReserveStockAsync(newItemId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Stock(newItemId, 10));

        // Act
        var basketId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(basket.Id, basketId);
        
        var updatedBasket = await _dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == basketId);
        
        Assert.NotNull(updatedBasket);
        Assert.Equal(2, updatedBasket.Items.Count);
        Assert.Contains(updatedBasket.Items, i => i.ItemId == newItemId && i.Quantity == 3);
    }

    [Fact]
    public async Task Handle_ItemAlreadyInBasket_IncreasesQuantity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        
        var item = new Item { Id = itemId, Name = "Test Item", RegularPrice = 10.00m };
        await _dbContext.Items.AddAsync(item);
        
        var basket = new Basket(userId, "GB");
        basket.AddBasketItem(itemId, 2, 10.00m, null);
        await _dbContext.Baskets.AddAsync(basket);
        await _dbContext.SaveChangesAsync();

        var command = new AddBasketItemCommand(
            UserId: userId,
            ItemId: itemId,
            Quantity: 3,
            ShippingCountryCode: "GB"
        );

        _mockShippingService.Setup(s => s.IsValidCountryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _mockStockService.Setup(s => s.ReserveStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Stock(itemId, 1));

        // Act
        var basketId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedBasket = await _dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == basketId);
        
        Assert.NotNull(updatedBasket);
        Assert.Single(updatedBasket.Items);
        Assert.Equal(5, updatedBasket.Items[0].Quantity);
    }

    [Fact]
    public async Task Handle_ItemHasSalePrice_StoresBothPrices()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var item = new Item { Id = itemId, Name = "Sale Item", RegularPrice = 20.00m, SalePrice = 15.00m };
        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        var command = new AddBasketItemCommand(
            UserId: userId,
            ItemId: itemId,
            Quantity: 1,
            ShippingCountryCode: "GB"
        );

        _mockShippingService.Setup(s => s.IsValidCountryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _mockStockService.Setup(s => s.ReserveStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Stock(itemId, 15));

        // Act
        var basketId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var basket = await _dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == basketId);
        
        Assert.NotNull(basket);
        var basketItem = basket.Items[0];
        Assert.Equal(20.00m, basketItem.RegularPrice);
        Assert.Equal(15.00m, basketItem.SalePrice);
    }

    [Theory]
    [InlineData("GB", "GB")]
    [InlineData("us", "US")]
    public async Task Handle_NormalizesCountryCodeToUppercase(string inputCode, string expectedCode)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var item = new Item { Id = itemId, Name = "Test Item", RegularPrice = 10.00m };
        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        var command = new AddBasketItemCommand(
            UserId: userId,
            ItemId: itemId,
            Quantity: 1,
            ShippingCountryCode: inputCode
        );

        _mockShippingService.Setup(s => s.IsValidCountryAsync(inputCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _mockStockService.Setup(s => s.ReserveStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Stock(itemId, 15));

        // Act
        var basketId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var basket = await _dbContext.Baskets.FindAsync(basketId);
        Assert.Equal(expectedCode, basket!.ShippingCountryCode);
    }
}