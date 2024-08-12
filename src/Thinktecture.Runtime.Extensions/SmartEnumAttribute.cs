using System.Numerics;

namespace Thinktecture;

/// <summary>
/// Marks the type as a Smart Enum that has no identifier (i.e. no key-member).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SmartEnumAttribute : Attribute
{
   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IEqualityOperators{TSelf,TOther,TResult}"/>.
   ///
   /// The value <see cref="OperatorsGeneration.DefaultWithKeyTypeOverloads"/> will generate the same code as <see cref="OperatorsGeneration.Default"/> because the Smart Enum has no key-member.
   /// </summary>
   public OperatorsGeneration EqualityComparisonOperators { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Switch</c>.
   /// </summary>
   public SwitchMapMethodsGeneration SwitchMethods { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Map</c>.
   /// </summary>
   public SwitchMapMethodsGeneration MapMethods { get; set; }
}

/// <summary>
/// Marks the type as a Smart Enum with an identifier of type <typeparamref name="TKey"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class SmartEnumAttribute<TKey> : Attribute
   where TKey : notnull
{
   /// <summary>
   /// The type of the key-property.
   /// </summary>
   public Type KeyMemberType { get; set; }

   /// <summary>
   /// Access modifier of the key member.
   /// Default is <see cref="ValueObjectAccessModifier.Public"/>.
   /// </summary>
   public ValueObjectAccessModifier KeyMemberAccessModifier { get; set; }

   /// <summary>
   /// Kind of the key member.
   /// Default is <see cref="ValueObjectMemberKind.Property"/>.
   /// </summary>
   public ValueObjectMemberKind KeyMemberKind { get; set; }

   private string? _keyMemberName;

   /// <summary>
   /// The name of the key member.
   /// Default: <c>_key</c> if the key member is a private field; otherwise <c>Key</c>.
   /// </summary>
   public string KeyMemberName
   {
      get => _keyMemberName ?? (KeyMemberAccessModifier == ValueObjectAccessModifier.Private && KeyMemberKind == ValueObjectMemberKind.Field ? "_key" : "Key");
      set => _keyMemberName = value;
   }

   /// <summary>
   /// Indication whether the Smart Enum should be "validatable" or always-valid one.
   /// Default is "false", i.e. always-valid.
   /// </summary>
   public bool IsValidatable { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IComparable{T}"/> or not.
   ///
   /// This setting has no effect if:
   /// - the key is not <see cref="IComparable{T}"/> itself.
   /// </summary>
   public bool SkipIComparable { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IParsable{T}"/> or not.
   ///
   /// This setting has no effect if:
   /// - the key is neither a <see cref="string"/> nor an <see cref="IParsable{T}"/> itself.
   /// </summary>
   public bool SkipIParsable { get; set; }

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
   public OperatorsGeneration EqualityComparisonOperators
   {
      get => ComparisonOperators > _equalityComparisonOperators ? ComparisonOperators : _equalityComparisonOperators;
      set => _equalityComparisonOperators = value;
   }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IFormattable"/> or not.
   ///
   /// This setting has no effect if:
   /// - the key is not an <see cref="IFormattable"/> itself.
   /// </summary>
   public bool SkipIFormattable { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of the method <see cref="object.ToString"/> or not.
   /// </summary>
   public bool SkipToString { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Switch</c>.
   /// </summary>
   public SwitchMapMethodsGeneration SwitchMethods { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Map</c>.
   /// </summary>
   public SwitchMapMethodsGeneration MapMethods { get; set; }

   /// <summary>
   /// Initializes new instance of <see cref="SmartEnumAttribute{TKey}"/>.
   /// </summary>
   public SmartEnumAttribute()
   {
      KeyMemberType = typeof(TKey);
      KeyMemberAccessModifier = ValueObjectAccessModifier.Public;
      KeyMemberKind = ValueObjectMemberKind.Property;
   }
}
