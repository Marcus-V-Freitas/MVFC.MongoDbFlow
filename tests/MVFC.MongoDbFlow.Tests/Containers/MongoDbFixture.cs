namespace MVFC.MongoDbFlow.Tests.Containers;

public sealed class MongoDbFixture : IAsyncLifetime
{
    public IServiceCollection Services { get; } = new ServiceCollection();
    private readonly MongoDbTestContainer _container = new();

    public async ValueTask InitializeAsync()
    {
        await _container.InitializeAsync().ConfigureAwait(false);

        try
        {
            var connString = _container.Container.GetConnectionString();
            if (!connString.Contains("uuidRepresentation=", StringComparison.OrdinalIgnoreCase))
            {
                connString += connString.Contains('?', StringComparison.Ordinal) ? "&uuidRepresentation=standard" : "?uuidRepresentation=standard";
            }

            Services.AddMongoFlow(
                new MongoOptions(
                    ConnectionString: connString,
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
        catch (BsonSerializationException)
        {
            // Fallback for cases where serializers are already registered
            var connString = _container.Container.GetConnectionString();
            if (!connString.Contains("uuidRepresentation=", StringComparison.OrdinalIgnoreCase))
            {
                connString += connString.Contains('?', StringComparison.Ordinal) ? "&uuidRepresentation=standard" : "?uuidRepresentation=standard";
            }

            Services.AddMongoFlow(
                new MongoOptions(
                    ConnectionString: connString,
                    DatabaseName: "integration-tests"),
                serializers: [],
                maps:
                [
                    new UserMap(),
                    new OrderMap(),
                ]);
        }
    }

    public async ValueTask DisposeAsync()
        => await _container.DisposeAsync().ConfigureAwait(false);
}
