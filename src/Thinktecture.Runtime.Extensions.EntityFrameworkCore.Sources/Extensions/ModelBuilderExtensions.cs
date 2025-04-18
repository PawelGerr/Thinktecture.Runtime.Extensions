using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
   /// <param name="validateOnWrite">If <c>true</c> and the type is a validatable Smart Enum, ensures that the item is valid before writing it to the database.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <exception cref="ArgumentNullException">If <paramref name="modelBuilder"/> is <c>null</c>.</exception>
   [Obsolete("Use 'AddThinktectureValueConverters' instead.")]
   public static void AddValueObjectConverters(
      this ModelBuilder modelBuilder,
      bool validateOnWrite,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      modelBuilder.AddThinktectureValueConverters(validateOnWrite, useConstructorForRead, configureEnumsAndKeyedValueObjects);
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="modelBuilder">EF model builder.</param>
   /// <param name="validateOnWrite">If <c>true</c> and the type is a validatable Smart Enum, ensures that the item is valid before writing it to the database.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <exception cref="ArgumentNullException">If <paramref name="modelBuilder"/> is <c>null</c>.</exception>
   public static void AddThinktectureValueConverters(
      this ModelBuilder modelBuilder,
      bool validateOnWrite,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      ArgumentNullException.ThrowIfNull(modelBuilder);

      configureEnumsAndKeyedValueObjects ??= Empty.Action;
      var converterLookup = new Dictionary<Type, ValueConverter>();

      foreach (var entity in modelBuilder.Model.GetEntityTypes())
      {
         entity.AddConvertersToEntity(
            validateOnWrite,
            useConstructorForRead,
            false, // no need for recursions, because GetEntityTypes will get them all
            converterLookup,
            configureEnumsAndKeyedValueObjects);
      }
   }
}
