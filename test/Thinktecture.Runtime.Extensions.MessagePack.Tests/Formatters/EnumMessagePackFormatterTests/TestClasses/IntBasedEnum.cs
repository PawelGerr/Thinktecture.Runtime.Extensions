namespace Thinktecture.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   public class IntBasedEnum : Enum<IntBasedEnum, int>
   {
      public static readonly IntBasedEnum Value1 = new IntBasedEnum(1);
      public static readonly IntBasedEnum Value2 = new IntBasedEnum(2);

      private IntBasedEnum(int key)
         : base(key)
      {
      }

      protected override IntBasedEnum CreateInvalid(int key)
      {
         return new IntBasedEnum(key);
      }
   }
}
