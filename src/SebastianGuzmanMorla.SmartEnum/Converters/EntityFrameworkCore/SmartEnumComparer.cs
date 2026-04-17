using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SebastianGuzmanMorla.SmartEnum.Converters.EntityFrameworkCore;

public sealed class SmartEnumComparer<TEnum, TValue>() : ValueComparer<TEnum>
(
    (x, y) => x == y,
    smartEnum => smartEnum.GetHashCode(),
    smartEnum => smartEnum.Clone()
)
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull;