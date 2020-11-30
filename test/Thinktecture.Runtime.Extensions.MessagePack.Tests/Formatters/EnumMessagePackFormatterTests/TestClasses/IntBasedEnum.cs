namespace Thinktecture.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   public partial class IntBasedEnum : IEnum<int>
   {
      public static readonly IntBasedEnum Value1 = new(1);
      public static readonly IntBasedEnum Value2 = new(2);

      IEnum<int> IEnum<int>.CreateInvalid(int key)
      {
         return new IntBasedEnum(key);
      }
   }
}
