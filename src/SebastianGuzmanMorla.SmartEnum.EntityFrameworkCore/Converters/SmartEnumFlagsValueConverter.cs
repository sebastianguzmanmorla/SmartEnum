using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SebastianGuzmanMorla.SmartEnum.EntityFrameworkCore.Converters;

public sealed class SmartEnumFlagsValueConverter<TFlags, TEnum, TValue>() : ValueConverter<TFlags, string>
(
    enumFlags => ToProviderValue(enumFlags),
    value => SmartEnumFlags<TFlags, TEnum, TValue>.Parse(value)
)
    where TFlags : SmartEnumFlags<TFlags, TEnum, TValue>, new()
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull
{
    private static string ToProviderValue(TFlags flags)
    {
        string value = flags.ToString();
        TFlags parsed = SmartEnumFlags<TFlags, TEnum, TValue>.Parse(value);
        if (flags.Flags.Count != parsed.Flags.Count
            || !flags.Flags.Select(flag => flag.Value).ToHashSet().SetEquals(parsed.Flags.Select(flag => flag.Value)))
            throw new SmartEnumException($"{typeof(TFlags).Name} cannot be represented safely as a delimited flag set.");

        return value;
    }
}
