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
            // ReadOnlySpan<char>-based object factories are excluded because a ref struct cannot be used as the
            // generic key argument of ThinktectureMessagePackFormatter, which mirrors the source generator and
            // lets the conversion fall back to the key-based metadata.
            f => f.ValueType != typeof(ReadOnlySpan<char>) && f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.MessagePack),
            _ => true);

         if (metadata is null)
            return;

         // FindMetadataForConversion may return metadata that belongs to a base type, for example when T is the
         // runtime type of an item of a derived (nested) Smart Enum. The formatter is created for metadata.Value.Type
         // (the base type) and, because IMessagePackFormatter<T> is invariant, cannot be cast to IMessagePackFormatter<T>
         // for the derived T. Return null (no formatter) so MessagePack continues with the next resolver, which is the
         // behavior before conversion metadata resolved base types. Nullable value types share the formatter of their
         // underlying type (ThinktectureStructMessagePackFormatter implements both IMessagePackFormatter<T> and
         // IMessagePackFormatter<T?>), so the underlying type is compared here.
         var underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

         if (metadata.Value.Type != underlyingType)
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

         Formatter = (IMessagePackFormatter<T>)formatter;
         HasMessagePackFormatterAttribute = metadata.Value.Type.GetCustomAttribute<MessagePackFormatterAttribute>() is not null;
      }
   }
}
