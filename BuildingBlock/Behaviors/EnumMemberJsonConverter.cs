using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Behaviors;

public sealed class EnumMemberJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString() ?? throw new JsonException($"Cannot convert null to {typeof(TEnum).Name}");
        foreach (FieldInfo field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            EnumMemberAttribute? attr = field.GetCustomAttribute<EnumMemberAttribute>();
            if (attr?.Value == value || field.Name == value)
            {
                return (TEnum)field.GetValue(null)!;
            }
        }

        throw new JsonException($"Cannot convert '{value}' to {typeof(TEnum).Name}");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        FieldInfo? field = typeof(TEnum).GetField(value.ToString()!);
        EnumMemberAttribute? attr = field?.GetCustomAttribute<EnumMemberAttribute>();
        writer.WriteStringValue(!string.IsNullOrWhiteSpace(attr?.Value) ? attr!.Value : value.ToString());
    }
}

public sealed class NullableEnumMemberJsonConverter<TEnum> : JsonConverter<TEnum?> where TEnum : struct, Enum
{
    public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        string? value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        foreach (FieldInfo field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            EnumMemberAttribute? attr = field.GetCustomAttribute<EnumMemberAttribute>();
            if (attr?.Value == value || field.Name == value)
            {
                return (TEnum)field.GetValue(null)!;
            }
        }

        throw new JsonException($"Cannot convert '{value}' to {typeof(TEnum).Name}");
    }

    public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        FieldInfo? field = typeof(TEnum).GetField(value.Value.ToString());
        EnumMemberAttribute? attr = field?.GetCustomAttribute<EnumMemberAttribute>();
        writer.WriteStringValue(!string.IsNullOrWhiteSpace(attr?.Value) ? attr!.Value : value.Value.ToString());
    }
}
