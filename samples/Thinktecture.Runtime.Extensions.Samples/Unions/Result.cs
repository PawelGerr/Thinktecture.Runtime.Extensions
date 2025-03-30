namespace Thinktecture.Unions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record Result<T>
{
   public sealed record Success(T Value) : Result<T>;

   public sealed record Failure(string Error) : Result<T>;
}
