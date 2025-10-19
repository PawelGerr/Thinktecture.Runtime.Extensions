using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

internal static class TypeDeclarationSyntaxExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool IsGeneric(this TypeDeclarationSyntax tds)
   {
      return tds.TypeParameterList is { Parameters.Count: > 0 };
   }
}
