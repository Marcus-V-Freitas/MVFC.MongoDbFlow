namespace MVFC.MongoDbFlow.Tests;

[Collection("Mongo collection")]
public sealed class MongoRepositoryTests(MongoDbFixture fixture)
{
    private readonly IMongoContextFactory _contextFactory = fixture.Services.BuildServiceProvider().GetRequiredService<IMongoContextFactory>();

    [Fact]
    public async Task GetById_Should_Return_Null_When_Not_Found()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();

        var loaded = await repo.GetOneAsync(MockEntities.MockId(), TestContext.Current.CancellationToken);

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task Should_Insert_Find_And_Project()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var users = MockEntities.MockUsers(2);

        await repo.InsertManyAsync(users, TestContext.Current.CancellationToken);

        var found = await repo.FindAsync(
            Builders<User>.Filter.Eq(u => u.Name, users[0].Name),
            ct: TestContext.Current.CancellationToken);
        found.Should().HaveCount(1);

        var names = await repo.FindAsync(
            Builders<User>.Filter.Eq(x => x.Id, users[0].Id),
            Builders<User>.Projection.Expression(u => u.Name),
            ct: TestContext.Current.CancellationToken);
        names.Should().Contain(users[0].Name);
    }

    [Fact]
    public async Task Should_Update_And_Return_True()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);

        var updated = await repo.UpdateAsync(
            user.Id,
            Builders<User>.Update.Set(x => x.Name, "Atualizado"),
            TestContext.Current.CancellationToken);

        updated.Should().BeTrue();
        var loaded = await repo.GetOneAsync(user.Id, TestContext.Current.CancellationToken);
        loaded!.Name.Should().Be("Atualizado");
    }

    [Fact]
    public async Task Should_Delete_And_Return_False_When_Nonexistent()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);

        (await repo.DeleteAsync(user.Id, TestContext.Current.CancellationToken)).Should().BeTrue();
        (await repo.DeleteAsync(user.Id, TestContext.Current.CancellationToken)).Should().BeFalse();
    }

    [Fact]
    public async Task Should_Check_Exists_True_And_False()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);

        (await repo.ExistsAsync(Builders<User>.Filter.Eq(x => x.Name, user.Name), TestContext.Current.CancellationToken)).Should().BeTrue();
        (await repo.ExistsAsync(Builders<User>.Filter.Eq(x => x.Name, "RANDOM"), TestContext.Current.CancellationToken)).Should().BeFalse();
    }

    [Fact]
    public async Task Should_Update_Many()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var users = MockEntities.MockUsers(2, "Atualizar");
        await repo.InsertManyAsync(users, TestContext.Current.CancellationToken);

        var updatedCount = await repo.UpdateManyAsync(
            Builders<User>.Filter.Eq(x => x.Name, "Atualizar"),
            Builders<User>.Update.Set(x => x.Name, "Atualizado"),
            TestContext.Current.CancellationToken);

        updatedCount.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task Should_Replace_By_Id_And_By_Filter()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser("Substituir");
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);

        await repo.ReplaceAsync(user with { Name = "PorId" }, user.Id, TestContext.Current.CancellationToken);
        (await repo.GetOneAsync(user.Id, TestContext.Current.CancellationToken))!.Name.Should().Be("PorId");

        await repo.ReplaceAsync(
            user with { Name = "PorFiltro" },
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            TestContext.Current.CancellationToken);
        (await repo.GetOneAsync(user.Id, TestContext.Current.CancellationToken))!.Name.Should().Be("PorFiltro");
    }

    [Fact]
    public async Task Should_Delete_Many()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var users = MockEntities.MockUsers(2, "Deletar");
        await repo.InsertManyAsync(users, TestContext.Current.CancellationToken);

        var deletedCount = await repo.DeleteManyAsync(
            Builders<User>.Filter.Eq(x => x.Name, "Deletar"),
            TestContext.Current.CancellationToken);

        deletedCount.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task Should_SoftDelete_And_Restore_By_Id_And_Filter()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);
        var idFilter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
        var isDeletedProjection = Builders<User>.Projection.Expression(u => u.IsDeleted);

        await repo.SoftDeleteAsync(user.Id, "IsDeleted", TestContext.Current.CancellationToken);
        (await repo.FindAsync(idFilter, isDeletedProjection, ct: TestContext.Current.CancellationToken)).First().Should().BeTrue();

        await repo.RestoreAsync(user.Id, "IsDeleted", TestContext.Current.CancellationToken);
        (await repo.FindAsync(idFilter, isDeletedProjection, ct: TestContext.Current.CancellationToken)).First().Should().BeFalse();

        await repo.SoftDeleteAsync(idFilter, "IsDeleted", TestContext.Current.CancellationToken);
        (await repo.FindAsync(idFilter, isDeletedProjection, ct: TestContext.Current.CancellationToken)).First().Should().BeTrue();

        await repo.RestoreAsync(idFilter, "IsDeleted", TestContext.Current.CancellationToken);
        (await repo.FindAsync(idFilter, isDeletedProjection, ct: TestContext.Current.CancellationToken)).First().Should().BeFalse();
    }

    [Fact]
    public async Task Should_UpdateFields_By_Id_And_By_Filter()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);

        await ((MongoRepository<User, Guid>)repo).UpdateFieldsAsync(
            user.Id,
            Builders<User>.Update.Set(x => x.Name, "PorId"),
            TestContext.Current.CancellationToken);
        (await repo.GetOneAsync(user.Id, TestContext.Current.CancellationToken))!.Name.Should().Be("PorId");

        await repo.UpdateFieldsAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            Builders<User>.Update.Set(x => x.Name, "PorFiltro"),
            TestContext.Current.CancellationToken);
        (await repo.GetOneAsync(user.Id, TestContext.Current.CancellationToken))!.Name.Should().Be("PorFiltro");
    }

    [Fact]
    public async Task Should_Distinct_With_And_Without_Filter()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var name = "DistinctName";
        await repo.InsertManyAsync(
        [
            MockEntities.MockUser() with { Name = name },
            MockEntities.MockUser() with { Name = name },
            MockEntities.MockUser() with { Name = "OtherName" },
        ], TestContext.Current.CancellationToken);

        var filtered = await repo.DistinctAsync(
            new ExpressionFieldDefinition<User, string>(x => x.Name),
            Builders<User>.Filter.Eq(x => x.Name, name),
            TestContext.Current.CancellationToken);
        filtered.Should().HaveCount(1).And.Contain(name);

        var all = await repo.DistinctAsync(
            new ExpressionFieldDefinition<User, string>(x => x.Name),
            ct: TestContext.Current.CancellationToken);
        all.Should().Contain(name).And.Contain("OtherName");
    }

    [Fact]
    public async Task Should_FindOneAndUpdate_And_FindOneAndDelete()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();
        await repo.InsertAsync(user, TestContext.Current.CancellationToken);
        var afterOpts = new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After };

        var byFilter = await repo.FindOneAndUpdateAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            Builders<User>.Update.Set(x => x.Name, "FOUA_Filter"),
            afterOpts,
            TestContext.Current.CancellationToken);
        byFilter!.Name.Should().Be("FOUA_Filter");

        var byId = await repo.FindOneAndUpdateAsync(
            user.Id,
            Builders<User>.Update.Set(x => x.Name, "FOUA_Id"),
            afterOpts,
            TestContext.Current.CancellationToken);
        byId!.Name.Should().Be("FOUA_Id");

        var deletedByFilter = await repo.FindOneAndDeleteAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            ct: TestContext.Current.CancellationToken);
        deletedByFilter.Should().NotBeNull();
        (await repo.GetOneAsync(user.Id, TestContext.Current.CancellationToken)).Should().BeNull();

        var user2 = MockEntities.MockUser();
        await repo.InsertAsync(user2, TestContext.Current.CancellationToken);
        var deletedById = await repo.FindOneAndDeleteAsync(user2.Id, ct: TestContext.Current.CancellationToken);
        deletedById.Should().NotBeNull();
        (await repo.GetOneAsync(user2.Id, TestContext.Current.CancellationToken)).Should().BeNull();
    }

    [Fact]
    public async Task Should_BulkWrite()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var userToUpdate = MockEntities.MockUser();
        var userToDelete = MockEntities.MockUser();
        await repo.InsertManyAsync([userToUpdate, userToDelete], TestContext.Current.CancellationToken);

        await repo.BulkWriteAsync(
        [
            new UpdateOneModel<User>(
                Builders<User>.Filter.Eq(x => x.Id, userToUpdate.Id),
                Builders<User>.Update.Set(x => x.Name, "BulkUpdated")),
            new DeleteOneModel<User>(
                Builders<User>.Filter.Eq(x => x.Id, userToDelete.Id)),
            new InsertOneModel<User>(MockEntities.MockUser()),
        ], TestContext.Current.CancellationToken);

        (await repo.GetOneAsync(userToUpdate.Id, TestContext.Current.CancellationToken))!.Name.Should().Be("BulkUpdated");
        (await repo.GetOneAsync(userToDelete.Id, TestContext.Current.CancellationToken)).Should().BeNull();
    }

    [Fact]
    public async Task Should_FindPaged_With_Sort()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var name = "PagedUser";
        var users = Enumerable.Range(0, 15).Select(_ => MockEntities.MockUser() with { Name = name }).ToList();
        await repo.InsertManyAsync(users, TestContext.Current.CancellationToken);

        var result = await repo.FindPagedAsync(
            Builders<User>.Filter.Eq(x => x.Name, name),
            pageIndex: 1,
            pageSize: 10,
            sort: Builders<User>.Sort.Ascending(x => x.Name),
            ct: TestContext.Current.CancellationToken);

        result.Items.Should().HaveCount(5);
        result.TotalCount.Should().Be(15);
        result.PageIndex.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.PageCount.Should().Be(2);
    }

    [Fact]
    public async Task Should_CountAsync()
    {
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var users = MockEntities.MockUsers(3);
        await repo.InsertManyAsync(users, TestContext.Current.CancellationToken);

        var count = await repo.CountAsync(
            Builders<User>.Filter.Eq(x => x.Name, users[0].Name),
            TestContext.Current.CancellationToken);

        count.Should().BeGreaterOrEqualTo(1);
    }
}
