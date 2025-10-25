Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SyntaxTokenListExtensions.cs

- Error (conditional): Missing explicit Roslyn using.
  - The file references SyntaxToken and SyntaxTokenList but has no using Microsoft.CodeAnalysis;. If there is no project-wide global using for Microsoft.CodeAnalysis, this file will not compile.
  - Fix: Add `using Microsoft.CodeAnalysis;` at the top, or ensure a project/global using is present.

- Warning: Avoid copying struct receiver.
  - SyntaxTokenList is a struct. The extension method currently takes it by value, which can cause unnecessary copies.
  - Fix (non-breaking): Change signature to `public static SyntaxToken FirstOrDefault(this in SyntaxTokenList list)` to pass by readonly reference.

- Warning: Method name overlaps with LINQ FirstOrDefault.
  - Defining an extension named FirstOrDefault on SyntaxTokenList can shadow System.Linq.Enumerable.FirstOrDefault for variables statically typed as SyntaxTokenList. While semantics here match LINQ&#39;s no-predicate overload, this can be surprising and may affect overload resolution in some contexts.
  - Consider whether a more explicit name (e.g., FirstTokenOrDefault) is preferable to avoid ambiguity.
