using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api;

public static class StatusInfo
{
    public static ProblemDetails InvalidCartProductId(int id) => new ProblemDetails
    {
        Title = "Invalid cartProduct ID",
        Detail = $"CartProduct with ID '{id}' does not exist"
    };

    public static ProblemDetails InvalidCartId(int id) => new ProblemDetails
    {
        Title = "Invalid cart ID",
        Detail = $"Cart with ID '{id}' does not exist"
    };

    public static ProblemDetails InvalidCustomerId(int id) => new ProblemDetails
    {
        Title = "Invalid customer ID",
        Detail = $"Customer with ID '{id}' does not exist"
    };

    public static ProblemDetails InvalidCustomerEmail(string email) => new ProblemDetails
    {
        Title = "Invalid customer email",
        Detail = $"Customer with email '{email}' does not exist"
    };

    public static ProblemDetails InvalidProductId(int id) => new ProblemDetails
    {
        Title = "Invalid product ID",
        Detail = $"Product with ID '{id}' does not exist"
    };

    public static ProblemDetails InvalidOrderId(int id) => new ProblemDetails
    {
        Title = "Invalid order ID",
        Detail = $"Order with ID '{id}' does not exist"
    };

    public static ProblemDetails InvalidTenantId(int id) => new ProblemDetails
    {
        Title = "Invalid tenant ID",
        Detail = $"Tenant with ID '{id}' does not exist"
    };

    public static ProblemDetails CartIdProcessed(int id) => new ProblemDetails
    {
        Title = "Cart ID already processed",
        Detail = $"Cart with ID '{id}' has already been processed. Calculating and making changes is not allowed anymore"
    };

    public static ProblemDetails InvalidDiscountRuleId(int id) => new ProblemDetails
    {
        Title = "Invalid tenant DiscountRule ID",
        Detail = $"DiscountRule with ID '{id}' does not exist"
    };

    public static ProblemDetails InvalidDiscountActionId(int id) => new ProblemDetails
    {
        Title = "Invalid DiscountAction ID",
        Detail = $"DiscountAction with ID '{id}' does not exist"
    };

    public static ProblemDetails AddToCartNotPossible(int cartId, int productId) => new ProblemDetails
    {
        Title = "Add to cart not possible",
        Detail = $"Cannot add product with ID '{productId}' to cart with ID '{cartId}'"
    };

    public static ProblemDetails DiscountActionDeleteNotPossible(int actionId) => new ProblemDetails
    {
        Title = "DiscountAction cannot get deleted",
        Detail = $"DiscountAction with ID '{actionId}' cannot get deleted. Associated Rules exist"
    };

    public static ProblemDetails DiscountActionInvalidAssignment(int actionId) => new ProblemDetails
    {
        Title = "DiscountAction cannot get assigned to DiscountRule",
        Detail = $"DiscountAction with ID '{actionId}' cannot get assigned to DiscountRule"
    };
}
