namespace MVFC.MongoDbFlow.Tests.Models;

public sealed record User(
    Guid Id, 
    string Name, 
    DateOnly BirthDate);