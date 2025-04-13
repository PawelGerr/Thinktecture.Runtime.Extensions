namespace Thinktecture.SmartEnums;

[SmartEnum]
public partial class OperatorWithoutIdentifier
{
   public static readonly OperatorWithoutIdentifier Item1 = new();
   public static readonly OperatorWithoutIdentifier Item2 = new GenericOperator<int>();
   public static readonly OperatorWithoutIdentifier Item3 = new GenericOperator<decimal>();
   public static readonly OperatorWithoutIdentifier Item4 = new GenericOperator<int>();

   // ReSharper disable once UnusedTypeParameter
   private sealed class GenericOperator<T> : OperatorWithoutIdentifier;
}
