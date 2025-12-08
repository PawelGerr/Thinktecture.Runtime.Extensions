namespace Thinktecture.Unions;

/// <summary>
/// Example demonstrating Simple parameter name generation for nested unions.
/// This generates shorter parameter names (notFound, unauthorized) instead of
/// the default longer names (failureNotFound, failureUnauthorized).
/// </summary>
[Union(NestedUnionParameterNames = NestedUnionParameterNameGeneration.Simple)]
[UnionSwitchMapOverload(StopAt = [typeof(Failure)])]
public partial class ApiResponseWithSimpleParameterNames
{
   public sealed class Success : ApiResponseWithSimpleParameterNames;

   [Union]
   public abstract partial class Failure : ApiResponseWithSimpleParameterNames
   {
      public sealed class NotFound : Failure;

      public sealed class Unauthorized : Failure;
   }
}
