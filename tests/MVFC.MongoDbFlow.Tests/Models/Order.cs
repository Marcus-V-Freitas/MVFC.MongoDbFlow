namespace MVFC.MongoDbFlow.Tests.Models;

public sealed record Order(
    Guid Id, 
    Guid UserId,
    OrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAt);