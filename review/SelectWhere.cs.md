# Review: src/Thinktecture.Runtime.Extensions.SourceGenerator/SelectWhereDelegate.cs

Status: Reviewed

Summary:
Defines two public delegates that implement a combined “select + where” (try-select) pattern:
- SelectWhereDelegate<T, TResult>(T item, out TResult result) → bool
- SelectWhereDelegate<T, TArg, TResult>(T item, TArg arg, out TResult result) → bool
Both use [MaybeNullWhen(false)] on the out result, matching BCL “TryXxx” patterns for nullable flow. Generic variance marks T and TArg as contravariant (in), which is appropriate for consumer-delegate scenarios.

Strengths:
- Correct nullable annotations: [MaybeNullWhen(false)] on out TResult result matches established TryXxx design, enabling correct flow analysis without forcing TResult? usage.
- Useful overload with an extra argument (TArg) to avoid closure allocations and pass state explicitly.
- Appropriate use of generic variance: in T and in TArg improve delegate assignability.
- Minimal and clear API surface without unnecessary dependencies.

Considerations and Suggestions:

1) XML documentation for public API
- Add XML docs to clarify semantics:
  - Returns true when result is considered produced/selected; returns false otherwise.
  - When returning false, result may be null (for reference types).
  - Describe expected usage (e.g., filtering + projection in one pass, “TrySelect” pattern).
- Improves discoverability and helps analyzers and consumers.

2) Naming alignment with common patterns
- Current name “SelectWhereDelegate” communicates intent, but a more canonical “TrySelect” naming may be clearer to .NET developers familiar with TryParse/TryGet patterns:
  - TrySelect<T, TResult>
  - TrySelect<T, TArg, TResult>
- This is optional; if the name is already used widely in the codebase, keep as-is.

3) Non-null guarantee on success (NotNullWhen(true)) — only if contract guarantees it
- If the contract guarantees that result is non-null when the delegate returns true, consider adding [NotNullWhen(true)] in addition to [MaybeNullWhen(false)]:
  - This strengthens flow analysis for reference types, enabling non-null result within if (delegate(...)) branches.
- Do this only if it’s valid across usages. If null might still be a valid “selected” value, keep the existing annotation.

4) Performance options for large structs (optional)
- For scenarios where T is a large struct, consider an additional ref-readonly variant to avoid copies:
  - delegate bool SelectWhereRefDelegate<in T, TResult>(in T item, [MaybeNullWhen(false)] out TResult result);
  - delegate bool SelectWhereRefDelegate<in T, in TArg, TResult>(in T item, TArg arg, [MaybeNullWhen(false)] out TResult result);
- Alternatively, adding the parameter modifier “in” to the existing signatures (resulting in delegate bool ... (in T item, ...)) is possible even with contravariant T, but could be confusing due to the double “in”. A separate “Ref” delegate often reads clearer.

5) Target framework compatibility of attributes
- The file uses System.Diagnostics.CodeAnalysis attributes. The project includes a NullableAttributes.cs shim, so older TFMs should be covered. No action needed, but keep this alignment consistent project-wide.

6) Usage example (for docs)
- Example for XML docs to illustrate the pattern:
  ```
  // Select only positive ints and project to string
  bool PositiveToString(int x, out string? s)
  {
      if (x > 0)
      {
          s = x.ToString();
          return true;
      }

      s = null;
      return false;
  }
  ```
  With [MaybeNullWhen(false)], consumers won’t get warnings for s being possibly null after a true-return check.

Overall Assessment:
- Solid, minimal delegate definitions following modern nullability practices and variance. Primary improvement would be adding XML documentation and deciding whether to also apply [NotNullWhen(true)] based on the intended contract. Consider adding ref-readonly variants if high-throughput scenarios with large structs are common.
