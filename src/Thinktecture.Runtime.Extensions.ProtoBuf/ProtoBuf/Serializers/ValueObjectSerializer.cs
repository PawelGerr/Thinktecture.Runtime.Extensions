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
public class ValueObjectSerializer<T, TKey, TValidationError> : ISerializer<T>
   where T : IValueObjectFactory<T, TKey, TValidationError>, IValueObjectConvertable<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));

   private readonly ISerializer<TKey> _keySerializer = KeySerializerCache<TKey>.Instance.Serializer;

   /// <inheritdoc />
   public SerializerFeatures Features => _keySerializer.Features;

   /// <inheritdoc />
   public T Read(ref ProtoReader.State state, T value)
   {
      var key = _keySerializer.Read(ref state, default!);

      if (key is null)
         return default!;

      var validationError = T.Validate(key, null, out var obj);

      if (validationError is not null && !_mayReturnInvalidObjects)
         throw new ProtoException(validationError.ToString() ?? "Protobuf deserialization failed.");

      return obj!;
   }

   /// <inheritdoc />
   public void Write(ref ProtoWriter.State state, T value)
   {
      if (value is null)
         throw new ArgumentNullException(nameof(value));

      var key = value.ToValue();

      _keySerializer.Write(ref state, key);
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
