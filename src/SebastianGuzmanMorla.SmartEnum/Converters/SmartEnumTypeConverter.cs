using System;
using System.ComponentModel;
using System.Globalization;

namespace SebastianGuzmanMorla.SmartEnum.Converters;

public class SmartEnumTypeConverter<TEnum, TValue> : TypeConverter
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }
            return SmartEnum<TEnum, TValue>.Parse(stringValue);
        }
        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is TEnum enumValue)
        {
            return enumValue.ToString();
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
