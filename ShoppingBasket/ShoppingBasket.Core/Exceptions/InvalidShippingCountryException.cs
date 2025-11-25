namespace ShoppingBasket.Core.Exceptions;

public class InvalidShippingCountryException(string code) : Exception($"Invalid shipping country: {code}");
