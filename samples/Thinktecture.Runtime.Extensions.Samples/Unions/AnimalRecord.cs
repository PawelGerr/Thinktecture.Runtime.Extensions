namespace Thinktecture.Unions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record AnimalRecord
{
   public sealed record None : AnimalRecord
   {
      public static readonly None Instance = new();
   }

   public sealed record Dog(string Name) : AnimalRecord;

   public sealed record Cat(string Name) : AnimalRecord;
}
