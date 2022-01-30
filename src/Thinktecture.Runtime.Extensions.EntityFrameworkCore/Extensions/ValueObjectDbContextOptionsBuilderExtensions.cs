using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EntityFrameworkCore.Infrastructure;

namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="DbContextOptionsBuilder"/>.
/// </summary>
public static class ValueObjectDbContextOptionsBuilderExtensions
{
   /// <summary>
   /// Enables automatic configuration of <see cref="ValueConverter"/> for Smart Enums and Value Objects.
   /// </summary>
   /// <param name="builder">Options builder.</param>
   /// <param name="useValueObjectConventions">Indication whether to enable or disable the feature.</param>
   /// <param name="validateOnWrite">In case of an <see cref="IValidatableEnum{TKey}"/>, ensures that the item is valid before writing it to database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The provided <paramref name="builder"/>.</returns>
   public static DbContextOptionsBuilder<T> UseValueObjectValueConverter<T>(
      this DbContextOptionsBuilder<T> builder,
      bool useValueObjectConventions = true,
      bool validateOnWrite = true,
      Action<IConventionProperty>? configureEnumsAndKeyedValueObjects = null)
      where T : DbContext
   {
      ((DbContextOptionsBuilder)builder).UseValueObjectValueConverter(useValueObjectConventions, validateOnWrite, configureEnumsAndKeyedValueObjects);
      return builder;
   }

   /// <summary>
   /// Enables automatic configuration of <see cref="ValueConverter"/> for Smart Enums and Value Objects.
   /// </summary>
   /// <param name="builder">Options builder.</param>
   /// <param name="useValueObjectConventions">Indication whether to enable or disable the feature.</param>
   /// <param name="validateOnWrite">In case of an <see cref="IValidatableEnum{TKey}"/>, ensures that the item is valid before writing it to database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The provided <paramref name="builder"/>.</returns>
   public static DbContextOptionsBuilder UseValueObjectValueConverter(
      this DbContextOptionsBuilder builder,
      bool useValueObjectConventions = true,
      bool validateOnWrite = true,
      Action<IConventionProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      builder.AddOrUpdateExtension(extension => extension.UseValueObjectValueConverter(useValueObjectConventions, validateOnWrite, configureEnumsAndKeyedValueObjects));
      return builder;
   }

   private static void AddOrUpdateExtension(
      this DbContextOptionsBuilder optionsBuilder,
      Func<ValueObjectDbContextOptionsExtension, ValueObjectDbContextOptionsExtension> callback)
   {
      if (optionsBuilder is null)
         throw new ArgumentNullException(nameof(optionsBuilder));
      if (callback is null)
         throw new ArgumentNullException(nameof(callback));

      var extension = optionsBuilder.Options.FindExtension<ValueObjectDbContextOptionsExtension>() ?? new ValueObjectDbContextOptionsExtension();

      extension = callback(extension);

      var builder = (IDbContextOptionsBuilderInfrastructure)optionsBuilder;
      builder.AddOrUpdateExtension(extension);
   }
}
