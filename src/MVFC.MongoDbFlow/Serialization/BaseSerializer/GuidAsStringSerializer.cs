namespace MVFC.MongoDbFlow.Serialization.BaseSerializer;

/// <summary>
/// Serializador para <see cref="Guid"/> como string no MongoDB.
/// </summary>
internal sealed class GuidAsStringSerializer : SerializerBase<Guid>
{
    /// <summary>
    /// Serializa o valor do <see cref="Guid"/> como string no contexto do MongoDB.
    /// </summary>
    /// <param name="context">Contexto de serialização BSON.</param>
    /// <param name="args">Argumentos de serialização BSON.</param>
    /// <param name="value">Valor do <see cref="Guid"/> a ser serializado.</param>
    public override void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        Guid value)
        => context.Writer.WriteString(value.ToString());

    /// <summary>
    /// Desserializa o valor do <see cref="Guid"/> a partir de uma string no contexto do MongoDB.
    /// </summary>
    /// <param name="context">Contexto de desserialização BSON.</param>
    /// <param name="args">Argumentos de desserialização BSON.</param>
    /// <returns>Valor do <see cref="Guid"/> desserializado.</returns>
    public override Guid Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args)
        => Guid.Parse(context.Reader.ReadString());
}