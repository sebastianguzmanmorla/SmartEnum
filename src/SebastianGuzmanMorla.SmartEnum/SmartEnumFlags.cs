namespace SebastianGuzmanMorla.SmartEnum;

public abstract class SmartEnumFlags<TFlags, TEnum, TValue> : IEquatable<TFlags>, IEqualityComparer<TFlags>
    where TFlags : SmartEnumFlags<TFlags, TEnum, TValue>, new()
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull
{
    private readonly HashSet<TEnum> _flags;

    protected SmartEnumFlags(params TEnum[] flags)
    {
        _flags = flags.Distinct().ToHashSet();
    }

    protected SmartEnumFlags()
    {
        _flags = [];
    }

    public IReadOnlyCollection<TEnum> Flags => _flags;

    public bool Equals(TFlags? x, TFlags? y)
    {
        return ReferenceEquals(x, y) || (x?.Equals(y) ?? false);
    }

    public int GetHashCode(TFlags obj)
    {
        return obj.GetHashCode();
    }

    public bool Equals(TFlags? obj)
    {
        return obj is not null && _flags.SetEquals(obj._flags);
    }

    public static TFlags Parse(string? raw)
    {
        TFlags instance = new();

        string[] parts = raw?.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries) ?? [];

        foreach (string part in parts)
        {
            TEnum value = SmartEnum<TEnum, TValue>.Parse(part);

            if (!instance._flags.Add(value))
            {
                throw new SmartEnumException($"Invalid {typeof(TEnum).Name}: {value} already exists");
            }
        }

        return instance;
    }

    public static TFlags Parse(TValue[] parts)
    {
        TFlags instance = new();

        foreach (TValue part in parts)
        {
            TEnum value = SmartEnum<TEnum, TValue>.Parse(part);

            instance._flags.Add(value);
        }

        return instance;
    }

    public bool ContainsAll(TFlags requested)
    {
        return requested.Flags.All(_flags.Contains);
    }

    public bool EqualsAll(params TEnum[] requested)
    {
        return _flags.SetEquals(requested);
    }

    public bool Has(TEnum flag)
    {
        return _flags.Contains(flag);
    }

    public TFlags CloneAdd(TEnum flag)
    {
        TFlags copy = new();
        foreach (TEnum f in _flags)
            copy._flags.Add(f);

        copy._flags.Add(flag);
        return copy;
    }

    public void Add(TEnum flag)
    {
        _flags.Add(flag);
    }

    public TFlags CloneRemove(TEnum flag)
    {
        TFlags copy = new();
        foreach (TEnum f in _flags)
            copy._flags.Add(f);

        copy._flags.Remove(flag);
        return copy;
    }

    public void Remove(TEnum flag)
    {
        _flags.Remove(flag);
    }

    public override string ToString()
    {
        return string.Join(" ", _flags.OrderBy(x => x.Value).Select(f => f.ToString()));
    }

    public TValue[] ToValueArray()
    {
        return _flags.OrderBy(x => x.Value).Select(f => f.Value).ToArray();
    }

    public override bool Equals(object? obj)
    {
        return obj is TFlags other && _flags.SetEquals(other._flags);
    }

    public override int GetHashCode()
    {
        return _flags.Aggregate(0, (current, f) => current ^ f.GetHashCode());
    }

    public TFlags Clone()
    {
        TFlags copy = new();

        return _flags.Aggregate(copy, (current, f) => current.CloneAdd(f));
    }

    public static TFlags operator |(SmartEnumFlags<TFlags, TEnum, TValue> a, TEnum b)
    {
        return ((TFlags)a).CloneAdd(b);
    }

    public static TFlags operator -(SmartEnumFlags<TFlags, TEnum, TValue> a, TEnum b)
    {
        return ((TFlags)a).CloneRemove(b);
    }

    public static bool operator ==(SmartEnumFlags<TFlags, TEnum, TValue> left, SmartEnumFlags<TFlags, TEnum, TValue> right)
    {
        if (ReferenceEquals(left, right))
            return true;

        return left.Equals(right);
    }

    public static bool operator !=(SmartEnumFlags<TFlags, TEnum, TValue> left, SmartEnumFlags<TFlags, TEnum, TValue> right)
    {
        return !(left == right);
    }
}