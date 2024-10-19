using Dto;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http.Json;

// Define service name and version
var serviceName = "Application.ConsoleApp";
var serviceVersion = "1.0.0";

// Set up an ActivitySource for traces
using var activitySource = new ActivitySource(serviceName);

// Set up a logger factory with OpenTelemetry integration
await using var serviceProvider = new ServiceCollection()
    .AddLogging(logging =>
    {
        logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion));
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;
            options.AddOtlpExporter(otelOptions =>
            {
                otelOptions.Endpoint = new Uri("http://localhost:4317");
            });
        });
    })
    .BuildServiceProvider();

// Get the ILogger instance
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

// Configure the OpenTelemetry TracerProvider
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddSource(activitySource.Name)
    .AddHttpClientInstrumentation()
    .AddOtlpExporter(otelOptions =>
    {
        otelOptions.Endpoint = new Uri("http://localhost:4317");
    })
    .Build();

using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5259/");

// Fetch the product list with tracing and logging
using (var activity = activitySource.StartActivity("GetProducts"))
{
    logger.LogInformation("Fetching product list...");
    var productsResponse = await client.GetAsync("/api/products");
    var products = await productsResponse.Content.ReadFromJsonAsync<Product[]>();
    logger.LogInformation("Fetched {ProductCount} products.", products?.Length ?? 0);
}

// Place an order with tracing and logging
using (var activity = activitySource.StartActivity("PlaceOrder"))
{
    var orderRequest = new OrderRequest(Random.Shared.Next(1, 4), "John Doe");
    logger.LogInformation("Placing an order for product ID {ProductId} by {CustomerName}", orderRequest.ProductId, orderRequest.CustomerName);
    var orderResponse = await client.PostAsJsonAsync("/api/orders", orderRequest);
    var order = await orderResponse.Content.ReadFromJsonAsync<Order>();
    logger.LogInformation("Order placed: {Order}", order);
}

// Fetch all orders with tracing and logging
using (var activity = activitySource.StartActivity("GetOrders"))
{
    logger.LogInformation("Fetching all orders...");
    var ordersResponse = await client.GetAsync("/api/orders");
    var orders = await ordersResponse.Content.ReadFromJsonAsync<Order[]>();
    logger.LogInformation("Fetched {OrderCount} orders.", orders?.Length ?? 0);
}
