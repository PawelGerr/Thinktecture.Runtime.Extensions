namespace Thinktecture.Runtime.Tests.TestEnums.Isolated
{
   /// <summary>
   /// This enum may be used in 1 test only.
   /// Otherwise it is initialized and the test is invalid.
   /// </summary>
   // ReSharper disable once InconsistentNaming
   public partial class StaticCtorTestEnum_Get : IValidatableEnum<string>
   {
      // ReSharper disable once UnusedMember.Global
      public static readonly StaticCtorTestEnum_Get Item = new("item");
   }
}
