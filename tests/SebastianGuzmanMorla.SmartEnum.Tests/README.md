# SebastianGuzmanMorla.SmartEnum - Test Suite

Este proyecto contiene la suite completa de pruebas unitarias e integración para la biblioteca SebastianGuzmanMorla.SmartEnum.

## Estructura de Pruebas

```
tests/
├── SebastianGuzmanMorla.SmartEnum.Tests.csproj    # Proyecto de pruebas
├── .runsettings                                   # Configuración de ejecución
├── UnitTests/                                     # Pruebas unitarias
│   ├── SmartEnumTests.cs                          # Pruebas de SmartEnum<TEnum, TValue>
│   ├── SmartEnumFlagsTests.cs                     # Pruebas de SmartEnumFlags<TFlags, TEnum, TValue>
│   ├── JsonConvertersTests.cs                     # Pruebas de conversores JSON
│   └── EfCoreConvertersTests.cs                   # Pruebas de conversores EF Core
├── IntegrationTests/                              # Pruebas de integración
│   ├── SourceGeneratorTests.cs                    # Pruebas del generador de código
│   └── EfCoreIntegrationTests.cs                  # Pruebas de integración con EF Core
├── TestData/                                      # Datos de prueba
│   └── TestDataGenerator.cs                       # Generadores de datos con Bogus
└── Helpers/                                       # Utilidades de prueba
    └── AutoFixtureCustomizations.cs               # Personalizaciones de AutoFixture
```

## Tecnologías Utilizadas

- **Framework de Pruebas**: xUnit 2.9.2
- **Aserciones**: FluentAssertions 7.0.0 (legibilidad y expresividad)
- **Mocks**: NSubstitute 5.3.0 (preferido sobre Moq por simplicidad)
- **Generación de Datos**:
  - AutoFixture 4.18.1 con AutoFixture.Xunit2 4.18.1 (datos de prueba automáticos)
  - Bogus 35.6.1 (datos fake realistas)
- **Base de Datos de Pruebas**: Microsoft.EntityFrameworkCore.InMemory 10.0.6

## Patrones de Prueba

### AAA (Arrange, Act, Assert)

Todas las pruebas siguen el patrón AAA para mantener consistencia y legibilidad:

```csharp
[Fact]
public void MethodName_Condition_ExpectedResult()
{
    // Arrange
    var sut = new SystemUnderTest();

    // Act
    var result = sut.Method();

    // Assert
    result.Should().Be(expectedValue);
}
```

### Nombres de Pruebas

Los nombres de pruebas siguen la convención: `MethodName_Condition_ExpectedResult`

### Datos de Prueba

- **AutoFixture**: Genera automáticamente objetos de prueba complejos
- **Bogus**: Crea datos fake realistas (nombres, direcciones, etc.)
- **InlineData**: Para casos específicos y edge cases

## Ejecución de Pruebas

### Ejecutar Todas las Pruebas

```bash
dotnet test
```

### Ejecutar con Cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Ejecutar Pruebas Específicas

```bash
# Unitarias
dotnet test --filter "UnitTests"

# De integración
dotnet test --filter "IntegrationTests"

# Una clase específica
dotnet test --filter "SmartEnumTests"
```

### Ejecutar en Paralelo

Las pruebas están configuradas para ejecutarse en paralelo (hasta 4 hilos) para mejorar performance.

## Mejores Prácticas Implementadas

### 1. **Clean Testing**

- Pruebas legibles y expresivas
- Nombres descriptivos
- Estructura consistente
- Separación clara de responsabilidades

### 2. **Test Data Management**

- Evitar datos "hardcodeados"
- Usar generadores automáticos
- Datos realistas con Bogus
- Edge cases con InlineData

### 3. **Mocking Strategy**

- NSubstitute para simplicidad
- Mínimo mocking necesario
- Interfaces limpias para testabilidad

### 4. **Integration Testing**

- Base de datos In-Memory para EF Core
- Verificación end-to-end del generador
- Pruebas de serialización/deserialización

### 5. **Performance**

- Pruebas paralelas
- Setup mínimo por prueba
- Limpieza automática de recursos

## Cobertura de Pruebas

### Unitarias

- ✅ SmartEnum: Parse, TryParse, operadores, propiedades
- ✅ SmartEnumFlags: operaciones de conjunto, parse, flags
- ✅ Conversores JSON: serialización y deserialización
- ✅ Conversores EF Core: conversiones y comparadores

### Integración

- ✅ Generador de código: verificación de código generado
- ✅ EF Core: consultas, guardado, change tracking
- ✅ JSON: serialización completa con conversores

## Configuración de CI/CD

Para integración continua, ejecutar:

```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Tests'
  inputs:
    command: 'test'
    projects: 'tests/SebastianGuzmanMorla.SmartEnum.Tests.csproj'
    arguments: '--configuration $(BuildConfiguration) --collect "Code coverage"'
```

## Troubleshooting

### Problemas Comunes

1. **Generador no funciona**: Verificar que el proyecto de generator esté referenciado
2. **EF Core falla**: Usar InMemory provider para pruebas
3. **Datos inconsistentes**: Usar fixtures con semillas fijas

### Debug de Pruebas

- Usar `Debugger.Launch()` para debugging interactivo
- Logs detallados con `ITestOutputHelper`
- Verificar estado de base de datos en pruebas de integración
