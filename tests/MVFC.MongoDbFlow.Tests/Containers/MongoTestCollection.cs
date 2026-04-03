namespace MVFC.MongoDbFlow.Tests.Containers;

[CollectionDefinition("Mongo collection")]
public class MongoDbCollectionDefinition : ICollectionFixture<MongoDbFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
