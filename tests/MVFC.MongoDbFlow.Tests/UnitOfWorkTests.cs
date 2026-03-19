namespace MVFC.MongoDbFlow.Tests;

public enum BonusEnum { One }

[Collection("Mongo collection")]
public sealed class UnitOfWorkTests(MongoDbFixture fixture)
{
    private readonly IMongoUnitOfWorkFactory _uowFactory = fixture.Services.BuildServiceProvider().GetRequiredService<IMongoUnitOfWorkFactory>();

    [Fact]
    public async Task UnitOfWork_Commit_Then_Noop_Commit_And_Abort()
    {
        var uow = (MongoUnitOfWork)_uowFactory.Create();
        uow.IsActive.Should().BeTrue();

        await uow.CommitAsync(TestContext.Current.CancellationToken);
        uow.IsActive.Should().BeFalse();

        // After committed, both Commit and Abort should be no-ops
        await uow.CommitAsync(TestContext.Current.CancellationToken);
        await uow.AbortAsync(TestContext.Current.CancellationToken);

        // DisposeAsync when state != Active
        await uow.DisposeAsync();
    }

    [Fact]
    public async Task UnitOfWork_Abort_Should_Deactivate()
    {
        var uow = (MongoUnitOfWork)_uowFactory.Create();

        await uow.AbortAsync(TestContext.Current.CancellationToken);

        uow.IsActive.Should().BeFalse();
        await uow.DisposeAsync();
    }

    [Fact]
    public void CollectionNameResolver_Should_Throw_When_No_Map()
    {
        var resolver = new CollectionNameResolver([]);
        var act = () => resolver.Resolve<BonusEnum>();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No collection mapping registered for BonusEnum");
    }

    [Fact]
    public void EntityMap_Should_Handle_Already_Registered()
    {
        var map = new UserMap();
        map.Register();
        map.Register(); // second call hits the "already registered" return

        map.EntityType.Should().Be(typeof(User));
    }

    [Fact]
    public void MongoBootstrap_Should_Handle_Nulls()
    {
        var options = new MongoOptions("mongodb://localhost:27017", "db");
        var bootstrap = new MVFC.MongoDbFlow.Bootstrap.MongoBootstrap(options, null, null);

        bootstrap.Client.Should().NotBeNull();
    }

    [Fact]
    public async Task Repository_With_Session_Should_Cover_All_Branches()
    {
        await using var uow = _uowFactory.Create();
        var repo = uow.GetRepository<User, Guid>();
        var user = MockEntities.MockUser();

        // Insert
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);
        await repo.InsertManyAsync([MockEntities.MockUser()], TestContext.Current.CancellationToken);

        // Read
        (await repo.GetOneAsync(user.Id, TestContext.Current.CancellationToken)).Should().NotBeNull();
        await repo.FindAsync(Builders<User>.Filter.Empty, ct: TestContext.Current.CancellationToken);
        await repo.FindAsync(
            Builders<User>.Filter.Empty,
            Builders<User>.Projection.Expression(x => x.Name),
            ct: TestContext.Current.CancellationToken);
        await repo.ExistsAsync(Builders<User>.Filter.Empty, TestContext.Current.CancellationToken);
        await repo.DistinctAsync(
            new ExpressionFieldDefinition<User, string>(x => x.Name),
            ct: TestContext.Current.CancellationToken);
        await repo.CountAsync(Builders<User>.Filter.Empty, TestContext.Current.CancellationToken);
        await repo.FindPagedAsync(Builders<User>.Filter.Empty, 1, 10, ct: TestContext.Current.CancellationToken);

        // Update
        await repo.UpdateAsync(user.Id, Builders<User>.Update.Set(x => x.Name, "U"), TestContext.Current.CancellationToken);
        await repo.UpdateManyAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            Builders<User>.Update.Set(x => x.Name, "UM"),
            TestContext.Current.CancellationToken);
        await repo.UpdateFieldsAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            Builders<User>.Update.Set(x => x.Name, "UF"),
            TestContext.Current.CancellationToken);
        await ((MongoRepository<User, Guid>)repo).UpdateFieldsAsync(
            user.Id,
            Builders<User>.Update.Set(x => x.Name, "UF2"),
            TestContext.Current.CancellationToken);

        // Replace
        await repo.ReplaceAsync(user with { Name = "R1" }, user.Id, TestContext.Current.CancellationToken);
        await repo.ReplaceAsync(
            user with { Name = "R2" },
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            TestContext.Current.CancellationToken);

        // FindOneAnd
        await repo.FindOneAndUpdateAsync(
            user.Id,
            Builders<User>.Update.Set(x => x.Name, "FOUA"),
            ct: TestContext.Current.CancellationToken);
        await repo.FindOneAndDeleteAsync(user.Id, ct: TestContext.Current.CancellationToken);

        // Delete
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);
        await repo.DeleteAsync(user.Id, TestContext.Current.CancellationToken);
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);
        await repo.DeleteManyAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            TestContext.Current.CancellationToken);

        // SoftDelete / Restore
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);
        await repo.SoftDeleteAsync(user.Id, ct: TestContext.Current.CancellationToken);
        await repo.RestoreAsync(user.Id, ct: TestContext.Current.CancellationToken);
        await repo.SoftDeleteAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            ct: TestContext.Current.CancellationToken);
        await repo.RestoreAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            ct: TestContext.Current.CancellationToken);

        // Bulk
        await repo.BulkWriteAsync(
            [new InsertOneModel<User>(MockEntities.MockUser())],
            TestContext.Current.CancellationToken);

        await uow.CommitAsync(TestContext.Current.CancellationToken);
    }
}
