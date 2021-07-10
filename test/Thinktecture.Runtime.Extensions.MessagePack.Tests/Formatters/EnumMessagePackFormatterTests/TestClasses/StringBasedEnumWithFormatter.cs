using System;
using MessagePack;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses
{
   public class StringBasedEnumWithFormatterMessagePackFormatter : ValueObjectMessagePackFormatter<StringBasedEnumWithFormatter, string>
   {
      public StringBasedEnumWithFormatterMessagePackFormatter()
         : base(StringBasedEnumWithFormatter.Get, obj => obj.Key)
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
