# SebastianGuzmanMorla.SmartEnum

Implementación de SmartEnums en .NET con soporte para Source Generators, Entity Framework Core y System.Text.Json.

## Descripción

`SebastianGuzmanMorla.SmartEnum` ofrece una forma moderna de definir enumeraciones de tipo seguro con valores personalizados, parseo robusto, comparación, flags y generación automática de mapas internos.

- Soporta `SmartEnum<TEnum, TValue>` para enums con valores de cualquier tipo no anulable.
- Incluye `SmartEnumFlags<TFlags, TEnum, TValue>` para trabajar con conjuntos de valores.
- Genera automáticamente la tabla de lookup usando el atributo `[GenerateSmartEnum]`.
- Incluye conversores para `System.Text.Json` y `Entity Framework Core`.

## Instalación

Instala el paquete desde NuGet:

```bash
dotnet add package SebastianGuzmanMorla.SmartEnum
```

## Compatibilidad

- Target framework: `net10.0`
- Generador de código: compatible con `netstandard2.0`
- Dependencia de EF Core: `Microsoft.EntityFrameworkCore` v10.0.6

## Uso básico

Define tu enum heredando de `SmartEnum<TEnum, TValue>` y aplica el atributo `[GenerateSmartEnum]`.

```csharp
using SebastianGuzmanMorla.SmartEnum;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;

[JsonConverter(typeof(SmartEnumJsonConverter<Color, string>))]
[GenerateSmartEnum]
public sealed partial class Color : SmartEnum<Color, string>
{
    public static readonly Color Red = new("red");
    public static readonly Color Green = new("green");
    public static readonly Color Blue = new("blue");

    private Color(string value) : base(value)
    {
    }
}
```

El generador crea internamente la tabla de lookup necesaria para `Parse` y `TryParse`.

```csharp
Color selected = Color.Parse("green");
if (Color.TryParse("blue", out Color? result))
{
    Console.WriteLine(result); // blue
}
```

## Uso de SmartEnumFlags

`SmartEnumFlags` permite combinar y comparar conjuntos de valores.

```csharp
using SebastianGuzmanMorla.SmartEnum;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;

[JsonConverter(typeof(SmartEnumJsonConverter<Permission, string>))]
[GenerateSmartEnum]
public partial class Permission : SmartEnum<Permission, string>
{
    public static readonly Permission Read = new("read");
    public static readonly Permission Write = new("write");
    public static readonly Permission Delete = new("delete");

    private Permission(string value) : base(value) { }
}

[JsonConverter(typeof(SmartEnumFlagsJsonConverter<PermissionSet, Permission, string>))]
public class PermissionSet : SmartEnumFlags<PermissionSet, Permission, string>
{
    public PermissionSet() { }
}

var flags = PermissionSet.Parse(new[] { "read", "write" });
if (flags.Has(Permission.Read))
{
    Console.WriteLine("Tiene permiso de lectura");
}
```

## Conversores JSON

Agrega los conversores de `System.Text.Json` para serializar y deserializar `SmartEnum` y `SmartEnumFlags`.

```csharp
using System.Text.Json;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;

var options = new JsonSerializerOptions();
options.Converters.Add(new SmartEnumJsonConverter<Color, string>());

var json = JsonSerializer.Serialize(Color.Red, options);
var color = JsonSerializer.Deserialize<Color>(json, options);
```

Para flags:

```csharp
options.Converters.Add(new SmartEnumFlagsJsonConverter<PermissionSet, Permission, string>());
```

## Conversores Entity Framework Core

Registra los conversores para mapear `SmartEnum` y `SmartEnumFlags` en modelos de EF Core.

```csharp
using Microsoft.EntityFrameworkCore;
using SebastianGuzmanMorla.SmartEnum.Converters.EntityFrameworkCore;

protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    base.ConfigureConventions(configurationBuilder);

    configurationBuilder.Properties<Color>()
        .HaveConversion<SmartEnumConverter<Color, string>, SmartEnumComparer<Color, string>>()
        .HaveColumnType("text");
}
```

Para flags:

```csharp
configurationBuilder.Properties<PermissionSet>()
    .HaveConversion<SmartEnumFlagsValueConverter<PermissionSet, Permission, string>,
        SmartEnumFlagsValueComparer<PermissionSet, Permission, string>>()
    .HaveColumnType("text");
```

## Excepciones

El paquete incluye `SmartEnumException` para manejar errores de parseo y valores inválidos.

## Licencia

Este proyecto se distribuye bajo la licencia MIT. Consulta el archivo `LICENSE` para más detalles.
