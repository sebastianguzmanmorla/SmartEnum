using System.Text.Json;
using System.Text.Json.Serialization;

namespace SebastianGuzmanMorla.SmartEnum.Converters.Json;

public class SmartEnumFlagsJsonConverter<TFlags, TEnum, TValue> : JsonConverter<TFlags?>
    where TFlags : SmartEnumFlags<TFlags, TEnum, TValue>, new()
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : notnull
{
    public override TFlags? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        List<string> parts = [];

        switch (reader.TokenType)
        {
            case JsonTokenType.String:
            {
                string raw = reader.GetString()!;
                parts = raw.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries).ToList();
                break;
            }
            case JsonTokenType.StartArray:
            {
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        parts.Add(reader.GetString()!);
                    }
                    else
                    {
                        throw new JsonException("Invalid SmartFlags format");
                    }

                break;
            }
            default:
                throw new JsonException("Invalid SmartFlags format");
        }

        TFlags flags = new();

        foreach (TEnum part in parts.Select(SmartEnum<TEnum, TValue>.Parse))
            flags.Add(part);

        return flags;
    }

    public override void Write(Utf8JsonWriter writer, TFlags? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(string.Join(" ", value.Flags.Select(f => f.Value)));
    }
}