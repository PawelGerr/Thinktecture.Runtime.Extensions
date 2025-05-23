namespace Thinktecture.Runtime.Tests.TestRegularUnions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record Result<T>
{
   public sealed record Success(T Value) : Result<T>;

   public sealed record Failure(string Error) : Result<T>;
}

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record Result
{
   public sealed record Success : Result;

   public sealed record Failure(string Error) : Result;
}
