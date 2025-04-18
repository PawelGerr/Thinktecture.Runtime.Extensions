using System.Reflection;
using MessagePack;
using MessagePack.Formatters;
using Thinktecture.Formatters;
using Thinktecture.Internal;

namespace Thinktecture;

/// <summary>
/// MessagePack formatter resolver for Value Objects.
/// </summary>
[Obsolete("Use 'ThinktectureMessageFormatterResolver' instead.")]
public class ValueObjectMessageFormatterResolver : ThinktectureMessageFormatterResolver;

/// <summary>
/// MessagePack formatter resolver for Value Objects.
/// </summary>
public class ThinktectureMessageFormatterResolver : IFormatterResolver
{
   /// <summary>
   /// Singleton.
   /// </summary>
   public static readonly IFormatterResolver Instance = new ThinktectureMessageFormatterResolver();

   private readonly bool _skipObjectsWithMessagePackFormatterAttribute;

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectMessageFormatterResolver"/>.
   /// </summary>
   public ThinktectureMessageFormatterResolver()
      : this(true)
   {
   }

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectMessageFormatterResolver"/>.
   /// </summary>
   /// <param name="skipObjectsWithMessagePackFormatterAttribute">
   /// Indication whether to skip value objects with <see cref="MessagePackFormatterAttribute"/>.
   /// </param>
   public ThinktectureMessageFormatterResolver(bool skipObjectsWithMessagePackFormatterAttribute)
   {
      _skipObjectsWithMessagePackFormatterAttribute = skipObjectsWithMessagePackFormatterAttribute;
   }

   /// <inheritdoc />
   public IMessagePackFormatter<T>? GetFormatter<T>()
   {
      if (_skipObjectsWithMessagePackFormatterAttribute && Cache<T>.HasMessagePackFormatterAttribute)
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

         var metadata = MetadataLookup.Find(type) as Metadata.Keyed;

         if (metadata is not null)
            type = metadata.Type;

         var customFactory = type.GetCustomAttributes<ObjectFactoryAttribute>()
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

         var validationErrorType = metadata?.ValidationErrorType ?? type.GetCustomAttribute<ValidationErrorAttribute>()?.Type ?? typeof(ValidationError);

         var formatterTypeDefinition = type.IsClass
                                          ? typeof(ThinktectureMessagePackFormatter<,,>)
                                          : typeof(ThinktectureStructMessagePackFormatter<,,>);
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
