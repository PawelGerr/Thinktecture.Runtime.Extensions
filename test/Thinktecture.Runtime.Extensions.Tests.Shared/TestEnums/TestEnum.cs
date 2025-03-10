using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>(IsValidatable = true,
                   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                   EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public partial class TestEnum
{
   public static readonly TestEnum Item1 = new("item1");
   public static readonly TestEnum Item2 = new("item2");

   // the following members will lead to compiler error
   // protected static readonly TestEnum Item3 = new("item3");
   // internal static readonly TestEnum Item4 = new("item4");
   // private static readonly TestEnum Item5 = new("item5");

   public static readonly IntegerEnum Item6 = IntegerEnum.Item1;
   public static readonly IntegerEnum Item7 = IntegerEnum.Get(42);

   // ReSharper disable once UnusedParameterInPartialMethod
   static partial void ValidateConstructorArguments(ref string key, bool isValid)
   {
      if (String.IsNullOrWhiteSpace(key))
         throw new ArgumentException("Key cannot be empty.");
   }
}
