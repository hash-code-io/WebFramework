namespace HashCode.SharedKernel.Auth;

public record AppResource(string Value)
{
    public static AppResource ToAppResource(string value) => new(value);
    public static implicit operator AppResource(string value) => ToAppResource(value);
    public override string ToString() => Value;
}