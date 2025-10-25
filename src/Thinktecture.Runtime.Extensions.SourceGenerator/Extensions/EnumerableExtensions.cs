namespace Thinktecture;

public static class EnumerableExtensions
{
   public static void Enumerate<T>(this IEnumerable<T> enumerable)
   {
      foreach (var _ in enumerable)
      {
      }
   }
}
