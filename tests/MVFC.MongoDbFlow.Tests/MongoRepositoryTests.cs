namespace MVFC.MongoDbFlow.Tests;

public sealed class MongoRepositoryTests(MongoDbFixture fixture) : IClassFixture<MongoDbFixture>
{
    private readonly IMongoContextFactory _contextFactory = fixture.Services.BuildServiceProvider().GetRequiredService<IMongoContextFactory>();

    [Fact]
    public async Task Should_insert_and_read_document()
    {
        var context = _contextFactory.Create();
        var repo = context.GetRepository<User, Guid>();

        var user = new User(
            Id: Guid.CreateVersion7(DateTimeOffset.UtcNow),
            Name: "Vinicius",
            BirthDate: new DateOnly(1990, 1, 1));

        await repo.InsertAsync(user);

        var loaded = await repo.GetByIdAsync(user.Id);

        loaded.Should().NotBeNull();
        user.Name.Should().Be(loaded!.Name);
    }

    [Fact]
    public async Task GetById_should_return_null_when_not_found()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();

        var loaded = await repo.GetByIdAsync(Guid.CreateVersion7());

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task Should_find_users_by_name()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();

        await repo.InsertManyAsync(
        [
            new User(Guid.CreateVersion7(), "Alice", new DateOnly(1990,1,1)),
            new User(Guid.CreateVersion7(), "Bob",   new DateOnly(1991,1,1))
        ]);

        var found = await repo.FindAsync(
            Builders<User>.Filter.Eq(u => u.Name, "Alice"));

        found.Should().HaveCount(1);
    }
}