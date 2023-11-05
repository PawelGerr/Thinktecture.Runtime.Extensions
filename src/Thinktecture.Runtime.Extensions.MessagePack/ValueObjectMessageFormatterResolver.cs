using System.Reflection;
using MessagePack;
using MessagePack.Formatters;
using Thinktecture.Formatters;
using Thinktecture.Internal;

namespace Thinktecture;

/// <summary>
/// MessagePack formatter resolver for Value Objects.
/// </summary>
public class ValueObjectMessageFormatterResolver : IFormatterResolver
{
   /// <summary>
   /// Singleton.
   /// </summary>
   public static readonly IFormatterResolver Instance = new ValueObjectMessageFormatterResolver();

   /// <inheritdoc />
   public IMessagePackFormatter<T>? GetFormatter<T>()
   {
      var formatter = Cache<T>.Formatter;

      if (formatter != null)
         return formatter;

      if (Cache<T>.InitError != null)
         throw new Exception(Cache<T>.InitError);

      return null;
   }

   private static class Cache<T>
   {
      public static readonly IMessagePackFormatter<T>? Formatter;

      // ReSharper disable once StaticMemberInGenericType
      public static readonly string? InitError;

      static Cache()
      {
         var type = typeof(T);
         var metadata = KeyedValueObjectMetadataLookup.Find(type);
         var customFactory = type.GetCustomAttributes<ValueObjectFactoryAttribute>()
                                 .LastOrDefault(a => a.UseForSerialization.HasFlag(SerializationFrameworks.MessagePack));

         Type modelType;
         Type keyType;

         if (customFactory is not null)
         {
            modelType = type;
            keyType = customFactory.Type;
         }
         else if (metadata is not null)
         {
            modelType = metadata.Type;
            keyType = metadata.KeyType;
         }
         else
         {
            return;
         }

         var formatterTypeDefinition = modelType.IsClass
                                          ? typeof(ValueObjectMessagePackFormatter<,>)
                                          : typeof(StructValueObjectMessagePackFormatter<,>);
         var formatterType = formatterTypeDefinition.MakeGenericType(modelType, keyType);

         var formatter = Activator.CreateInstance(formatterType);

         if (formatter is null)
         {
            InitError = $"The formatter of '{formatterType.Name}' could not be instantiated.";
            return;
         }

         Formatter = (IMessagePackFormatter<T>)formatter;
      }
   }
}
