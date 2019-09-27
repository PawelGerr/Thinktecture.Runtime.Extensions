using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Thinktecture.Text.Json.Serialization
{
   /// <summary>
   /// Factory for creation of <see cref="EnumJsonConverter{TEnum,TKey}"/>.
   /// </summary>
   public class EnumJsonConverterFactory : JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert([NotNull] Type typeToConvert)
      {
         return typeof(IEnum).IsAssignableFrom(typeToConvert);
      }

      /// <inheritdoc />
      [NotNull]
      public override JsonConverter CreateConverter([NotNull] Type typeToConvert, [NotNull] JsonSerializerOptions options)
      {
         var enumType = typeToConvert.FindGenericEnumTypeDefinition();

         if (enumType == null)
            throw new InvalidOperationException($"The provided type does not derive from 'Enum<,>'. Type: {typeToConvert.Name}");

         var keyConverter = options.GetConverter(enumType.GenericTypeArguments[1]);

         if (keyConverter is null)
            throw new ArgumentException($"The enum '{typeToConvert.Name}' is not JSON-serializable because there is no {nameof(JsonConverter)} for its key of type '{enumType.GenericTypeArguments[1].Name}'.", nameof(typeToConvert));

         var converterType = typeof(EnumJsonConverter<,>).MakeGenericType(enumType.GenericTypeArguments);

         return (JsonConverter)Activator.CreateInstance(converterType, keyConverter);
      }
   }
}
