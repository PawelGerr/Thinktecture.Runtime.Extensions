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
   /// Indication whether the generator should skip the implementation of implicit conversions from value to union type.
   /// </summary>
   public bool SkipImplicitConversionFromValue { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Switch</c>.
   /// </summary>
   public SwitchMapMethodsGeneration SwitchMethods { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Map</c>.
   /// </summary>
   public SwitchMapMethodsGeneration MapMethods { get; set; }
}
