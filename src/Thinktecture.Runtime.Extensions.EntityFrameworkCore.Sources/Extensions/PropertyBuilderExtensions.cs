using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thinktecture.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

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
   [Obsolete("Use the overload accepting Configuration instead. This method will be removed in a future version.")]
   public static PropertyBuilder<TProperty> HasThinktectureValueConverter<TProperty>(
      this PropertyBuilder<TProperty> propertyBuilder,
      bool useConstructorForRead = true)
   {
      var converter = ThinktectureValueConverterFactory.Create(typeof(TProperty), useConstructorForRead);
      propertyBuilder.HasConversion(converter);

      return propertyBuilder;
   }

   /// <summary>
   /// Configures a property to use value object conversion.
   /// </summary>
   /// <typeparam name="TProperty">The property type.</typeparam>
   /// <param name="propertyBuilder">The property builder.</param>
   /// <returns>The property builder for chaining.</returns>
   public static PropertyBuilder<TProperty> HasThinktectureValueConverter<TProperty>(
      this PropertyBuilder<TProperty> propertyBuilder)
   {
      return HasThinktectureValueConverter(propertyBuilder, Configuration.Default);
   }

   /// <summary>
   /// Configures a property to use value object conversion.
   /// </summary>
   /// <typeparam name="TProperty">The property type.</typeparam>
   /// <param name="propertyBuilder">The property builder.</param>
   /// <param name="configuration">Configuration options for Thinktecture EF Core integration.</param>
   /// <returns>The property builder for chaining.</returns>
   public static PropertyBuilder<TProperty> HasThinktectureValueConverter<TProperty>(
      this PropertyBuilder<TProperty> propertyBuilder,
      Configuration configuration)
   {
      var converter = ThinktectureValueConverterFactory.Create(typeof(TProperty), configuration.UseConstructorForRead);
      propertyBuilder.HasConversion(converter);
      new MutableItem(typeof(TProperty), propertyBuilder.Metadata).ApplyMaxLengthFromStrategy(configuration);

      return propertyBuilder;
   }
}
