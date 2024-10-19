using Dto;

var builder = WebApplication.CreateBuilder(args);

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
        logger.LogInformation("Fetching the list of products.");
        return products;
    })
    .WithName("GetProducts")
    .WithOpenApi();

// Place an order
app.MapPost("/api/orders", (ILogger<Program> logger, OrderRequest orderRequest) =>
    {
        logger.LogInformation("Attempting to place an order for product ID {ProductId} by {CustomerName}",
            orderRequest.ProductId, orderRequest.CustomerName);
    
        // Simulate processing time for order placement
        Thread.Sleep(Random.Shared.Next(200, 1000));
        var product = products.FirstOrDefault(p => p.Id == orderRequest.ProductId);
        if (product == null)
        {
            logger.LogWarning("Product with ID {ProductId} not found.", orderRequest.ProductId);
            return Results.NotFound($"Product with ID {orderRequest.ProductId} not found.");
        }
        
        Thread.Sleep(Random.Shared.Next(200, 1000));
        var order = new Order(Guid.NewGuid(), orderRequest.CustomerName, product, DateTime.Now);
        orders.Add(order);
        
        logger.LogInformation("Order for {CustomerName} placed successfully with ID {OrderId}.",
            orderRequest.CustomerName, order.Id);
        return Results.Ok(order);
    })
    .WithName("PlaceOrder")
    .WithOpenApi();

// Get all orders
app.MapGet("/api/orders", (ILogger<Program> logger) =>
    {
        logger.LogInformation("Fetching the list of all orders.");
        
        // Simulate delay in retrieving data
        Thread.Sleep(Random.Shared.Next(100, 500));
        return orders;
    })
    .WithName("GetOrders")
    .WithOpenApi();

// Get a specific order by ID
app.MapGet("/api/orders/{id}", (ILogger<Program> logger, Guid id) =>
    {
        logger.LogInformation("Fetching order with ID {OrderId}.", id);
        
        var order = orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            logger.LogWarning("Order with ID {OrderId} not found.", id);
            return Results.NotFound($"Order with ID {id} not found.");
        }
        return Results.Ok(order);
    })
    .WithName("GetOrderById")
    .WithOpenApi();

app.Run();