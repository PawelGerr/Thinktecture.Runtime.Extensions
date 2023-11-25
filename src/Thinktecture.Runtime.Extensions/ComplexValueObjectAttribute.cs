using System.Numerics;

namespace Thinktecture;

/// <summary>
/// Marks the type as a Value Object.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ComplexValueObjectAttribute : Attribute
{
   /// <summary>
   /// Indication whether the methods "Create", "Validate" and "TryCreate" should be generated or not.
   /// </summary>
   public bool SkipFactoryMethods { get; set; }

   private string? _createFactoryMethodName;

   /// <summary>
   /// The name of the factory method "Create".
   /// </summary>
   public string CreateFactoryMethodName
   {
      get => _createFactoryMethodName ?? "Create";
      set => _createFactoryMethodName = String.IsNullOrWhiteSpace(value) ? null : value.Trim();
   }

   private string? _tryCreateFactoryMethodName;

   /// <summary>
   /// The name of the factory method "TryCreate".
   /// </summary>
   public string TryCreateFactoryMethodName
   {
      get => _tryCreateFactoryMethodName ?? "TryCreate";
      set => _tryCreateFactoryMethodName = String.IsNullOrWhiteSpace(value) ? null : value.Trim();
   }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IEqualityOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - on non-keyed value objects (i.e. has more than 1 field/property)
   /// - if key-member is not an <see cref="IEqualityOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Equality</c>, <c>op_Inequality</c>).
   /// </summary>
   public OperatorsGeneration EqualityComparisonOperators { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of the method <see cref="object.ToString"/> or not.
   /// </summary>
   public bool SkipToString { get; set; }

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
