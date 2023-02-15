using MessagePack;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;

[MessagePackFormatter(typeof(ValueObjectMessagePackFormatter<StringBasedEnumWithFormatter, string>))]
public sealed partial class StringBasedEnumWithFormatter : IValidatableEnum<string>
{
   public static readonly StringBasedEnumWithFormatter ValueA = new("A");
   public static readonly StringBasedEnumWithFormatter ValueB = new("B");
}
