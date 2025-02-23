namespace Thinktecture.Unions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class Animal
{
   public sealed class None : Animal
   {
      public static readonly None Instance = new();

      private None()
      {
      }

      public override string ToString()
      {
         return "None";
      }
   }

   [ValueObject<string>]
   [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
   [ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
   public partial class Dog : Animal;

   public sealed class Cat : Animal
   {
      public string Name { get; }

      public Cat(string name)
      {
         Name = name;
      }

      public override string ToString()
      {
         return Name;
      }
   }
}
