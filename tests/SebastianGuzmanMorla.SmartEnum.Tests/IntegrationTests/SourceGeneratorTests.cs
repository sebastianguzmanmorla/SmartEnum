using System.Runtime.Loader;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using SebastianGuzmanMorla.SmartEnum.Generator;
using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.IntegrationTests;

public class SourceGeneratorTests
{
    [Fact]
    public void SmartEnumGenerator_GeneratesLookupForMarkedClass()
    {
        // Arrange
        var sourceCode = @"
using SebastianGuzmanMorla.SmartEnum;
using SebastianGuzmanMorla.SmartEnum.Attributes;

namespace TestNamespace
{
    [GenerateSmartEnum]
    public sealed partial class TestEnum(string value) : SmartEnum<TestEnum, string>(value)
    {
        public static readonly TestEnum Value1 = new(""Value1"");
        public static readonly TestEnum Value2 = new(""Value2"");
    }
}";

        var compilation = CreateCompilation(sourceCode);
        var generator = new SmartEnumGenerator();

        // Act
        var driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGenerators(compilation);
        var result = driver.GetRunResult();

        // Assert
        result.GeneratedTrees.Should().HaveCount(1);
        var generatedCode = result.GeneratedTrees[0].ToString();

        generatedCode.Should().Contain("namespace TestNamespace;");
        generatedCode.Should().Contain("partial class TestEnum");
        generatedCode.Should().Contain("\"Value1\"");
        generatedCode.Should().Contain("\"Value2\"");
        generatedCode.Should().Contain("ToFrozenDictionary()");
    }

    [Fact]
    public void SmartEnumGenerator_SkipsUnmarkedClass()
    {
        // Arrange
        var sourceCode = @"
using SebastianGuzmanMorla.SmartEnum;

namespace TestNamespace
{
    public sealed class TestEnum(string value) : SmartEnum<TestEnum, string>(value)
    {
        public static readonly TestEnum Value1 = new(""Value1"");
    }
}";

        var compilation = CreateCompilation(sourceCode);
        var generator = new SmartEnumGenerator();

        // Act
        var driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGenerators(compilation);
        var result = driver.GetRunResult();

        // Assert
        result.GeneratedTrees.Should().BeEmpty();
    }

    [Fact]
    public void SmartEnumGenerator_SkipsClassWithoutFields()
    {
        // Arrange
        var sourceCode = @"
using SebastianGuzmanMorla.SmartEnum;
using SebastianGuzmanMorla.SmartEnum.Attributes;

namespace TestNamespace
{
    [GenerateSmartEnum]
    public sealed partial class TestEnum(string value) : SmartEnum<TestEnum, string>(value)
    {
        // No static readonly fields
    }
}";

        var compilation = CreateCompilation(sourceCode);
        var generator = new SmartEnumGenerator();

        // Act
        var driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGenerators(compilation);
        var result = driver.GetRunResult();

        // Assert
        result.GeneratedTrees.Should().BeEmpty();
    }

    [Fact]
    public void GeneratedCode_CompilesSuccessfully()
    {
        // Arrange
        var sourceCode = @"
using SebastianGuzmanMorla.SmartEnum;
using SebastianGuzmanMorla.SmartEnum.Attributes;

namespace TestNamespace
{
    [GenerateSmartEnum]
    public sealed partial class TestEnum(string value) : SmartEnum<TestEnum, string>(value)
    {
        public static readonly TestEnum Value1 = new(""Value1"");
        public static readonly TestEnum Value2 = new(""Value2"");
    }
}";

        var compilation = CreateCompilation(sourceCode);
        var generator = new SmartEnumGenerator();

        // Act
        var driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGenerators(compilation);
        var result = driver.GetRunResult();

        // Assert
        var outputCompilation = compilation.AddSyntaxTrees(result.GeneratedTrees.ToArray());
        var diagnostics = outputCompilation.GetDiagnostics();

        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void GeneratedLookup_IsAccessibleAtRuntime()
    {
        // Arrange - Use the actual TestStatus from unit tests
        var status = TestStatus.Active;

        // Act
        var keys = TestStatus.Keys;

        // Assert
        keys.Should().NotBeEmpty();
        keys.Should().Contain(status.Value);
    }

    private static Compilation CreateCompilation(string sourceCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        // Get the path to System.Runtime from the test project's references
        var executingAssembly = Assembly.GetExecutingAssembly();
        var testProjectDir = Path.GetDirectoryName(executingAssembly.Location) ?? string.Empty;

        // Try common paths where System.Runtime might be available
        var systemRuntimePaths = new[]
        {
            Path.Combine(testProjectDir, "ref", "net10.0", "System.Runtime.dll"),
            Path.Combine(testProjectDir, "..", "..", "..", "..", "..", "..", "..", "Microsoft.NETCore.App", 
                AppContext.BaseDirectory, "System.Runtime.dll"),
            Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "System.Runtime.dll"),
        };

        var systemRuntimePath = systemRuntimePaths.FirstOrDefault(p => p != null && File.Exists(p));

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SmartEnum<,>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(GenerateSmartEnumAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Collections.Frozen.FrozenDictionary).Assembly.Location)
        };

        // Add System.Runtime if it exists and wasn't already added
        if (systemRuntimePath != null && !references.Contains(MetadataReference.CreateFromFile(systemRuntimePath)))
        {
            references = references.Append(MetadataReference.CreateFromFile(systemRuntimePath)).ToArray();
        }

        return CSharpCompilation.Create(
            "TestCompilation",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}