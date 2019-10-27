using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture.EntityFrameworkCore.Storage.ValueConversion
{
   /// <summary>
   /// Value converter for <see cref="Enum{TEnum}"/>.
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   public class EnumValueConverter<TEnum> : EnumValueConverter<TEnum, string>
      where TEnum : Enum<TEnum>
   {
   }

   /// <summary>
   /// Value converter for <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class EnumValueConverter<TEnum, TKey> : ValueConverter<TEnum, TKey>
      where TEnum : Enum<TEnum, TKey>
      where TKey : notnull
   {
      /// <summary>
      /// Initializes new instance <see cref="EnumValueConverter{TEnum,TKey}"/>.
      /// </summary>
      // ReSharper disable once MemberCanBeProtected.Global
      public EnumValueConverter()
         : base(item => item.Key, key => Enum<TEnum, TKey>.Get(key)!)
      {
      }
   }
}
