using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture.EntityFrameworkCore.Storage
{
   /// <summary>
   /// Type mapping for <see cref="Enum{TEnum}"/> and <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   // ReSharper disable once ClassNeverInstantiated.Global
   public class EnumTypeMappingPlugin : IRelationalTypeMappingSourcePlugin
   {
      /// <inheritdoc />
      [CanBeNull]
      public RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
      {
         // EF caches the mappings per type so we don't need to do anything
         if (typeof(IEnum).IsAssignableFrom(mappingInfo.ClrType))
         {
            var genericTypeDef = mappingInfo.ClrType.FindGenericEnumTypeDefinition();
            var converterType = typeof(EnumValueConverter<,>).MakeGenericType(genericTypeDef.GenericTypeArguments);
            var converter = (ValueConverter)Activator.CreateInstance(converterType);

            return new EnumTypeMapping(mappingInfo.ClrType, converter);
         }

         return null;
      }
   }
}
