namespace SebastianGuzmanMorla.SmartEnum.Tests.Types;

public sealed class TestPermissionFlags : SmartEnumFlags<TestPermissionFlags, TestPermission, string>
{
    public TestPermissionFlags(params TestPermission[] flags) : base(flags) { }
    public TestPermissionFlags() { }
}