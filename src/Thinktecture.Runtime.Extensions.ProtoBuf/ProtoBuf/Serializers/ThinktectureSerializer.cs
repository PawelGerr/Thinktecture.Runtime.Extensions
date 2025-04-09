using ProtoBuf;
using ProtoBuf.Meta;
using ProtoBuf.Serializers;

namespace Thinktecture.ProtoBuf.Serializers;

/// <summary>
/// ProtoBuf serializer for keyed value objects and smart enums.
/// </summary>
/// <typeparam name="T">The type of the value object or smart enum.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValidationError">The type of the validation error.</typeparam>
public class ThinktectureSerializer<T, TKey, TValidationError> : ISerializer<T>
   where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly bool _disallowDefaultValues = typeof(IDisallowDefaultValue).IsAssignableFrom(typeof(T));
   private static readonly TKey? _keyDefaultValue = default;

   private readonly ISerializer<TKey> _keySerializer = KeySerializerCache<TKey>.Instance.Serializer;

   /// <inheritdoc />
   public SerializerFeatures Features => _keySerializer.Features;

   /// <inheritdoc />
   public T Read(ref ProtoReader.State state, T value)
   {
      TKey? key;

      if ((_keySerializer.Features & SerializerFeatures.CategoryMessage) == SerializerFeatures.CategoryMessage)
      {
         key = state.ReadMessage(_keySerializer.Features, default!, _keySerializer);
      }
      else
      {
         key = _keySerializer.Read(ref state, default!);
      }

      if (key is null)
      {
         if (_disallowDefaultValues)
            throw new ProtoException($"Cannot convert null to type \"{typeof(T).Name}\" because it doesn't allow default values.");

         return default!;
      }

      if (_disallowDefaultValues && key.Equals(_keyDefaultValue))
         throw new ProtoException($"Cannot convert the value {_keyDefaultValue} to type \"{typeof(T).Name}\" because it doesn't allow default values.");

      var validationError = T.Validate(key, null, out var obj);

      if (validationError is not null)
         throw new ProtoException(validationError.ToString() ?? "Protobuf deserialization failed.");

      return obj!;
   }

   /// <inheritdoc />
   public void Write(ref ProtoWriter.State state, T value)
   {
      if (value is null)
         throw new ArgumentNullException(nameof(value));

      var key = value.ToValue();

      if ((_keySerializer.Features & SerializerFeatures.CategoryMessage) == SerializerFeatures.CategoryMessage)
      {
         state.WriteMessage(_keySerializer.Features, key, _keySerializer);
      }
      else
      {
         _keySerializer.Write(ref state, key);
      }
   }
}

file sealed class KeySerializerCache<T>
{
   public static readonly KeySerializerCache<T> Instance = new();

   public ISerializer<T> Serializer { get; }

   private KeySerializerCache()
   {
      using var state = ProtoReader.State.Create(Stream.Null, RuntimeTypeModel.Default);

      Serializer = state.GetSerializer<T>();
   }
}
