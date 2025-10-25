TypeDeclarationSyntaxExtensions.cs – Issues

- Warning: IsGeneric only inspects the declaration’s own type parameter list.
  - Details: It checks `tds.TypeParameterList` and counts parameters, which ignores generic arity contributed by containing (outer) generic types. For a nested type inside a generic outer type, this returns false even though the constructed type is generic.
  - Impact: If used to drive code generation decisions (naming, constraints, emitted members), nested generic types may be mishandled.
  - Recommendation: If the intention is “generic in the semantic sense,” resolve the `INamedTypeSymbol` and use its `Arity` (including outer types). Keep this helper only for “syntactic” generics on the current declaration and name/comment it accordingly to avoid misuse.

- Warning: Null receiver risk for extension method.
  - Details: Extension methods can be invoked with a null receiver (e.g., static call or via reflection), which would throw `NullReferenceException`.
  - Impact: In normal Roslyn pipelines with NRT this is unlikely, but reflection/tooling can bypass nullability.
  - Recommendation: If this API is exposed across boundaries, add a guard or Debug.Assert. If strictly internal and validated upstream, this can be ignored.

- Minor: AggressiveInlining on trivial method.
  - Details: `[MethodImpl(MethodImplOptions.AggressiveInlining)]` on a single property-pattern check is unlikely to produce measurable benefit, and the JIT in Roslyn host processes may inline without the attribute.
  - Recommendation: Remove unless a benchmark shows a hotspot; prefer relying on JIT heuristics.
