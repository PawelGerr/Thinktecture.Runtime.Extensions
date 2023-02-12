using MessagePack;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;

public class StringBasedEnumWithFormatterMessagePackFormatter : ValueObjectMessagePackFormatterBase<StringBasedEnumWithFormatter, string>
{
   public StringBasedEnumWithFormatterMessagePackFormatter()
      : base(true)
   {
   }
}

[MessagePackFormatter(typeof(StringBasedEnumWithFormatterMessagePackFormatter))]
public sealed partial class StringBasedEnumWithFormatter : IValidatableEnum<string>
{
   public static readonly StringBasedEnumWithFormatter ValueA = new("A");
   public static readonly StringBasedEnumWithFormatter ValueB = new("B");
}
