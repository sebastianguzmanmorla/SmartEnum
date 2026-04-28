namespace SebastianGuzmanMorla.SmartEnum.Tests.TestData;

public static class TestDataGenerator
{
    public static Faker<string> CreateValidStringFaker()
    {
        return new Faker<string>()
            .RuleFor(s => s, f => f.Lorem.Word());
    }

    public static Faker<string> CreateInvalidStringFaker()
    {
        return new Faker<string>()
            .RuleFor(s => s, f => f.Lorem.Word() + "_invalid");
    }

    public static IEnumerable<object[]> GetInvalidStringValues()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { "InvalidValue" };
        yield return new object[] { "123" };
    }

    public static IEnumerable<object[]> GetValidStringValues()
    {
        yield return new object[] { "Active" };
        yield return new object[] { "Inactive" };
        yield return new object[] { "Read" };
        yield return new object[] { "Write" };
    }
}