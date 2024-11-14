namespace Thinktecture;

/// <summary>
/// Marks a type as a discriminated union.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UnionAttribute : Attribute
{
   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Switch</c>.
   /// </summary>
   public SwitchMapMethodsGeneration SwitchMethods { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the methods <c>Map</c>.
   /// </summary>
   public SwitchMapMethodsGeneration MapMethods { get; set; }
}
