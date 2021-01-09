using System;
using MessagePack;
using MessagePack.Formatters;

namespace Thinktecture.Formatters
{
   /// <summary>
   /// MessagePack formatter for <see cref="IEnum{TKey}"/>.
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class EnumMessagePackFormatter<TEnum, TKey> : IMessagePackFormatter<TEnum>
      where TEnum : IEnum<TKey>
      where TKey : notnull
   {
      private readonly Func<TKey?, TEnum?> _convert;
      private IMessagePackFormatter<TKey>? _keyFormatter;

      /// <summary>
      /// Initializes a new instance of <see cref="EnumMessagePackFormatter{TEnum,TKey}"/>.
      /// </summary>
      /// <param name="convert">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="TEnum"/>.</param>
      public EnumMessagePackFormatter(Func<TKey?, TEnum?> convert)
      {
         _convert = convert ?? throw new ArgumentNullException(nameof(convert));
      }

      /// <inheritdoc />
      public void Serialize(ref MessagePackWriter writer, TEnum? value, MessagePackSerializerOptions options)
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
      public TEnum? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
#pragma warning restore CS8766
      {
         if (reader.TryReadNil())
            return default;

         var key = GetKeyConverter(options).Deserialize(ref reader, options);

         return _convert(key);
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
