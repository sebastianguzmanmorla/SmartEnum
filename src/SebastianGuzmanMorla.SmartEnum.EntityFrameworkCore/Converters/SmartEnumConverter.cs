using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SebastianGuzmanMorla.SmartEnum.EntityFrameworkCore.Converters;

public sealed class SmartEnumConverter<TEnum, TValue>() : ValueConverter<TEnum, TValue>
(
    smartEnum => smartEnum.Value,
    value => SmartEnum<TEnum, TValue>.Parse(value)
)
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull;
