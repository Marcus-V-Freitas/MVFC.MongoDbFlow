namespace MVFC.MongoDbFlow.Extensions;

/// <summary>
/// Métodos de extensão para registrar serviços do MongoDbFlow na DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona e configura os serviços do MongoDbFlow no container de injeção de dependência.
    /// </summary>
    /// <param name="services">A coleção de serviços da aplicação.</param>
    /// <param name="options">Opções de configuração do MongoDB.</param>
    /// <param name="serializers">Serializadores customizados a serem registrados (opcional).</param>
    /// <param name="maps">Mapeamentos de entidades a serem registrados (opcional).</param>
    /// <returns>A própria coleção de serviços, permitindo encadeamento.</returns>
    public static IServiceCollection AddMongoFlow(
        this IServiceCollection services,
        MongoOptions options,
        IEnumerable<ISerializerRegistration>? serializers = null,
        IEnumerable<IEntityMap>? maps = null)
    {
        var bootstrap = new MongoBootstrap(options, serializers, maps);

        services.AddSingleton(bootstrap.Client);
        services.AddSingleton(bootstrap.Database);
        services.AddSingleton(bootstrap.Resolver);

        services.AddSingleton<IMongoContextFactory, MongoContextFactory>();
        services.AddScoped<IMongoUnitOfWorkFactory, MongoUnitOfWorkFactory>();

        return services;
    }
}