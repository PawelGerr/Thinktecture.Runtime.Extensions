namespace Thinktecture.TestEnums
{
   public partial class ValidTestEnum : IEnum<string>
   {
      public static readonly ValidTestEnum Item1 = new("item1");
      public static readonly ValidTestEnum Item2 = new("item2");
   }
}
