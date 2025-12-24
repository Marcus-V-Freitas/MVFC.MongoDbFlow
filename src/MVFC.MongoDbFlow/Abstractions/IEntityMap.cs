namespace MVFC.MongoDbFlow.Abstractions;

/// <summary>
/// Interface para mapeamento de entidades do MongoDB.
/// </summary>
public interface IEntityMap
{
    /// <summary>
    /// Obtém o tipo da entidade mapeada.
    /// </summary>
    Type EntityType { get; }

    /// <summary>
    /// Obtém o nome da coleção associada à entidade.
    /// </summary>
    string CollectionName { get; }

    /// <summary>
    /// Realiza o registro do mapeamento da entidade no MongoDB.
    /// Deve ser chamado durante a inicialização para garantir o correto mapeamento dos tipos.
    /// </summary>
    void Register();
}