using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture.EntityFrameworkCore.Storage
{
   /// <summary>
   /// Type mapping for constants and parameters of type <see cref="Enum{TEnum}"/> and <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   public class EnumTypeMapping : RelationalTypeMapping
   {
      /// <summary>
      /// Initializes new instance of type
      /// </summary>
      /// <param name="type">Type of the enum.</param>
      /// <param name="valueConverter">Value converter for the provided <paramref name="type"/>.</param>
      public EnumTypeMapping([NotNull] Type type, [NotNull] ValueConverter valueConverter)
         : base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(type, valueConverter), "StoreTypeIsBeingIgnored"))
      {
         if (valueConverter == null)
            throw new ArgumentNullException(nameof(valueConverter));
      }
   }
}
