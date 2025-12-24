namespace MVFC.MongoDbFlow.Serialization.Serializers;

/// <summary>
/// Registro de serializador para <see cref="DateOnly"/> como string no MongoDB.
/// </summary>
public sealed class DateOnlySerializerRegistration : ISerializerRegistration
{
    /// <summary>
    /// Registra o serializador de <see cref="DateOnly"/> para que seja armazenado como string no MongoDB.
    /// Deve ser chamado durante a inicialização da aplicação para garantir a serialização correta dos valores <see cref="DateOnly"/>.
    /// </summary>
    public void Register()
    {
        BsonSerializer.TryRegisterSerializer(new DateOnlySerializer(BsonType.String));
    }
}