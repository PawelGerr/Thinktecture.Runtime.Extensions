using System;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Thinktecture.Formatters;

namespace Thinktecture
{
   /// <summary>
   /// MessagePack formatter resolver for <see cref="IEnum{TKey}"/>.
   /// </summary>
   // TODO:
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

            if (typeof(IEnum<>).IsAssignableFrom(type))
            {
               var enumMetadata = EnumMetadataLookup.FindEnum(type);

               if (enumMetadata is null)
               {
                  InitError = $"The type '{type.Name}' implements '{typeof(IEnum<>)}' but not the base class 'IEnum<>' or 'Enum<,>'.";
                  return;
               }

               var formatterType = typeof(EnumMessagePackFormatter<,>).MakeGenericType(enumMetadata.EnumType, enumMetadata.KeyType);
               var formatter = Activator.CreateInstance(formatterType, enumMetadata.ConvertFromKey);

               if (formatter is null)
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
