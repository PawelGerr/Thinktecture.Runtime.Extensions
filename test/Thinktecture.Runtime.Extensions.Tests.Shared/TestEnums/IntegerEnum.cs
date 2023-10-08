namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(IsValidatable = true,
                ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
public sealed partial class IntegerEnum
{
   public static readonly IntegerEnum Item1 = new(1);
   public static readonly IntegerEnum Item2 = new(2);
}
