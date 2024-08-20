using MessagePack;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;

[SmartEnum<int>(IsValidatable = true)]
[MessagePackFormatter(typeof(ValueObjectMessagePackFormatter<IntBasedEnumWithFormatter, int, ValidationError>))]
public partial class IntBasedEnumWithFormatter
{
   public static readonly IntBasedEnumWithFormatter Value1 = new(1);
   public static readonly IntBasedEnumWithFormatter Value2 = new(2);
}
