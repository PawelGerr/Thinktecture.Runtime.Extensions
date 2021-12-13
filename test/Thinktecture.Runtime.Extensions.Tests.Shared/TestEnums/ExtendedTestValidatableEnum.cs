namespace Thinktecture.Runtime.Tests.TestEnums;

[EnumGeneration]
public partial class ExtendedTestValidatableEnum : ExtensibleTestValidatableEnum
{
   public static readonly ExtendedTestValidatableEnum Item2 = new("Item2", Empty.Action);
}