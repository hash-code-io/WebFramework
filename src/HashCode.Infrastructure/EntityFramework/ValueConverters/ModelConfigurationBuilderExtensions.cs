using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HashCode.Infrastructure.EntityFramework.ValueConverters;

/// <summary>
///     Extensions
/// </summary>
public static class ModelConfigurationBuilderExtensions
{
    /// <summary>
    ///     Scans the given assemblies for <see cref="ValueConverter" /> and applies these to
    ///     <see cref="ModelConfigurationBuilder" />
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="assemblyMarkers"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void ApplyValueConvertersFromAssemblies(this ModelConfigurationBuilder builder, params Type[] assemblyMarkers)
    {
        ArgumentNullException.ThrowIfNull(builder);
        List<ValueConverterDescriptor> valueConverterTypes = ValueConverterCollector.Collect(assemblyMarkers);

        foreach (ValueConverterDescriptor converterTypes in valueConverterTypes)
            builder.Properties(converterTypes.PropertyType).HaveConversion(converterTypes.ConverterType);
    }
}