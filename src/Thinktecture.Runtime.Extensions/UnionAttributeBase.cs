namespace Thinktecture;

/// <summary>
/// Base class for marking a type as a discriminated union.
/// </summary>
public abstract class UnionAttributeBase : Attribute
{
   /// <summary>
   /// Defines the <see cref="StringComparison"/>.
   /// Default <see cref="StringComparison"/> is <see cref="StringComparison.OrdinalIgnoreCase"/>.
   /// </summary>
   public StringComparison DefaultStringComparison { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of the method <see cref="object.ToString"/> or not.
   /// </summary>
   public bool SkipToString { get; set; }

   /// <summary>
   /// Defines the access modifier of the constructors.
   /// Default is <see cref="UnionConstructorAccessModifier.Public"/>.
   /// </summary>
   /// <remarks>
   /// The access modifier also applies to the generated factory methods.
   /// It does <b>not</b> apply to the generated conversion operators, because C# requires user-defined
   /// conversion operators to be <c>public</c>. To restrict union creation to custom factory methods,
   /// set <see cref="ConstructorAccessModifier"/> to <see cref="UnionConstructorAccessModifier.Private"/>
   /// <b>and</b> additionally disable the inbound conversions via
   /// <see cref="ConversionFromValue"/> = <see cref="ConversionOperatorsGeneration.None"/>.
   /// </remarks>
   public UnionConstructorAccessModifier ConstructorAccessModifier { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the conversion operators from value to union type.
   /// Default is <see cref="ConversionOperatorsGeneration.Implicit"/>.
   /// </summary>
   public ConversionOperatorsGeneration ConversionFromValue { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the conversion operators from union type to value.
   /// Default is <see cref="ConversionOperatorsGeneration.Explicit"/>.
   /// </summary>
   public ConversionOperatorsGeneration ConversionToValue { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Switch</c>.
   /// </summary>
   public SwitchMapMethodsGeneration SwitchMethods { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Map</c>.
   /// </summary>
   public SwitchMapMethodsGeneration MapMethods { get; set; }

   /// <summary>
   /// The name of the "state" parameter used in <c>Switch</c> and <c>Map</c> methods.
   /// Default is <c>state</c>.
   /// </summary>
   public string? SwitchMapStateParameterName { get; set; }

   /// <summary>
   /// Indication whether the generator should use a single backing field of type <see cref="object"/> for all members, even for structs.
   /// Default is <c>false</c>.
   /// </summary>
   public bool UseSingleBackingField { get; set; }

   /// <summary>
   /// When set, the generator emits the single shared backing field (and the <c>Value</c> property)
   /// typed as the provided base type instead of <see cref="object"/>.
   /// </summary>
   /// <remarks>
   /// <para>
   /// Setting this property implies <see cref="UseSingleBackingField"/> = <c>true</c>.
   /// </para>
   /// <para>
   /// Specifying <c>typeof(object)</c> is normalized to "not set" and behaves identically to
   /// <see cref="UseSingleBackingField"/> = <c>true</c> alone.
   /// </para>
   /// <para>
   /// For generic unions, <c>SingleBackingFieldType</c> supports the <c>TypeParamRef1</c>–<c>TypeParamRef5</c> placeholders to reference the union's own type parameters.
   /// Nested usage (e.g. <c>typeof(IFoo&lt;TypeParamRef1&gt;)</c>) is supported.
   /// </para>
   /// </remarks>
   public Type? SingleBackingFieldType { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <c>IEquatable{T}</c> and any comparison operators.
   /// This includes the <c>Equals</c> and <c>GetHashCode</c> methods.
   /// </summary>
   public bool SkipEqualityComparison { get; set; }

   /// <summary>
   /// Controls factory method generation.
   /// <c>Default</c> = auto-detect, <c>None</c> = suppress all, <c>Always</c> = generate for all members.
   /// </summary>
   public FactoryMethodGeneration FactoryMethodGeneration { get; set; }

   /// <summary>
   /// Defines the access modifier of the generated <c>Value</c> property.
   /// Default is <see cref="AccessModifier.Public"/>.
   /// </summary>
   /// <remarks>
   /// This setting applies to ad-hoc unions only. Regular unions have no generated <c>Value</c> property.
   /// The generated code still reads the property internally (for metadata and partial switch callbacks),
   /// so a non-public value only hides it from the type's consumers.
   /// </remarks>
   public AccessModifier ValueMemberAccessModifier { get; set; }

   /// <summary>
   /// The name of the generated raw-value property.
   /// Default: <c>Value</c>.
   /// </summary>
   /// <remarks>
   /// This setting applies to ad-hoc unions only. Rename the generated property to free the
   /// <c>Value</c> identifier, for example to hand-write a <c>Value</c> property of a different type
   /// in the partial part. The name is emitted as-is; an invalid or colliding identifier produces a
   /// C# compiler error in the generated code, matching how <c>KeyMemberName</c> behaves.
   /// </remarks>
   public string ValueMemberName
   {
      get => field ?? "Value";
      set;
   }

   /// <summary>
   /// Controls how the default value of an ad-hoc union struct is treated.
   /// Default is <see cref="UnionDefaultValueHandling.Disallow"/>.
   /// </summary>
   /// <remarks>
   /// This setting applies to ad-hoc unions only and requires a struct union whose first member (T1)
   /// is stateless (<c>T1IsStateless = true</c>). With <see cref="UnionDefaultValueHandling.MapToFirstMember"/>
   /// the value <c>default(TUnion)</c> represents the first member instead of an invalid state.
   /// </remarks>
   public UnionDefaultValueHandling DefaultValueHandling { get; set; }

   /// <summary>
   /// Initializes a new instance of <see cref="UnionAttributeBase"/>.
   /// </summary>
   private protected UnionAttributeBase()
   {
      DefaultStringComparison = StringComparison.OrdinalIgnoreCase;
      ConstructorAccessModifier = UnionConstructorAccessModifier.Public;
      ConversionFromValue = ConversionOperatorsGeneration.Implicit;
      ConversionToValue = ConversionOperatorsGeneration.Explicit;
      ValueMemberAccessModifier = AccessModifier.Public;
   }
}
