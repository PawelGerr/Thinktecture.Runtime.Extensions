namespace Thinktecture.Text.Json.Serialization.EnumJsonConverterTests.TestClasses
{
   public class IntBasedEnum : Enum<IntBasedEnum, int>
   {
      public static readonly IntBasedEnum Value1 = new(1);
      public static readonly IntBasedEnum Value2 = new(2);

      private IntBasedEnum(int key)
         : base(key)
      {
      }

      protected override IntBasedEnum CreateInvalid(int key)
      {
         return new(key);
      }
   }
}
