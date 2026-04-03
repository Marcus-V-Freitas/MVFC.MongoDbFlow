namespace MVFC.MongoDbFlow.Tests.Serialization;

public sealed class EnumAsStringSerializerTests
{
    [Fact]
    public void Should_Serialize_Enum_As_String()
    {
        var serializer = new EnumAsStringSerializer<TestValue>();
        using var stringWriter = new StringWriter();
        using var bsonWriter = new JsonWriter(stringWriter);
        var context = BsonSerializationContext.CreateRoot(bsonWriter);

        serializer.Serialize(context, new BsonSerializationArgs(), TestValue.Value1);
        bsonWriter.Flush();

        stringWriter.ToString().Should().Be("\"Value1\"");
    }

    [Fact]
    public void Should_Deserialize_Enum_From_String()
    {
        var serializer = new EnumAsStringSerializer<TestValue>();
        using var jsonReader = new JsonReader("\"Value2\"");
        var context = BsonDeserializationContext.CreateRoot(jsonReader);

        var result = serializer.Deserialize(context, new BsonDeserializationArgs());

        result.Should().Be(TestValue.Value2);
    }
}
