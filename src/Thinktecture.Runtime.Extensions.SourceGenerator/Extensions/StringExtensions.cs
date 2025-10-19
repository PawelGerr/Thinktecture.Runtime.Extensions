namespace Thinktecture;

public static class StringExtensions
{
   public static string MakeArgumentName(this string name)
   {
      return name.Length switch
      {
         0 => name,
         1 => name.ToLowerInvariant(),
         _ => ToCamelCase(name, false)
      };
   }

   public static string MakeBackingFieldName(this string name)
   {
      return name.Length switch
      {
         0 => name,
         1 => name[0] == '_' ? name : $"_{Char.ToLowerInvariant(name[0])}",
         _ => ToCamelCase(name, true)
      };
   }

   private static string ToCamelCase(
      string name,
      bool leadingUnderscore)
   {
      var startsWithUnderscore = name[0] == '_';

      for (var i = 0; i < name.Length; i++)
      {
         var charValue = name[i];

         if (Char.IsDigit(charValue))
            return name;

         if (Char.IsLower(charValue))
            break;

         if (!Char.IsLetter(charValue))
            continue;

         if (i == 0)
            return $"{(leadingUnderscore ? "_" : String.Empty)}{Char.ToLowerInvariant(charValue)}{name.Substring(1)}";

         var prefix = leadingUnderscore && !startsWithUnderscore ? "_" : String.Empty;

         if (i == name.Length - 1)
            return $"{prefix}{name.Substring(startsWithUnderscore ? 1 : 0, i)}{Char.ToLowerInvariant(charValue)}";

         return $"{prefix}{name.Substring(startsWithUnderscore ? 1 : 0, i)}{Char.ToLowerInvariant(charValue)}{name.Substring(i)}";
      }

      if (leadingUnderscore)
         return !startsWithUnderscore ? $"_{name}" : name;

      return startsWithUnderscore && name.Length > 1 ? name.Substring(1) : name;
   }
}
