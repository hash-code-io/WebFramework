using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HashCode.Infrastructure.EntityFramework.ValueConverters;

internal sealed record ValueConverterDescriptor(Type PropertyType, Type ConverterType);

internal static class ValueConverterCollector
{
    public static List<ValueConverterDescriptor> Collect(IEnumerable<Type>? assemblyMarkers)
    {
        var markers = new List<Type> { typeof(ValueConverterCollector) };
        if (assemblyMarkers != null) markers.AddRange(assemblyMarkers);

        return markers
            .SelectMany(x => x.Assembly.GetTypes())
            .Where(t =>
            {
                if (t.BaseType is null
                    || !t.BaseType.IsGenericType
                    || t.BaseType.GenericTypeArguments.Length != 2)
                    return false;

                Type valueConverterType = typeof(ValueConverter<,>).MakeGenericType(t.BaseType.GenericTypeArguments);
                return valueConverterType == t.BaseType;
            })
            .Select(t => new ValueConverterDescriptor(t.BaseType!.GenericTypeArguments[0], t))
            .Distinct()
            .ToList();
    }
}