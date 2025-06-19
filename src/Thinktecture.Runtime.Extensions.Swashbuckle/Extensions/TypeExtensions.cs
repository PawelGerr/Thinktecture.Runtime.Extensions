namespace Thinktecture;

internal static class TypeExtensions
{
   public static Type NormalizeStructType(this Type sourceType, Type targetType)
   {
      // Make Nullable<TargetType> if it is a struct and the sourceType is a nullable struct as well.
      if (!sourceType.IsValueType || !targetType.IsValueType)
         return targetType;

      if (Nullable.GetUnderlyingType(sourceType) is null || Nullable.GetUnderlyingType(targetType) is not null)
         return targetType;

      return typeof(Nullable<>).MakeGenericType(targetType);
   }
}
