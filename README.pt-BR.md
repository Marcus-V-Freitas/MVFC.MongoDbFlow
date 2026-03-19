# MVFC.MongoDbFlow

> 🇺🇸 [Read in English](README.md)

[![CI](https://github.com/Marcus-V-Freitas/MVFC.MongoDbFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/Marcus-V-Freitas/MVFC.MongoDbFlow/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/Marcus-V-Freitas/MVFC.MongoDbFlow/branch/main/graph/badge.svg)](https://codecov.io/gh/Marcus-V-Freitas/MVFC.MongoDbFlow)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue)](LICENSE)
![Platform](https://img.shields.io/badge/.NET-9%20%7C%2010-blue)
[![NuGet](https://img.shields.io/nuget/dt/MVFC.MongoDbFlow)](https://www.nuget.org/packages/MVFC.MongoDbFlow)

Biblioteca .NET para acesso genérico ao MongoDB, mapeamento de entidades e operações CRUD assíncronas — incluindo abstração de repositório, serializadores customizados, transações, soft-delete, paginação e integração com Dependency Injection.

## Motivação

Trabalhar diretamente com o driver C# do MongoDB significa lidar com:

- Boilerplate repetitivo para cada coleção (insert, find, update, delete, paginação…).
- Registro manual de BSON class-maps espalhados pelo código de inicialização.
- Nenhum padrão definido para transações, soft-delete ou operações em lote.
- Configuração manual de serializadores para tipos comuns (`Guid`, `DateOnly`, enums).

**MVFC.MongoDbFlow** resolve isso fornecendo uma camada fina e opinada sobre o driver oficial:

- Uma única chamada `AddMongoFlow(...)` registra tudo — client, database, serializadores, maps, context factory e unit-of-work factory.
- `IMongoRepository<T, TId>` expõe **mais de 25 métodos assíncronos** cobrindo CRUD, paginação, projeções, soft-delete/restore, consultas distintas e bulk writes.
- `MongoTransactionScope` oferece um escopo de transação simples, baseado em `IAsyncDisposable`.
- `EntityMap<T>` mantém o mapeamento BSON junto à entidade, facilitando a localização e manutenção.

O objetivo é simples: deixar você focar na **lógica de domínio** ao invés de encanamento de infraestrutura.

## Funcionalidades

| Categoria | Capacidades |
|---|---|
| **Repositório** | Insert · InsertMany · GetOne · Find · FindPaged · Exists · Count · Distinct · Projeções |
| **Atualizações** | Update (por id/filtro) · UpdateMany · UpdateFields · Replace · FindOneAndUpdate |
| **Exclusões** | Delete (por id/filtro) · DeleteMany · FindOneAndDelete |
| **Soft Delete** | SoftDelete (por id/filtro) · Restore (por id/filtro) |
| **Lote** | BulkWrite (operações mistas) |
| **Transações** | MongoTransactionScope com rollback automático no dispose |
| **Mapeamento** | EntityMap\<T\> com configuração fluente de BSON |
| **Serializadores** | Guid · DateOnly · Enum-as-string · UTC DateTime |
| **DI** | Registro em uma linha com `AddMongoFlow(...)` |
| **Testes** | Testes de integração com Testcontainers |

---

## Instalação

```sh
dotnet add package MVFC.MongoDbFlow
```

---

## Exemplos de Uso

### 1. Defina Suas Entidades

```csharp
public sealed record User(Guid Id, string Name, DateOnly BirthDate);

public enum OrderStatus { Created, Paid, Cancelled, Shipped }

public sealed record Order(
    Guid Id,
    Guid UserId,
    OrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAt);
```

### 2. Crie os Mapeamentos de Entidade

Cada mapeamento define o nome da coleção e a configuração BSON da entidade:

```csharp
public sealed class UserMap : EntityMap<User>
{
    public override string CollectionName => "users";
    protected override void Configure(BsonClassMap<User> cm)
    {
        cm.AutoMap();
        cm.MapIdMember(x => x.Id);
        cm.MapMember(x => x.Name).SetIsRequired(true);
        cm.MapMember(x => x.BirthDate);
    }
}

public sealed class OrderMap : EntityMap<Order>
{
    public override string CollectionName => "orders";
    protected override void Configure(BsonClassMap<Order> cm)
    {
        cm.AutoMap();
        cm.MapIdMember(x => x.Id);
        cm.MapMember(x => x.Status);
        cm.MapMember(x => x.TotalAmount);
        cm.MapMember(x => x.CreatedAt);
    }
}
```

### 3. Registre com Dependency Injection

Uma única chamada registra o client MongoDB, database, serializadores, mapeamentos de entidade, context factory e unit-of-work factory:

```csharp
var services = new ServiceCollection();

services.AddMongoFlow(
    new MongoOptions("mongodb://localhost:27017", "meu-banco"),
    serializers:
    [
        new GuidSerializerRegistration(),
        new DateOnlySerializerRegistration(),
        new UtcDateTimeSerializerRegistration(),
        new EnumAsStringSerializerRegistration()
    ],
    maps: [new UserMap(), new OrderMap()]);
```

### 4. Operações CRUD Básicas

```csharp
// Resolva o context factory (normalmente injetado via construtor)
var contextFactory = provider.GetRequiredService<IMongoContextFactory>();
var context = contextFactory.Create();
var repo = context.GetRepository<User, Guid>();

// Inserir
var user = new User(Guid.NewGuid(), "Alice", new DateOnly(1995, 6, 15));
await repo.InsertAsync(user);

// Inserir vários
await repo.InsertManyAsync([
    new User(Guid.NewGuid(), "Bob",     new DateOnly(1988, 3, 22)),
    new User(Guid.NewGuid(), "Charlie", new DateOnly(2001, 11, 5))
]);

// Buscar por ID
var loaded = await repo.GetOneAsync(user.Id);

// Buscar por filtro
var alice = await repo.GetOneAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Alice"));

// Listar com filtro
var allUsers = await repo.FindAsync(Builders<User>.Filter.Empty);

// Verificar existência
bool exists = await repo.ExistsAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Alice"));

// Contar
long total = await repo.CountAsync(Builders<User>.Filter.Empty);

// Atualizar por ID
await repo.UpdateAsync(user.Id,
    Builders<User>.Update.Set(x => x.Name, "Alice Smith"));

// Atualizar vários
await repo.UpdateManyAsync(
    Builders<User>.Filter.Gte(u => u.BirthDate, new DateOnly(2000, 1, 1)),
    Builders<User>.Update.Set(x => x.Name, "Usuário Jovem"));

// Substituir documento inteiro
var updated = user with { Name = "Alice Johnson" };
await repo.ReplaceAsync(updated, user.Id);

// Remover por ID
await repo.DeleteAsync(user.Id);

// Remover vários
await repo.DeleteManyAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Usuário Jovem"));
```

### 5. Paginação

`FindPagedAsync` retorna um `PagedResult<T>` com `Items`, `TotalCount`, `PageIndex`, `PageSize` e um `PageCount` calculado:

```csharp
var page = await repo.FindPagedAsync(
    filter:    Builders<User>.Filter.Empty,
    pageIndex: 0,
    pageSize:  10,
    sort:      Builders<User>.Sort.Ascending(u => u.Name));

Console.WriteLine($"Página {page.PageIndex + 1} de {page.PageCount}");
Console.WriteLine($"Total de itens: {page.TotalCount}");

foreach (var item in page.Items)
    Console.WriteLine($"  {item.Name}");
```

### 6. Projeções

Retorne apenas os campos necessários especificando um tipo de projeção:

```csharp
public sealed record UserSummary(Guid Id, string Name);

var summaries = await repo.FindAsync(
    Builders<User>.Filter.Empty,
    Builders<User>.Projection.Expression(u => new UserSummary(u.Id, u.Name)));
```

### 7. Valores Distintos

Recupere valores distintos para um campo específico:

```csharp
var uniqueNames = await repo.DistinctAsync<string>(
    new StringFieldDefinition<User, string>("Name"),
    Builders<User>.Filter.Empty);
```

### 8. Soft Delete e Restauração

Marque documentos como excluídos sem removê-los fisicamente, e restaure quando necessário:

```csharp
// Soft-delete por ID (define o campo "IsDeleted" como true)
await repo.SoftDeleteAsync(user.Id);

// Soft-delete por filtro
await repo.SoftDeleteAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Bob"));

// Restaurar por ID
await repo.RestoreAsync(user.Id);

// Restaurar por filtro
await repo.RestoreAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Bob"));
```

### 9. Find-and-Modify (Operações Atômicas)

Encontre, atualize (ou exclua) e retorne o documento de forma atômica:

```csharp
// Encontrar e atualizar — retorna o documento APÓS a atualização
var result = await repo.FindOneAndUpdateAsync(
    user.Id,
    Builders<User>.Update.Set(u => u.Name, "Alice Atualizada"),
    new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After });

// Encontrar e excluir — retorna o documento removido
var removed = await repo.FindOneAndDeleteAsync(user.Id);
```

### 10. Bulk Write

Execute múltiplas operações de escrita em uma única viagem ao servidor:

```csharp
var newId = Guid.NewGuid();

await repo.BulkWriteAsync([
    new InsertOneModel<User>(new User(newId, "Usuário Bulk", new DateOnly(1990, 1, 1))),
    new UpdateOneModel<User>(
        Builders<User>.Filter.Eq(u => u.Id, newId),
        Builders<User>.Update.Set(u => u.Name, "Renomeado")),
    new DeleteOneModel<User>(
        Builders<User>.Filter.Eq(u => u.Name, "Charlie"))
]);
```

### 11. Transações

`MongoTransactionScope` encapsula uma `IMongoUnitOfWork` com `IAsyncDisposable` — se `CommitAsync()` não for chamado, a transação é automaticamente revertida no dispose:

```csharp
var uowFactory = provider.GetRequiredService<IMongoUnitOfWorkFactory>();

await using (var tx = new MongoTransactionScope(uowFactory))
{
    var userRepo  = tx.Uow.GetRepository<User, Guid>();
    var orderRepo = tx.Uow.GetRepository<Order, Guid>();

    var userId = Guid.NewGuid();
    await userRepo.InsertAsync(
        new User(userId, "Usuário Transacional", new DateOnly(2000, 1, 1)));

    await orderRepo.InsertAsync(
        new Order(Guid.NewGuid(), userId, OrderStatus.Created, 99.90m, DateTime.UtcNow));

    // Ambas inserções são commitadas atomicamente
    await tx.CommitAsync();
}
// Se uma exceção ocorrer antes do CommitAsync(), ambas operações são revertidas.
```

---

## Estrutura do Projeto

```
src/
  MVFC.MongoDbFlow/
    Abstractions/       # Interfaces (IMongoRepository, IMongoContext, etc.)
    Bootstrap/          # MongoBootstrap — inicialização de client/database
    Config/             # MongoOptions
    Context/            # MongoContext, MongoContextFactory
    Extensions/         # AddMongoFlow, GetRepository
    Mapping/            # EntityMap<T>, MongoMappingRegistry
    Models/             # PagedResult<T>
    Repositories/       # MongoRepository<T, TId>
    Resolver/           # CollectionNameResolver
    Serialization/      # Serializadores customizados (Guid, DateOnly, Enum, UTC)
    UnitOfWork/         # MongoTransactionScope, MongoUnitOfWork
tests/
  MVFC.MongoDbFlow.Tests/
```

---

## Requisitos

- .NET 9 ou .NET 10
- MongoDB (local, Atlas ou container)
- Docker (para rodar os testes de integração com Testcontainers)

---

## Testes de Integração

O projeto de testes utiliza [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) para criar instâncias MongoDB isoladas durante a execução dos testes, garantindo confiabilidade e reprodutibilidade. Os testes cobrem:

- Inserção, leitura, filtragem e exclusão de documentos
- Paginação e projeções
- Soft-delete e restauração
- Commit e rollback de transações
- Múltiplos repositórios na mesma transação
- Operações de bulk write

---

## Contribuindo

Veja [CONTRIBUTING.md](CONTRIBUTING.md).

## Licença

[Apache-2.0](LICENSE)
