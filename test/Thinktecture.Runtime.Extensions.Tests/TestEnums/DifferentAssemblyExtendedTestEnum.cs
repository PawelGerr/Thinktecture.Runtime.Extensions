namespace Thinktecture.Runtime.Tests.TestEnums;

[EnumGeneration]
public partial class DifferentAssemblyExtendedTestEnum : ExtensibleTestEnum
{
   public static readonly DifferentAssemblyExtendedTestEnum Item2 = new("Item2", Empty.Action);
}