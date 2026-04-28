using SebastianGuzmanMorla.SmartEnum.Attributes;

namespace SebastianGuzmanMorla.SmartEnum.Tests.Types;

[GenerateSmartEnum]
public sealed partial class TestPermission(string value) : SmartEnum<TestPermission, string>(value)
{
    public static readonly TestPermission Read = new("Read");
    public static readonly TestPermission Write = new("Write");
    public static readonly TestPermission Execute = new("Execute");
}