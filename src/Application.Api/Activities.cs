namespace Application.Api;

using System.Diagnostics;

public static class Activities
{
    public static ActivitySource Source = new ActivitySource("Ordering.API"); 
    
    public const string GetProducts = "GetProducts";
    public const string PlaceOrder = "PlaceOrder";
    public const string GetOrders = "GetOrders";
    public const string GetOrderById = "GetOrderById";
    public const string SearchProduct = "SearchProduct";
    public const string ProcessOrder = "ProcessOrder";
}