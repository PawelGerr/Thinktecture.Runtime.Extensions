namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(IsValidatable = true)]
public partial class TestEnumWithInvalidCreateInvalidItem
{
   public const int INVALID_KEY_FOR_TESTING_KEY_REUSE = 0;
   public const int INVALID_KEY_FOR_TESTING_IS_VALID_TRUE = -1;

   public static readonly TestEnumWithInvalidCreateInvalidItem Item1 = new(1);

   private static TestEnumWithInvalidCreateInvalidItem CreateInvalidItem(int key)
   {
      if (key == INVALID_KEY_FOR_TESTING_KEY_REUSE)
         return new(Item1.Key, false);

      if (key == INVALID_KEY_FOR_TESTING_IS_VALID_TRUE)
         return new(key, true);

      return new(key, false);
   }
}
