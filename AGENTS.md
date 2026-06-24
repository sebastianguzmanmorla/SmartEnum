# SebastianGuzmanMorla.SmartEnum - AI Agent Guidelines

This document provides instructions, rules, and code patterns for AI coding assistants (LLMs, Copilot, Cursor, etc.) to correctly implement, extend, and consume the `SebastianGuzmanMorla.SmartEnum` library within this codebase.

---

## 🚀 Core Capabilities

* **Type-Safe Enumerations**: Replace traditional C# `enum`s with object-oriented smart enums using `SmartEnum<TEnum, TValue>`.
* **Reflection-Free Lookups**: A Source Generator automatically builds static dictionary maps used for `Parse` and `TryParse` operations at compile time when classes are decorated with `[GenerateSmartEnum]`.
* **Flag/Combinatorial Sets**: Group and manipulate sets of enums using `SmartEnumFlags<TFlags, TEnum, TValue>`.
* **First-Class Serialization & Persistence**: Fully compatible with `System.Text.Json` and `Entity Framework Core`.

---

## 🛠️ Implementation Rules for AI Agents

> [!IMPORTANT]
> When creating or editing a SmartEnum in the codebase, you MUST adhere to the following rules to ensure the source generator compile-time lookups and runtime behavior work correctly.

### 1. SmartEnum Definition Structure

* **Class Modifiers**: The SmartEnum class **must** be marked as `public sealed partial` or `public partial`.
* **Inheritance**: It must inherit from `SmartEnum<TEnum, TValue>` where `TEnum` is the class itself and `TValue` is the underlying value type (which must be non-nullable, e.g., `string`, `int`, `Guid`).
* **Source Generator Attribute**: The class **must** be decorated with `[GenerateSmartEnum]` from the `SebastianGuzmanMorla.SmartEnum.Attributes` namespace.
* **Constructor**: Define a `private` or `protected` constructor taking a parameter of type `TValue` (along with any other custom properties) and passing the value to the base constructor: `base(value)`.
* **Enum Options**: Define options as `public static readonly TEnum Name = new(value, ...Properties)`.

### 2. SmartEnumFlags Definition Structure

* **Inheritance**: Inherit from `SmartEnumFlags<TFlags, TEnum, TValue>`.
* **Constructor**: A public parameterless constructor is **required** so the flags parser and JSON deserializer can instantiate it.
* **Storage/Format**: The flag combination is represented internally as a set of `TEnum` elements. The string representation is a space-separated string of the enum options.
* **Helper Constructor**: Provide a constructor overload taking `params TEnum[]` to simplify initialization.

### 3. Comparison and Equality

* Use direct comparison operators `==` and `!=`. The library overloads these operators to compare:
  * Two `SmartEnum` instances (`enum1 == enum2`).
  * A `SmartEnum` instance and its underlying value type (`enum1 == "value"` or `"value" == enum1`).
* **Do NOT** write `enum1.Value == enum2.Value` or `enum1.Value == "value"`. Use direct equality: `enum1 == "value"`.

---

## 📖 Code Templates

### Basic SmartEnum Definition

```csharp
using SebastianGuzmanMorla.SmartEnum;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using System.Text.Json.Serialization;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;

namespace MyProject.Domain;

[JsonConverter(typeof(SmartEnumJsonConverter<UserStatus, string>))]
[GenerateSmartEnum]
public sealed partial class UserStatus : SmartEnum<UserStatus, string>
{
    public static readonly UserStatus Active = new("active");
    public static readonly UserStatus Inactive = new("inactive");
    public static readonly UserStatus Suspended = new("suspended");

    private UserStatus(string value) : base(value)
    {
    }
}
```

### SmartEnumFlags Definition

```csharp
using SebastianGuzmanMorla.SmartEnum;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using System.Text.Json.Serialization;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;

namespace MyProject.Domain;

[JsonConverter(typeof(SmartEnumJsonConverter<Permission, string>))]
[GenerateSmartEnum]
public sealed partial class Permission : SmartEnum<Permission, string>
{
    public static readonly Permission Read = new("read");
    public static readonly Permission Write = new("write");
    public static readonly Permission Delete = new("delete");

    private Permission(string value) : base(value)
    {
    }
}

[JsonConverter(typeof(SmartEnumFlagsJsonConverter<PermissionSet, Permission, string>))]
public class PermissionSet : SmartEnumFlags<PermissionSet, Permission, string>
{
    // A public parameterless constructor is required for deserialization and parsing
    public PermissionSet() : base()
    {
    }

    // Optional helper constructor for specific combinations
    public PermissionSet(params Permission[] permissions) : base(permissions)
    {
    }
}
```

---

## 🔄 Common Operations

### Parsing Enums

```csharp
// Exact value parsing (returns the SmartEnum instance or throws SmartEnumException)
UserStatus status = UserStatus.Parse("active");

// Case-insensitive string representation parsing (throws if invalid)
UserStatus statusFromStr = UserStatus.Parse("Active"); 

// Safe parsing
if (UserStatus.TryParse("suspended", out var suspendedStatus))
{
    // Use suspendedStatus here
}
```

### Working with Flags

```csharp
// Parse from space- or comma-separated string
var permissions = PermissionSet.Parse("read, write");

// Parse from array of values
var permissionsFromArray = PermissionSet.Parse(new[] { "read", "delete" });

// Check presence of a flag
bool canWrite = permissions.Has(Permission.Write);

// Check if all requested flags are present
bool isSuperUser = permissions.ContainsAll(new PermissionSet(Permission.Read, Permission.Write, Permission.Delete));

// Adding/Removing flags (returns a new cloned instance)
var updatedPermissions = permissions | Permission.Delete; // via bitwise-like operator
var fewerPermissions = permissions - Permission.Read;     // via subtraction operator

// In-place mutation (modifies the current instance directly)
permissions.Add(UserPermission.Write);
permissions.Remove(UserPermission.Read);
```

---

## 💾 Integration with Entity Framework Core

To store `SmartEnum` and `SmartEnumFlags` properties in your EF Core database context, configure the properties using the built-in converters and comparers in `ConfigureConventions` or in individual entity configurations.

```csharp
using Microsoft.EntityFrameworkCore;
using SebastianGuzmanMorla.SmartEnum.Converters.EntityFrameworkCore;
using MyProject.Domain;

namespace MyProject.Infrastructure;

public class MyDbContext : DbContext
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Register standard SmartEnum Conversion (stores underlying value type in DB)
        configurationBuilder.Properties<UserStatus>()
            .HaveConversion<SmartEnumConverter<UserStatus, string>, SmartEnumComparer<UserStatus, string>>()
            .HaveColumnType("varchar(50)");

        // Register SmartEnumFlags Conversion (stores space-separated string of values in DB)
        configurationBuilder.Properties<PermissionSet>()
            .HaveConversion<SmartEnumFlagsValueConverter<PermissionSet, Permission, string>, 
                            SmartEnumFlagsValueComparer<PermissionSet, Permission, string>>()
            .HaveColumnType("varchar(500)");
    }
}
```
