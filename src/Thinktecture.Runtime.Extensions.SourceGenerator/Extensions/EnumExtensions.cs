namespace Thinktecture;

public static class EnumExtensions
{
   public static bool IsValid<T>(this T item)
      where T : Enum
   {
      return Array.IndexOf(EnumHelper<T>.Items, item) >= 0;
   }

   private static class EnumHelper<T>
      where T : Enum
   {
      public static readonly T[] Items = (T[])Enum.GetValues(typeof(T));
   }
}
