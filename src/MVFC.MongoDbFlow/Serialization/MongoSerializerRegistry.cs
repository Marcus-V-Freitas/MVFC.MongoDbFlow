namespace MVFC.MongoDbFlow.Serialization;

/// <summary>
/// Registra todos os serializadores customizados para MongoDB.
/// </summary>
internal static class MongoSerializerRegistry
{
    /// <summary>
    /// Realiza o registro de todos os serializadores customizados fornecidos.
    /// </summary>
    /// <param name="regs">Coleção de registradores de serializadores a serem executados.</param>
    internal static void RegisterAll(IEnumerable<ISerializerRegistration> regs)
    {
        foreach (var reg in regs)
            reg.Register();
    }
}