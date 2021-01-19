using System;
using MessagePack;
using MessagePack.Formatters;

namespace Thinktecture.Formatters
{
   /// <summary>
   /// MessagePack formatter for value types.
   /// </summary>
   /// <typeparam name="T">Type of the value type.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class ValueTypeMessagePackFormatter<T, TKey> : IMessagePackFormatter<T>
      where TKey : notnull
   {
      private readonly Func<TKey, T> _convertFromKey;
      private readonly Func<T, TKey> _convertToKey;

      /// <summary>
      /// Initializes a new instance of <see cref="ValueTypeMessagePackFormatter{T,TKey}"/>.
      /// </summary>
      /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
      /// <param name="convertToKey">Converts an instance of type <typeparamref name="T"/> to an instance of <typeparamref name="TKey"/>.</param>
      public ValueTypeMessagePackFormatter(
         Func<TKey, T> convertFromKey,
         Func<T, TKey> convertToKey)
      {
         _convertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
         _convertToKey = convertToKey ?? throw new ArgumentNullException(nameof(convertToKey));
      }

      /// <inheritdoc />
      public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
      {
         if (value is null)
         {
            writer.WriteNil();
         }
         else
         {
            var formatter = options.Resolver.GetFormatterWithVerify<TKey>();
            formatter.Serialize(ref writer, _convertToKey(value), options);
         }
      }

      /// <inheritdoc />
#pragma warning disable CS8766
      public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
#pragma warning restore CS8766
      {
         if (reader.TryReadNil())
            return default;

         var formatter = options.Resolver.GetFormatterWithVerify<TKey>();
         var key = formatter.Deserialize(ref reader, options);

         if (key is null)
            return default;

         return _convertFromKey(key);
      }
   }
}
