namespace Thinktecture.Runtime.Tests.TestEnums
{
   public partial class IntegerEnum : IValidatableEnum<int>
   {
      public static readonly IntegerEnum Item1 = new(1);
      public static readonly IntegerEnum Item2 = new(2);
   }
}
