# MVFC.MongoDbFlow

> 🇧🇷 [Leia em Português](README.pt-BR.md)

[![CI](https://github.com/Marcus-V-Freitas/MVFC.MongoDbFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/Marcus-V-Freitas/MVFC.MongoDbFlow/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/Marcus-V-Freitas/MVFC.MongoDbFlow/branch/main/graph/badge.svg)](https://codecov.io/gh/Marcus-V-Freitas/MVFC.MongoDbFlow)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue)](LICENSE)
![Platform](https://img.shields.io/badge/.NET-9%20%7C%2010-blue)
[![NuGet](https://img.shields.io/nuget/dt/MVFC.MongoDbFlow)](https://www.nuget.org/packages/MVFC.MongoDbFlow)

A .NET library for generic MongoDB access, entity mapping, and async CRUD operations — including repository abstraction, custom serializers, transactions, soft-delete, pagination, and Dependency Injection integration.

## Motivation

Working with the MongoDB C# driver directly means dealing with:

- Repetitive boilerplate for every collection (insert, find, update, delete, paging…).
- Manual BSON class-map registration scattered across startup code.
- No standard pattern for transactions, soft-delete or bulk operations.
- Wiring serializers for common types (`Guid`, `DateOnly`, enums) by hand.

**MVFC.MongoDbFlow** solves this by providing a thin, opinionated layer on top of the official driver:

- A single `AddMongoFlow(...)` call registers everything — client, database, serializers, maps, context factory and unit-of-work factory.
- `IMongoRepository<T, TId>` exposes **25+ async methods** covering CRUD, paging, projections, soft-delete/restore, distinct queries and bulk writes.
- `MongoTransactionScope` gives you a simple, `IAsyncDisposable`-based transaction scope.
- `EntityMap<T>` keeps BSON mapping next to the entity, making it easy to find and maintain.

The goal is simple: let you focus on **domain logic** instead of infrastructure plumbing.

## Features

| Category | Capabilities |
|---|---|
| **Repository** | Insert · InsertMany · GetOne · Find · FindPaged · Exists · Count · Distinct · Projections |
| **Updates** | Update (by id/filter) · UpdateMany · UpdateFields · Replace · FindOneAndUpdate |
| **Deletes** | Delete (by id/filter) · DeleteMany · FindOneAndDelete |
| **Soft Delete** | SoftDelete (by id/filter) · Restore (by id/filter) |
| **Bulk** | BulkWrite (mixed operations) |
| **Transactions** | MongoTransactionScope with auto-rollback on dispose |
| **Mapping** | EntityMap\<T\> with fluent BSON configuration |
| **Serializers** | Guid · DateOnly · Enum-as-string · UTC DateTime |
| **DI** | One-line `AddMongoFlow(...)` registration |
| **Testing** | Integration tests with Testcontainers |

---

## Installation

```sh
dotnet add package MVFC.MongoDbFlow
```

---

## Usage Examples

### 1. Define Your Entities

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

### 2. Create Entity Maps

Each map defines the collection name and the BSON-level mapping for its entity:

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

### 3. Register with Dependency Injection

A single call registers the MongoDB client, database, serializers, entity maps, context factory and unit-of-work factory:

```csharp
var services = new ServiceCollection();

services.AddMongoFlow(
    new MongoOptions("mongodb://localhost:27017", "my-database"),
    serializers:
    [
        new GuidSerializerRegistration(),
        new DateOnlySerializerRegistration(),
        new UtcDateTimeSerializerRegistration(),
        new EnumAsStringSerializerRegistration()
    ],
    maps: [new UserMap(), new OrderMap()]);
```

### 4. Basic CRUD Operations

```csharp
// Resolve the context factory (typically injected via constructor)
var contextFactory = provider.GetRequiredService<IMongoContextFactory>();
var context = contextFactory.Create();
var repo = context.GetRepository<User, Guid>();

// Insert
var user = new User(Guid.NewGuid(), "Alice", new DateOnly(1995, 6, 15));
await repo.InsertAsync(user);

// Insert many
await repo.InsertManyAsync([
    new User(Guid.NewGuid(), "Bob",     new DateOnly(1988, 3, 22)),
    new User(Guid.NewGuid(), "Charlie", new DateOnly(2001, 11, 5))
]);

// Get by ID
var loaded = await repo.GetOneAsync(user.Id);

// Get by filter
var alice = await repo.GetOneAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Alice"));

// Find (list) with filter
var allUsers = await repo.FindAsync(Builders<User>.Filter.Empty);

// Check existence
bool exists = await repo.ExistsAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Alice"));

// Count
long total = await repo.CountAsync(Builders<User>.Filter.Empty);

// Update by ID
await repo.UpdateAsync(user.Id,
    Builders<User>.Update.Set(x => x.Name, "Alice Smith"));

// Update many
await repo.UpdateManyAsync(
    Builders<User>.Filter.Gte(u => u.BirthDate, new DateOnly(2000, 1, 1)),
    Builders<User>.Update.Set(x => x.Name, "Young User"));

// Replace entire document
var updated = user with { Name = "Alice Johnson" };
await repo.ReplaceAsync(updated, user.Id);

// Delete by ID
await repo.DeleteAsync(user.Id);

// Delete many
await repo.DeleteManyAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Young User"));
```

### 5. Pagination

`FindPagedAsync` returns a `PagedResult<T>` with `Items`, `TotalCount`, `PageIndex`, `PageSize` and a computed `PageCount`:

```csharp
var page = await repo.FindPagedAsync(
    filter:    Builders<User>.Filter.Empty,
    pageIndex: 0,
    pageSize:  10,
    sort:      Builders<User>.Sort.Ascending(u => u.Name));

Console.WriteLine($"Page {page.PageIndex + 1} of {page.PageCount}");
Console.WriteLine($"Total items: {page.TotalCount}");

foreach (var item in page.Items)
    Console.WriteLine($"  {item.Name}");
```

### 6. Projections

Return only the fields you need by specifying a projection type:

```csharp
public sealed record UserSummary(Guid Id, string Name);

var summaries = await repo.FindAsync(
    Builders<User>.Filter.Empty,
    Builders<User>.Projection.Expression(u => new UserSummary(u.Id, u.Name)));
```

### 7. Distinct Values

Retrieve distinct values for a specific field:

```csharp
var uniqueNames = await repo.DistinctAsync<string>(
    new StringFieldDefinition<User, string>("Name"),
    Builders<User>.Filter.Empty);
```

### 8. Soft Delete & Restore

Mark documents as deleted without physically removing them, then restore when needed:

```csharp
// Soft-delete by ID (sets an "IsDeleted" field to true)
await repo.SoftDeleteAsync(user.Id);

// Soft-delete by filter
await repo.SoftDeleteAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Bob"));

// Restore by ID
await repo.RestoreAsync(user.Id);

// Restore by filter
await repo.RestoreAsync(
    Builders<User>.Filter.Eq(u => u.Name, "Bob"));
```

### 9. Find-and-Modify (Atomic Operations)

Atomically find, update (or delete) and return the document:

```csharp
// Find one and update — returns the document AFTER the update
var result = await repo.FindOneAndUpdateAsync(
    user.Id,
    Builders<User>.Update.Set(u => u.Name, "Updated Alice"),
    new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After });

// Find one and delete — returns the removed document
var removed = await repo.FindOneAndDeleteAsync(user.Id);
```

### 10. Bulk Write

Execute multiple write operations in a single round-trip:

```csharp
var newId = Guid.NewGuid();

await repo.BulkWriteAsync([
    new InsertOneModel<User>(new User(newId, "Bulk User", new DateOnly(1990, 1, 1))),
    new UpdateOneModel<User>(
        Builders<User>.Filter.Eq(u => u.Id, newId),
        Builders<User>.Update.Set(u => u.Name, "Renamed")),
    new DeleteOneModel<User>(
        Builders<User>.Filter.Eq(u => u.Name, "Charlie"))
]);
```

### 11. Transactions

`MongoTransactionScope` wraps a `IMongoUnitOfWork` with `IAsyncDisposable` — if `CommitAsync()` is not called, the transaction is automatically rolled back on dispose:

```csharp
var uowFactory = provider.GetRequiredService<IMongoUnitOfWorkFactory>();

await using (var tx = new MongoTransactionScope(uowFactory))
{
    var userRepo  = tx.Uow.GetRepository<User, Guid>();
    var orderRepo = tx.Uow.GetRepository<Order, Guid>();

    var userId = Guid.NewGuid();
    await userRepo.InsertAsync(
        new User(userId, "Transactional User", new DateOnly(2000, 1, 1)));

    await orderRepo.InsertAsync(
        new Order(Guid.NewGuid(), userId, OrderStatus.Created, 99.90m, DateTime.UtcNow));

    // Both inserts are committed atomically
    await tx.CommitAsync();
}
// If an exception occurs before CommitAsync(), both operations are rolled back.
```

---

## Project Structure

```
src/
  MVFC.MongoDbFlow/
    Abstractions/       # Interfaces (IMongoRepository, IMongoContext, etc.)
    Bootstrap/          # MongoBootstrap — client/database initialization
    Config/             # MongoOptions
    Context/            # MongoContext, MongoContextFactory
    Extensions/         # AddMongoFlow, GetRepository
    Mapping/            # EntityMap<T>, MongoMappingRegistry
    Models/             # PagedResult<T>
    Repositories/       # MongoRepository<T, TId>
    Resolver/           # CollectionNameResolver
    Serialization/      # Custom serializers (Guid, DateOnly, Enum, UTC)
    UnitOfWork/         # MongoTransactionScope, MongoUnitOfWork
tests/
  MVFC.MongoDbFlow.Tests/
```

---

## Requirements

- .NET 9 or .NET 10
- MongoDB (local, Atlas or container)
- Docker (for running integration tests with Testcontainers)

---

## Integration Tests

The test project uses [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) to spin up isolated MongoDB instances during test execution, ensuring reliability and reproducibility. Tests cover:

- Document insertion, retrieval, filtering and deletion
- Pagination and projections
- Soft-delete and restore
- Transaction commit and rollback
- Multiple repositories within the same transaction
- Bulk write operations

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

[Apache-2.0](LICENSE)
