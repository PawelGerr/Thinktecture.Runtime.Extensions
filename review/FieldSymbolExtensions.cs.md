Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/FieldSymbolExtensions.cs

1) Warning: Implicit dependency on global usings for Roslyn types
- Problem: The file references SyntaxToken, IFieldSymbol, and IPropertySymbol without a local using Microsoft.CodeAnalysis;. Only using Microsoft.CodeAnalysis.CSharp.Syntax; is present, which does not import SyntaxToken nor the symbol interfaces.
- Impact: Compilation relies on a global using elsewhere. If that global using is removed or not included in certain build contexts (e.g., tests, analyzers, or consumers embedding sources), this file will fail to compile.
- Recommendation: Add an explicit using to remove the hidden dependency.

Suggested change:
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;
...

2) Warning: Backing field detection may be broader than intended
- Problem: IsPropertyBackingField checks only field.AssociatedSymbol is IPropertySymbol. This is generally correct for compiler-generated backing fields, but if the intention is to strictly detect compiler-generated fields, it should also verify field.IsImplicitlyDeclared to avoid any edge cases where a field might be associated but not implicitly generated.
- Impact: Potential false positives in rare/edge cases if non-implicit fields with an associated property symbol appear (uncommon, but tightening the check is safer).
- Recommendation: Narrow the check if strictness is desired.

Suggested change:
public static bool IsPropertyBackingField(this IFieldSymbol field)
{
   return field.IsImplicitlyDeclared && field.AssociatedSymbol is IPropertySymbol;
}
