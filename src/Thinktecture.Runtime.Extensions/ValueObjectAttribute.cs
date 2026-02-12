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
   /// Default is <see cref="AccessModifier.Private"/>.
   /// </summary>
   public AccessModifier KeyMemberAccessModifier { get; set; }

   /// <summary>
   /// Kind of the key member.
   /// Default is <see cref="MemberKind.Field"/>.
   /// </summary>
   public MemberKind KeyMemberKind { get; set; }

   /// <summary>
   /// The name of the key member.
   /// Default: <c>_value</c> if the key member is a private field; otherwise <c>Value</c>.
   /// </summary>
   public string KeyMemberName
   {
      get => field ?? (KeyMemberAccessModifier == AccessModifier.Private && KeyMemberKind == MemberKind.Field ? "_value" : "Value");
      set;
   }

   /// <summary>
   /// Indication whether to generate the key member of type <typeparamref name="TKey"/>.
   /// If set to <c>true</c> then the key member must be implemented manually.
   /// Use <see cref="KeyMemberName"/> to tell source generator the chosen name of the field/property.
   /// If the member is a property with a backing field, then the property must have an <c>init</c> setter.
   /// </summary>
   public bool SkipKeyMember { get; set; }

   /// <summary>
   /// By default, providing <c>null</c> to methods "Create", "Validate" and "TryCreate" is not allowed.
   /// If this property is set to <c>true</c>, then providing a <c>null</c> will return <c>null</c>.
   /// </summary>
   /// <remarks>
   /// <para>This setting has no effect:</para>
   /// <para>If <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>.</para>
   /// <para>If the value object is a struct.</para>
   /// <para>If key-member is a struct.</para>
   /// </remarks>
   public bool NullInFactoryMethodsYieldsNull
   {
      get => field || EmptyStringInFactoryMethodsYieldsNull;
      set;
   }

   /// <summary>
   /// By default, having a key property of type of <see cref="string"/> and providing an empty <see cref="string"/> or whitespaces to methods "Create" and "TryCreate" leads to creation of new value object.
   /// If this property is set to <c>true</c>, then providing an empty string or whitespaces will return <c>null</c>.
   /// By settings this property to <c>true</c>, the property <see cref="NullInFactoryMethodsYieldsNull"/> will be also be <c>true</c>.
   /// </summary>
   /// <remarks>
   /// <para>This setting has no effect:</para>
   /// <para>If <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>.</para>
   /// <para>If the value object is a struct.</para>
   /// <para>If key-member is not a <see cref="string"/>.</para>
   /// </remarks>
   public bool EmptyStringInFactoryMethodsYieldsNull { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IComparable{T}"/> or not.
   /// </summary>
   /// <remarks>
   /// <para>This setting has no effect:</para>
   /// <para>If key-member is not <see cref="IComparable{T}"/> itself and no custom comparer is provided via <see cref="KeyMemberComparerAttribute{T,TMember}"/>.</para>
   /// </remarks>
   public bool SkipIComparable { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IParsable{T}"/> or not.
   /// </summary>
   /// <remarks>
   /// <para>Forced to <c>true</c> when <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>.</para>
   /// <para>Setting this to <c>true</c> also forces <see cref="SkipISpanParsable"/> to <c>true</c>,
   /// because <see cref="ISpanParsable{T}"/> inherits from <see cref="IParsable{T}"/>.</para>
   /// <para>This setting has no effect if key-member is neither a <see cref="string"/> nor an <see cref="IParsable{T}"/> itself.</para>
   /// </remarks>
   public bool SkipIParsable
   {
      get => field || SkipFactoryMethods;
      set;
   }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="ISpanParsable{T}"/> or not.
   /// </summary>
   /// <remarks>
   /// <para>Forced to <c>true</c> when <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>.</para>
   /// <para>
   /// Note that this property is dependent on <see cref="SkipIParsable"/>.
   /// If <see cref="SkipIParsable"/> is set to <c>true</c> then <see cref="SkipISpanParsable"/> is set to <c>true</c> as well.
   /// </para>
   /// <para>This setting has no effect if the key is neither a <see cref="string"/> nor an <see cref="ISpanParsable{T}"/> itself.</para>
   /// </remarks>
   public bool SkipISpanParsable
   {
      get => field || SkipIParsable;
      set;
   }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IAdditionOperators{TSelf,TOther,TResult}"/>.
   /// </summary>
   /// <remarks>
   /// <para>Forced to <see cref="OperatorsGeneration.None"/> when <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>.</para>
   /// <para>This setting has no effect if key-member is not an <see cref="IAdditionOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Addition</c>, <c>op_CheckedAddition</c>).</para>
   /// </remarks>
   public OperatorsGeneration AdditionOperators
   {
      get => SkipFactoryMethods ? OperatorsGeneration.None : field;
      set;
   }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="ISubtractionOperators{TSelf,TOther,TResult}"/>.
   /// </summary>
   /// <remarks>
   /// <para>Forced to <see cref="OperatorsGeneration.None"/> when <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>.</para>
   /// <para>This setting has no effect if key-member is not an <see cref="ISubtractionOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Subtraction</c>, <c>op_CheckedSubtraction</c>).</para>
   /// </remarks>
   public OperatorsGeneration SubtractionOperators
   {
      get => SkipFactoryMethods ? OperatorsGeneration.None : field;
      set;
   }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IMultiplyOperators{TSelf,TOther,TResult}"/>.
   /// </summary>
   /// <remarks>
   /// <para>Forced to <see cref="OperatorsGeneration.None"/> when <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>.</para>
   /// <para>This setting has no effect if key-member is not an <see cref="IMultiplyOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Multiply</c>, <c>op_CheckedMultiply</c>).</para>
   /// </remarks>
   public OperatorsGeneration MultiplyOperators
   {
      get => SkipFactoryMethods ? OperatorsGeneration.None : field;
      set;
   }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IDivisionOperators{TSelf,TOther,TResult}"/>.
   /// </summary>
   /// <remarks>
   /// <para>Forced to <see cref="OperatorsGeneration.None"/> when <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>.</para>
   /// <para>This setting has no effect if key-member is not an <see cref="IDivisionOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Division</c>, <c>op_CheckedDivision</c>).</para>
   /// </remarks>
   public OperatorsGeneration DivisionOperators
   {
      get => SkipFactoryMethods ? OperatorsGeneration.None : field;
      set;
   }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IComparisonOperators{TSelf,TOther,TResult}"/>.
   ///
   /// Please note that the comparison operators depend on <see cref="EqualityComparisonOperators"/>. For example, if <see cref="ComparisonOperators"/> are set to <see cref="OperatorsGeneration.DefaultWithKeyTypeOverloads"/>
   /// then the <see cref="EqualityComparisonOperators"/> are set to <see cref="OperatorsGeneration.DefaultWithKeyTypeOverloads"/> as well.
   /// </summary>
   /// <remarks>
   /// <para>This setting has no effect:</para>
   /// <para>If key-member is not an <see cref="IComparisonOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_GreaterThan</c>, <c>op_GreaterThanOrEqual</c>, <c>op_LessThan</c>, <c>op_LessThanOrEqual</c>).</para>
   /// <para>If <see cref="ValueObjectAttributeBase.SkipEqualityComparison"/> is set to <c>true</c>.</para>
   /// <para>In generated code, this is also forced to <see cref="OperatorsGeneration.None"/> when <see cref="EqualityComparisonOperators"/> is <see cref="OperatorsGeneration.None"/>,
   /// because comparison operators require equality operators.</para>
   /// </remarks>
   public OperatorsGeneration ComparisonOperators
   {
      get => SkipEqualityComparison ? OperatorsGeneration.None : field;
      set;
   }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IEqualityOperators{TSelf,TOther,TResult}"/>.
   /// </summary>
   /// <remarks>
   /// <para>This setting has no effect:</para>
   /// <para>If key-member is not an <see cref="IEqualityOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Equality</c>, <c>op_Inequality</c>).</para>
   /// <para>If <see cref="ValueObjectAttributeBase.SkipEqualityComparison"/> is set to <c>true</c>.</para>
   /// </remarks>
   public OperatorsGeneration EqualityComparisonOperators
   {
      get
      {
         if (SkipEqualityComparison)
            return OperatorsGeneration.None;

         return ComparisonOperators > field
                   ? ComparisonOperators
                   : field;
      }
      set;
   }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IFormattable"/> or not.
   /// </summary>
   /// <remarks>
   /// <para>This setting has no effect:</para>
   /// <para>The key-member is not an <see cref="IFormattable"/> itself.</para>
   /// </remarks>
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
   /// <remarks>
   /// <para>Not generated when <see cref="ValueObjectAttributeBase.SkipFactoryMethods"/> is set <c>true</c>, because the conversion operator relies on the factory method internally.</para>
   /// </remarks>
   public ConversionOperatorsGeneration ConversionFromKeyMemberType
   {
      get => SkipFactoryMethods ? ConversionOperatorsGeneration.None : field;
      set;
   }

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectAttribute{TKey}"/>.
   /// </summary>
   public ValueObjectAttribute()
   {
      KeyMemberType = typeof(TKey);
      KeyMemberAccessModifier = AccessModifier.Private;
      ConversionToKeyMemberType = ConversionOperatorsGeneration.Implicit;
      UnsafeConversionToKeyMemberType = ConversionOperatorsGeneration.Explicit;
      ConversionFromKeyMemberType = ConversionOperatorsGeneration.Explicit;
   }
}
