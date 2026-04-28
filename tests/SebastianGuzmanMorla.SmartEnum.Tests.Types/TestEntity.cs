namespace SebastianGuzmanMorla.SmartEnum.Tests.Types;

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TestStatus Status { get; set; } = TestStatus.Active;
    public TestPermissionFlags Permissions { get; set; } = new();
}