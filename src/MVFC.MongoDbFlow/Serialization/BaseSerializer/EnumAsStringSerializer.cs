namespace MVFC.MongoDbFlow.Serialization.BaseSerializer;

/// <summary>
/// Serializador para enums como string no MongoDB.
/// </summary>
/// <typeparam name="TEnum">Tipo do enum a ser serializado e desserializado.</typeparam>
internal sealed class EnumAsStringSerializer<TEnum> : SerializerBase<TEnum>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Serializa o valor do enum como string no contexto do MongoDB.
    /// </summary>
    /// <param name="context">Contexto de serialização BSON.</param>
    /// <param name="args">Argumentos de serialização BSON.</param>
    /// <param name="value">Valor do enum a ser serializado.</param>
    public override void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        TEnum value)
        => context.Writer.WriteString(value.ToString());

    /// <summary>
    /// Desserializa o valor do enum a partir de uma string no contexto do MongoDB.
    /// </summary>
    /// <param name="context">Contexto de desserialização BSON.</param>
    /// <param name="args">Argumentos de desserialização BSON.</param>
    /// <returns>Valor do enum desserializado.</returns>
    public override TEnum Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args)
        => Enum.Parse<TEnum>(
            context.Reader.ReadString(), ignoreCase: false);
}