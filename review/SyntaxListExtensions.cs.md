.Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SyntaxListExtensions.cs

- Severity: Warning
  Title: Unnecessary public API surface in generator assembly
  Details: The extension class is declared as public. Source generator helper extensions are typically internal to avoid expanding the public surface of the package and to reduce the risk of accidental usage by consumers.
  Recommendation: Change `public static class SyntaxListExtensions` to `internal static class SyntaxListExtensions`. Ensure consistency with other internal Roslyn helper extensions.

- Severity: Warning
  Title: Method name overlaps with LINQ FirstOrDefault
  Details: The extension method name `FirstOrDefault` overlaps with `System.Linq`’s extension. While overload resolution will prefer this method for `SyntaxList<T>`, the overlap can be confusing when `System.Linq` is in scope, especially if this API ever becomes visible to consumers.
  Recommendation: Keeping the class internal will mitigate confusion. If a public API is required, consider a more explicit name (e.g., `FirstOrDefaultFast`) to signal non-LINQ semantics/perf intent.

- Severity: Info (Perf/Micro)
  Title: Avoid copying SyntaxList<T> struct and repeated Count property access
  Details: `SyntaxList<T>` is a struct passed by value. The loop also reads `list.Count` each iteration. Both are very small overheads, but can be avoided.
  Recommendation (optional): 
    - Change the signature to `this in SyntaxList<T> list` to avoid copying the struct.
    - Cache `var count = list.Count;` before the loop. Impact is minor.
