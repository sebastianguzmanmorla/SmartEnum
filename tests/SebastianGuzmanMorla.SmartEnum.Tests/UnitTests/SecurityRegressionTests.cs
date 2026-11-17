using System.Collections.Frozen;
using System.Text.Json;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;
using SebastianGuzmanMorla.SmartEnum.EntityFrameworkCore.Converters;
using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.UnitTests;

public class SecurityRegressionTests
{
    public sealed class Permission : SmartEnum<Permission, string>
    {
        public static readonly Permission Read = new("read");
        public static readonly Permission UpperRead = new("READ");
        public static readonly Permission Admin = new("admin");
        public static readonly Permission Space = new("read admin");
        public static readonly Permission Comma = new("read,admin");
        private Permission(string value) : base(value) { }
        static Permission() => Lookup = new[] { Read, UpperRead, Admin, Space, Comma }.ToFrozenDictionary(x => x.Value);
    }
    public sealed class Seeded : SmartEnumFlags<Seeded, Permission, string>
    {
        public Seeded() : base(Permission.Admin) { }
    }

    public sealed class FormattedFlags : SmartEnumFlags<FormattedFlags, FormattedSecurityPermission, string>
    {
        public FormattedFlags() { }
        public FormattedFlags(params FormattedSecurityPermission[] flags) : base(flags) { }
    }

    public sealed class CustomTextFlags : SmartEnumFlags<CustomTextFlags, Permission, string>
    {
        public CustomTextFlags() { }
        public CustomTextFlags(params Permission[] flags) : base(flags) { }

        public override string ToString() => "read";
    }

    public sealed class RejectConstructionFlags : SmartEnumFlags<RejectConstructionFlags, Permission, string>
    {
        public RejectConstructionFlags() => throw new InvalidOperationException("Unexpected constructor call.");
    }
    private static JsonSerializerOptions Options()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new SmartEnumFlagsJsonConverter<Seeded, Permission, string>());
        return options;
    }
    [Fact]
    public void TextLookupPreservesExactIdentityAndRejectsAmbiguousFallback()
    {
        Assert.Same(Permission.Read, SmartEnum<Permission, string>.Parse("read"));
        Assert.Same(Permission.UpperRead, SmartEnum<Permission, string>.Parse("READ"));
        Assert.Throws<SmartEnumException>(() => SmartEnum<Permission, string>.Parse("Read"));
        Assert.Same(Permission.Admin, SmartEnum<Permission, string>.Parse("ADMIN"));
    }
    [Fact]
    public void ParseAndCopiesDoNotRestoreConstructorDefaults()
    {
        Assert.Empty(Seeded.Parse((string?)null).Flags);
        Assert.Empty(Seeded.Parse(Array.Empty<string>()).Flags);
        var parsed = Seeded.Parse("read");
        Assert.Equal(new[] { Permission.Read }, parsed.Flags);
        Assert.Equal(new[] { Permission.Read }, parsed.Clone().Flags);
        Assert.Empty(parsed.CloneRemove(Permission.Read).Flags);
        Assert.Equal(new[] { Permission.Read }, new Seeded().CloneRemove(Permission.Admin).CloneAdd(Permission.Read).Flags);
        Assert.Empty(JsonSerializer.Deserialize<Seeded>("[]", Options())!.Flags);
        Assert.Equal(new[] { Permission.Read }, JsonSerializer.Deserialize<Seeded>("[\"read\"]", Options())!.Flags);
    }
    [Theory]
    [InlineData("read admin")]
    [InlineData("read,admin")]
    public void DelimitedWritersRejectValuesThatWouldChangeMembership(string value)
    {
        var flags = Seeded.Parse(new[] { value });
        Assert.Throws<SmartEnumException>(() => flags.ToString());
        Assert.Throws<SmartEnumException>(() => JsonSerializer.Serialize(flags, Options()));
        var converter = new SebastianGuzmanMorla.SmartEnum.EntityFrameworkCore.Converters.SmartEnumFlagsValueConverter<Seeded, Permission, string>();
        Assert.Throws<SmartEnumException>(() => converter.ConvertToProviderExpression.Compile()(flags));
    }
    [Fact]
    public void ArrayValidationRejectsInvalidMemberBeforeReadingRemainingTokens()
    {
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("[\"unknown\",123]"));
        reader.Read();
        var converter = new SmartEnumFlagsJsonConverter<Seeded, Permission, string>();
        try { converter.Read(ref reader, typeof(Seeded), Options()); Assert.Fail("Expected unknown member rejection"); }
        catch (SmartEnumException) { Assert.Equal("unknown", reader.GetString()); }
    }
    [Fact]
    public void LegacyDelimitedAndArrayInputsRemainSupported()
    {
        Assert.Equal(new[] { Permission.Read, Permission.Admin }, JsonSerializer.Deserialize<Seeded>("\"read, admin\"", Options())!.Flags);
        Assert.Equal(new[] { Permission.Read }, JsonSerializer.Deserialize<Seeded>("[\"read\",\"read\"]", Options())!.Flags);
        Assert.Null(JsonSerializer.Deserialize<Seeded>("null", Options()));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Seeded>("[123]", Options()));
    }

    [Fact]
    public void CanonicalKeyTakesPrecedenceOverDisplayAliasesAndJsonKeepsTheKey()
    {
        Assert.Same(FormattedSecurityPermission.Canonical, FormattedSecurityPermission.Parse("read"));
        Assert.Same(FormattedSecurityPermission.Canonical, FormattedSecurityPermission.Parse("read-alias"));

        var options = new JsonSerializerOptions();
        options.Converters.Add(new SmartEnumFlagsJsonConverter<FormattedFlags, FormattedSecurityPermission, string>());
        string json = JsonSerializer.Serialize(new FormattedFlags(FormattedSecurityPermission.Canonical), options);

        Assert.Equal("\"read\"", json);
        Assert.Same(FormattedSecurityPermission.Canonical, Assert.Single(JsonSerializer.Deserialize<FormattedFlags>(json, options)!.Flags));
    }

    [Fact]
    public void EfFlagsConverterPreservesSafeCustomTextAndRejectsChangedMembership()
    {
        var converter = new SmartEnumFlagsValueConverter<CustomTextFlags, Permission, string>();
        var write = converter.ConvertToProviderExpression.Compile();
        var read = converter.ConvertFromProviderExpression.Compile();

        string stored = write(new CustomTextFlags(Permission.Read));
        Assert.Equal("read", stored);
        Assert.Same(Permission.Read, Assert.Single(read(stored).Flags));
        Assert.Throws<SmartEnumException>(() => write(new CustomTextFlags(Permission.Admin)));
    }

    [Fact]
    public void DisplayConvertersPreserveAnExistingLabelThatMatchesAnotherKey()
    {
        var efConverter = new SmartEnumFlagsValueConverter<FormattedFlags, FormattedSecurityPermission, string>();
        var write = efConverter.ConvertToProviderExpression.Compile();
        var read = efConverter.ConvertFromProviderExpression.Compile();

        Assert.Same(FormattedSecurityPermission.Legacy, Assert.Single(read("read").Flags));
        Assert.Equal("read", write(new FormattedFlags(FormattedSecurityPermission.Legacy)));

        var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(FormattedSecurityPermission));
        Assert.Equal("read", typeConverter.ConvertToInvariantString(FormattedSecurityPermission.Legacy));
        Assert.Same(FormattedSecurityPermission.Legacy, typeConverter.ConvertFromInvariantString("read"));
    }

    [Fact]
    public void InvalidTopLevelJsonShapeDoesNotConstructAFlagSet()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new SmartEnumFlagsJsonConverter<RejectConstructionFlags, Permission, string>());

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<RejectConstructionFlags>("123", options));
    }
}
