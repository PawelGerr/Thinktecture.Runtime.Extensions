using System.Numerics;

namespace Thinktecture;

/// <summary>
/// Marks the type as a Value Object.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ValueObjectAttribute : Attribute
{
   /// <summary>
   /// Indication whether the methods "Create", "Validate" and "TryCreate" should be generated or not.
   /// </summary>
   public bool SkipFactoryMethods { get; set; }

   private bool _nullInFactoryMethodsYieldsNull;

   /// <summary>
   /// By default, providing <c>null</c> to methods "Create", "Validate" and "TryCreate" of an keyed value object is not allowed.
   /// If this property is set to <c>true</c>, then providing a <c>null</c> will return <c>null</c>.
   ///
   /// This setting has no effect on:
   /// - non-keyed value objects (i.e. has more than 1 field/property)
   /// - if <see cref="SkipFactoryMethods"/> is set <c>true</c>
   /// - if the value object is a struct
   /// - if key-member is a struct
   /// </summary>
   public bool NullInFactoryMethodsYieldsNull
   {
      get => _nullInFactoryMethodsYieldsNull || EmptyStringInFactoryMethodsYieldsNull;
      set => _nullInFactoryMethodsYieldsNull = value;
   }

   /// <summary>
   /// By default, having a key property of type of <see cref="string"/> and providing an empty <see cref="string"/> or whitespaces to methods "Create" and "TryCreate" leads to creation of new value object.
   /// If this property is set to <c>true</c>, then providing a an empty string or whitespaces will return <c>null</c>.
   /// By settings this property to <c>true</c>, the property <see cref="NullInFactoryMethodsYieldsNull"/> will be also <c>true</c>.
   ///
   /// This setting has no effect on:
   /// - non-keyed value objects (i.e. has more than 1 field/property)
   /// - if <see cref="SkipFactoryMethods"/> is set <c>true</c>
   /// - if the value object is a struct
   /// - if key-member is not a <see cref="string"/>.
   /// </summary>
   public bool EmptyStringInFactoryMethodsYieldsNull { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IComparable{T}"/> or not.
   ///
   /// This setting has no effect if:
   /// - non-keyed value objects (i.e. has more than 1 field/property)
   /// - key-member is not <see cref="IComparable{T}"/> itself and <see cref="ValueObjectMemberComparerAttribute{T,TMember}"/> is not set.
   /// </summary>
   public bool SkipIComparable { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IParsable{T}"/> or not.
   ///
   /// This setting has no effect if:
   /// - if <see cref="SkipFactoryMethods"/> is set <c>true</c>
   /// - non-keyed value objects (i.e. has more than 1 field/property)
   /// - key-member is neither a <see cref="string"/> nor an <see cref="IParsable{T}"/> itself.
   /// </summary>
   public bool SkipIParsable { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IAdditionOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - if <see cref="SkipFactoryMethods"/> is set <c>true</c>
   /// - on non-keyed value objects (i.e. has more than 1 field/property)
   /// - if key-member is not an <see cref="IAdditionOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Addition</c>, <c>op_CheckedAddition</c>).
   /// </summary>
   public OperatorsGeneration AdditionOperators { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="ISubtractionOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - if <see cref="SkipFactoryMethods"/> is set <c>true</c>
   /// - on non-keyed value objects (i.e. has more than 1 field/property)
   /// - if key-member is not an <see cref="ISubtractionOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Subtraction</c>, <c>op_CheckedSubtraction</c>).
   /// </summary>
   public OperatorsGeneration SubtractionOperators { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IMultiplyOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - if <see cref="SkipFactoryMethods"/> is set <c>true</c>
   /// - on non-keyed value objects (i.e. has more than 1 field/property)
   /// - if key-member is not an <see cref="IMultiplyOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Multiply</c>, <c>op_CheckedMultiply</c>).
   /// </summary>
   public OperatorsGeneration MultiplyOperators { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IDivisionOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - if <see cref="SkipFactoryMethods"/> is set <c>true</c>
   /// - on non-keyed value objects (i.e. has more than 1 field/property)
   /// - if key-member is not an <see cref="IDivisionOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Division</c>, <c>op_CheckedDivision</c>).
   /// </summary>
   public OperatorsGeneration DivisionOperators { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IComparisonOperators{TSelf,TOther,TResult}"/>.
   ///
   /// Please note that the comparison operators depend on <see cref="EqualityComparisonOperators"/>. For example, if <see cref="ComparisonOperators"/> are set to <see cref="OperatorsGeneration.DefaultWithKeyTypeOverloads"/>
   /// then the <see cref="EqualityComparisonOperators"/> are set to <see cref="OperatorsGeneration.DefaultWithKeyTypeOverloads"/> as well.
   ///
   /// This setting has no effect:
   /// - on non-keyed value objects (i.e. has more than 1 field/property)
   /// - if key-member is not an <see cref="IComparisonOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_GreaterThan</c>, <c>op_GreaterThanOrEqual</c>, <c>op_LessThan</c>, <c>op_LessThanOrEqual</c>).
   /// </summary>
   public OperatorsGeneration ComparisonOperators { get; set; }

   private OperatorsGeneration _equalityComparisonOperators;

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IEqualityOperators{TSelf,TOther,TResult}"/>.
   ///
   /// Please note that the comparison operators depend on <see cref="EqualityComparisonOperators"/>. For example, if <see cref="ComparisonOperators"/> are set to <see cref="OperatorsGeneration.DefaultWithKeyTypeOverloads"/>
   /// then the <see cref="EqualityComparisonOperators"/> are set to <see cref="OperatorsGeneration.DefaultWithKeyTypeOverloads"/> as well.
   ///
   /// This setting has no effect:
   /// - on non-keyed value objects (i.e. has more than 1 field/property)
   /// - if key-member is not an <see cref="IEqualityOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Equality</c>, <c>op_Inequality</c>).
   /// </summary>
   public OperatorsGeneration EqualityComparisonOperators
   {
      get => ComparisonOperators > _equalityComparisonOperators ? ComparisonOperators : _equalityComparisonOperators;
      set => _equalityComparisonOperators = value;
   }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IFormattable"/> or not.
   ///
   /// This setting has no effect if:
   /// - the key-member is not an <see cref="IFormattable"/> itself.
   /// </summary>
   public bool SkipIFormattable { get; set; }

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
