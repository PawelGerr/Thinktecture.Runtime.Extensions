using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture.EntityFrameworkCore.Storage.ValueConversion
{
   /// <summary>
   /// Value converter for <see cref="IEnum{TKey}"/>.
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class EnumValueConverter<TEnum, TKey> : ValueConverter<TEnum, TKey>
      where TEnum : IEnum<TKey>
      where TKey : notnull
   {
      /// <summary>
      /// Initializes new instance <see cref="EnumValueConverter{TEnum,TKey}"/>.
      /// </summary>
      // ReSharper disable once MemberCanBeProtected.Global
      public EnumValueConverter(Expression<Func<TKey, TEnum>> convertFromProvider)
         : base(item => item.GetKey(), convertFromProvider)
      {
      }
   }
}
