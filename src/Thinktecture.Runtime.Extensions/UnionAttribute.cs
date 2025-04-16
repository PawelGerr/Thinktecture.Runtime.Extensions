namespace Thinktecture;

/// <summary>
/// Marks a type as a discriminated union.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UnionAttribute : Attribute
{
   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Switch</c>.
   /// </summary>
   public SwitchMapMethodsGeneration SwitchMethods { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Map</c>.
   /// </summary>
   public SwitchMapMethodsGeneration MapMethods { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the conversion operators from value to union type.
   /// Default is <see cref="ConversionOperatorsGeneration.Implicit"/>.
   /// </summary>
   public ConversionOperatorsGeneration ConversionFromValue { get; set; }

   /// <summary>
   /// Initializes a new instance of <see cref="UnionAttribute"/>.
   /// </summary>
   public UnionAttribute()
   {
      ConversionFromValue = ConversionOperatorsGeneration.Implicit;
   }
}
