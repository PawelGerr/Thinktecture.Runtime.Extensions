using MessagePack;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;

[MessagePackFormatter(typeof(ValueObjectMessagePackFormatter<IntBasedEnumWithFormatter, int>))]
public sealed partial class IntBasedEnumWithFormatter : IValidatableEnum<int>
{
   public static readonly IntBasedEnumWithFormatter Value1 = new(1);
   public static readonly IntBasedEnumWithFormatter Value2 = new(2);
}
