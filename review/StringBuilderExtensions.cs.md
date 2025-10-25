Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/StringBuilderExtensions.cs

1) Incorrect Accessibility mapping (bug)
- Problem: AppendAccessibility maps Accessibility.ProtectedAndInternal to "protected internal". In Roslyn, ProtectedAndInternal corresponds to the C# modifier "private protected" (intersection). "protected internal" (union) corresponds to Accessibility.ProtectedOrInternal, which is not handled at all.
- Impact: Generated members may have broader or narrower accessibility than intended, changing visibility and potentially causing compilation or behavioral issues.
- Fix:
  - Map ProtectedAndInternal => "private protected"
  - Add handling for ProtectedOrInternal => "protected internal"
  - Optionally handle NotApplicable with a no-op/default
- Suggested patch:
  ```
  public static StringBuilder AppendAccessibility(
     this StringBuilder sb,
     Accessibility accessibility)
  {
     switch (accessibility)
     {
        case Accessibility.Private:
           sb.Append("private");
           break;
        case Accessibility.ProtectedAndInternal:
           sb.Append("private protected"); // FIX
           break;
        case Accessibility.ProtectedOrInternal:
           sb.Append("protected internal"); // FIX
           break;
        case Accessibility.Protected:
           sb.Append("protected");
           break;
        case Accessibility.Internal:
           sb.Append("internal");
           break;
        case Accessibility.Public:
           sb.Append("public");
           break;
        // case Accessibility.NotApplicable:
        //    break; // optionally no-op
     }

     return sb;
  }
  ```

2) ToCamelCase duplicates character for non-leading uppercase position (bug)
- Problem: When the first letter to convert is at index i > 0 (e.g., name starts with underscore), the code appends the lowercased character at i and then appends name.Substring(i), duplicating that character.
- Repro:
  - name: "_AValue", leadingUnderscore: false
  - Current output: "AaAValue" (duplicated "A")
- Root cause:
  ```
  sb.Append(name.Substring(startsWithUnderscore ? 1 : 0, i))
    .Append(Char.ToLowerInvariant(charValue));

  if (i != name.Length - 1)
  {
     sb.Append(name.Substring(i)); // should be i + 1
  }
  ```
- Impact: Invalid/malformed generated identifiers, potential compilation errors or style violations.
- Fix:
  ```
  if (i != name.Length - 1)
  {
     sb.Append(name.Substring(i + 1)); // FIX
  }
  ```
  (The rest of the logic can remain, but consider covering more edge cases with unit tests.)

3) Missing coverage for Accessibility.NotApplicable (warning)
- Problem: NotApplicable is unhandled in AppendAccessibility. While it may not be used in your code paths, leaving it unhandled risks silent misgeneration if ever encountered.
- Impact: Potentially emits an empty modifier string where one wasn&#39;t intended, or misses a diagnostic.
- Fix: Add a case with a documented no-op or throw/log if this state should never occur in your pipeline.

4) Potential loss of generic arity/arguments in AppendTypeFullyQualified (warning)
- Problem: The overload:
  ```
  AppendTypeFullyQualified(this StringBuilder sb, INamespaceAndName type, IReadOnlyList<ContainingTypeState> containingTypes)
  ```
  appends only containing type names without generic arguments/arity markers.
- Impact: If used to render nested generic types in contexts requiring type arguments (e.g., "Outer<T>.Inner"), output may be ambiguous/incorrect.
- Fix: Ensure this overload is only used for declarations where generic parameters are appended separately (e.g., via AppendGenericTypeParameters), or extend it to include generic arity/arguments when required.

5) Implicit dependency on global usings for Roslyn enums (warning)
- Problem: The file references Accessibility, RefKind, and NullableAnnotation without namespace qualification or local using directives.
- Impact: Compilation depends on global usings existing elsewhere in the project. If removed/changed, this file will no longer compile.
- Fix: Either:
  - Add explicit using Microsoft.CodeAnalysis; at the top of the file, or
  - Fully qualify these enum references.

Recommended follow-up
- Add unit tests (or snapshot tests on representative generated snippets) covering:
  - Accessibility mapping for all Accessibility values.
  - ToCamelCase edge cases: leading underscore, digits, non-letter prefixes, all-uppercase prefixes, mixed cases, single-letter names.
  - Nested generic containing types rendering for AppendTypeFullyQualified overload.
