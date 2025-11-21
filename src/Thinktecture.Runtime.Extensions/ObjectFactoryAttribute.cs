namespace Thinktecture;

/// <summary>
/// Signals a desire for an (additional) factory for creation of objects from a value of type <see cref="Type"/>
/// </summary>
public abstract class ObjectFactoryAttribute : Attribute
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
   /// Indication whether to use the corresponding factory with Entity Framework Core or not.
   /// </summary>
   public bool UseWithEntityFramework { get; set; }

   /// <summary>
   /// Indication whether to use the corresponding factory with ASP.NET Core model binding or not.
   /// </summary>
   public bool UseForModelBinding { get; init; }

   /// <summary>
   /// Indicates whether there is a corresponding constructor that takes single parameter of type <see cref="Type"/>.
   /// </summary>
   public bool HasCorrespondingConstructor { get; init; }

   /// <summary>
   /// Initializes new instance of type <see cref="ObjectFactoryAttribute"/>.
   /// </summary>
   /// <param name="type">Type of the value to create value objects from.</param>
   private protected ObjectFactoryAttribute(Type type)
   {
      Type = type;
   }
}

/// <summary>
/// Signals a desire for an (additional) factory for creation of objects from a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the value to be able to create a value object from.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public class ObjectFactoryAttribute<T> : ObjectFactoryAttribute
#if NET9_0_OR_GREATER
   where T : allows ref struct
#endif
{
   /// <summary>
   /// Initializes new instance of type <see cref="ObjectFactoryAttribute{T}"/>.
   /// </summary>
   public ObjectFactoryAttribute()
      : base(typeof(T))
   {
   }
}
