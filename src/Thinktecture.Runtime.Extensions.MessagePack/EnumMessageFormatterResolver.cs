using System;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Thinktecture.Formatters;

namespace Thinktecture
{
   /// <summary>
   /// MessagePack formatter resolver for <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   public class EnumMessageFormatterResolver : IFormatterResolver
   {
      /// <summary>
      /// Singleton.
      /// </summary>
      public static readonly IFormatterResolver Instance = new EnumMessageFormatterResolver();

      /// <inheritdoc />
      public IMessagePackFormatter<T> GetFormatter<T>()
      {
         return Cache<T>.Formatter;
      }

      private static class Cache<T>
      {
         public static readonly IMessagePackFormatter<T> Formatter;

         static Cache()
         {
            var type = typeof(T);

            if (typeof(IEnum).IsAssignableFrom(type))
            {
               var genericTypeDef = type.FindGenericEnumTypeDefinition();
               var formatterType = typeof(EnumMessagePackFormatter<,>).MakeGenericType(genericTypeDef.GenericTypeArguments);
               Formatter = (IMessagePackFormatter<T>)Activator.CreateInstance(formatterType);
               return;
            }

            Formatter = StandardResolver.Instance.GetFormatter<T>();
         }
      }
   }
}
