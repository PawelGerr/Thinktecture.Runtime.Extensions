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
         return Cache<T>.Formatter ?? throw new Exception(Cache<T>.InitError);
      }

      private static class Cache<T>
      {
         public static readonly IMessagePackFormatter<T>? Formatter;

         // ReSharper disable once StaticMemberInGenericType
         public static readonly string? InitError;

#pragma warning disable CA1810
         static Cache()
         {
            var type = typeof(T);

            if (typeof(IEnum).IsAssignableFrom(type))
            {
               var genericTypeDef = type.FindGenericEnumTypeDefinition();

               if (genericTypeDef is null)
               {
                  InitError = $"The type '{type.Name}' implements '{nameof(IEnum)}' but not the base class 'Enum<>' or 'Enum<,>'.";
                  return;
               }

               var formatterType = typeof(EnumMessagePackFormatter<,>).MakeGenericType(genericTypeDef.GenericTypeArguments);
               var formatter = Activator.CreateInstance(formatterType);

               if(formatter is null)
               {
                  InitError = $"The formatter of '{formatterType.Name}' could not be instantiated.";
                  return;
               }

               Formatter = (IMessagePackFormatter<T>)formatter;
               return;
            }

            Formatter = StandardResolver.Instance.GetFormatter<T>();
         }
      }
   }
}
