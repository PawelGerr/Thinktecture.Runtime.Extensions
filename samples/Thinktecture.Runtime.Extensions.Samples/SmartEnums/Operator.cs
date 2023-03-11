namespace Thinktecture.SmartEnums;

public partial class Operator : IEnum<string>
{
   public static readonly Operator Item1 = new("Operator 1");
   public static readonly Operator Item2 = new GenericOperator<int>("Operator 2");
   public static readonly Operator Item3 = new GenericOperator<decimal>("Operator 3");
   public static readonly Operator Item4 = new GenericOperator<int>("Operator 4");

   // ReSharper disable once UnusedTypeParameter
   private sealed class GenericOperator<T> : Operator
   {
      public GenericOperator(string key)
         : base(key)
      {
      }
   }
}
