using System.Text.Json;
using System.Text.Json.Serialization;

namespace HashCode.SharedKernel.Serialization;

public static class AppSerializer
{
    public static readonly JsonSerializerOptions Options = new() { Converters = { new JsonStringEnumConverter() } };
    public static T Deserialize<T>(string text) => JsonSerializer.Deserialize<T>(text, Options) ?? throw new InvalidOperationException($"Could not deserialize type {typeof(T).Name}");

    public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, Options);
    public static string Serialize(object obj, Type type) => JsonSerializer.Serialize(obj, type, Options);
}