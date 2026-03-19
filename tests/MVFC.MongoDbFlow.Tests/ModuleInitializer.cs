namespace MVFC.MongoDbFlow.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        try { BsonSerializer.RegisterSerializationProvider(new GuidStandardProvider()); } catch { }
        try { BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard)); } catch { }
    }

    private sealed class GuidStandardProvider : IBsonSerializationProvider
    {
        public IBsonSerializer? GetSerializer(Type type)
            => type == typeof(Guid) ? new GuidSerializer(GuidRepresentation.Standard) : null;
    }
}
