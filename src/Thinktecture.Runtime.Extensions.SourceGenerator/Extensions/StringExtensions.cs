using System;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class StringExtensions
   {
      public static string MakeArgumentName(this string name)
      {
         return $"{Char.ToLowerInvariant(name[0])}{name.Substring(1)}";
      }

      public static string? TrimmAndNullify(this string? text)
      {
         if (String.IsNullOrWhiteSpace(text))
            return null;

         return text!.Trim();
      }
   }
}
