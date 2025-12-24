namespace MVFC.MongoDbFlow.Serialization.Serializers;

/// <summary>
/// Registro de serializador para <see cref="Guid"/> padrão no MongoDB.
/// </summary>
public sealed class GuidSerializerRegistration : ISerializerRegistration
{
    /// <summary>
    /// Registra o serializador de <see cref="Guid"/> utilizando a representação padrão no MongoDB.
    /// Deve ser chamado durante a inicialização da aplicação para garantir a serialização correta dos valores Guid.
    /// </summary>
    public void Register()
    {
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    }
}