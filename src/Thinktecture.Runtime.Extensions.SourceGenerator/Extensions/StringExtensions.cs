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
   }
}
