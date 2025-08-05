namespace Thinktecture.Runtime.Tests.TestRegularUnions;

[Union]
[UnionSwitchMapOverload(StopAt = [typeof(Failure)])]
public partial class ApiResponse
{
   public sealed class Success : ApiResponse;

   [Union]
   public abstract partial class Failure : ApiResponse
   {
      private Failure()
      {
      }

      public sealed class NotFound : Failure;

      public sealed class Unauthorized : Failure;
   }
}
