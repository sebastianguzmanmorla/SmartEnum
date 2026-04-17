using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SebastianGuzmanMorla.SmartEnum.Converters.EntityFrameworkCore;

public sealed class SmartEnumFlagsValueComparer<TFlags, TEnum, TValue>() : ValueComparer<TFlags>
(
    (x, y) => ReferenceEquals(x, y) || (x != null && y != null && x.Equals(y)),
    enumFlags => enumFlags.GetHashCode(),
    enumFlags => enumFlags.Clone()
)
    where TFlags : SmartEnumFlags<TFlags, TEnum, TValue>, new()
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull;