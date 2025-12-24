namespace MVFC.MongoDbFlow.Abstractions;

/// <summary>
/// Interface para resolução do nome da coleção no MongoDB a partir de um tipo de entidade.
/// </summary>
public interface ICollectionNameResolver
{
    /// <summary>
    /// Resolve o nome da coleção correspondente ao tipo de entidade informado.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade para a qual o nome da coleção será resolvido.</typeparam>
    /// <returns>Nome da coleção no MongoDB.</returns>
    string Resolve<T>();
}