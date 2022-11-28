using MessagePack;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;

public class IntBasedEnumWithFormatterMessagePackFormatter : ValueObjectMessagePackFormatterBase<IntBasedEnumWithFormatter, int>
{
#if NET7_0
   public IntBasedEnumWithFormatterMessagePackFormatter()
      : base(true)
   {
   }
#else
   public IntBasedEnumWithFormatterMessagePackFormatter()
      : base(IntBasedEnumWithFormatter.Get)
   {
   }
#endif
}

[MessagePackFormatter(typeof(IntBasedEnumWithFormatterMessagePackFormatter))]
public sealed partial class IntBasedEnumWithFormatter : IValidatableEnum<int>
{
   public static readonly IntBasedEnumWithFormatter Value1 = new(1);
   public static readonly IntBasedEnumWithFormatter Value2 = new(2);
}
