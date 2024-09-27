using System.Numerics;

namespace Thinktecture;

/// <summary>
/// Base class for <see cref="ValueObjectAttribute{TKey}"/> and <see cref="ComplexValueObjectAttribute"/>.
/// </summary>
public abstract class ValueObjectAttributeBase : Attribute
{
   /// <summary>
   /// Indication whether the methods "Create", "Validate" and "TryCreate" should be generated or not.
   /// </summary>
   public bool SkipFactoryMethods { get; set; }

   /// <summary>
   /// Access modifier of the constructor.
   /// Default is <see cref="ValueObjectAccessModifier.Private"/>.
   /// </summary>
   public ValueObjectAccessModifier ConstructorAccessModifier { get; set; }

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
   /// </summary>
   public abstract OperatorsGeneration EqualityComparisonOperators { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of the method <see cref="object.ToString"/> or not.
   /// </summary>
   public bool SkipToString { get; set; }

   /// <summary>
   /// By default, explicit creation of a struct value objects using keyword <c>default</c> or via parameterless constructor is disallowed.
   /// Set this property to <c>true</c> to allow creation of value objects with default values.
   ///
   /// This setting has no effect if:
   /// - value object is not a struct
   /// </summary>
   public bool AllowDefaultStructs { get; set; }

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

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectAttributeBase"/>.
   /// </summary>
   protected ValueObjectAttributeBase()
   {
      ConstructorAccessModifier = ValueObjectAccessModifier.Private;
   }
}
