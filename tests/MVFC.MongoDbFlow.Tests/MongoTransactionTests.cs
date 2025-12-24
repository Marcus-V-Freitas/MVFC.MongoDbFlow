namespace MVFC.MongoDbFlow.Tests;

public sealed class MongoTransactionTests(MongoDbFixture fixture) : IClassFixture<MongoDbFixture>
{
    private readonly IMongoUnitOfWorkFactory _uowFactory = fixture.Services.BuildServiceProvider().GetRequiredService<IMongoUnitOfWorkFactory>();
    private readonly IMongoContextFactory _contextFactory = fixture.Services.BuildServiceProvider().GetRequiredService<IMongoContextFactory>();

    [Fact]
    public async Task Should_rollback_when_commit_is_not_called()
    {
        var userId = Guid.CreateVersion7(DateTimeOffset.UtcNow);

        await using (var tx = new MongoTransactionScope(_uowFactory))
        {
            var repo = tx.Uow.GetRepository<User, Guid>();

            await repo.InsertAsync(new User(
                Id: userId,
                Name: "Rollback Test",
                BirthDate: new DateOnly(2000, 1, 1)));
        }

        var context = _contextFactory.Create();
        var repoAfter = context.GetRepository<User, Guid>();

        var loaded = await repoAfter.GetByIdAsync(userId);

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task Should_commit_transaction_and_persist_data()
    {
        var id = Guid.CreateVersion7(DateTimeOffset.UtcNow);

        await using (var tx = new MongoTransactionScope(_uowFactory))
        {
            var repo = tx.Uow.GetRepository<User, Guid>();

            await repo.InsertAsync(new User(
                Id: id,
                Name: "Commit Test",
                BirthDate: new DateOnly(1995, 5, 5)));

            await tx.CommitAsync();
        }

        var context = _contextFactory.Create();
        var repoAfter = context.GetRepository<User, Guid>();

        var loaded = await repoAfter.GetByIdAsync(id);

        loaded.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_abort_transaction_explicitly()
    {
        var id = Guid.CreateVersion7(DateTimeOffset.UtcNow);

        await using (var tx = new MongoTransactionScope(_uowFactory))
        {
            var repo = tx.Uow.GetRepository<User, Guid>();

            await repo.InsertAsync(new User(
                Id: id,
                Name: "Abort",
                BirthDate: new DateOnly(1990, 1, 2)));

            await tx.Uow.AbortAsync();
        }

        var repoAfter = _contextFactory.Create().GetRepository<User, Guid>();

        var loaded = await repoAfter.GetByIdAsync(id);

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task Should_use_same_session_for_multiple_repositories()
    {
        var userId = Guid.CreateVersion7(DateTimeOffset.UtcNow);
        var orderId = Guid.CreateVersion7(DateTimeOffset.UtcNow);

        await using (var tx = new MongoTransactionScope(_uowFactory))
        {
            var users = tx.Uow.GetRepository<User, Guid>();
            var orders = tx.Uow.GetRepository<Order, Guid>();

            await users.InsertAsync(new User(
                Id: userId, 
                Name: "User",
                BirthDate: new DateOnly(2020, 03, 01)));


            await orders.InsertAsync(new Order(
                Id: orderId,
                UserId: userId,
                Status: OrderStatus.Created,
                TotalAmount: 1000,
                CreatedAt: DateTimeOffset.UtcNow.Date));

            await tx.CommitAsync();
        }

        var user = await _contextFactory.Create().GetRepository<User, Guid>().GetByIdAsync(userId);
        var order = await _contextFactory.Create().GetRepository<Order, Guid>().GetByIdAsync(orderId);

        user.Should().NotBeNull();
        order.Should().NotBeNull();
    }
}