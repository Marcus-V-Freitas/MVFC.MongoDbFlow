namespace MVFC.MongoDbFlow.Tests.Containers;

public sealed class MongoDbFixture : IAsyncLifetime
{
    public IServiceCollection Services = new ServiceCollection();
    private readonly MongoDbTestContainer _container = new();

    public async Task InitializeAsync()
    {
        await _container.InitializeAsync();

        Services.AddMongoFlow(
            new MongoOptions(
                ConnectionString: _container.Container.GetConnectionString(),
                DatabaseName: "integration-tests"),
            serializers:
            [
                new DateOnlySerializerRegistration(),
                new GuidSerializerRegistration(),
            ],
            maps:
            [
                new UserMap(),
                new OrderMap(),
            ]);

    }

    public async Task DisposeAsync()
        => await _container.DisposeAsync();
}