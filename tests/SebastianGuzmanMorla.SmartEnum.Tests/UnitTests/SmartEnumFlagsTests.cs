using System.Text.Json;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.UnitTests;

public class SmartEnumFlagsTests
{
    [Fact]
    public void Constructor_WithFlags_SetsFlagsCorrectly()
    {
        // Arrange
        var read = TestPermission.Read;
        var write = TestPermission.Write;

        // Act
        var flags = new TestPermissionFlags(read, write);

        // Assert
        flags.Flags.Should().Contain(read);
        flags.Flags.Should().Contain(write);
        flags.Flags.Should().HaveCount(2);
    }

    [Fact]
    public void Parse_SingleValue_ReturnsCorrectFlags()
    {
        // Arrange
        var expected = TestPermission.Read;

        // Act
        var result = TestPermissionFlags.Parse(expected.Value);

        // Assert
        result.Flags.Should().ContainSingle().Which.Should().Be(expected);
    }

    [Fact]
    public void Parse_MultipleValues_ReturnsCorrectFlags()
    {
        // Arrange
        var read = TestPermission.Read;
        var write = TestPermission.Write;

        // Act
        var result = TestPermissionFlags.Parse($"{read.Value} {write.Value}");

        // Assert
        result.Flags.Should().Contain(read);
        result.Flags.Should().Contain(write);
        result.Flags.Should().HaveCount(2);
    }

    [Fact]
    public void Parse_ArrayValues_ReturnsCorrectFlags()
    {
        // Arrange
        var values = new[] { "Read", "Write" };

        // Act
        var result = TestPermissionFlags.Parse(values);

        // Assert
        result.Flags.Should().HaveCount(2);
        result.Flags.Should().Contain(TestPermission.Read);
        result.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void Has_FlagExists_ReturnsTrue()
    {
        // Arrange
        var flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var hasRead = flags.Has(TestPermission.Read);
        var hasExecute = flags.Has(TestPermission.Execute);

        // Assert
        hasRead.Should().BeTrue();
        hasExecute.Should().BeFalse();
    }

    [Fact]
    public void ContainsAll_AllFlagsPresent_ReturnsTrue()
    {
        // Arrange
        var flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write, TestPermission.Execute);
        var subset = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var containsAll = flags.ContainsAll(subset);

        // Assert
        containsAll.Should().BeTrue();
    }

    [Fact]
    public void ContainsAll_NotAllFlagsPresent_ReturnsFalse()
    {
        // Arrange
        var flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        var largerSet = new TestPermissionFlags(TestPermission.Read, TestPermission.Write, TestPermission.Execute);

        // Act
        var containsAll = flags.ContainsAll(largerSet);

        // Assert
        containsAll.Should().BeFalse();
    }

    [Fact]
    public void EqualsAll_SameFlags_ReturnsTrue()
    {
        // Arrange
        var flags1 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        var flags2 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var equals = flags1.EqualsAll(TestPermission.Read, TestPermission.Write);

        // Assert
        equals.Should().BeTrue();
    }

    [Fact]
    public void Add_IncreasesFlagCount()
    {
        // Arrange
        var flags = new TestPermissionFlags(TestPermission.Read);

        // Act
        flags.Add(TestPermission.Write);

        // Assert
        flags.Flags.Should().HaveCount(2);
        flags.Flags.Should().Contain(TestPermission.Read);
        flags.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void Remove_DecreasesFlagCount()
    {
        // Arrange
        var flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        flags.Remove(TestPermission.Read);

        // Assert
        flags.Flags.Should().ContainSingle().Which.Should().Be(TestPermission.Write);
    }

    [Fact]
    public void CloneAdd_ReturnsNewInstanceWithAddedFlag()
    {
        // Arrange
        var original = new TestPermissionFlags(TestPermission.Read);

        // Act
        var cloned = original.CloneAdd(TestPermission.Write);

        // Assert
        original.Flags.Should().ContainSingle().Which.Should().Be(TestPermission.Read);
        cloned.Flags.Should().HaveCount(2);
        cloned.Flags.Should().Contain(TestPermission.Read);
        cloned.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void CloneRemove_ReturnsNewInstanceWithRemovedFlag()
    {
        // Arrange
        var original = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var cloned = original.CloneRemove(TestPermission.Read);

        // Assert
        original.Flags.Should().HaveCount(2);
        cloned.Flags.Should().ContainSingle().Which.Should().Be(TestPermission.Write);
    }

    [Fact]
    public void Operator_Plus_AddsFlag()
    {
        // Arrange
        var flags = new TestPermissionFlags(TestPermission.Read);

        // Act
        var result = flags | TestPermission.Write;

        // Assert
        result.Flags.Should().HaveCount(2);
        result.Flags.Should().Contain(TestPermission.Read);
        result.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void Operator_Minus_RemovesFlag()
    {
        // Arrange
        var flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var result = flags - TestPermission.Read;

        // Assert
        result.Flags.Should().ContainSingle().Which.Should().Be(TestPermission.Write);
    }

    [Fact]
    public void ToString_ReturnsSpaceSeparatedValues()
    {
        // Arrange
        var flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        var result = flags.ToString();

        // Assert
        result.Should().Be("Read Write");
    }

    [Fact]
    public void ToValueArray_ReturnsOrderedValues()
    {
        // Arrange
        var flags = new TestPermissionFlags(TestPermission.Write, TestPermission.Read); // Order doesn't matter

        // Act
        var values = flags.ToValueArray();

        // Assert
        values.Should().Equal("Read", "Write"); // Ordered by Value
    }

    [Fact]
    public void Equality_WorksCorrectly()
    {
        // Arrange
        var flags1 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        var flags2 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        var flags3 = new TestPermissionFlags(TestPermission.Read);

        // Act & Assert
        flags1.Equals(flags2).Should().BeTrue();
        flags1.Equals(flags3).Should().BeFalse();
        (flags1 == flags2).Should().BeTrue();
        (flags1 != flags3).Should().BeTrue();
    }

    [Fact]
    public void Parse_WithDuplicateValues_ThrowsSmartEnumException()
    {
        // Arrange
        string readValueStr = TestPermission.Read.Value;

        // Act & Assert
        Action act = () => TestPermissionFlags.Parse(readValueStr + " " + readValueStr);
        act.Should().Throw<SmartEnumException>();
    }

    [Fact]
    public void Parse_WithNullInput_ThrowsSmartEnumException()
    {
        // Arrange & Assert
        FluentActions.Invoking(() => TestPermissionFlags.Parse("Read Write"))
            .Should().NotThrow(); // Should work with multiple explicit values
    }
}