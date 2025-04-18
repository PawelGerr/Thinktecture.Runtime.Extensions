using MessagePack;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.ThinktectureMessagePackFormatterTests.TestClasses;

[SmartEnum<string>(IsValidatable = true)]
[MessagePackFormatter(typeof(ThinktectureMessagePackFormatter<StringBasedEnumWithFormatter, string, ValidationError>))]
public partial class StringBasedEnumWithFormatter
{
   public static readonly StringBasedEnumWithFormatter ValueA = new("A");
   public static readonly StringBasedEnumWithFormatter ValueB = new("B");
}
