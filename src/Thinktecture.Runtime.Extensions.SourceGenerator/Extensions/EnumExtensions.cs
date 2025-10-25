using System.Globalization;

namespace Thinktecture;

public static class EnumExtensions
{
   public static bool IsValid<T>(this T item)
      where T : struct, Enum
   {
      if (EnumHelper<T>.IsFlags)
      {
         var intValue = (int)(object)item;
         return (intValue & ~EnumHelper<T>.AllowedBits) == 0;
      }

      return Array.IndexOf(EnumHelper<T>.Items, item) >= 0;
   }

   public static T? GetValidValue<T>(this int value)
      where T : struct, Enum
   {
      if (EnumHelper<T>.IsFlags)
      {
         if ((value & ~EnumHelper<T>.AllowedBits) != 0)
            return null;

         return (T)Enum.ToObject(typeof(T), value);
      }

      var index = Array.IndexOf(EnumHelper<T>.Values, value);

      return index < 0 ? null : EnumHelper<T>.Items[index];
   }

   public static T? GetValidValue<T>(this byte value)
      where T : struct, Enum
   {
      return ((int)value).GetValidValue<T>();
   }

   public static T? GetValidValue<T>(this sbyte value)
      where T : struct, Enum
   {
      return ((int)value).GetValidValue<T>();
   }

   public static T? GetValidValue<T>(this short value)
      where T : struct, Enum
   {
      return ((int)value).GetValidValue<T>();
   }

   public static T? GetValidValue<T>(this ushort value)
      where T : struct, Enum
   {
      return ((int)value).GetValidValue<T>();
   }

   public static T? GetValidValue<T>(this uint value)
      where T : struct, Enum
   {
      return ((int)value).GetValidValue<T>();
   }

   public static T? GetValidValue<T>(this long value)
      where T : struct, Enum
   {
      return ((int)value).GetValidValue<T>();
   }

   public static T? GetValidValue<T>(this ulong value)
      where T : struct, Enum
   {
      return ((int)value).GetValidValue<T>();
   }

   private static class EnumHelper<T>
      where T : Enum
   {
      public static readonly T[] Items = (T[])Enum.GetValues(typeof(T));
      public static readonly int[] Values = Enum.GetValues(typeof(T)).Cast<object>().Select(v => Convert.ToInt32(v, CultureInfo.InvariantCulture)).ToArray();
      public static readonly bool IsFlags = typeof(T).IsDefined(typeof(FlagsAttribute), false);
      public static readonly int AllowedBits = Enum.GetValues(typeof(T)).Cast<object>().Select(v => Convert.ToInt32(v, CultureInfo.InvariantCulture)).Aggregate(0, (current, v) => current | v);
   }
}
