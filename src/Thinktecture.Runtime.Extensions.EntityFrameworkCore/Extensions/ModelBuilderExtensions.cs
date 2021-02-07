using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   /// <summary>
   /// Extensions for <see cref="ModelBuilder"/>.
   /// </summary>
   public static class ModelBuilderExtensions
   {
      /// <summary>
      /// Adds value converter to all properties implementing <see cref="IEnum{TKey}"/>/<see cref="IValidatableEnum{TKey}"/>
      /// and having the <see cref="ValueTypeAttribute"/>.
      /// Properties with a value provider are skipped.
      /// </summary>
      /// <param name="modelBuilder">EF model builder.</param>
      /// <param name="validateOnWrite">In case of an <see cref="IValidatableEnum{TKey}"/>, ensures that the item is valid before writing it to database.</param>
      /// <param name="configure">Action for further configuration of the property.</param>
      /// <exception cref="ArgumentNullException">If <paramref name="modelBuilder"/> is <c>null</c>.</exception>
      public static void AddEnumAndValueTypeConverters(
         this ModelBuilder modelBuilder,
         bool validateOnWrite,
         Action<IMutableProperty>? configure = null)
      {
         if (modelBuilder is null)
            throw new ArgumentNullException(nameof(modelBuilder));

         configure ??= Empty.Action;

         foreach (var entity in modelBuilder.Model.GetEntityTypes())
         {
            AddConverterForScalarProperties(entity, validateOnWrite, configure);
            AddConvertersForNavigations(entity, modelBuilder, validateOnWrite, configure);
         }
      }

      private static void AddConvertersForNavigations(
         IMutableEntityType entity,
         ModelBuilder modelBuilder,
         bool validateOnWrite,
         Action<IMutableProperty> configure)
      {
         List<IMutableNavigation>? navigationsToConvert = null;

         foreach (var navigation in entity.GetNavigations())
         {
            if (ValueTypeMetadataLookup.Find(navigation.ClrType) is not null)
               (navigationsToConvert ??= new List<IMutableNavigation>()).Add(navigation);
         }

         if (navigationsToConvert is not null)
         {
            foreach (var navigation in navigationsToConvert)
            {
               var valueConverter = ValueTypeValueConverterFactory.Create(navigation.ClrType, validateOnWrite);
               var property = modelBuilder.Entity(entity.ClrType).Property(navigation.Name);
               property.HasConversion(valueConverter);
               configure(property.Metadata);
            }
         }
      }

      private static void AddConverterForScalarProperties(
         IMutableEntityType entity,
         bool validateOnWrite,
         Action<IMutableProperty> configure)
      {
         foreach (var property in entity.GetProperties())
         {
            var valueConverter = property.GetValueConverter();

            if (valueConverter is not null)
               continue;

            if (ValueTypeMetadataLookup.Find(property.ClrType) is null)
               continue;

            valueConverter = ValueTypeValueConverterFactory.Create(property.ClrType, validateOnWrite);
            property.SetValueConverter(valueConverter);
            configure(property);
         }
      }
   }
}
