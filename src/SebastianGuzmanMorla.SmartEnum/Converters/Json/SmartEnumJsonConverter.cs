using System.Text.Json;
using System.Text.Json.Serialization;

namespace SebastianGuzmanMorla.SmartEnum.Converters.Json;

public class SmartEnumJsonConverter<TEnum, TValue>
    : JsonConverter<TEnum?>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull
{
    public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        TValue? value = JsonSerializer.Deserialize<TValue>(ref reader, options);

        return value is not null ? SmartEnum<TEnum, TValue>.Parse(value) : null;
    }

    public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }
}