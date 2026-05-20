using System.Text.Json;
using SebastianGuzmanMorla.SmartEnum.Converters.Json;
using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.UnitTests;

public class JsonConvertersTests
{
    private readonly JsonSerializerOptions _options;

    public JsonConvertersTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new SmartEnumJsonConverter<TestStatus, string>());
        _options.Converters.Add(new SmartEnumFlagsJsonConverter<TestPermissionFlags, TestPermission, string>());
    }

    [Fact]
    public void SmartEnumJsonConverter_Serialize_ReturnsValueAsString()
    {
        // Arrange
        TestStatus status = TestStatus.Active;

        // Act
        string json = JsonSerializer.Serialize(status, _options);

        // Assert
        json.Should().Be("\"Active\"");
    }

    [Fact]
    public void SmartEnumJsonConverter_Deserialize_ValidValue_ReturnsCorrectInstance()
    {
        // Arrange
        string json = "\"Active\"";

        // Act
        TestStatus? result = JsonSerializer.Deserialize<TestStatus>(json, _options);

        // Assert
        result.Should().Be(TestStatus.Active);
    }

    [Fact]
    public void SmartEnumJsonConverter_Deserialize_Null_ReturnsNull()
    {
        // Arrange
        string json = "null";

        // Act
        TestStatus? result = JsonSerializer.Deserialize<TestStatus?>(json, _options);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SmartEnumJsonConverter_Deserialize_InvalidValue_ThrowsSmartEnumException()
    {
        // Arrange
        string json = "\"Invalid\"";

        // Act
        Action act = () => JsonSerializer.Deserialize<TestStatus>(json, _options);

        // Assert
        act.Should().Throw<SmartEnumException>();
    }

    [Fact]
    public void SmartEnumFlagsJsonConverter_Serialize_ReturnsSpaceSeparatedValues()
    {
        // Arrange
        TestPermissionFlags flags = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);

        // Act
        string json = JsonSerializer.Serialize(flags, _options);

        // Assert
        json.Should().Be("\"Read Write\"");
    }

    [Fact]
    public void SmartEnumFlagsJsonConverter_Deserialize_String_ReturnsCorrectFlags()
    {
        // Arrange
        string json = "\"Read Write\"";

        // Act
        TestPermissionFlags? result = JsonSerializer.Deserialize<TestPermissionFlags>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result.Flags.Should().HaveCount(2);
        result.Flags.Should().Contain(TestPermission.Read);
        result.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void SmartEnumFlagsJsonConverter_Deserialize_Array_ReturnsCorrectFlags()
    {
        // Arrange
        string json = "[\"Read\", \"Write\"]";

        // Act
        TestPermissionFlags? result = JsonSerializer.Deserialize<TestPermissionFlags>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result.Flags.Should().HaveCount(2);
        result.Flags.Should().Contain(TestPermission.Read);
        result.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public void SmartEnumFlagsJsonConverter_Deserialize_Null_ReturnsNull()
    {
        // Arrange
        string json = "null";

        // Act
        TestPermissionFlags? result = JsonSerializer.Deserialize<TestPermissionFlags?>(json, _options);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SmartEnumFlagsJsonConverter_Deserialize_InvalidFormat_ThrowsSmartEnumException()
    {
        // Arrange
        string json = "\"Invalid\"";

        // Act
        Action act = () => JsonSerializer.Deserialize<TestPermissionFlags>(json, _options);

        // Assert
        act.Should().Throw<SmartEnumException>();
    }

    [Fact]
    public void SmartEnumFlagsJsonConverter_Deserialize_InvalidArrayElement_ThrowsJsonException()
    {
        // Arrange
        string json = "[\"Read\", \"Invalid\"]";

        // Act
        Action act = () => JsonSerializer.Deserialize<TestPermissionFlags>(json, _options);

        // Assert
        act.Should().Throw<SmartEnumException>();
    }

    [Fact]
    public void SmartEnumJsonConverter_Deserialize_Whitespace_ShouldThrow()
    {
        // Arrange
        string json = "\"   \"";

        // Act
        Action act = () => JsonSerializer.Deserialize<TestStatus>(json, _options);

        // Assert
        act.Should().Throw<SmartEnumException>();
    }
}