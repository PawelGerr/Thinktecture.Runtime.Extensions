using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EntityFrameworkCore.Infrastructure;

namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="DbContextOptionsBuilder"/>.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
   /// <summary>
   /// Enables automatic configuration of <see cref="ValueConverter"/> for Smart Enums and Value Objects.
   /// </summary>
   /// <param name="builder">Options builder.</param>
   /// <param name="useThinktectureConverters">Indication whether to enable or disable the feature.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The provided <paramref name="builder"/>.</returns>
   [Obsolete("Use 'UseThinktectureValueConverters' instead.")]
   public static DbContextOptionsBuilder<T> UseValueObjectValueConverter<T>(
      this DbContextOptionsBuilder<T> builder,
      bool useThinktectureConverters = true,
      bool useConstructorForRead = true,
      Action<IConventionProperty>? configureEnumsAndKeyedValueObjects = null)
      where T : DbContext
   {
      return builder.UseThinktectureValueConverters(useThinktectureConverters, useConstructorForRead, configureEnumsAndKeyedValueObjects);
   }

   /// <summary>
   /// Enables automatic configuration of <see cref="ValueConverter"/> for Smart Enums and Value Objects.
   /// </summary>
   /// <param name="builder">Options builder.</param>
   /// <param name="useThinktectureConverters">Indication whether to enable or disable the feature.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from a database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The provided <paramref name="builder"/>.</returns>
   [Obsolete("Use 'UseThinktectureValueConverters' instead.")]
   public static DbContextOptionsBuilder UseValueObjectValueConverter(
      this DbContextOptionsBuilder builder,
      bool useThinktectureConverters = true,
      bool useConstructorForRead = true,
      Action<IConventionProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      return builder.UseThinktectureValueConverters(useThinktectureConverters, useConstructorForRead, configureEnumsAndKeyedValueObjects);
   }

   /// <summary>
   /// Enables automatic configuration of <see cref="ValueConverter"/> for Smart Enums and Value Objects.
   /// </summary>
   /// <param name="builder">Options builder.</param>
   /// <param name="useThinktectureConverters">Indication whether to enable or disable the feature.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from a database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The provided <paramref name="builder"/>.</returns>
   public static DbContextOptionsBuilder<T> UseThinktectureValueConverters<T>(
      this DbContextOptionsBuilder<T> builder,
      bool useThinktectureConverters = true,
      bool useConstructorForRead = true,
      Action<IConventionProperty>? configureEnumsAndKeyedValueObjects = null)
      where T : DbContext
   {
      ((DbContextOptionsBuilder)builder).UseThinktectureValueConverters(useThinktectureConverters, useConstructorForRead, configureEnumsAndKeyedValueObjects);
      return builder;
   }

   /// <summary>
   /// Enables automatic configuration of <see cref="ValueConverter"/> for Smart Enums and Value Objects.
   /// </summary>
   /// <param name="builder">Options builder.</param>
   /// <param name="useThinktectureConverters">Indication whether to enable or disable the feature.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from a database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The provided <paramref name="builder"/>.</returns>
   public static DbContextOptionsBuilder UseThinktectureValueConverters(
      this DbContextOptionsBuilder builder,
      bool useThinktectureConverters = true,
      bool useConstructorForRead = true,
      Action<IConventionProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      builder.AddOrUpdateExtension(extension => extension.UseThinktectureValueConverters(useThinktectureConverters, useConstructorForRead, configureEnumsAndKeyedValueObjects));
      return builder;
   }

   private static void AddOrUpdateExtension(
      this DbContextOptionsBuilder optionsBuilder,
      Func<ThinktectureDbContextOptionsExtension, ThinktectureDbContextOptionsExtension> callback)
   {
      ArgumentNullException.ThrowIfNull(optionsBuilder);
      ArgumentNullException.ThrowIfNull(callback);

      var extension = optionsBuilder.Options.FindExtension<ThinktectureDbContextOptionsExtension>() ?? new ThinktectureDbContextOptionsExtension();

      extension = callback(extension);

      var builder = (IDbContextOptionsBuilderInfrastructure)optionsBuilder;
      builder.AddOrUpdateExtension(extension);
   }
}
