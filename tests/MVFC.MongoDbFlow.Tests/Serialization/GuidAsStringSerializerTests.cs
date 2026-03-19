namespace MVFC.MongoDbFlow.Tests.Serialization;

public sealed class GuidAsStringSerializerTests
{
    [Fact]
    public void Should_Serialize_Guid_As_String()
    {
        var serializer = new GuidAsStringSerializer();
        using var stringWriter = new StringWriter();
        using var bsonWriter = new JsonWriter(stringWriter);
        var context = BsonSerializationContext.CreateRoot(bsonWriter);
        var value = Guid.Parse("d7b0f0b0-0b0b-0b0b-0b0b-0b0b0b0b0b0b");

        serializer.Serialize(context, new BsonSerializationArgs(), value);
        bsonWriter.Flush();

        stringWriter.ToString().Should().Be("\"d7b0f0b0-0b0b-0b0b-0b0b-0b0b0b0b0b0b\"");
    }

    [Fact]
    public void Should_Deserialize_Guid_From_String()
    {
        var serializer = new GuidAsStringSerializer();
        var guid = Guid.NewGuid();
        using var jsonReader = new JsonReader($"\"{guid}\"");
        var context = BsonDeserializationContext.CreateRoot(jsonReader);

        var result = serializer.Deserialize(context, new BsonDeserializationArgs());

        result.Should().Be(guid);
    }
}
