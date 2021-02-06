namespace Thinktecture.Runtime.Tests.TestEnums
{
   [EnumGeneration]
   public partial class ExtendedTestEnum : ExtensibleTestEnum
   {
      public static readonly ExtendedTestEnum Item2 = new("Item2", Empty.Action);
   }
}
