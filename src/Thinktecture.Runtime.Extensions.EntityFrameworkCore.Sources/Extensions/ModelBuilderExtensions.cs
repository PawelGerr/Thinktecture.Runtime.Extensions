using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="modelBuilder">EF model builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <exception cref="ArgumentNullException">If <paramref name="modelBuilder"/> is <c>null</c>.</exception>
   public static void AddThinktectureValueConverters(
      this ModelBuilder modelBuilder,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      ArgumentNullException.ThrowIfNull(modelBuilder);

      configureEnumsAndKeyedValueObjects ??= Empty.Action;

      foreach (var entity in modelBuilder.Model.GetEntityTypes())
      {
         entity.AddConvertersToEntity(
            useConstructorForRead,
            false, // no need for recursions, because GetEntityTypes will get them all
            configureEnumsAndKeyedValueObjects);
      }
   }
}
