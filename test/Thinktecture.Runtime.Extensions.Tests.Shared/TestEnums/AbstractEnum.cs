namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(IsValidatable = true)]
public abstract partial class AbstractEnum
{
   public static readonly AbstractEnum Item = new ValidItem(1);

   private static AbstractEnum CreateInvalidItem(int key)
   {
      return new InvalidItem(key);
   }

   private sealed class InvalidItem : AbstractEnum
   {
      public InvalidItem(int key)
         : base(key, false)
      {
      }
   }

   private sealed class ValidItem : AbstractEnum
   {
      public ValidItem(int key)
         : base(key)
      {
      }
   }
}
