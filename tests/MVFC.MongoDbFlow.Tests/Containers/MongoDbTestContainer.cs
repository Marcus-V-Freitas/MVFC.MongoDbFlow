namespace MVFC.MongoDbFlow.Tests.Containers;

public sealed class MongoDbTestContainer : IAsyncLifetime
{
    public MongoDbContainer Container = new MongoDbBuilder()
                                                .WithReplicaSet()
                                                .Build();

    public async Task InitializeAsync()
        => await Container.StartAsync();

    public async Task DisposeAsync()
        => await Container.DisposeAsync();
}