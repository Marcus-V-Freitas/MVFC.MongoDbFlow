namespace MVFC.MongoDbFlow.Tests.Containers;

public sealed class MongoDbTestContainer : IAsyncLifetime
{
    public MongoDbContainer Container = new MongoDbBuilder("mongo:8.0")
                                                .WithReplicaSet()
                                                .Build();

    public async ValueTask InitializeAsync()
        => await Container.StartAsync().ConfigureAwait(false);

    public async ValueTask DisposeAsync()
        => await Container.DisposeAsync().ConfigureAwait(false);
}
