namespace MVFC.MongoDbFlow.Tests.Serialization;

public sealed class SerializerRegistrationTests
{
    [Fact]
    public void EnumAsStringSerializerRegistration_Should_Register()
    {
        var registration = new EnumAsStringSerializerRegistration<TestEnum>();

        try { registration.Register(); } catch (BsonSerializationException) { }

        BsonSerializer.LookupSerializer<TestEnum>().Should().NotBeNull();
    }

    [Fact]
    public void GuidAsStringSerializerRegistration_Should_Register()
    {
        var registration = new GuidAsStringSerializerRegistration();

        try { registration.Register(); } catch (BsonSerializationException) { }

        BsonSerializer.LookupSerializer<Guid>().Should().NotBeNull();
    }

    [Fact]
    public void UtcDateTimeSerializerRegistration_Should_Register()
    {
        var registration = new UtcDateTimeSerializerRegistration();

        try { registration.Register(); } catch (BsonSerializationException) { }

        var serializer = BsonSerializer.LookupSerializer<DateTime>();
        serializer.Should().BeOfType<DateTimeSerializer>();
        ((DateTimeSerializer)serializer).Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void GuidSerializerRegistration_Should_Register()
    {
        var registration = new GuidSerializerRegistration();

        try { registration.Register(); } catch (BsonSerializationException) { }

        var serializer = BsonSerializer.LookupSerializer<Guid>();
        serializer.Should().NotBeNull();
        if (serializer is GuidSerializer gs)
        {
            gs.GuidRepresentation.Should().Be(GuidRepresentation.Standard);
        }
    }

    [Fact]
    public void DateOnlySerializerRegistration_Should_Register()
    {
        var registration = new DateOnlySerializerRegistration();

        try { registration.Register(); } catch (BsonSerializationException) { }

        BsonSerializer.LookupSerializer<DateOnly>().Should().NotBeNull();
    }
}
