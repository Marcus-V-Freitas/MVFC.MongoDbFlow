# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.2.1] - 2026-03-21

### Changed
- CI/CD workflow refinements for automated publishing and coverage reporting
- Minor adjustments to Codecov configuration for status checks precision

## [3.2.0] - 2026-03-18

### Changed

- Enhanced documentation with comprehensive usage examples for the entire API surface.
- Added full multi-language support (English and Portuguese-BR) for root and package README files.

## [3.1.0] - 2026-03-18

### Added

- Comprehensive unit test suite achieving 100% line and branch coverage across the entire project.
- New asynchronous methods in `IMongoRepository`:
  - `CountAsync` for document counting.
  - `ExistsAsync` for Existence checks.
  - `FindPagedAsync` for paginated queries with optional sorting.
  - `BulkWriteAsync` for high-performance batch operations.
- Specialized BSON serializers and registrations:
  - `DateOnlySerializerRegistration` for storing `DateOnly` as strings.
  - `GuidSerializerRegistration` (Standard) and `GuidAsStringSerializerRegistration`.
  - `UtcDateTimeSerializerRegistration` to ensure global date consistency.
- `MongoTransactionScope` and enhanced `MongoUnitOfWork` for easier transaction management.

### Changed

- Standardized all unit test projects:
  - Consolidated tests into specialized classes (`MongoRepositoryTests`, `UnitOfWorkTests`, `SerializerRegistrationTests`).
  - Standardized naming convention to PascalCase for all test methods.
  - Unified code style and indentation across the test suite.
- Optimized README files:
  - Standardized project badges (CI, CodeCov, License, Platform, NuGet).
  - Added multi-language support (English and Portuguese-BR).
- Refined code coverage collection in `coverage.runsettings`:
  - Excluded `Playground`, `AppHost`, and `Tests` assemblies to ensure accurate reporting.

### Fixed

- Resolved coverage gaps in session-based execution paths and `ReplaceAsync(entity, filter)` method.
- Fixed inconsistent indentation and coding styles in the test suite.

## [1.0.2] - 2026-01-04

### Changed

- Updated project metadata and README documentation.
- Optimized NuGet package assets (reduced icon size).

## [1.0.0] - 2025-12-23

### Added

- Initial release of the MVFC.MongoDbFlow library.
- Base repository pattern for MongoDB.
- Entity mapping and collection name resolution.
- Integration with Microsoft.Extensions.DependencyInjection.

[3.2.1]: https://github.com/Marcus-V-Freitas/MVFC.MongoDbFlow/compare/v3.1.0...v3.2.0
[3.2.0]: https://github.com/Marcus-V-Freitas/MVFC.MongoDbFlow/compare/v3.1.0...v3.2.0
[3.1.0]: https://github.com/Marcus-V-Freitas/MVFC.MongoDbFlow/compare/v1.0.2...v3.1.0
[1.0.2]: https://github.com/Marcus-V-Freitas/MVFC.MongoDbFlow/compare/v1.0.0...v1.0.2
[1.0.0]: https://github.com/Marcus-V-Freitas/MVFC.MongoDbFlow/releases/tag/v1.0.0
