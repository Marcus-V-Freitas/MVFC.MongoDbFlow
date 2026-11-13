namespace MVFC.MongoDbFlow.Tests.Containers;

public sealed class MongoDbTestContainer : IAsyncLifetime
{
    public MongoDbContainer Container { get; } = new MongoDbBuilder("mongodb/mongodb-community-server:8.0.38-ubuntu2204-slim")
                                                        .WithReplicaSet()
                                                        .Build();

    public async ValueTask InitializeAsync()
        => await Container.StartAsync().ConfigureAwait(false);

    public async ValueTask DisposeAsync()
        => await Container.DisposeAsync().ConfigureAwait(false);
}
