using System;
using Microsoft.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class SymbolExtensions
   {
      public static bool IsString(this ISymbol symbol)
      {
         if (symbol is null)
            throw new ArgumentNullException(nameof(symbol));

         return symbol.ContainingNamespace.Name == "System" && symbol.Name == "String";
      }
   }
}
