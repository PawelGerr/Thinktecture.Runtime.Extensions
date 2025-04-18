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
   /// Access modifier of the constructors will have effect on the access modifier of implicit casts.
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
   /// Initializes a new instance of <see cref="UnionAttributeBase"/>.
   /// </summary>
   private protected UnionAttributeBase()
   {
      ConversionFromValue = ConversionOperatorsGeneration.Implicit;
      ConversionToValue = ConversionOperatorsGeneration.Explicit;
   }
}
