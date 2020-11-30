using MessagePack;

namespace Thinktecture.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   [MessagePackFormatter(typeof(EnumMessagePackFormatter<IntBasedEnumWithFormatter, int>))]
   public partial class IntBasedEnumWithFormatter : IEnum<int>
   {
      public static readonly IntBasedEnumWithFormatter Value1 = new(1);
      public static readonly IntBasedEnumWithFormatter Value2 = new(2);

      IEnum<int> IEnum<int>.CreateInvalid(int key)
      {
         return new IntBasedEnumWithFormatter(key);
      }
   }
}
