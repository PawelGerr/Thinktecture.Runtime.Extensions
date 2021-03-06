namespace Thinktecture.Runtime.Tests.TestEnums
{
   public abstract partial class AbstractEnum : IValidatableEnum<int>
   {
      public static readonly AbstractEnum Item = new ValidItem(1);

      private static AbstractEnum CreateInvalidItem(int key)
      {
         return new InvalidItem(key);
      }

      private class InvalidItem : AbstractEnum
      {
         public InvalidItem(int key)
            : base(key, false)
         {
         }
      }

      private class ValidItem : AbstractEnum
      {
         public ValidItem(int key)
            : base(key)
         {
         }
      }
   }
}
