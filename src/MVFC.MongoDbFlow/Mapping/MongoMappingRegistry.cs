namespace MVFC.MongoDbFlow.Mapping;

/// <summary>
/// Registra todos os mapeamentos de entidades para MongoDB.
/// </summary>
internal static class MongoMappingRegistry
{
    /// <summary>
    /// Realiza o registro de todos os mapeamentos de entidades fornecidos.
    /// </summary>
    /// <param name="maps">Coleção de mapeamentos de entidades a serem registrados.</param>
    internal static void RegisterAll(IEnumerable<IEntityMap> maps)
    {
        foreach (var map in maps)
            map.Register();
    }
}