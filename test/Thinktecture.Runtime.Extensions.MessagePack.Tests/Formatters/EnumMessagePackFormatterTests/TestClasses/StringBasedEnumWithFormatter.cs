using MessagePack;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;

public class StringBasedEnumWithFormatterMessagePackFormatter : ValueObjectMessagePackFormatterBase<StringBasedEnumWithFormatter, string>
{
#if NET7_0
   public StringBasedEnumWithFormatterMessagePackFormatter()
      : base(true)
   {
   }
#else
   public StringBasedEnumWithFormatterMessagePackFormatter()
      : base(StringBasedEnumWithFormatter.Get)
   {
   }
#endif
}

[MessagePackFormatter(typeof(StringBasedEnumWithFormatterMessagePackFormatter))]
public sealed partial class StringBasedEnumWithFormatter : IValidatableEnum<string>
{
   public static readonly StringBasedEnumWithFormatter ValueA = new("A");
   public static readonly StringBasedEnumWithFormatter ValueB = new("B");
}
