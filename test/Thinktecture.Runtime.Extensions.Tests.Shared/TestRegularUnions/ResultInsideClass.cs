namespace Thinktecture.Runtime.Tests.TestRegularUnions;

public partial class ResultInsideClass
{
   [Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
          MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
   public partial record Result
   {
      public sealed record Success : Result;

      public sealed record Failure(string Error) : Result;
   }
}
