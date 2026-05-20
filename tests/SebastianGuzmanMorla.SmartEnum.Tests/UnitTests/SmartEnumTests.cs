using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.UnitTests;

public class SmartEnumTests
{
    [Fact]
    public void Parse_ValidValue_ReturnsCorrectInstance()
    {
        // Arrange
        string value = "Active";
        TestStatus expected = TestStatus.Active;

        // Act
        TestStatus result = TestStatus.Parse(value);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Parse_InvalidValue_ThrowsSmartEnumException()
    {
        // Arrange
        string invalidValue = "Invalid";

        // Act
        Action act = () => TestStatus.Parse(invalidValue);

        // Assert
        act.Should().Throw<SmartEnumException>()
            .WithMessage($"Invalid {nameof(TestStatus)}: {invalidValue}");
    }

    [Theory]
    [InlineData("Active", true)]
    [InlineData("Invalid", false)]
    public void TryParse_ValidAndInvalidValues_ReturnsExpectedResult(string value, bool expectedSuccess)
    {
        // Arrange & Act
        bool success = TestStatus.TryParse(value, out TestStatus? result);

        // Assert
        success.Should().Be(expectedSuccess);
        if (expectedSuccess)
        {
            result.Should().NotBeNull();
            result.Value.Should().Be(value);
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
        string[] expectedKeys = ["Active", "Inactive"];

        // Act
        IReadOnlyCollection<string> keys = TestStatus.Keys;

        // Assert
        keys.Should().BeEquivalentTo(expectedKeys);
    }

    [Fact]
    public void EqualityOperators_WorkCorrectly()
    {
        // Arrange
        TestStatus status1 = TestStatus.Active;
        TestStatus status2 = TestStatus.Active;
        TestStatus status3 = TestStatus.Inactive;

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
        TestStatus status = TestStatus.Active;

        // Act
        string result = status.ToString();

        // Assert
        result.Should().Be(status.Value);
    }

    [Fact]
    public void Clone_ReturnsSameInstance()
    {
        // Arrange
        TestStatus original = TestStatus.Active;

        // Act
        TestStatus cloned = original.Clone();

        // Assert
        cloned.Should().BeSameAs(original);
    }

    [Fact]
    public void GetHashCode_IsConsistent()
    {
        // Arrange
        TestStatus status1 = TestStatus.Active;
        TestStatus status2 = TestStatus.Active;

        // Act
        int hash1 = status1.GetHashCode();
        int hash2 = status2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Parse_EmptyOrWhitespaceString_ThrowsSmartEnumException(string value)
    {
        // Act
        Action act = () => TestStatus.Parse(value);

        // Assert
        act.Should().Throw<SmartEnumException>()
            .WithMessage($"{nameof(TestStatus)} cannot be null or empty.");
    }

    [Fact]
    public void Parse_CaseInsensitiveString_ReturnsCorrectInstance()
    {
        // Arrange
        string value = "aCtIvE";
        TestStatus expected = TestStatus.Active;

        // Act
        TestStatus result = TestStatus.Parse(value);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Equals_WithNullOrDifferentType_ReturnsFalse()
    {
        // Arrange
        TestStatus? status = TestStatus.Active;

        // Act & Assert
        status.Equals(null).Should().BeFalse();
        status.Equals("Active").Should().BeFalse();
    }

    [Fact]
    public void Operator_Equals_WithNull_ReturnsCorrectResult()
    {
        // Arrange
        TestStatus? status1 = null;
        TestStatus? status2 = null;
        TestStatus status3 = TestStatus.Active;

        // Act & Assert
        (status1 == status2).Should().BeTrue();
        (status1 == status3).Should().BeFalse();
        (status3 == status1).Should().BeFalse();
        (status1 != status2).Should().BeFalse();
        (status1 != status3).Should().BeTrue();
    }

    [Fact]
    public void Operator_Equals_SymmetricWithTValue_Works()
    {
        // Arrange
        TestStatus status = TestStatus.Active;
        string value = "Active";

        // Act & Assert
        (value == status).Should().BeTrue();
        (status == value).Should().BeTrue();
        (value != status).Should().BeFalse();
        (status != value).Should().BeFalse();
    }
}