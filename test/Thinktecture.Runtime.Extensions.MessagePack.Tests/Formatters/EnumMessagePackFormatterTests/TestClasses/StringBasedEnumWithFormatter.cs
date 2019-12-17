using MessagePack;

namespace Thinktecture.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [MessagePackFormatter(typeof(EnumMessagePackFormatter<StringBasedEnumWithFormatter>))]
   public class StringBasedEnumWithFormatter : Enum<StringBasedEnumWithFormatter>
   {
      public static readonly StringBasedEnumWithFormatter ValueA = new StringBasedEnumWithFormatter("A");
      public static readonly StringBasedEnumWithFormatter ValueB = new StringBasedEnumWithFormatter("B");

      private StringBasedEnumWithFormatter(string key)
         : base(key)
      {
      }

      protected override StringBasedEnumWithFormatter CreateInvalid(string key)
      {
         return new StringBasedEnumWithFormatter(key);
      }
   }
}
