using SebastianGuzmanMorla.SmartEnum.Converters.EntityFrameworkCore;
using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.UnitTests;

public class EfCoreConvertersTests
{
    [Fact]
    public void SmartEnumConverter_ConvertToProvider_ReturnsValue()
    {
        // Arrange
        var converter = new SmartEnumConverter<TestStatus, string>();
        var status = TestStatus.Active;

        // Act
        var result = converter.ConvertToProvider(status);

        // Assert
        result.Should().Be(status.Value);
    }

    [Fact]
    public void SmartEnumConverter_ConvertFromProvider_ReturnsCorrectInstance()
    {
        // Arrange
        var converter = new SmartEnumConverter<TestStatus, string>();
        var value = "Active";

        // Act
        var result = converter.ConvertFromProvider(value);

        // Assert
        result.Should().Be(TestStatus.Active);
    }

    [Fact]
    public void SmartEnumComparer_Equals_SameInstances_ReturnsTrue()
    {
        // Arrange
        var comparer = new SmartEnumComparer<TestStatus, string>();
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Active;

        // Act
        var result = comparer.Equals(status1, status2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SmartEnumComparer_Equals_DifferentInstances_ReturnsFalse()
    {
        // Arrange
        var comparer = new SmartEnumComparer<TestStatus, string>();
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Inactive;

        // Act
        var result = comparer.Equals(status1, status2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SmartEnumComparer_GetHashCode_ReturnsConsistentHash()
    {
        // Arrange
        var comparer = new SmartEnumComparer<TestStatus, string>();
        var status = TestStatus.Active;

        // Act
        var hash1 = comparer.GetHashCode(status);
        var hash2 = comparer.GetHashCode(status);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void SmartEnumComparer_Snapshot_ReturnsClone()
    {
        // Arrange
        var comparer = new SmartEnumComparer<TestStatus, string>();
        var original = TestStatus.Active;

        // Act
        var snapshot = comparer.Snapshot(original);

        // Assert
        snapshot.Should().Be(original);
    }

    [Fact]
    public void SmartEnumFlagsValueConverter_ConvertToProvider_ReturnsValueArray()
    {
        // Arrange
        var converter = new SmartEnumFlagsValueConverter<TestPermissionFlags, TestPermission, string>();
        var flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var result = converter.ConvertToProvider(flags);

        // Assert
        result.Should().BeEquivalentTo("Read Write");
    }

    [Fact]
    public void SmartEnumFlagsValueConverter_ConvertFromProvider_ReturnsCorrectFlags()
    {
        // Arrange
        var converter = new SmartEnumFlagsValueConverter<TestPermissionFlags, TestPermission, string>();

        // Act
        var result = (TestPermissionFlags)converter.ConvertFromProvider("Read Write")!;

        // Assert
        result.Flags.Should().HaveCount(2);
        result.Flags.Should().Contain(TestPermission.Read);
        result.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void SmartEnumFlagsValueComparer_Equals_SameInstances_ReturnsTrue()
    {
        // Arrange
        var comparer = new SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string>();
        var flags1 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        var flags2 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var result = comparer.Equals(flags1, flags2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SmartEnumFlagsValueComparer_Equals_DifferentInstances_ReturnsFalse()
    {
        // Arrange
        var comparer = new SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string>();
        var flags1 = new TestPermissionFlags(TestPermission.Read);
        var flags2 = new TestPermissionFlags(TestPermission.Write);

        // Act
        var result = comparer.Equals(flags1, flags2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SmartEnumFlagsValueComparer_GetHashCode_ReturnsConsistentHash()
    {
        // Arrange
        var comparer = new SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string>();
        var flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var hash1 = comparer.GetHashCode(flags);
        var hash2 = comparer.GetHashCode(flags);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void SmartEnumFlagsValueComparer_Snapshot_ReturnsClone()
    {
        // Arrange
        var comparer = new SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string>();
        var original = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var snapshot = comparer.Snapshot(original);

        // Assert
        snapshot.Flags.Should().Equal(original.Flags);
        snapshot.Should().NotBeSameAs(original);
    }
}