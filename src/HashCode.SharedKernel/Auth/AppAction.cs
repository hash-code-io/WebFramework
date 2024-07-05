namespace HashCode.SharedKernel.Auth;

public record AppAction(string Value)
{
    public static AppAction ToAppAction(string value) => new(value);
    public static implicit operator AppAction(string value) => ToAppAction(value);
    public override string ToString() => Value;
}