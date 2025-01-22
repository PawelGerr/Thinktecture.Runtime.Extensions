using System.Numerics;

namespace Thinktecture;

/// <summary>
/// Marks the type as a Value Object.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class ValueObjectAttribute<TKey> : ValueObjectAttributeBase
   where TKey : notnull
{
   /// <summary>
   /// Type of the key member.
   /// </summary>
   public Type KeyMemberType { get; }

   /// <summary>
   /// Access modifier of the key member.
   /// Default is <see cref="ValueObjectAccessModifier.Private"/>.
   /// </summary>
   public ValueObjectAccessModifier KeyMemberAccessModifier { get; set; }

   /// <summary>
   /// Kind of the key member.
   /// Default is <see cref="ValueObjectMemberKind.Field"/>.
   /// </summary>
   public ValueObjectMemberKind KeyMemberKind { get; set; }

   private string? _keyMemberName;

   /// <summary>
   /// The name of the key member.
   /// Default: <c>_value</c> if the key member is a private field; otherwise <c>Value</c>.
   /// </summary>
   public string KeyMemberName
   {
      get => _keyMemberName ?? (KeyMemberAccessModifier == ValueObjectAccessModifier.Private && KeyMemberKind == ValueObjectMemberKind.Field ? "_value" : "Value");
      set => _keyMemberName = value;
   }

   /// <summary>
   /// Indication whether to generate the key member of type <typeparamref name="TKey"/>.
   /// If set to <c>true</c> then the key member must be implemented manually.
   /// Use <see cref="KeyMemberName"/> to tell source generator the chosen name of the field/property.
   /// If the member is a property with a backing field, then the property must have an <c>init</c> setter.
   /// </summary>
   public bool SkipKeyMember { get; set; }

   private bool _nullInFactoryMethodsYieldsNull;

   /// <summary>
   /// By default, providing <c>null</c> to methods "Create", "Validate" and "TryCreate" is not allowed.
   /// If this property is set to <c>true</c>, then providing a <c>null</c> will return <c>null</c>.
   ///
   /// This setting has no effect on:
   /// - if <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>
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
   /// If this property is set to <c>true</c>, then providing an empty string or whitespaces will return <c>null</c>.
   /// By settings this property to <c>true</c>, the property <see cref="NullInFactoryMethodsYieldsNull"/> will be also be <c>true</c>.
   ///
   /// This setting has no effect on:
   /// - if <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>
   /// - if the value object is a struct
   /// - if key-member is not a <see cref="string"/>.
   /// </summary>
   public bool EmptyStringInFactoryMethodsYieldsNull { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IComparable{T}"/> or not.
   ///
   /// This setting has no effect if:
   /// - key-member is not <see cref="IComparable{T}"/> itself and no custom comparer is provided via <see cref="ValueObjectKeyMemberComparerAttribute{T,TMember}"/>.
   /// </summary>
   public bool SkipIComparable { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IParsable{T}"/> or not.
   ///
   /// This setting has no effect if:
   /// - if <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>
   /// - key-member is neither a <see cref="string"/> nor an <see cref="IParsable{T}"/> itself.
   /// </summary>
   public bool SkipIParsable { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IAdditionOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - if <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>
   /// - if key-member is not an <see cref="IAdditionOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Addition</c>, <c>op_CheckedAddition</c>).
   /// </summary>
   public OperatorsGeneration AdditionOperators { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="ISubtractionOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - if <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>
   /// - if key-member is not an <see cref="ISubtractionOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Subtraction</c>, <c>op_CheckedSubtraction</c>).
   /// </summary>
   public OperatorsGeneration SubtractionOperators { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IMultiplyOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - if <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>
   /// - if key-member is not an <see cref="IMultiplyOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Multiply</c>, <c>op_CheckedMultiply</c>).
   /// </summary>
   public OperatorsGeneration MultiplyOperators { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IDivisionOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - if <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>
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
   /// - if key-member is not an <see cref="IComparisonOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_GreaterThan</c>, <c>op_GreaterThanOrEqual</c>, <c>op_LessThan</c>, <c>op_LessThanOrEqual</c>).
   /// </summary>
   public OperatorsGeneration ComparisonOperators { get; set; }

   private OperatorsGeneration _equalityComparisonOperators;

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IEqualityOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - if key-member is not an <see cref="IEqualityOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Equality</c>, <c>op_Inequality</c>).
   /// </summary>
   public override OperatorsGeneration EqualityComparisonOperators
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
   /// Indication whether and how the generator should generate the conversion operators from value object to <see cref="KeyMemberType"/>.
   /// Default is <see cref="ConversionOperatorsGeneration.Implicit"/>.
   /// </summary>
   public ConversionOperatorsGeneration ConversionToKeyMemberType { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate "unsafe" conversion operators from value object to <see cref="KeyMemberType"/>.
   /// Conversion is considered unsafe if it can throw an <see cref="Exception"/>.
   /// Default is <see cref="ConversionOperatorsGeneration.Explicit"/>.
   /// </summary>
   public ConversionOperatorsGeneration UnsafeConversionToKeyMemberType { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the conversion operators from <see cref="KeyMemberType"/> to enum type.
   /// Default is <see cref="ConversionOperatorsGeneration.Explicit"/>.
   /// </summary>
   public ConversionOperatorsGeneration ConversionFromKeyMemberType { get; set; }

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectAttribute{TKey}"/>.
   /// </summary>
   public ValueObjectAttribute()
   {
      KeyMemberType = typeof(TKey);
      KeyMemberAccessModifier = ValueObjectAccessModifier.Private;
      ConversionToKeyMemberType = ConversionOperatorsGeneration.Implicit;
      UnsafeConversionToKeyMemberType = ConversionOperatorsGeneration.Explicit;
      ConversionFromKeyMemberType = ConversionOperatorsGeneration.Explicit;
   }
}
