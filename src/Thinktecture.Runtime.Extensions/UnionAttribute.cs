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
   /// The name of the "state" parameter used in <c>Switch</c> and <c>Map</c> methods.
   /// Default is <c>state</c>.
   /// </summary>
   public string? SwitchMapStateParameterName { get; set; }

   /// <summary>
   /// Controls how parameter names are generated in Switch/Map methods for nested Regular Union types.
   /// Default is <see cref="NestedUnionParameterNameGeneration.Default"/>, which includes intermediate type names.
   /// </summary>
   /// <example>
   /// <code>
   /// [Union(NestedUnionParameterNames = NestedUnionParameterNameGeneration.Simple)]
   /// public partial class ApiResponse
   /// {
   ///    public sealed class Success : ApiResponse;
   ///
   ///    public abstract partial class Failure : ApiResponse
   ///    {
   ///       private Failure() {}
   ///
   ///       public sealed class NotFound : Failure;      // Parameter: notFound (Simple)
   ///       public sealed class Unauthorized : Failure;  // Parameter: unauthorized (Simple)
   ///       // With Default: failureNotFound, failureUnauthorized
   ///    }
   /// }
   /// </code>
   /// </example>
   public NestedUnionParameterNameGeneration NestedUnionParameterNames { get; set; }

   /// <summary>
   /// Initializes a new instance of <see cref="UnionAttribute"/>.
   /// </summary>
   public UnionAttribute()
   {
      ConversionFromValue = ConversionOperatorsGeneration.Implicit;
   }
}
