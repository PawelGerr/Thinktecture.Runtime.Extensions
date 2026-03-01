using System;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Thinktecture;

public static class StringExtensions
{
   [return: NotNullIfNotNull(nameof(text))]
   public static string? NormalizeLineEndings(this string? text)
   {
      if (text is null)
         return null;

      text = text.Replace("\r\n", "\n");

      if (Environment.NewLine == "\r\n")
         text = text.Replace("\n", "\r\n");

      return text;
   }
}
