namespace MVFC.MongoDbFlow.Tests;

public sealed class MongoRepositoryTests(MongoDbFixture fixture) : IClassFixture<MongoDbFixture>
{
    private readonly IMongoContextFactory _contextFactory = fixture.Services.BuildServiceProvider().GetRequiredService<IMongoContextFactory>();

    [Fact]
    public async Task Should_insert_and_read_document()
    {
        // Arrange
        var context = _contextFactory.Create();
        var repo = context.GetRepository<User, Guid>();
        var user = MockEntities.MockUser();

        // Act
        await repo.InsertAsync(user);

        // Assert
        var loaded = await repo.GetByIdAsync(user.Id);
        loaded.Should().NotBeNull();
        user.Name.Should().Be(loaded!.Name);
    }

    [Fact]
    public async Task GetById_should_return_null_when_not_found()
    {
        // Arrange
        var id = MockEntities.MockId();
        var repo = _contextFactory.Create().GetRepository<User, Guid>();

        // Act
        var loaded = await repo.GetByIdAsync(id);

        // Assert
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task Should_find_users_by_name()
    {
        // Arrange
        var users = MockEntities.MockUsers(2);
        var repo = _contextFactory.Create().GetRepository<User, Guid>();

        // Act
        await repo.InsertManyAsync(users);

        // Assert
        var found = await repo.FindAsync(Builders<User>.Filter.Eq(u => u.Name, users[0].Name));
        found.Should().HaveCount(1);
    }

    [Fact]
    public async Task Should_update_user_name()
    {
        // Arrange
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();
        var newName = "Atualizado";

        // Act & Assert
        await repo.InsertAsync(user);

        var updated = await repo.UpdateAsync(
            user.Id,
            Builders<User>.Update.Set(x => x.Name, newName));

        updated.Should().BeTrue();

        var loaded = await repo.GetByIdAsync(user.Id);
        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Should_delete_user_and_return_false_when_deleting_nonexistent()
    {
        // Arrange
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();

        // Act & Assert
        await repo.InsertAsync(user);

        var deleted = await repo.DeleteAsync(user.Id);
        deleted.Should().BeTrue();

        var deletedAgain = await repo.DeleteAsync(user.Id);
        deletedAgain.Should().BeFalse();
    }

    [Fact]
    public async Task Should_insert_many_and_count_by_name()
    {
        // Arrange
        var users = MockEntities.MockUsers(3);
        var repo = _contextFactory.Create().GetRepository<User, Guid>();

        // Act
        await repo.InsertManyAsync(users);

        // Assert
        var count = await repo.CountAsync(Builders<User>.Filter.Eq(x => x.Name, users[0].Name));
        count.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task Should_find_with_projection()
    {
        // Arrange
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();

        // Act
        await repo.InsertAsync(user);

        // Assert
        var projection = Builders<User>.Projection.Expression(u => u.Name);
        var names = await repo.FindAsync(Builders<User>.Filter.Eq(x => x.Id, user.Id), projection);

        names.Should().Contain(user.Name);
    }

    [Fact]
    public async Task Should_check_exists()
    {
        // Arrange
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser();

        // Act
        await repo.InsertAsync(user);

        // Assert
        var exists = await repo.ExistsAsync(Builders<User>.Filter.Eq(x => x.Name, user.Name));
        exists.Should().BeTrue();

        var notExists = await repo.ExistsAsync(Builders<User>.Filter.Eq(x => x.Name, "RANDOM"));
        notExists.Should().BeFalse();
    }

    [Fact]
    public async Task Should_update_many()
    {
        // Arrange
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var defaultName = "Atualizar";
        var users = MockEntities.MockUsers(2, defaultName);

        // Act
        await repo.InsertManyAsync(users);

        var updatedCount = await repo.UpdateManyAsync(Builders<User>.Filter.Eq(x => x.Name, "Atualizar"),
                                                      Builders<User>.Update.Set(x => x.Name, "Atualizado"));

        // Assert
        updatedCount.Should().BeGreaterOrEqualTo(2);

        var found = await repo.FindAsync(Builders<User>.Filter.Eq(x => x.Name, "Atualizado"));
        found.Should().HaveCount((int)updatedCount);
    }

    [Fact]
    public async Task Should_replace_user()
    {
        // Arrange
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var user = MockEntities.MockUser("Substituir");

        // Act
        await repo.InsertAsync(user);

        var replaced = user with { Name = "Substituido" };
        await repo.ReplaceAsync(replaced);

        // Assert
        var loaded = await repo.GetByIdAsync(user.Id);
        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("Substituido");
    }

    [Fact]
    public async Task Should_delete_many()
    {
        // Arrange
        var repo = _contextFactory.Create().GetRepository<User, Guid>();
        var users = MockEntities.MockUsers(2, "Deletar");

        // Act
        await repo.InsertManyAsync(users);

        var deletedCount = await repo.DeleteManyAsync(Builders<User>.Filter.Eq(x => x.Name, "Deletar"));
        deletedCount.Should().BeGreaterOrEqualTo(2);

        // Assert
        var count = await repo.CountAsync(Builders<User>.Filter.Eq(x => x.Name, "Deletar"));
        count.Should().Be(0);
    }
}