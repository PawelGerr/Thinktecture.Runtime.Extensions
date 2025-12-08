namespace Thinktecture.Runtime.Tests.TestRegularUnions;

[Union(NestedUnionParameterNames = NestedUnionParameterNameGeneration.Simple)]
[UnionSwitchMapOverload(StopAt = [typeof(Failure)])]
public partial class ApiResponseWithSimpleNaming
{
   public sealed class Success : ApiResponseWithSimpleNaming;

   [Union]
   public abstract partial class Failure : ApiResponseWithSimpleNaming
   {
      private Failure()
      {
      }

      public sealed class NotFound : Failure;

      public sealed class Unauthorized : Failure;
   }
}
