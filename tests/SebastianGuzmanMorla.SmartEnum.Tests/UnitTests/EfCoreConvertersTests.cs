using SebastianGuzmanMorla.SmartEnum.EntityFrameworkCore.Converters;
using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.UnitTests;

public class EfCoreConvertersTests
{
    [Fact]
    public void SmartEnumConverter_ConvertToProvider_ReturnsValue()
    {
        // Arrange
        SmartEnumConverter<TestStatus, string> converter = new SmartEnumConverter<TestStatus, string>();
        TestStatus status = TestStatus.Active;

        // Act
        object? result = converter.ConvertToProvider(status);

        // Assert
        result.Should().Be(status.Value);
    }

    [Fact]
    public void SmartEnumConverter_ConvertFromProvider_ReturnsCorrectInstance()
    {
        // Arrange
        SmartEnumConverter<TestStatus, string> converter = new SmartEnumConverter<TestStatus, string>();
        string value = "Active";

        // Act
        object? result = converter.ConvertFromProvider(value);

        // Assert
        result.Should().Be(TestStatus.Active);
    }

    [Fact]
    public void SmartEnumComparer_Equals_SameInstances_ReturnsTrue()
    {
        // Arrange
        SmartEnumComparer<TestStatus, string> comparer = new SmartEnumComparer<TestStatus, string>();
        TestStatus status1 = TestStatus.Active;
        TestStatus status2 = TestStatus.Active;

        // Act
        bool result = comparer.Equals(status1, status2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SmartEnumComparer_Equals_DifferentInstances_ReturnsFalse()
    {
        // Arrange
        SmartEnumComparer<TestStatus, string> comparer = new SmartEnumComparer<TestStatus, string>();
        TestStatus status1 = TestStatus.Active;
        TestStatus status2 = TestStatus.Inactive;

        // Act
        bool result = comparer.Equals(status1, status2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SmartEnumComparer_GetHashCode_ReturnsConsistentHash()
    {
        // Arrange
        SmartEnumComparer<TestStatus, string> comparer = new SmartEnumComparer<TestStatus, string>();
        TestStatus status = TestStatus.Active;

        // Act
        int hash1 = comparer.GetHashCode(status);
        int hash2 = comparer.GetHashCode(status);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void SmartEnumComparer_Snapshot_ReturnsClone()
    {
        // Arrange
        SmartEnumComparer<TestStatus, string> comparer = new SmartEnumComparer<TestStatus, string>();
        TestStatus original = TestStatus.Active;

        // Act
        TestStatus snapshot = comparer.Snapshot(original);

        // Assert
        snapshot.Should().Be(original);
    }

    [Fact]
    public void SmartEnumFlagsValueConverter_ConvertToProvider_ReturnsValueArray()
    {
        // Arrange
        SmartEnumFlagsValueConverter<TestPermissionFlags, TestPermission, string> converter = new SmartEnumFlagsValueConverter<TestPermissionFlags, TestPermission, string>();
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        object? result = converter.ConvertToProvider(flags);

        // Assert
        result.Should().BeEquivalentTo("Read Write");
    }

    [Fact]
    public void SmartEnumFlagsValueConverter_ConvertFromProvider_ReturnsCorrectFlags()
    {
        // Arrange
        SmartEnumFlagsValueConverter<TestPermissionFlags, TestPermission, string> converter = new SmartEnumFlagsValueConverter<TestPermissionFlags, TestPermission, string>();

        // Act
        TestPermissionFlags result = (TestPermissionFlags)converter.ConvertFromProvider("Read Write")!;

        // Assert
        result.Flags.Should().HaveCount(2);
        result.Flags.Should().Contain(TestPermission.Read);
        result.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void SmartEnumFlagsValueComparer_Equals_SameInstances_ReturnsTrue()
    {
        // Arrange
        SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string> comparer = new SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string>();
        TestPermissionFlags flags1 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        TestPermissionFlags flags2 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        bool result = comparer.Equals(flags1, flags2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SmartEnumFlagsValueComparer_Equals_DifferentInstances_ReturnsFalse()
    {
        // Arrange
        SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string> comparer = new SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string>();
        TestPermissionFlags flags1 = new TestPermissionFlags(TestPermission.Read);
        TestPermissionFlags flags2 = new TestPermissionFlags(TestPermission.Write);

        // Act
        bool result = comparer.Equals(flags1, flags2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SmartEnumFlagsValueComparer_GetHashCode_ReturnsConsistentHash()
    {
        // Arrange
        SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string> comparer = new SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string>();
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        int hash1 = comparer.GetHashCode(flags);
        int hash2 = comparer.GetHashCode(flags);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void SmartEnumFlagsValueComparer_Snapshot_ReturnsClone()
    {
        // Arrange
        SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string> comparer = new SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string>();
        TestPermissionFlags original = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        TestPermissionFlags snapshot = comparer.Snapshot(original);

        // Assert
        snapshot.Flags.Should().Equal(original.Flags);
        snapshot.Should().NotBeSameAs(original);
    }
}