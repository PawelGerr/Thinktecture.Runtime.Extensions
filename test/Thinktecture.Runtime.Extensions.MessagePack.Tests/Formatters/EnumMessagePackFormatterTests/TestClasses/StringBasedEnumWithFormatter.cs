using MessagePack;

namespace Thinktecture.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [MessagePackFormatter(typeof(StringBasedEnumWithFormatter_EnumMessagePackFormatter))]
   public partial class StringBasedEnumWithFormatter : IEnum<string>
   {
      public static readonly StringBasedEnumWithFormatter ValueA = new("A");
      public static readonly StringBasedEnumWithFormatter ValueB = new("B");
   }
}
