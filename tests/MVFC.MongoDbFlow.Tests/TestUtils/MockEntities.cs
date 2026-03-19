namespace MVFC.MongoDbFlow.Tests.TestUtils;

internal static class MockEntities
{
    internal static Guid MockId() =>
        Guid.CreateVersion7(DateTimeOffset.UtcNow);

    internal static User MockUser(string? name = null) =>
        MockUserTemplate(name).Generate();

    internal static IList<User> MockUsers(int count, string? name = null) =>
        MockUserTemplate(name).Generate(count);

    internal static Order MockOrder(Guid? userId = null) =>
        MockOrderTemplate(userId).Generate();

    internal static IList<Order> MockOrders(int count, Guid? userId = null) =>
        MockOrderTemplate(userId).Generate(count);

    private static Faker<Order> MockOrderTemplate(Guid? userId = null) =>
        new Faker<Order>().CustomInstantiator(f =>
        new Order(
            Id: MockId(),
            UserId: userId ?? MockId(),
            Status: f.PickRandom<OrderStatus>(),
            TotalAmount: f.Finance.Amount(10, 1000),
            CreatedAt: DateTimeOffset.UtcNow.Date));

    private static Faker<User> MockUserTemplate(string? name) =>
        new Faker<User>().CustomInstantiator(f =>
        new User(
            Id: MockId(),
            Name: name ?? f.Name.FullName(),
            BirthDate: DateOnly.FromDateTime(f.Date.Past(30, DateTime.Now.AddYears(-18)))));
}