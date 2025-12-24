namespace MVFC.MongoDbFlow.Serialization.Serializers;

/// <summary>
/// Registro de serializador para enums como string no MongoDB.
/// </summary>
/// <typeparam name="TEnum">Tipo do enum a ser serializado e desserializado.</typeparam>
public sealed class EnumAsStringSerializerRegistration<TEnum> : ISerializerRegistration
    where TEnum : struct, Enum
{
    /// <summary>
    /// Registra o serializador de <typeparamref name="TEnum"/> para que seja armazenado como string no MongoDB.
    /// Se já houver um serializador do tipo <see cref="EnumAsStringSerializer{TEnum}"/> registrado, não faz nada.
    /// </summary>
    public void Register()
    {
        if (BsonSerializer.LookupSerializer<TEnum>() is EnumAsStringSerializer<TEnum>)
            return;

        BsonSerializer.TryRegisterSerializer(new EnumAsStringSerializer<TEnum>());
    }
}