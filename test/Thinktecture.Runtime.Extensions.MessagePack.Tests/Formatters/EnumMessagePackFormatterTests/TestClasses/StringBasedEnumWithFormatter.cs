using System;
using MessagePack;

namespace Thinktecture.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   public class StringBasedEnumWithFormatterMessagePackFormatter : EnumMessagePackFormatter<StringBasedEnumWithFormatter, string>
   {
      public StringBasedEnumWithFormatterMessagePackFormatter()
         : base(StringBasedEnumWithFormatter.Get)
      {
      }
   }

   [MessagePackFormatter(typeof(StringBasedEnumWithFormatterMessagePackFormatter))]
   public partial class StringBasedEnumWithFormatter : IValidatableEnum<string>
   {
      public static readonly StringBasedEnumWithFormatter ValueA = new("A");
      public static readonly StringBasedEnumWithFormatter ValueB = new("B");
   }
}
