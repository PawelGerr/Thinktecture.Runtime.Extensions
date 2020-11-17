using System;
using System.Diagnostics.CodeAnalysis;
using MessagePack;
using MessagePack.Formatters;

namespace Thinktecture.Formatters
{
   /// <summary>
   /// MessagePack formatter for <see cref="Enum{TEnum}"/>.
   /// </summary>
   /// <typeparam name="T">Type of the enum.</typeparam>
   public class EnumMessagePackFormatter<T> : EnumMessagePackFormatter<T, string>
      where T : Enum<T>
   {
   }

   /// <summary>
   /// MessagePack formatter for <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   /// <typeparam name="T">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class EnumMessagePackFormatter<T, TKey> : IMessagePackFormatter<T>
      where T : Enum<T, TKey>
      where TKey : notnull
   {
      private IMessagePackFormatter<TKey>? _keyFormatter;

      /// <inheritdoc />
      public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
      {
         if (value is null)
         {
            writer.WriteNil();
         }
         else
         {
            GetKeyConverter(options).Serialize(ref writer, value.Key, options);
         }
      }

      /// <inheritdoc />
#pragma warning disable CS8766
      public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
#pragma warning restore CS8766
      {
         if (reader.TryReadNil())
            return default;

         var key = GetKeyConverter(options).Deserialize(ref reader, options);

         return Enum<T, TKey>.Get(key);
      }

      private IMessagePackFormatter<TKey> GetKeyConverter(MessagePackSerializerOptions options)
      {
         if (_keyFormatter is null)
         {
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            _keyFormatter = options.Resolver.GetFormatter<TKey>();
         }

         return _keyFormatter;
      }
   }
}
