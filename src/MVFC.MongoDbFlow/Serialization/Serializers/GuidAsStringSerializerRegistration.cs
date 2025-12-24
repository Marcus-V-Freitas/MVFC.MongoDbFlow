namespace MVFC.MongoDbFlow.Serialization.Serializers;

/// <summary>
/// Registro de serializador para <see cref="Guid"/> como string no MongoDB.
/// </summary>
public sealed class GuidAsStringSerializerRegistration : ISerializerRegistration
{
    /// <summary>
    /// Registra o serializador de <see cref="Guid"/> para que seja armazenado como string no MongoDB.
    /// Se já houver um serializador do tipo <see cref="GuidAsStringSerializer"/> registrado, não faz nada.
    /// </summary>
    public void Register()
    {
        if (BsonSerializer.LookupSerializer<Guid>() is GuidAsStringSerializer)
            return;

        BsonSerializer.TryRegisterSerializer(new GuidAsStringSerializer());
    }
}