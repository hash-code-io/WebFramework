using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HashCode.SharedKernel.Auth;

[JsonConverter(typeof(AppPermissionJsonConverter))]
public record AppPermission
{
    public const int MaxLength = 150;
    public const string Prefix = "Permissions";

    public AppPermission(AppResource resource, AppAction action)
    {
        Value = $"{Prefix}.{resource}.{action}";
        if (Value.Length > MaxLength)
            throw new InvalidOperationException($"Total length of AppPermission max not exceed {MaxLength}. Value was: [{Value}] with a length of {Value.Length}");
    }

    public string Value { get; }

    [return: NotNullIfNotNull(nameof(claim))]
    public static Claim? ToClaim(AppPermission? claim) => claim is null ? null : new Claim(AuthConstants.Auth.ClaimTypes.Permission, claim.Value);

    [return: NotNullIfNotNull(nameof(claim))]
    public static implicit operator string?(AppPermission? claim) => claim?.Value;

    [return: NotNullIfNotNull(nameof(claim))]
    public static implicit operator Claim?(AppPermission? claim) => ToClaim(claim);

    public static AppPermission From(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.Split('.') switch
        {
            [Prefix, { } resource, { } action] => new AppPermission(resource, action),
            _ => throw new InvalidOperationException("AppPermission Value was not in the expected format. Expected format is: [Prefix.Resource.Action]")
        };
    }

    private sealed class AppPermissionJsonConverter : JsonConverter<AppPermission>
    {
        public override AppPermission? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? id = reader.GetString();
            return id is null ? null : From(id);
        }

        public override void Write(Utf8JsonWriter writer, AppPermission value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value);
    }
}