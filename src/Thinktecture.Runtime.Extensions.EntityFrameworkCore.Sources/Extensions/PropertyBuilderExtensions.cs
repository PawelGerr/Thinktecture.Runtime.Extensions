using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture;

/// <summary>
/// Provides extension methods for the <see cref="PropertyBuilder{TProperty}"/> class to configure value object conversions.
/// </summary>
public static class PropertyBuilderExtensions
{
   /// <summary>
   /// Configures a property to use value object conversion.
   /// </summary>
   /// <typeparam name="TProperty">The property type.</typeparam>
   /// <param name="propertyBuilder">The property builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Whether to use the constructor when reading from the database.</param>
   /// <returns>The property builder for chaining.</returns>
   public static PropertyBuilder<TProperty> HasThinktectureValueConverter<TProperty>(
      this PropertyBuilder<TProperty> propertyBuilder,
      bool useConstructorForRead = true)
   {
      var converter = ThinktectureValueConverterFactory.Create(typeof(TProperty), useConstructorForRead);
      propertyBuilder.HasConversion(converter);

      return propertyBuilder;
   }
}
