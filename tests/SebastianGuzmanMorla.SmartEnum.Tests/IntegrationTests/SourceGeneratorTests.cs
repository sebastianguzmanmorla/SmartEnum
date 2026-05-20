using System.Collections.Immutable;
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
        string sourceCode = @"
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

        Compilation compilation = CreateCompilation(sourceCode);
        SmartEnumGenerator generator = new SmartEnumGenerator();

        // Act
        CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGenerators(compilation);
        GeneratorDriverRunResult result = driver.GetRunResult();

        // Assert
        result.GeneratedTrees.Should().HaveCount(1);
        string generatedCode = result.GeneratedTrees[0].ToString();

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
        string sourceCode = @"
using SebastianGuzmanMorla.SmartEnum;

namespace TestNamespace
{
    public sealed class TestEnum(string value) : SmartEnum<TestEnum, string>(value)
    {
        public static readonly TestEnum Value1 = new(""Value1"");
    }
}";

        Compilation compilation = CreateCompilation(sourceCode);
        SmartEnumGenerator generator = new SmartEnumGenerator();

        // Act
        CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGenerators(compilation);
        GeneratorDriverRunResult result = driver.GetRunResult();

        // Assert
        result.GeneratedTrees.Should().BeEmpty();
    }

    [Fact]
    public void SmartEnumGenerator_SkipsClassWithoutFields()
    {
        // Arrange
        string sourceCode = @"
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

        Compilation compilation = CreateCompilation(sourceCode);
        SmartEnumGenerator generator = new SmartEnumGenerator();

        // Act
        CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGenerators(compilation);
        GeneratorDriverRunResult result = driver.GetRunResult();

        // Assert
        result.GeneratedTrees.Should().BeEmpty();
    }

    [Fact]
    public void GeneratedCode_CompilesSuccessfully()
    {
        // Arrange
        string sourceCode = @"
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

        Compilation compilation = CreateCompilation(sourceCode);
        SmartEnumGenerator generator = new SmartEnumGenerator();

        // Act
        CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGenerators(compilation);
        GeneratorDriverRunResult result = driver.GetRunResult();

        // Assert
        Compilation outputCompilation = compilation.AddSyntaxTrees(result.GeneratedTrees.ToArray());
        ImmutableArray<Diagnostic> diagnostics = outputCompilation.GetDiagnostics();

        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void GeneratedLookup_IsAccessibleAtRuntime()
    {
        // Arrange - Use the actual TestStatus from unit tests
        TestStatus status = TestStatus.Active;

        // Act
        IReadOnlyCollection<string> keys = TestStatus.Keys;

        // Assert
        keys.Should().NotBeEmpty();
        keys.Should().Contain(status.Value);
    }

    private static Compilation CreateCompilation(string sourceCode)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        // Get the path to System.Runtime from the test project's references
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        string testProjectDir = Path.GetDirectoryName(executingAssembly.Location) ?? string.Empty;

        // Try common paths where System.Runtime might be available
        string[] systemRuntimePaths =
        [
            Path.Combine(testProjectDir, "ref", "net10.0", "System.Runtime.dll"),
            Path.Combine(testProjectDir, "..", "..", "..", "..", "..", "..", "..", "Microsoft.NETCore.App", 
                AppContext.BaseDirectory, "System.Runtime.dll"),
            Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "System.Runtime.dll")
        ];

        string? systemRuntimePath = systemRuntimePaths.FirstOrDefault(p => p != null && File.Exists(p));

        PortableExecutableReference[] references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SmartEnum<,>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(GenerateSmartEnumAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Collections.Frozen.FrozenDictionary).Assembly.Location)
        ];

        // Add System.Runtime if it exists and wasn't already added
        if (systemRuntimePath != null && !references.Contains(MetadataReference.CreateFromFile(systemRuntimePath)))
        {
            references = references.Append(MetadataReference.CreateFromFile(systemRuntimePath)).ToArray();
        }

        return CSharpCompilation.Create(
            "TestCompilation",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}