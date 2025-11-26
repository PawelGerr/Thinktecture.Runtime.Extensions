using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects
   /// with default configuration.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="modelBuilder">EF model builder.</param>
   /// <exception cref="ArgumentNullException">If <paramref name="modelBuilder"/> is <c>null</c>.</exception>
   public static void AddThinktectureValueConverters(
      this ModelBuilder modelBuilder)
   {
      modelBuilder.AddThinktectureValueConverters(Configuration.Default);
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="modelBuilder">EF model builder.</param>
   /// <param name="configuration">Configuration options for Thinktecture EF Core integration.</param>
   /// <exception cref="ArgumentNullException">If <paramref name="modelBuilder"/> is <c>null</c>.</exception>
   public static void AddThinktectureValueConverters(
      this ModelBuilder modelBuilder,
      Configuration configuration)
   {
      ArgumentNullException.ThrowIfNull(modelBuilder);
      ArgumentNullException.ThrowIfNull(configuration);

      foreach (var entity in modelBuilder.Model.GetEntityTypes())
      {
         entity.AddConvertersToEntity(
            configuration,
            false, // no need for recursions, because GetEntityTypes will get them all
            Empty.Action);
      }
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="modelBuilder">EF model builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <exception cref="ArgumentNullException">If <paramref name="modelBuilder"/> is <c>null</c>.</exception>
   [Obsolete("Use the overload accepting Configuration instead. This method will be removed in a future version.")]
   public static void AddThinktectureValueConverters(
      this ModelBuilder modelBuilder,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      ArgumentNullException.ThrowIfNull(modelBuilder);

      configureEnumsAndKeyedValueObjects ??= Empty.Action;

      var configuration = new Configuration
      {
         UseConstructorForRead = useConstructorForRead
      };

      foreach (var entity in modelBuilder.Model.GetEntityTypes())
      {
         entity.AddConvertersToEntity(
            configuration,
            false, // no need for recursions, because GetEntityTypes will get them all
            configureEnumsAndKeyedValueObjects);
      }
   }
}
