using System.Reflection;
using MessagePack;
using MessagePack.Formatters;
using Thinktecture.Formatters;
using Thinktecture.Internal;

namespace Thinktecture;

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
   /// Initializes new instance of <see cref="ThinktectureMessageFormatterResolver"/>.
   /// </summary>
   public ThinktectureMessageFormatterResolver()
      : this(true)
   {
   }

   /// <summary>
   /// Initializes new instance of <see cref="ThinktectureMessageFormatterResolver"/>.
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

   /// <summary>
   /// Delegates to the formatter of the base type <typeparamref name="TBase"/>.
   /// Required because <see cref="IMessagePackFormatter{T}"/> is invariant, so the formatter created for the base
   /// type of a derived (nested) Smart Enum cannot be cast to <see cref="IMessagePackFormatter{T}"/> of the
   /// derived type.
   /// </summary>
   private sealed class DerivedTypeFormatter<T, TBase>(IMessagePackFormatter<TBase?> baseFormatter) : IMessagePackFormatter<T?>
      where T : class, TBase
      where TBase : class
   {
      public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
      {
         baseFormatter.Serialize(ref writer, value, options);
      }

      public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
      {
         // Throws an InvalidCastException if the payload belongs to another derived type than the requested one.
         // That is correct: the caller explicitly asked for this derived type.
         return (T?)baseFormatter.Deserialize(ref reader, options);
      }
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
         var metadata = MetadataLookup.FindMetadataForConversion(
            typeof(T),
            // Object factories with a ref struct value type are excluded because a ref struct cannot be used as the
            // generic key argument of ThinktectureMessagePackFormatter. Such factories fall back to the key-based
            // metadata.
            f => !f.ValueType.IsByRefLike && f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.MessagePack),
            _ => true);

         if (metadata is null)
            return;

         // FindMetadataForConversion may return metadata that belongs to a base type, for example when T is the
         // runtime type of an item of a derived (nested) Smart Enum. Nullable value types share the formatter of their
         // underlying type (ThinktectureStructMessagePackFormatter implements both IMessagePackFormatter<T> and
         // IMessagePackFormatter<T?>), so the underlying type is compared here.
         var underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
         var isDerivedType = metadata.Value.Type != underlyingType;

         // Metadata of an unrelated type yields no formatter, so MessagePack continues with the next resolver.
         if (isDerivedType && !(metadata.Value.Type.IsClass && metadata.Value.Type.IsAssignableFrom(typeof(T))))
            return;

         var formatterTypeDefinition = metadata.Value.Type.IsClass
                                          ? typeof(ThinktectureMessagePackFormatter<,,>)
                                          : typeof(ThinktectureStructMessagePackFormatter<,,>);
         var formatterType = formatterTypeDefinition.MakeGenericType(metadata.Value.Type, metadata.Value.KeyType, metadata.Value.ValidationErrorType);
         var formatter = Activator.CreateInstance(formatterType);

         if (formatter is null)
         {
            InitError = $"The formatter of '{formatterType.Name}' could not be instantiated.";
            return;
         }

         if (isDerivedType)
         {
            // The formatter belongs to the base type and IMessagePackFormatter<T> is invariant, so it is wrapped in a
            // casting formatter. "HasMessagePackFormatterAttribute" stays false on purpose: the derived type inherits
            // the [MessagePackFormatter] attribute of the base type, but MessagePack's AttributeFormatterResolver
            // cannot honor it (its cast to IMessagePackFormatter<derived> throws). GetFormatter must therefore not
            // defer to the attribute path for a derived type.
            var wrapperType = typeof(DerivedTypeFormatter<,>).MakeGenericType(typeof(T), metadata.Value.Type);
            var wrapper = Activator.CreateInstance(wrapperType, formatter);

            if (wrapper is null)
            {
               InitError = $"The formatter of '{wrapperType.Name}' could not be instantiated.";
               return;
            }

            Formatter = (IMessagePackFormatter<T>)wrapper;
            return;
         }

         Formatter = (IMessagePackFormatter<T>)formatter;
         HasMessagePackFormatterAttribute = metadata.Value.Type.GetCustomAttribute<MessagePackFormatterAttribute>() is not null;
      }
   }
}
