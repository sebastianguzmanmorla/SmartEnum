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

        TFlags flags;

        switch (reader.TokenType)
        {
            case JsonTokenType.String:
            {
                flags = SmartEnumFlags<TFlags, TEnum, TValue>.CreateEmpty();
                ReadOnlySpan<char> remaining = reader.GetString().AsSpan();
                while (!remaining.IsEmpty)
                {
                    int separator = remaining.IndexOfAny(' ', ',');
                    ReadOnlySpan<char> part = separator < 0 ? remaining : remaining[..separator];
                    if (!part.IsEmpty)
                        flags.Add(SmartEnum<TEnum, TValue>.Parse(part.ToString()));
                    if (separator < 0)
                        break;
                    remaining = remaining[(separator + 1)..];
                }
                break;
            }
            case JsonTokenType.StartArray:
            {
                flags = SmartEnumFlags<TFlags, TEnum, TValue>.CreateEmpty();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType != JsonTokenType.String)
                        throw new JsonException("Invalid SmartFlags format");
                    flags.Add(SmartEnum<TEnum, TValue>.Parse(reader.GetString()!));
                }
                if (reader.TokenType != JsonTokenType.EndArray)
                    throw new JsonException("Incomplete SmartFlags array");
                break;
            }
            default:
                throw new JsonException("Invalid SmartFlags format");
        }
        return flags;
    }

    public override void Write(Utf8JsonWriter writer, TFlags? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(string.Join(" ", value.Flags.Select(flag =>
            SmartEnumFlags<TFlags, TEnum, TValue>.ToDelimitedValue(flag, flag.Value.ToString(), preferUnderlyingKey: true))));
    }
}
