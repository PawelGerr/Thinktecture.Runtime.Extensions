namespace Thinktecture.SmartEnums;

[SmartEnum<int>(IsValidatable = true,
                ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct ProductGroupStruct
{
   public static readonly ProductGroupStruct Apple = new(1);
   public static readonly ProductGroupStruct Orange = new(2);
}
