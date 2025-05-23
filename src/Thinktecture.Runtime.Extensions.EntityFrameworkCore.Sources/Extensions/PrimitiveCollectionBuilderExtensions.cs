#if PRIMITIVE_COLLECTIONS
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture;

/// <summary>
/// Provides extension methods for the <see cref="PrimitiveCollectionBuilder{TProperty}"/> class to configure value object conversions.
/// </summary>
public static class PrimitiveCollectionBuilderExtensions
{
   /// <summary>
   /// Configures a primitive collection to use value object conversion.
   /// </summary>
   /// <typeparam name="TProperty">The element type of the collection.</typeparam>
   /// <param name="primitiveCollectionBuilder">The primitive collection builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Whether to use the constructor when reading from the database.</param>
   /// <returns>The primitive collection builder for chaining.</returns>
   public static PrimitiveCollectionBuilder<TProperty> HasThinktectureValueConverter<TProperty>(
      this PrimitiveCollectionBuilder<TProperty> primitiveCollectionBuilder,
      bool useConstructorForRead = true)
   {
      var elementType = primitiveCollectionBuilder.ElementType();
      var converter = ThinktectureValueConverterFactory.Create(elementType.Metadata.ClrType, useConstructorForRead);
      elementType.HasConversion(converter);

      return primitiveCollectionBuilder;
   }
}
#endif
