namespace Thinktecture.Json.EnumJsonConverterTests
{
   public partial class IntBasedEnum : IValidatableEnum<int>
   {
      public static readonly IntBasedEnum Value1 = new(1);
      public static readonly IntBasedEnum Value2 = new(2);
   }
}
