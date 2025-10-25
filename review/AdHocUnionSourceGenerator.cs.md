Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AdHocUnions/AdHocUnionSourceGenerator.cs

1) Duplicate member type suffix logic produces incorrect names
- Location: Method GetSourceGenContextOrNull(..), inside for (var i = 0; i < memberTypeSymbols.Count; i++) loop, the block computing typeDuplicateCounter and then:
  var name = memberTypeSettings.Name ?? (typeDuplicateCounter == 0 ? defaultName : defaultName + typeDuplicateCounter);
- Problem: The counting algorithm increments typeDuplicateCounter for matches encountered before and after i, and even increments when j == i if a prior duplicate existed. As a result, the first occurrence of a duplicate type gets a suffix "1" (e.g., String1), the second "2", etc. Expected is that the first occurrence should be unsuffixed (String), the second String2, third String3, etc.
- Repro sketch: Member types [string, string, string]
  - i=0 yields suffix 1 -> "String1"
  - i=1 yields suffix 2 -> "String2"
  - i=2 yields suffix 3 -> "String3"
- Suggested fix:
  - Compute occurrence index based only on number of equal types before i (occurrenceIndex = count of j < i where equal) and then:
    name = memberTypeSettings.Name ?? (occurrenceIndex == 0 ? defaultName : defaultName + (occurrenceIndex + 1));
  - Alternatively, maintain a Dictionary<ITypeSymbol,int> with SymbolEqualityComparer.Default to track the number of seen occurrences and derive suffix from that.

2) Records are not considered candidates
- Location: IsCandidate(SyntaxNode ...)
- Problem: switch only handles ClassDeclarationSyntax and StructDeclarationSyntax. Record classes/structs use RecordDeclarationSyntax and will be skipped, even though attributes target class/struct and records are classes/structs semantically.
- Impact: Applying [Union<...>] or [AdHocUnion(...)] to a record will not trigger generation (silent skip).
- Suggested fix: Accept RecordDeclarationSyntax (and check !recordDeclaration.IsGeneric()). If records are intentionally unsupported, emit a diagnostic via the analyzer rather than silently skipping.

3) Silent skips instead of diagnostics for invalid configurations
- Locations:
  - GetSourceGenContextOrNull(..):
    - When attributes length != 1 → returns null (no diagnostic).
    - When memberTypeSymbols is null or Count < 2 → returns null.
    - When member type TypeKind == Error → returns null.
    - When computed name is null/whitespace → returns null.
- Problem: Returning null in these cases suppresses generation without user feedback. This leads to poor UX and hard-to-diagnose issues.
- Suggested fix: Use SourceGenError for user-facing diagnostics in these cases (similar to AttributeInfo.TryCreate failure handling). Example messages:
  - "Ad-hoc union requires exactly one Union attribute"
  - "Ad-hoc union must define at least two member types"
  - "Ad-hoc union member type must be a valid named or array type"
  - "Ad-hoc union member name cannot be null or whitespace"

4) Multiple attribute instances are ignored without feedback
- Location: GetSourceGenContextOrNull(..) requires context.Attributes.Length == 1
- Problem: If multiple Union attributes are accidentally applied (or mixed generic and non-generic), generator silently skips. If multiple usage is invalid, a diagnostic should indicate the problem.
- Suggested fix: Emit a SourceGenError when more than one matching attribute is present.

5) Non-generic constructor argument parsing rejects mid-null with silent skip
- Location: GetSourceGenContextOrNullForNonGeneric(..)
- Behavior: After encountering a null constructor argument, any subsequent non-null argument yields a return null; the generator silently skips. This enforces "contiguous non-nulls then nulls" but without feedback.
- Suggested fix: If this is invalid input, produce a diagnostic stating that member types must be provided contiguously without gaps (e.g., T1..Tn then optional nulls only), or accept and filter trailing nulls explicitly and then validate Count >= 2 with a diagnostic if not.

6) Minor consistency/robustness improvements
- In GetSourceGenContextOrNullForNonGeneric(..), the method returns types.Count > 0 but the caller requires Count >= 2. Consider enforcing Count >= 2 in the helper to keep intent local and reduce confusion.
- Consider using attributeData instead of attributeClass for type arguments for clarity of intent (though attributeClass.TypeArguments is fine, attributeData.ConstructorArguments and related info are already being used).

No performance or threading issues detected (max 5 types; O(n^2) duplicate check is negligible). Cancellation handling is adequate in the main semantic extraction method.
