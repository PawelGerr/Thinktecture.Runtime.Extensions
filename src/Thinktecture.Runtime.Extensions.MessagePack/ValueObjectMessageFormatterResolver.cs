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

   private readonly bool _skipValueObjectsWithMessagePackFormatterAttribute;

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectMessageFormatterResolver"/>.
   /// </summary>
   public ValueObjectMessageFormatterResolver()
      : this(true)
   {
   }

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectMessageFormatterResolver"/>.
   /// </summary>
   /// <param name="skipValueObjectsWithMessagePackFormatterAttribute">
   /// Indication whether to skip value objects with <see cref="MessagePackFormatterAttribute"/>.
   /// </param>
   public ValueObjectMessageFormatterResolver(bool skipValueObjectsWithMessagePackFormatterAttribute)
   {
      _skipValueObjectsWithMessagePackFormatterAttribute = skipValueObjectsWithMessagePackFormatterAttribute;
   }

   /// <inheritdoc />
   public IMessagePackFormatter<T>? GetFormatter<T>()
   {
      if (_skipValueObjectsWithMessagePackFormatterAttribute && Cache<T>.HasMessagePackFormatterAttribute)
         return null;

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

      // ReSharper disable StaticMemberInGenericType
      public static readonly bool HasMessagePackFormatterAttribute;
      public static readonly string? InitError;
      // ReSharper restore StaticMemberInGenericType

      static Cache()
      {
         // T could be derived type (like nested Smart Enum)
         var type = typeof(T);

         var metadata = KeyedValueObjectMetadataLookup.Find(type);

         if (metadata is not null)
            type = metadata.Type;

         var customFactory = type.GetCustomAttributes<ValueObjectFactoryAttribute>()
                                 .LastOrDefault(a => a.UseForSerialization.HasFlag(SerializationFrameworks.MessagePack));

         Type keyType;

         if (customFactory is not null)
         {
            keyType = customFactory.Type;
         }
         else if (metadata is not null)
         {
            keyType = metadata.KeyType;
         }
         else
         {
            return;
         }

         var validationErrorType = type.GetCustomAttribute<ValueObjectValidationErrorAttribute>()?.Type ?? typeof(ValidationError);

         var formatterTypeDefinition = type.IsClass
                                          ? typeof(ValueObjectMessagePackFormatter<,,>)
                                          : typeof(StructValueObjectMessagePackFormatter<,,>);
         var formatterType = formatterTypeDefinition.MakeGenericType(type, keyType, validationErrorType);

         var formatter = Activator.CreateInstance(formatterType);

         if (formatter is null)
         {
            InitError = $"The formatter of '{formatterType.Name}' could not be instantiated.";
            return;
         }

         Formatter = (IMessagePackFormatter<T>)formatter;
         HasMessagePackFormatterAttribute = type.GetCustomAttribute<MessagePackFormatterAttribute>() is not null;
      }
   }
}
