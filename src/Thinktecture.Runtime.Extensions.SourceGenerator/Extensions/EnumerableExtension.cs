namespace Thinktecture;

public static class EnumerableExtension
{
   public static void Enumerate<T>(this IEnumerable<T> enumerable)
   {
      foreach (var _ in enumerable)
      {
      }
   }
}
