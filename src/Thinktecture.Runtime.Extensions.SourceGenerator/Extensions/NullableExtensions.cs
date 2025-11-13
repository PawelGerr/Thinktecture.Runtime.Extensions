namespace Thinktecture;

public static class NullableExtensions
{
   public static bool NullableEquals<T>(this T? obj, T? other)
      where T : struct, IEquatable<T>
   {
      if (!obj.HasValue)
         return !other.HasValue;

      if (!other.HasValue)
         return false;

      return obj.Value.Equals(other.Value);
   }
}
