namespace Thinktecture;

/// <summary>
/// Signals a desire for an (additional) factory for creation of value objects from a value of type <see cref="Type"/>
/// </summary>
public abstract class ValueObjectFactoryAttribute : Attribute
{
   /// <summary>
   /// Type of the value to create value objects from.
   /// </summary>
   public Type Type { get; }

   /// <summary>
   /// Serialization frameworks that should use the type <see cref="Type"/> for serialization and deserialization.
   /// </summary>
   public SerializationFrameworks UseForSerialization { get; set; }

   /// <summary>
   /// Initializes new instance of type <see cref="ValueObjectFactoryAttribute"/>.
   /// </summary>
   /// <param name="type">Type of the value to create value objects from.</param>
   protected ValueObjectFactoryAttribute(Type type)
   {
      Type = type;
   }
}

/// <summary>
/// Signals a desire for an (additional) factory for creation of value objects from a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the value to be able to create a value object from.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public sealed class ValueObjectFactoryAttribute<T> : ValueObjectFactoryAttribute
{
   /// <summary>
   /// Initializes new instance of type <see cref="ValueObjectFactoryAttribute{T}"/>.
   /// </summary>
   public ValueObjectFactoryAttribute()
      : base(typeof(T))
   {
   }
}
