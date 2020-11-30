using MessagePack;

namespace Thinktecture.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [MessagePackFormatter(typeof(EnumMessagePackFormatter<StringBasedEnumWithFormatter, string>))]
   public partial class StringBasedEnumWithFormatter : IEnum<string>
   {
      public static readonly StringBasedEnumWithFormatter ValueA = new("A");
      public static readonly StringBasedEnumWithFormatter ValueB = new("B");

      IEnum<string> IEnum<string>.CreateInvalid(string key)
      {
         return new StringBasedEnumWithFormatter(key);
      }
   }
}
