using MessagePack;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   public class IntBasedEnumWithFormatterMessagePackFormatter : ValueObjectMessagePackFormatter<IntBasedEnumWithFormatter, int>
   {
      public IntBasedEnumWithFormatterMessagePackFormatter()
         : base(IntBasedEnumWithFormatter.Get, obj => obj.Key)
      {
      }
   }

   [MessagePackFormatter(typeof(IntBasedEnumWithFormatterMessagePackFormatter))]
   public partial class IntBasedEnumWithFormatter : IValidatableEnum<int>
   {
      public static readonly IntBasedEnumWithFormatter Value1 = new(1);
      public static readonly IntBasedEnumWithFormatter Value2 = new(2);
   }
}
