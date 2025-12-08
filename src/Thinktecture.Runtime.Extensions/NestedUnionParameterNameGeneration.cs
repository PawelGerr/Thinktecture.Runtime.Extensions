namespace Thinktecture;

/// <summary>
/// Controls how parameter names are generated in Switch/Map methods for nested Regular Union types.
/// </summary>
public enum NestedUnionParameterNameGeneration
{
   /// <summary>
   /// Default naming strategy: Includes intermediate type names in the parameter name.
   /// For example, a nested type <c>Failure.NotFound</c> generates the parameter name <c>failureNotFound</c>.
   /// This ensures uniqueness when multiple nested unions exist but can result in longer parameter names.
   /// </summary>
   Default = 0,

   /// <summary>
   /// Simple naming strategy: Uses only the final type name as the parameter name.
   /// For example, a nested type <c>Failure.NotFound</c> generates the parameter name <c>notFound</c>.
   /// <para>
   /// <b>WARNING:</b> This can cause parameter name collisions if multiple nested unions have types
   /// with the same name. The C# compiler will report duplicate parameter errors if conflicts occur.
   /// </para>
   /// <example>
   /// This will cause a compilation error:
   /// <code>
   /// [Union(NestedUnionParameterNames = NestedUnionParameterNameGeneration.Simple)]
   /// public partial class Response
   /// {
   ///    [Union]
   ///    public abstract partial class Success : Response
   ///    {
   ///       public sealed class Status : Success;  // Parameter: status
   ///    }
   ///
   ///    [Union]
   ///    public abstract partial class Failure : Response
   ///    {
   ///       public sealed class Status : Failure;  // Parameter: status (CONFLICT!)
   ///    }
   /// }
   /// </code>
   /// </example>
   /// </summary>
   Simple = 1
}
