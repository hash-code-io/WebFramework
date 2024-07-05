using System.ComponentModel;

namespace HashCode.SharedKernel.Extension;

public static class SerializationExtensions
{
    public static bool IsTriviallySerializable(this Type type) => TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
}