namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(IsValidatable = true)]
public abstract partial class AbstractEnum
{
   public static readonly AbstractEnum Item = new ValidItem(1, 200);

   public abstract int Value { get; }

   private static AbstractEnum CreateInvalidItem(int key)
   {
      return new InvalidItem(key);
   }

   private sealed class InvalidItem : AbstractEnum
   {
      public override int Value => 100;

      public InvalidItem(int key)
         : base(key, false)
      {
      }
   }

   private sealed class ValidItem : AbstractEnum
   {
      public override int Value { get; }

      public ValidItem(int key, int value)
         : base(key)
      {
         Value = value;
      }
   }
}
