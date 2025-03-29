namespace Thinktecture.Unions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record Result<T>
{
   public record Success(T Value) : Result<T>;

   public record Failure(string Error) : Result<T>;
}
