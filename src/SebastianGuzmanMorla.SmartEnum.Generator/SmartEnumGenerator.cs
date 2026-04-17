using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SebastianGuzmanMorla.SmartEnum.Generator;

[Generator]
public sealed class SmartEnumGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<INamedTypeSymbol> enums = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "SebastianGuzmanMorla.SmartEnum.Attributes.GenerateSmartEnumAttribute",
                static (n, _) => n is ClassDeclarationSyntax,
                static (ctx, _) => (INamedTypeSymbol)ctx.TargetSymbol);

        context.RegisterSourceOutput(enums, Generate);
    }

    private static void Generate(SourceProductionContext ctx, INamedTypeSymbol enumType)
    {
        INamedTypeSymbol? baseType = enumType.BaseType;
        if (baseType is null ||
            baseType.Name != "SmartEnum" ||
            baseType.TypeArguments.Length != 2)
        {
            return;
        }

        ITypeSymbol valueType = baseType.TypeArguments[1];

        List<IFieldSymbol> fields = enumType.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f =>
                f.IsStatic &&
                f.DeclaredAccessibility == Accessibility.Public &&
                SymbolEqualityComparer.Default.Equals(f.Type, enumType))
            .ToList();

        if (fields.Count == 0)
        {
            return;
        }

        string ns = enumType.ContainingNamespace.ToDisplayString();
        string name = enumType.Name;
        string valueTypeName = valueType.ToDisplayString();

        string valueMap = string.Join(",\n            ",
            fields.Select(f => $"[{GetValueExpression(f)}] = {f.Name}"));

        string source = $$"""
                          using System.Collections.Frozen;
                          using System.Collections.Generic;

                          namespace {{ns}};

                          partial class {{name}}
                          {
                              static {{name}}()
                              {
                                  Lookup = new Dictionary<{{valueTypeName}}, {{name}}>
                                  {
                                      {{valueMap}}
                                  }.ToFrozenDictionary();
                              }
                          }
                          """;

        ctx.AddSource($"{name}.SmartEnum.g.cs", source);
    }

    private static string GetValueExpression(IFieldSymbol field)
    {
        VariableDeclaratorSyntax declarator = (VariableDeclaratorSyntax)
            field.DeclaringSyntaxReferences[0].GetSyntax();

        ExpressionSyntax? initializer = declarator.Initializer?.Value;

        return initializer switch
        {
            ObjectCreationExpressionSyntax o => o.ArgumentList!.Arguments[0].Expression.ToString(),
            ImplicitObjectCreationExpressionSyntax i => i.ArgumentList.Arguments[0].Expression.ToString(),
            _ => throw new InvalidOperationException(
                $"Unsupported initializer for SmartEnum field '{field.Name}'")
        };
    }
}