using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.UnitTests;

public class SmartEnumFlagsTests
{
    [Fact]
    public void Constructor_WithFlags_SetsFlagsCorrectly()
    {
        // Arrange
        TestPermission read = TestPermission.Read;
        TestPermission write = TestPermission.Write;

        // Act
        TestPermissionFlags flags = new TestPermissionFlags(read, write);

        // Assert
        flags.Flags.Should().Contain(read);
        flags.Flags.Should().Contain(write);
        flags.Flags.Should().HaveCount(2);
    }

    [Fact]
    public void Parse_SingleValue_ReturnsCorrectFlags()
    {
        // Arrange
        TestPermission expected = TestPermission.Read;

        // Act
        TestPermissionFlags result = TestPermissionFlags.Parse(expected.Value);

        // Assert
        result.Flags.Should().ContainSingle().Which.Should().Be(expected);
    }

    [Fact]
    public void Parse_MultipleValues_ReturnsCorrectFlags()
    {
        // Arrange
        TestPermission read = TestPermission.Read;
        TestPermission write = TestPermission.Write;

        // Act
        TestPermissionFlags result = TestPermissionFlags.Parse($"{read.Value} {write.Value}");

        // Assert
        result.Flags.Should().Contain(read);
        result.Flags.Should().Contain(write);
        result.Flags.Should().HaveCount(2);
    }

    [Fact]
    public void Parse_ArrayValues_ReturnsCorrectFlags()
    {
        // Arrange
        string[] values = ["Read", "Write"];

        // Act
        TestPermissionFlags result = TestPermissionFlags.Parse(values);

        // Assert
        result.Flags.Should().HaveCount(2);
        result.Flags.Should().Contain(TestPermission.Read);
        result.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void Has_FlagExists_ReturnsTrue()
    {
        // Arrange
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        bool hasRead = flags.Has(TestPermission.Read);
        bool hasExecute = flags.Has(TestPermission.Execute);

        // Assert
        hasRead.Should().BeTrue();
        hasExecute.Should().BeFalse();
    }

    [Fact]
    public void ContainsAll_AllFlagsPresent_ReturnsTrue()
    {
        // Arrange
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write, TestPermission.Execute);
        TestPermissionFlags subset = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        bool containsAll = flags.ContainsAll(subset);

        // Assert
        containsAll.Should().BeTrue();
    }

    [Fact]
    public void ContainsAll_NotAllFlagsPresent_ReturnsFalse()
    {
        // Arrange
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        TestPermissionFlags largerSet = new TestPermissionFlags(TestPermission.Read, TestPermission.Write, TestPermission.Execute);

        // Act
        bool containsAll = flags.ContainsAll(largerSet);

        // Assert
        containsAll.Should().BeFalse();
    }

    [Fact]
    public void EqualsAll_SameFlags_ReturnsTrue()
    {
        // Arrange
        TestPermissionFlags flags1 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        TestPermissionFlags flags2 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        bool equals = flags1.EqualsAll(TestPermission.Read, TestPermission.Write);

        // Assert
        equals.Should().BeTrue();
    }

    [Fact]
    public void Add_IncreasesFlagCount()
    {
        // Arrange
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read);

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
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        flags.Remove(TestPermission.Read);

        // Assert
        flags.Flags.Should().ContainSingle().Which.Should().Be(TestPermission.Write);
    }

    [Fact]
    public void CloneAdd_ReturnsNewInstanceWithAddedFlag()
    {
        // Arrange
        TestPermissionFlags original = new TestPermissionFlags(TestPermission.Read);

        // Act
        TestPermissionFlags cloned = original.CloneAdd(TestPermission.Write);

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
        TestPermissionFlags original = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        TestPermissionFlags cloned = original.CloneRemove(TestPermission.Read);

        // Assert
        original.Flags.Should().HaveCount(2);
        cloned.Flags.Should().ContainSingle().Which.Should().Be(TestPermission.Write);
    }

    [Fact]
    public void Operator_Plus_AddsFlag()
    {
        // Arrange
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read);

        // Act
        TestPermissionFlags result = flags | TestPermission.Write;

        // Assert
        result.Flags.Should().HaveCount(2);
        result.Flags.Should().Contain(TestPermission.Read);
        result.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void Operator_Minus_RemovesFlag()
    {
        // Arrange
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        TestPermissionFlags result = flags - TestPermission.Read;

        // Assert
        result.Flags.Should().ContainSingle().Which.Should().Be(TestPermission.Write);
    }

    [Fact]
    public void ToString_ReturnsSpaceSeparatedValues()
    {
        // Arrange
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        string result = flags.ToString();

        // Assert
        result.Should().Be("Read Write");
    }

    [Fact]
    public void ToValueArray_ReturnsOrderedValues()
    {
        // Arrange
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Write, TestPermission.Read); // Order doesn't matter

        // Act
        string[] values = flags.ToValueArray();

        // Assert
        values.Should().Equal("Read", "Write"); // Ordered by Value
    }

    [Fact]
    public void Equality_WorksCorrectly()
    {
        // Arrange
        TestPermissionFlags flags1 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        TestPermissionFlags flags2 = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        TestPermissionFlags flags3 = new TestPermissionFlags(TestPermission.Read);

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
    public void Parse_WithNullInput_ReturnsEmptyFlags()
    {
        // Act
        TestPermissionFlags result = TestPermissionFlags.Parse((string?)null);

        // Assert
        result.Flags.Should().BeEmpty();
    }

    [Fact]
    public void Parse_WithEmptyString_ReturnsEmptyFlags()
    {
        // Act
        TestPermissionFlags result = TestPermissionFlags.Parse("   ");

        // Assert
        result.Flags.Should().BeEmpty();
    }
}