# SebastianGuzmanMorla.SmartEnum

A modern, high-performance, and feature-rich Smart Enum (strongly typed enum) implementation for .NET. This library leverages **Source Generators** to automatically build lookup tables at compile time, eliminating reflection overhead. It also includes first-class support for **Entity Framework Core** and **System.Text.Json**.

[![NuGet Version](https://img.shields.io/nuget/v/SebastianGuzmanMorla.SmartEnum.svg)](https://www.nuget.org/packages/SebastianGuzmanMorla.SmartEnum)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## Key Features

- 🚀 **Zero Reflection**: Compile-time lookup tables generated via Source Generators using the `[GenerateSmartEnum]` attribute.
- 🔒 **Type-Safe Enumerations**: Replace primitive enums with robust, object-oriented classes containing behavior and custom properties.
- 📦 **Generic Underlying Values**: Supports any non-nullable value type (`string`, `int`, `Guid`, custom types, etc.) as the underlying key.
- 🏷️ **SmartEnum Flags**: Combine and manipulate sets of enums with `SmartEnumFlags<TFlags, TEnum, TValue>`—perfect for permissions, roles, and configuration sets.
- ⚙️ **Operators Overloading**: Built-in implicit conversions, direct equality (`==`, `!=`), and bitwise operators (`|`, `-`) for flag sets.
- 🌐 **JSON Serialization**: Direct serialization to and from the underlying value type using `System.Text.Json` converters.
- 💾 **Entity Framework Core Support**: Out-of-the-box converters and value comparers for seamless database persistence.

---

## Installation

Install the core package containing the SmartEnum base classes, Source Generator, and JSON converters:

```bash
dotnet add package SebastianGuzmanMorla.SmartEnum
```

If you are using Entity Framework Core, install the EF Core integration package:

```bash
dotnet add package SebastianGuzmanMorla.SmartEnum.EntityFrameworkCore
```

### Compatibility

- **Target Frameworks**: `.NET 8.0`, `.NET 9.0`, `.NET 10.0`
- **Source Generator**: Compatible with any project targeting `netstandard2.0` or higher
- **EF Core Dependency**: `Microsoft.EntityFrameworkCore` (>= v8.0.0)

---

## Basic Usage

### 1. Define a SmartEnum

To create a smart enum, inherit from `SmartEnum<TEnum, TValue>`, declare your values as `public static readonly` fields, and decorate the class with `[GenerateSmartEnum]`. The class **must** be marked as `partial`.

```csharp
using SebastianGuzmanMorla.SmartEnum;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using System.Text.Json.Serialization;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;

namespace MyDomain.Enums;

[JsonConverter(typeof(SmartEnumJsonConverter<SubscriptionTier, string>))]
[GenerateSmartEnum]
public sealed partial class SubscriptionTier : SmartEnum<SubscriptionTier, string>
{
    public static readonly SubscriptionTier Free = new("free", price: 0.00, maxProjects: 3);
    public static readonly SubscriptionTier Professional = new("pro", price: 19.99, maxProjects: 20);
    public static readonly SubscriptionTier Enterprise = new("enterprise", price: 99.99, maxProjects: int.MaxValue);

    // Custom properties
    public double Price { get; }
    public int MaxProjects { get; }

    private SubscriptionTier(string value, double price, int maxProjects) : base(value)
    {
        Price = price;
        MaxProjects = maxProjects;
    }
}
```

> [!NOTE]
> The Source Generator will scan classes annotated with `[GenerateSmartEnum]` and generate the static lookup dictionary needed for fast, reflection-free parsing.

### 2. Equality and Comparison

You can compare SmartEnums directly with each other or directly with their underlying values using `==` and `!=`.

```csharp
SubscriptionTier myTier = SubscriptionTier.Professional;

// Direct object comparison
bool isPro = myTier == SubscriptionTier.Professional; // true

// Comparison with the underlying value (implicit conversion is supported)
bool isProValue = myTier == "pro"; // true
```

### 3. Parsing and TryParse

Convert underlying values or string representations back to the SmartEnum instance safely and efficiently.

```csharp
// 1. Parsing by value (throws SmartEnumException if not found)
SubscriptionTier tier = SubscriptionTier.Parse("pro");

// 2. Case-insensitive parsing by name (throws SmartEnumException if not found)
SubscriptionTier tierByName = SubscriptionTier.Parse("Professional");

// 3. Safe TryParse by value
if (SubscriptionTier.TryParse("enterprise", out var enterpriseTier))
{
    Console.WriteLine($"Max projects: {enterpriseTier.MaxProjects}");
}
```

---

## Working with SmartEnumFlags

For flag-like enums (like bitwise combinations of permissions or options), the library provides `SmartEnumFlags`.

### 1. Define the Flags and Flag Set

```csharp
using SebastianGuzmanMorla.SmartEnum;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using System.Text.Json.Serialization;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;

namespace MyDomain.Enums;

// A. Define the individual flags
[JsonConverter(typeof(SmartEnumJsonConverter<UserPermission, string>))]
[GenerateSmartEnum]
public sealed partial class UserPermission : SmartEnum<UserPermission, string>
{
    public static readonly UserPermission Read = new("read");
    public static readonly UserPermission Write = new("write");
    public static readonly UserPermission Delete = new("delete");
    public static readonly UserPermission Admin = new("admin");

    private UserPermission(string value) : base(value) { }
}

// B. Define the set container (requires a public parameterless constructor)
[JsonConverter(typeof(SmartEnumFlagsJsonConverter<UserPermissionSet, UserPermission, string>))]
public class UserPermissionSet : SmartEnumFlags<UserPermissionSet, UserPermission, string>
{
    // Required for deserialization and parsing
    public UserPermissionSet() : base() { }

    // Optional convenience constructor
    public UserPermissionSet(params UserPermission[] permissions) : base(permissions) { }
}
```

### 2. Flag Set Operations

Modify and inspect the flag sets using clean operators and helper methods:

```csharp
// Parse from space-separated or comma-separated values
var permissions = UserPermissionSet.Parse("read, write");

// Check if a single flag is present
if (permissions.Has(UserPermission.Read))
{
    Console.WriteLine("User can read.");
}

// Add/Remove flags using bitwise-like operators (creates a new cloned instance)
permissions = permissions | UserPermission.Delete; // adds 'delete'
permissions = permissions - UserPermission.Read;   // removes 'read'

// Mutate set directly (in-place modification)
permissions.Add(UserPermission.Admin);
permissions.Remove(UserPermission.Write);

// Checks
bool hasAll = permissions.ContainsAll(new UserPermissionSet(UserPermission.Delete, UserPermission.Admin));
bool exactMatch = permissions.EqualsAll(UserPermission.Delete, UserPermission.Admin);

// Representation
Console.WriteLine(permissions.ToString()); // "admin delete" (space-separated, ordered by value)
```

---

## JSON Serialization

To serialize and deserialize SmartEnums using `System.Text.Json`, use the `SmartEnumJsonConverter` and `SmartEnumFlagsJsonConverter`.

### Direct Property Serialization
Apply `[JsonConverter]` attributes directly on the class definitions (as shown in the code examples above) or register them globally:

```csharp
using System.Text.Json;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;

var options = new JsonSerializerOptions();
options.Converters.Add(new SmartEnumJsonConverter<SubscriptionTier, string>());
options.Converters.Add(new SmartEnumFlagsJsonConverter<UserPermissionSet, UserPermission, string>());

string json = JsonSerializer.Serialize(SubscriptionTier.Professional, options); // Output: "pro"
SubscriptionTier tier = JsonSerializer.Deserialize<SubscriptionTier>(json, options)!;
```

---

## Entity Framework Core Integration

To store `SmartEnum` or `SmartEnumFlags` properties in your database, configure the conventions in your `DbContext`. The converters map the smart enums to/from their underlying database type (e.g., storing a `string` or an `int` in the table).

Install the `SebastianGuzmanMorla.SmartEnum.EntityFrameworkCore` package and override `ConfigureConventions`:

```csharp
using Microsoft.EntityFrameworkCore;
using SebastianGuzmanMorla.SmartEnum.Converters.EntityFrameworkCore;
using MyDomain.Enums;

namespace MyInfrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Map SmartEnum to DB column (maps to Text in database)
        configurationBuilder.Properties<SubscriptionTier>()
            .HaveConversion<SmartEnumConverter<SubscriptionTier, string>, SmartEnumComparer<SubscriptionTier, string>>()
            .HaveColumnType("varchar(50)");

        // Map SmartEnumFlags to DB column (maps to Text in database as space-separated values)
        configurationBuilder.Properties<UserPermissionSet>()
            .HaveConversion<SmartEnumFlagsValueConverter<UserPermissionSet, UserPermission, string>,
                            SmartEnumFlagsValueComparer<UserPermissionSet, UserPermission, string>>()
            .HaveColumnType("varchar(500)");
    }
}
```

---

## Exceptions

The library throws `SmartEnumException` when validation or parsing fails. This exception inherits from `Exception` and provides details about the invalid lookup values.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.
