namespace Thinktecture;

/// <summary>
/// Marks the type as a Value Object.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ValueObjectAttribute : Attribute
{
   /// <summary>
   /// Indication whether the methods "Create" and "TryCreate" should be generated or not.
   /// </summary>
   public bool SkipFactoryMethods { get; set; }

   /// <summary>
   /// By default, providing <c>null</c> to methods "Create" and "TryCreate" of an keyed value object is not allowed.
   /// If this property is set to <c>true</c>, then providing a <c>null</c> will return <c>null</c>.
   ///
   /// This setting has no effect on:
   /// - non-keyed value objects (i.e. has more than 1 field/property)
   /// - if <see cref="SkipFactoryMethods"/> is set <c>true</c>
   /// - if the value object is a struct
   /// - if key-member is a struct
   /// </summary>
   public bool NullInFactoryMethodsYieldsNull { get; set; }

   /// <summary>
   /// Indication whether the generator should implement <see cref="IComparable{T}"/> interface or not.
   ///
   /// This setting has no effect on:
   /// - non-keyed value objects (i.e. has more than 1 field/property)
   /// - if key-member is not <see cref="IComparable{T}"/> itself and <see cref="ValueObjectEqualityMemberAttribute.Comparer"/> is not set.
   /// </summary>
   public bool SkipCompareTo { get; set; }

   private string? _defaultInstancePropertyName;

   /// <summary>
   /// The name of the static property containing the <c>default</c> instance of the struct.
   /// Default name is "Empty" (analogous to <c>Guid.Empty</c>).
   ///
   /// This setting has no effect on:
   /// - value objects that are classes
   /// </summary>
   public string DefaultInstancePropertyName
   {
      get => _defaultInstancePropertyName ?? "Empty";
      set => _defaultInstancePropertyName = value;
   }
}
