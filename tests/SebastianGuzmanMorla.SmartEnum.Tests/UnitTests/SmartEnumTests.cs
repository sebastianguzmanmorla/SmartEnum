using AutoFixture.Xunit2;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.UnitTests;

public class SmartEnumTests
{
    [Fact]
    public void Parse_ValidValue_ReturnsCorrectInstance()
    {
        // Arrange
        var value = "Active";
        var expected = TestStatus.Active;

        // Act
        var result = TestStatus.Parse(value);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Parse_InvalidValue_ThrowsSmartEnumException()
    {
        // Arrange
        var invalidValue = "Invalid";

        // Act
        Action act = () => TestStatus.Parse(invalidValue);

        // Assert
        act.Should().Throw<SmartEnumException>()
            .WithMessage($"Invalid {typeof(TestStatus).Name}: {invalidValue}");
    }

    [Theory]
    [InlineData("Active", true)]
    [InlineData("Invalid", false)]
    public void TryParse_ValidAndInvalidValues_ReturnsExpectedResult(string value, bool expectedSuccess)
    {
        // Arrange & Act
        var success = TestStatus.TryParse(value, out var result);

        // Assert
        success.Should().Be(expectedSuccess);
        if (expectedSuccess)
        {
            result.Should().NotBeNull();
            result!.Value.Should().Be(value);
        }
        else
        {
            result.Should().BeNull();
        }
    }

    [Fact]
    public void Keys_ReturnsAllDefinedValues()
    {
        // Arrange
        var expectedKeys = new[] { "Active", "Inactive" };

        // Act
        var keys = TestStatus.Keys;

        // Assert
        keys.Should().BeEquivalentTo(expectedKeys);
    }

    [Fact]
    public void EqualityOperators_WorkCorrectly()
    {
        // Arrange
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Active;
        var status3 = TestStatus.Inactive;

        // Act & Assert
        (status1 == status2).Should().BeTrue();
        (status1 == status3).Should().BeFalse();
        (status1 != status3).Should().BeTrue();
        (status1 != status2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var status = TestStatus.Active;

        // Act
        var result = status.ToString();

        // Assert
        result.Should().Be(status.Value);
    }

    [Fact]
    public void Clone_ReturnsSameInstance()
    {
        // Arrange
        var original = TestStatus.Active;

        // Act
        var cloned = original.Clone();

        // Assert
        cloned.Should().BeSameAs(original);
    }

    [Fact]
    public void GetHashCode_IsConsistent()
    {
        // Arrange
        var status1 = TestStatus.Active;
        var status2 = TestStatus.Active;

        // Act
        var hash1 = status1.GetHashCode();
        var hash2 = status2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }
}