using System.Net;
using System.Text.Json;
using ShoppingBasket.Core.Exceptions;

namespace ShoppingBasket.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidShippingCountryException ex)
        {
            _logger.LogWarning(ex, "Invalid shipping country: {Code}", ex.Message);
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (InsufficientStockException ex)
        {
            _logger.LogWarning(ex, "Insufficient stock: {ItemId}", ex.Message);
            await WriteError(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteError(context, HttpStatusCode.InternalServerError, "Something went wrong.");
        }
    }

    private static Task WriteError(HttpContext context, HttpStatusCode code, string message)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";

        var response = JsonSerializer.Serialize(new { error = message });

        return context.Response.WriteAsync(response);
    }
}