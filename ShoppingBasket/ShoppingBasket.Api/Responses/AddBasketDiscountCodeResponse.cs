namespace ShoppingBasket.Api.Responses;

public record AddBasketDiscountCodeResponse(
    string DiscountCode,
    decimal DiscountAmount
);
// It could have been useful to also provide a basket total including discount here in this response.
// However, given there is VAT logic that applies to the total basket that would need to be considered,
// I opted to deprioritise this given the time frame, and keep the final total logic within the GetBasketSummary response.