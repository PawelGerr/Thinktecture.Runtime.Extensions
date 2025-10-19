namespace Thinktecture;

public static class EnumExtensions
{
   public static bool IsValid<T>(this T item)
      where T : Enum
   {
      return Array.IndexOf(EnumHelper<T>.Items, item) >= 0;
   }

   public static T? GetValidValue<T>(this int value)
      where T : struct, Enum
   {
      var index = Array.IndexOf(EnumHelper<T>.Values, value);

      return index < 0 ? default : EnumHelper<T>.Items[index];
   }

   private static class EnumHelper<T>
      where T : Enum
   {
      public static readonly T[] Items = (T[])Enum.GetValues(typeof(T));
      public static readonly int[] Values = (int[])Enum.GetValues(typeof(T));
   }
}
