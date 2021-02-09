namespace Thinktecture.Runtime.Tests.TestEnums
{
   [EnumGeneration]
   public partial class ExtendedSiblingTestEnum : ExtensibleTestEnum
   {
      public static readonly ExtendedSiblingTestEnum Item2 = new("Item2", Empty.Action);
   }
}
