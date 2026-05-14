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
   public StringComparison DefaultStringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

   /// <summary>
   /// Indication whether the generator should skip the implementation of the method <see cref="object.ToString"/> or not.
   /// </summary>
   public bool SkipToString { get; set; }

   /// <summary>
   /// Defines the access modifier of the constructors.
   /// Default is <see cref="UnionConstructorAccessModifier.Public"/>.
   /// </summary>
   /// <remarks>
   /// The access modifier also applies to the generated implicit conversion operators (from member types to union type).
   /// For example, setting this to <see cref="UnionConstructorAccessModifier.Private"/> makes both the constructors
   /// and the implicit conversion operators private, requiring custom factory methods for union creation.
   /// </remarks>
   public UnionConstructorAccessModifier ConstructorAccessModifier { get; set; } = UnionConstructorAccessModifier.Public;

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
   /// Initializes a new instance of <see cref="UnionAttributeBase"/>.
   /// </summary>
   private protected UnionAttributeBase()
   {
      ConversionFromValue = ConversionOperatorsGeneration.Implicit;
      ConversionToValue = ConversionOperatorsGeneration.Explicit;
   }
}
