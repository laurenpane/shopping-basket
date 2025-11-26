# Shopping Basket API

A .NET shopping basket API built with clean architecture, CQRS, and DDD patterns

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Running the Application

```bash
cd ShoppingBasket.Api
dotnet run
```

Access Swagger UI at: `https://localhost:5001/swagger`

The application uses an in-memory database that auto-seeds with sample items, stock, shipping countries, and discount codes, which resets on each application restart.

### Running Tests

```bash
# Unit tests
dotnet test ShoppingBasket.UnitTests

# Integration tests (please see comment in class within this project)
dotnet test ShoppingBasket.IntegrationTests

# All tests
dotnet test
```

---

## API Endpoints

### Implemented Endpoints and Decision Reasoning

| Endpoint                                   | Method | Description                                                                                           |
|--------------------------------------------|--------|-------------------------------------------------------------------------------------------------------|
| `/v1/baskets/{userId}/items`            | POST | Add item to basket with stock validation                                                              |
| `/v1/baskets/{userId}/items/{itemId}`   | DELETE | Remove item from basket                                                                               |
| `/v1/baskets/{userId}/discount-code`    | POST | Apply discount code (only to full-priced items)                                                       |
| `/v1/baskets/{userId}/summary`          | GET | Paginated basket summary with calculated totals (includes VAT logic which was a requested feature) |

#### Why?
These endpoints cover the core shopping basket features requested and demonstrate:
- POST, GET and DELETE operations
- Various calculations/logic outlined in requirements (largely in GetBasketSummary) such as discounts with constraints, and VAT
- Stock management (see comment within StockService explaining the decision made)
- Domain validation (shipping countries, duplicate discount codes)

The paths follow REST conventions: `/baskets/{userId}` as the resource base, with sub-resources for items and applying a discount code

---

## Architecture Overview

### Clean Architecture + DDD

The solution uses Clean Architecture with three layers:

```
ShoppingBasket.Api
ShoppingBasket.Infrastructure
ShoppingBasket.Core
```

Key principles:
- Core contains pure business logic: entities, commands/queries, interfaces, domain exceptions, and DTOs (BasketSummaryDto, BasketItemDto). DTOs live here in this implementation because they're returned by Infrastructure handlers, avoiding a dependency on the Api layer
- Infrastructure contains handlers, services, and EF Core DbContext - handlers return Core DTOs
- Api contains endpoints, request DTOs, and validation - endpoints map between requests and Core contracts

Dependencies flow inward - Core has no references to outer layers, making it framework-agnostic and testable

### CQRS Pattern

Uses MediatR to implement segregation between commands and queries
- Commands modify state: `AddBasketItemCommand`, `RemoveBasketItemCommand`, `AddBasketDiscountCodeCommand`
- Queries read state: `GetBasketSummaryQuery`
- Handlers orchestrate operations in the Infrastructure layer

Benefits:
- Clear separation between reads and writes
- Single responsibility per handler
- Scales naturally - commands and queries can use different data models

### Domain-Driven Design

Aggregate Root: `Basket` entity encapsulates all basket business rules:
- Adding/removing items
- Applying discount codes
- Calculating subtotals

Domain Services: `IShippingService`, `IStockService` handle cross-entity logic

Domain Exceptions: `InsufficientStockException`, `InvalidShippingCountryException` represent business rule violations,
with built-in .NET exceptions such as `KeyNotFoundException` used where possible.

This way, business logic lives in the domain, and is not scattered across services or controllers.
---

## Technology Choices

### Why FastEndpoints?

I chose FastEndpoints for cleaner vertical slices, built-in validation support, and simpler versioning compared to traditional MVC controllers.

---

## Testing Strategy

### Why Only Unit Test One Endpoint?

Given time constraints, I opted to fully unit test the `AddBasketItemHandler` (9 tests) instead of lightly covering all tests. This way I could demonstrate:
- mocking dependencies (DbContext, domain services)
- testing business rule violations (stock, shipping, item existence)
- testing happy paths (new basket, existing basket, quantity increment)
- testing edge cases (sale prices, country code normalisation)
---

## Future Extensions

### Authentication & Authorization

This was omitted for this tech test to focus on architecture and business logic, but would be a priority for production.
I'd opt to use secure token-based auth like OAuth 2.0 with a managed identity provider, requiring a JWT bearer token for authentication. I'd then enforce role-based access control for user actions. Currently, a temporary UserId is generated when a basket is created, but I would opt to have a Users table and proper authentication/authorization flow with more time.

---

### FluentValidation (Full Implementation)

Current state: Partial implementation - validators exist (`AddItemRequestValidator` was created to demonstrate a good use case for FluentValidation) but need proper wiring. Although validation exists in other layers, validating API requests at this point is good practice to avoid unnecessary downstream processing and DB calls.

---

### API Versioning

All endpoints are versioned with `/v1/` prefix using FastEndpoints' built-in versioning. Future versions can be added by creating new endpoint classes with `Version(2)`.

---

## Event-Driven Architecture & Microservices

The CQRS structure with MediatR handlers extends well to event-driven systems. Handlers could publish domain events (e.g., `OrderCreated`, `ItemOutOfStock`) to a message bus, allowing separate microservices to react independently - email notifications, inventory updates, shipping coordination, etc.

## Prioritisation Decisions

Given time constraints, I prioritised:

1. Establishing a clean architecture foundation that makes adding future features easier
2. Fewer, quality endpoints over quantity, with each demonstrating production patterns
3. Testability with proper dependency injection and interfaces
4. Keeping domain rules (discount logic, stock validation) in entities

I deprioritised:
- Implementing all features
- Auth, which would have taken a significant amount of time without demonstrating good design/patterns
- Real database (in-memory is sufficient for demonstration)
- 100% test coverage (focused on one fully-tested component)
---

## Key Technologies

- .NET 8
- FastEndpoints
- MediatR for CQRS pattern implementation
- Entity Framework Core
- FluentValidation for request validation
- xUnit/Moq for testing and mocking
---
