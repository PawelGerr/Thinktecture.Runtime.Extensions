using System;
using System.Diagnostics.CodeAnalysis;
using MessagePack;
using MessagePack.Formatters;

namespace Thinktecture.Formatters
{
   /// <summary>
   /// MessagePack formatter for <see cref="IEnum{TKey}"/>.
   /// </summary>
   /// <typeparam name="T">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public abstract class EnumMessagePackFormatter<T, TKey> : IMessagePackFormatter<T>
      where T : IEnum<TKey>
      where TKey : notnull
   {
      private IMessagePackFormatter<TKey>? _keyFormatter;

      /// <summary>
      /// Converts <paramref name="key"/> to an instance of <typeparamref name="T"/>.
      /// </summary>
      /// <param name="key">Key to convert.</param>
      /// <returns>An instance of <typeparamref name="T"/>.</returns>
      [return: NotNullIfNotNull("key")]
      protected abstract T? ConvertFrom(TKey? key);

      /// <inheritdoc />
      public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
      {
         if (value is null)
         {
            writer.WriteNil();
         }
         else
         {
            GetKeyConverter(options).Serialize(ref writer, value.GetKey(), options);
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

         return ConvertFrom(key);
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
