namespace MVFC.MongoDbFlow.Tests.TestUtils;

internal static class MockEntities
{
    internal static Guid MockId() =>
        Guid.CreateVersion7(DateTimeOffset.UtcNow);

    internal static User MockUser(string? name = null) =>
        MockUserTemplate(name).Generate();

    internal static IList<User> MockUsers(int count, string? name = null) =>
        MockUserTemplate(name).Generate(count);

    private static Faker<User> MockUserTemplate(string? name) =>
        new Faker<User>().CustomInstantiator(f =>
        new User(
            Id: MockId(),
            Name: name ?? f.Name.FullName(),
            BirthDate: DateOnly.FromDateTime(f.Date.Past(30, DateTime.Now.AddYears(-18)))));
}