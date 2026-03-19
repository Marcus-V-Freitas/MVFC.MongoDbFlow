namespace MVFC.MongoDbFlow.Serialization.Serializers;

/// <summary>
/// Registro de serializador para <see cref="DateTime"/> UTC no MongoDB.
/// </summary>
public sealed class UtcDateTimeSerializerRegistration : ISerializerRegistration
{
    /// <summary>
    /// Registra o serializador de <see cref="DateTime"/> para garantir que datas sejam tratadas como UTC no MongoDB.
    /// Se já houver um serializador configurado para UTC, não faz nada.
    /// </summary>
    public void Register() =>
        BsonSerializer.TryRegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc));
}
