using SebastianGuzmanMorla.SmartEnum.Attributes;

namespace SebastianGuzmanMorla.SmartEnum.Tests.Types;

[GenerateSmartEnum]
public sealed partial class TestStatus(string value) : SmartEnum<TestStatus, string>(value)
{
    public static readonly TestStatus Active = new("Active");
    public static readonly TestStatus Inactive = new("Inactive");
}