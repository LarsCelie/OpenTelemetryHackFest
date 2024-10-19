using Application.Api;
using Dto;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

var otel = builder.Services.AddOpenTelemetry();

// Add Metrics for ASP.NET Core and our custom metrics and export via OTLP
otel.WithMetrics(metrics =>
{
    // Metrics provider from OpenTelemetry
    metrics.AddAspNetCoreInstrumentation();
    //Our custom metrics
    metrics.AddMeter(Counters.Source.Name);
    // Metrics provides by ASP.NET Core in .NET 8
    metrics.AddMeter("Microsoft.AspNetCore.Hosting");
    metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
});

// Add Tracing for ASP.NET Core and our custom ActivitySource and export via OTLP
otel.WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddSource(Activities.Source.Name);
});

var OtlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
if (OtlpEndpoint != null)
{
    otel.UseOtlpExporter();
}

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Seed some data
var products = new List<Product>
{
    new Product(1, "Laptop", 999.99m),
    new Product(2, "Smartphone", 699.99m),
    new Product(3, "Tablet", 399.99m),
};

var orders = new List<Order>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Get all products
app.MapGet("/api/products", (ILogger<Program> logger) =>
    {
        using var activity = Activities.Source.StartActivity(Activities.GetProducts);
        Counters.ApiCallsCounter.Add(1, new KeyValuePair<string, object>("endpoint", "/api/products"));
        logger.LogInformation("Fetching the list of products.");
        return products;
    })
    .WithName(Activities.GetProducts)
    .WithOpenApi();

// Place an order
app.MapPost("/api/orders", (ILogger<Program> logger, OrderRequest orderRequest) =>
    {
        using var activity = Activities.Source.StartActivity(Activities.PlaceOrder);
        Counters.ApiCallsCounter.Add(1, new KeyValuePair<string, object>("endpoint", "/api/orders"));
        
        logger.LogInformation("Attempting to place an order for product ID {ProductId} by {CustomerName}",
            orderRequest.ProductId, orderRequest.CustomerName);
        
        using (var fetchProductActivity = Activities.Source.StartActivity(Activities.SearchProduct))
        {
            // Simulate processing time for order placement
            Thread.Sleep(Random.Shared.Next(200, 1000));

            var product = products.FirstOrDefault(p => p.Id == orderRequest.ProductId);
            if (product == null)
            {
                logger.LogWarning("Product with ID {ProductId} not found.", orderRequest.ProductId);
                return Results.NotFound($"Product with ID {orderRequest.ProductId} not found.");
            }
        }

        using (var processOrderActivity = Activities.Source.StartActivity(Activities.ProcessOrder))
        {
            Thread.Sleep(Random.Shared.Next(200, 1000));
            var foundProduct = products.First(p => p.Id == orderRequest.ProductId);
            var order = new Order(Guid.NewGuid(), orderRequest.CustomerName, foundProduct, DateTime.Now);
            orders.Add(order);
        }
        
        logger.LogInformation("Order for {CustomerName} placed successfully with ID {OrderId}.",
            orderRequest.CustomerName, orders.Last().Id);

        return Results.Ok(orders.Last());
    })
    .WithName(Activities.PlaceOrder)
    .WithOpenApi();

// Get all orders
app.MapGet("/api/orders", (ILogger<Program> logger) =>
    {
        using var activity = Activities.Source.StartActivity(Activities.GetOrders);
        Counters.ApiCallsCounter.Add(1, new KeyValuePair<string, object>("endpoint", "/api/orders"));
        
        logger.LogInformation("Fetching the list of all orders.");
        
        // Simulate delay in retrieving data
        Thread.Sleep(Random.Shared.Next(100, 500));
        return orders;
    })
    .WithName(Activities.GetOrders)
    .WithOpenApi();

// Get a specific order by ID
app.MapGet("/api/orders/{id}", (ILogger<Program> logger, Guid id) =>
    {
        using var activity = Activities.Source.StartActivity(Activities.GetOrderById);
        Counters.ApiCallsCounter.Add(1, new KeyValuePair<string, object>("endpoint", "/api/orders/{id}"));
        
        logger.LogInformation("Fetching order with ID {OrderId}.", id);
        
        var order = orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            logger.LogWarning("Order with ID {OrderId} not found.", id);
            return Results.NotFound($"Order with ID {id} not found.");
        }

        return Results.Ok(order);
    })
    .WithName(Activities.GetOrderById)
    .WithOpenApi();

app.Run();