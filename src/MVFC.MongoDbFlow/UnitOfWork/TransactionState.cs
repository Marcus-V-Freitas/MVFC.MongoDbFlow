namespace MVFC.MongoDbFlow.UnitOfWork;

/// <summary>
/// Representa o estado de uma transação no MongoDB.
/// </summary>
internal enum TransactionState
{
    /// <summary>
    /// A transação está ativa e pode receber operações.
    /// </summary>
    Active,

    /// <summary>
    /// A transação foi confirmada (commit) com sucesso.
    /// </summary>
    Committed,

    /// <summary>
    /// A transação foi abortada (rollback).
    /// </summary>
    Aborted,
}