using Dto;
using System.Net.Http.Json;

using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5259/");

Console.WriteLine("Fetching product list...");
var productsResponse = await client.GetAsync("/api/products");
var products = await productsResponse.Content.ReadFromJsonAsync<Product[]>();


Console.WriteLine("Placing an order...");
var orderRequest = new OrderRequest(Random.Shared.Next(1, 4),"John Doe");
var orderResponse = await client.PostAsJsonAsync("/api/orders", orderRequest);
var order = await orderResponse.Content.ReadFromJsonAsync<Order>();
Console.WriteLine($"Order: {order}");

// Fetch all orders
Console.WriteLine("Fetching all orders...");
var ordersResponse = await client.GetAsync("/api/orders");
var orders = await ordersResponse.Content.ReadFromJsonAsync<Order[]>();
Console.WriteLine($"Orders: {orders}");