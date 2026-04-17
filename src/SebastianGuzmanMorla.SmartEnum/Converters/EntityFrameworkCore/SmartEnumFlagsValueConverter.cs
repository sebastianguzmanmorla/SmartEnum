using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SebastianGuzmanMorla.SmartEnum.Converters.EntityFrameworkCore;

public sealed class SmartEnumFlagsValueConverter<TFlags, TEnum, TValue>() : ValueConverter<TFlags, TValue[]>
(
    enumFlags => enumFlags.ToValueArray(),
    value => SmartEnumFlags<TFlags, TEnum, TValue>.Parse(value)
)
    where TFlags : SmartEnumFlags<TFlags, TEnum, TValue>, new()
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull;