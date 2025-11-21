namespace Thinktecture;

public static class GenericExtensions
{
   public static bool NullableEquals<T>(this T? obj, T? other)
      where T : class, IEquatable<T>
   {
      if (obj is null)
         return other is null;

      if (other is null)
         return false;

      return obj.Equals(other);
   }
}
