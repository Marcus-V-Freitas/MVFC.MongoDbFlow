# MVFC.MongoDbFlow

Biblioteca .NET para acesso, mapeamento e operações CRUD genéricas com MongoDB, incluindo abstrações de repositório, mapeamento de entidades, transações e integração com Dependency Injection.

## Recursos

- Abstração de repositório (`IMongoRepository<T, TId>`) para operações CRUD assíncronas
- Mapeamento de entidades via `EntityMap<T>`
- Serializadores customizados (`Guid`, `DateOnly`)
- Suporte a transações MongoDB
- Extensões para integração com `Microsoft.Extensions.DependencyInjection`
- Testes de integração com Testcontainers

## Instalação

```sh
dotnet add package MVFC.MongoDbFlow
```

## Exemplos reais de uso

### 1. Defina suas entidades

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

### 2. Crie o mapeamento das entidades

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

### 3. Configuração e registro

```csharp
// Exemplo simplificado de configuração
var services = new ServiceCollection();
services.AddMongoFlow(
    new MongoOptions("mongodb://localhost:27017", "sua-db"),
    serializers: [ new DateOnlySerializerRegistration(), new GuidSerializerRegistration() ],
    maps: [ new UserMap(), new OrderMap() ]);
```

### 4. Operações CRUD

```csharp
var contextFactory = services.BuildServiceProvider().GetRequiredService<IMongoContextFactory>();
var context = contextFactory.Create();
var repo = context.GetRepository<User, Guid>();

// Inserir
var user = new User(Guid.NewGuid(), "Vinicius", new DateOnly(1990, 1, 1));
await repo.InsertAsync(user);

// Buscar por ID
var loaded = await repo.GetByIdAsync(user.Id);

// Buscar por filtro
var found = await repo.FindAsync(Builders<User>.Filter.Eq(u => u.Name, "Vinicius"));

// Atualizar
await repo.UpdateAsync(user.Id, Builders<User>.Update.Set(x => x.Name, "Novo Nome"));

// Remover
await repo.DeleteAsync(user.Id);
```

### 5. Transações

```csharp
var uowFactory = services.BuildServiceProvider().GetRequiredService<IMongoUnitOfWorkFactory>();
var userId = Guid.NewGuid();
await using (var tx = new MongoTransactionScope(uowFactory))
{
    var repo = tx.Uow.GetRepository<User, Guid>();
    await repo.InsertAsync(new User(userId, "Transação", new DateOnly(2000, 1, 1)));
    await tx.CommitAsync();
}
```

## Testes de Integração

O projeto de testes utiliza [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) para criar instâncias MongoDB isoladas durante a execução dos testes, garantindo confiabilidade e reprodutibilidade. Os testes cobrem:

- Inserção, leitura, busca e deleção de documentos
- Rollback e commit de transações
- Uso de múltiplos repositórios na mesma transação

## Licença

Este projeto está licenciado sob a licença Apache-2.0.
