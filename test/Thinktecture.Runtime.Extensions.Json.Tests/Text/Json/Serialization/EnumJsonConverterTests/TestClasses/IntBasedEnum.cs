namespace Thinktecture.Text.Json.Serialization.EnumJsonConverterTests.TestClasses
{
   public partial class IntBasedEnum : IEnum<int>
   {
      public static readonly IntBasedEnum Value1 = new(1);
      public static readonly IntBasedEnum Value2 = new(2);
   }
}
