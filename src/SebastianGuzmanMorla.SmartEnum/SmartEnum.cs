using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SebastianGuzmanMorla.SmartEnum;

public abstract class SmartEnum<TEnum, TValue>(TValue value)
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull
{
    private static readonly object Lock = new();

    protected static FrozenDictionary<TValue, TEnum>? Lookup;
    public TValue Value { get; } = value;

    public static IReadOnlyCollection<TValue> Keys => EnsureLookupInitialized().Keys;

    private static FrozenDictionary<TValue, TEnum> EnsureLookupInitialized()
    {
        lock (Lock)
        {
            if (Lookup is not null)
            {
                return Lookup;
            }

            RuntimeHelpers.RunClassConstructor(typeof(TEnum).TypeHandle);

            return Lookup ?? throw new InvalidOperationException(
                $"SmartEnum '{typeof(TEnum).FullName}' was not initialized. Did you forget [GenerateSmartEnum]?");
        }
    }

    public static bool TryParse(TValue? value, [NotNullWhen(true)] out TEnum? result)
    {
        if (value is not null)
        {
            return EnsureLookupInitialized().TryGetValue(value, out result);
        }

        result = null;
        return false;
    }

    public static TEnum Parse(TValue value)
    {
        return EnsureLookupInitialized().TryGetValue(value, out TEnum? result)
            ? result
            : throw new SmartEnumException($"Invalid {typeof(TEnum).Name}: {value}");
    }

    public static TEnum Parse(string value) => ParseText(value, preferUnderlyingKey: true);

    internal static TEnum ParseText(string value, bool preferUnderlyingKey)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new SmartEnumException($"{typeof(TEnum).Name} cannot be null or empty.");
        }

        FrozenDictionary<TValue, TEnum> lookup = EnsureLookupInitialized();
        if (preferUnderlyingKey && value is TValue key && lookup.TryGetValue(key, out TEnum? keyed)
            && EqualityComparer<TValue>.Default.Equals(keyed.Value, key))
            return keyed;

        TEnum? exact = null;
        TEnum? insensitive = null;
        bool ambiguousExact = false;
        bool ambiguousInsensitive = false;
        foreach (TEnum e in lookup.Values)
        {
            string text = e.ToString();
            if (string.Equals(text, value, StringComparison.Ordinal))
            {
                ambiguousExact |= exact is not null;
                exact = e;
            }
            if (string.Equals(text, value, StringComparison.InvariantCultureIgnoreCase))
            {
                ambiguousInsensitive |= insensitive is not null;
                insensitive = e;
            }
        }

        if (!ambiguousExact && exact is not null)
            return exact;
        if (exact is null && !ambiguousInsensitive && insensitive is not null)
            return insensitive;
        throw new SmartEnumException($"Invalid {typeof(TEnum).Name}: {value}");
    }

    public TEnum Clone()
    {
        return (TEnum)this;
    }

    public override string ToString()
    {
        return Value.ToString() ?? string.Empty;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(typeof(TEnum), Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is TEnum other && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
    }

    public static implicit operator TValue(SmartEnum<TEnum, TValue> smartEnum)
    {
        return smartEnum.Value;
    }

    public static bool operator ==(SmartEnum<TEnum, TValue>? a, SmartEnum<TEnum, TValue>? b)
    {
        return ReferenceEquals(a, b)
               || (a is not null && b is not null &&
                   EqualityComparer<TValue>.Default.Equals(a.Value, b.Value));
    }

    public static bool operator !=(SmartEnum<TEnum, TValue>? a, SmartEnum<TEnum, TValue>? b)
    {
        return !(a == b);
    }
}
