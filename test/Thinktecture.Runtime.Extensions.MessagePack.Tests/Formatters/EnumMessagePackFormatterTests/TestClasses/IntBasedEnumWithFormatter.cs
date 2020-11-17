using MessagePack;

namespace Thinktecture.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [MessagePackFormatter(typeof(EnumMessagePackFormatter<IntBasedEnumWithFormatter, int>))]
   public class IntBasedEnumWithFormatter : Enum<IntBasedEnumWithFormatter, int>
   {
      public static readonly IntBasedEnumWithFormatter Value1 = new(1);
      public static readonly IntBasedEnumWithFormatter Value2 = new(2);

      private IntBasedEnumWithFormatter(int key)
         : base(key)
      {
      }

      protected override IntBasedEnumWithFormatter CreateInvalid(int key)
      {
         return new(key);
      }
   }
}
