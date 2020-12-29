namespace Thinktecture.TestEnums
{
   public abstract partial class AbstractEnum : IValidatableEnum<int>
   {
      public static readonly AbstractEnum Item = new ValidItem(1);

      private static AbstractEnum CreateInvalidItem(int key)
      {
         return new InvalidItem(key) { IsValid = false };
      }

      private class InvalidItem : AbstractEnum
      {
         public InvalidItem(int key)
            : base(key)
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
