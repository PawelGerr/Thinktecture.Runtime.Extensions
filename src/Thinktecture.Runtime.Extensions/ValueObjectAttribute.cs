namespace Thinktecture;

/// <summary>
/// Marks the type as a Value Object.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ValueObjectAttribute : Attribute
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
   /// - non-keyed value objects
   /// - if <see cref="SkipFactoryMethods"/> is <c>true</c>
   /// - on value objects which are structs
   /// - on key-members which are structs
   /// </summary>
   public bool NullInFactoryMethodsYieldsNull { get; set; }

   /// <summary>
   /// Indication whether the generator should implement <see cref="IComparable{T}"/> interface or not.
   /// </summary>
   public bool SkipCompareTo { get; set; }
}
