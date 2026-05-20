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
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return ["InvalidValue"];
        yield return ["123"];
    }

    public static IEnumerable<object[]> GetValidStringValues()
    {
        yield return ["Active"];
        yield return ["Inactive"];
        yield return ["Read"];
        yield return ["Write"];
    }
}