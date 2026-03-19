namespace MVFC.MongoDbFlow.Tests;

[Collection("Mongo collection")]
public sealed class MongoTransactionTests(MongoDbFixture fixture)
{
    private readonly IMongoUnitOfWorkFactory _uowFactory = fixture.Services.BuildServiceProvider().GetRequiredService<IMongoUnitOfWorkFactory>();
    private readonly IMongoContextFactory _contextFactory = fixture.Services.BuildServiceProvider().GetRequiredService<IMongoContextFactory>();

    [Fact]
    public async Task Should_Rollback_When_Commit_Is_Not_Called()
    {
        var user = MockEntities.MockUser();

        await using (var tx = new MongoTransactionScope(_uowFactory))
        {
            var repo = tx.Uow.GetRepository<User, Guid>();
            await repo.InsertAsync(user, TestContext.Current.CancellationToken);
        }

        var repoAfter = _contextFactory.Create().GetRepository<User, Guid>();
        var loaded = await repoAfter.GetOneAsync(user.Id, TestContext.Current.CancellationToken);

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task Should_Commit_Transaction_And_Persist_Data()
    {
        var user = MockEntities.MockUser();

        await using (var tx = new MongoTransactionScope(_uowFactory))
        {
            var repo = tx.Uow.GetRepository<User, Guid>();
            await repo.InsertAsync(user, TestContext.Current.CancellationToken);
            await tx.CommitAsync();
        }

        var repoAfter = _contextFactory.Create().GetRepository<User, Guid>();
        var loaded = await repoAfter.GetOneAsync(user.Id, TestContext.Current.CancellationToken);

        loaded.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Abort_Transaction_Explicitly()
    {
        var user = MockEntities.MockUser();

        await using (var tx = new MongoTransactionScope(_uowFactory))
        {
            var repo = tx.Uow.GetRepository<User, Guid>();
            await repo.InsertAsync(user, TestContext.Current.CancellationToken);
            await tx.Uow.AbortAsync(TestContext.Current.CancellationToken);
        }

        var repoAfter = _contextFactory.Create().GetRepository<User, Guid>();
        var loaded = await repoAfter.GetOneAsync(user.Id, TestContext.Current.CancellationToken);

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task Should_Use_Same_Session_For_Multiple_Repositories()
    {
        var user = MockEntities.MockUser();
        var order = MockEntities.MockOrder(user.Id);

        await using (var tx = new MongoTransactionScope(_uowFactory))
        {
            var users = tx.Uow.GetRepository<User, Guid>();
            var orders = tx.Uow.GetRepository<Order, Guid>();

            await users.InsertAsync(user, TestContext.Current.CancellationToken);
            await orders.InsertAsync(order, TestContext.Current.CancellationToken);

            await tx.CommitAsync();
        }

        var savedUser = await _contextFactory.Create().GetRepository<User, Guid>().GetOneAsync(user.Id, TestContext.Current.CancellationToken);
        var savedOrder = await _contextFactory.Create().GetRepository<Order, Guid>().GetOneAsync(order.Id, TestContext.Current.CancellationToken);

        savedUser.Should().NotBeNull();
        savedOrder.Should().NotBeNull();
    }
}
