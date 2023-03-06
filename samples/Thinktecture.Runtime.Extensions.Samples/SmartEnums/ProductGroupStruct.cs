namespace Thinktecture.SmartEnums;

[EnumGeneration(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public readonly partial struct ProductGroupStruct : IValidatableEnum<int>
{
   public static readonly ProductGroupStruct Apple = new(1);
   public static readonly ProductGroupStruct Orange = new(2);
}
