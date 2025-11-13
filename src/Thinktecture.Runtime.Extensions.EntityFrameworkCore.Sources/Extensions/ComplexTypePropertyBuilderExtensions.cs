using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture;

/// <summary>
/// Provides extension methods for the <see cref="ComplexTypePropertyBuilder{TProperty}"/> class to configure value object conversions.
/// </summary>
public static class ComplexTypePropertyBuilderExtensions
{
   /// <summary>
   /// Configures a complex type property to use value converter for Smart Enums and Value Objects.
   /// </summary>
   /// <typeparam name="TProperty">The property type.</typeparam>
   /// <param name="propertyBuilder">The complex type property builder.</param>
   /// <param name="useConstructorForRead">For keyed Value Objects only. Whether to use the constructor when reading from the database.</param>
   /// <returns>The complex type property builder for chaining.</returns>
   public static ComplexTypePropertyBuilder<TProperty> HasThinktectureValueConverter<TProperty>(
      this ComplexTypePropertyBuilder<TProperty> propertyBuilder,
      bool useConstructorForRead = true)
   {
      var converter = ThinktectureValueConverterFactory.Create(typeof(TProperty), useConstructorForRead);
      propertyBuilder.HasConversion(converter);

      return propertyBuilder;
   }
}
