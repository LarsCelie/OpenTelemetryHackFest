namespace Dto;

public record Order(Guid Id, string CustomerName, Product Product, DateTime OrderDate);