using System;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class StringExtensions
   {
      public static string MakeArgumentName(this string name)
      {
         if (name.Length == 1)
            return name.ToLowerInvariant();

         return name.StartsWith("_", StringComparison.Ordinal)
                   ? $"{Char.ToLowerInvariant(name[1])}{name.Substring(2)}"
                   : $"{Char.ToLowerInvariant(name[0])}{name.Substring(1)}";
      }

      public static string? TrimAndNullify(this string? text)
      {
         if (String.IsNullOrWhiteSpace(text))
            return null;

         return text!.Trim();
      }
   }
}
