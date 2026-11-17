# SmartEnum 2.0.0

Security and correctness release for SmartEnum and its Entity Framework Core integration.

## Migration notes
- Exact textual identities take precedence; ambiguous case-insensitive matches are rejected.
- Delimited flag serialization rejects values that cannot round-trip without changing membership. Use unambiguous values and review persisted permission sets before upgrading.
- Parsing and copying no longer restore permissions from constructor defaults.
- JSON arrays validate each member immediately instead of buffering every value.
- Existing valid JSON/EF formats and aliases remain supported.

## Maintenance
- Release publishing no longer passes API keys as command-line arguments.
- Removed unused SQLite test dependency.
- Fixed the source-generator test fixture's missing TypeConverterAttribute reference.

## Verification
- Release test suite: 80/80 passing on net10.0.
- Libraries build and pack for net8.0, net9.0 and net10.0.
- git diff --check passed. Existing nullable warning remains in SmartEnumTests.cs.