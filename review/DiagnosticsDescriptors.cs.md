Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DiagnosticsDescriptors.cs

- Potential missing Roslyn using
  - The file references `DiagnosticDescriptor` and `DiagnosticSeverity` but has no `using Microsoft.CodeAnalysis;`. If there is no project-wide global using, this will not compile.
  - Recommendation: Add `using Microsoft.CodeAnalysis;` or fully-qualify the types.

- Incorrect category for Internal Usage descriptor
  - `InternalApiUsage` likely belongs to `ThinktectureRuntimeExtensionsInternalUsageAnalyzer`, but the `category` parameter is `nameof(ThinktectureRuntimeExtensionsAnalyzer)` for all descriptors.
  - Recommendation: Use the correct analyzer name as category for descriptors owned by that analyzer, e.g. `nameof(ThinktectureRuntimeExtensionsInternalUsageAnalyzer)` for internal-usage diagnostics.

- Grammar/typos in titles/messages
  - TTRESG046 Title: "The arguments of 'Switch' and 'Map' must named" → should be "must be named".
  - TTRESG047 Title: "Variable must be initialed with non-default value" → should be "initialized". Message: "non default" → "non-default".
  - TTRESG048 Message: "to defined the equality comparer" → should be "to define".
  - TTRESG049 Message: "to defined the equality comparer" → should be "to define".
  - General consistency: Use consistent hyphenation and terminology (e.g., "first level" vs "first-level" appears, standardize).

- Inconsistent or misleading guidance for complex value objects with string members (TTRESG049)
  - The message recommends `KeyMemberEqualityComparerAttribute<TAccessor, TKey>` for complex value objects. Per project guidelines, complex value objects should use `MemberEqualityComparer<...>` per member, not KeyMemberEqualityComparer (which is for keyed/simple VOs).
  - Recommendation: Update message to recommend `MemberEqualityComparer<MyType, TMember, ...>` for string members in complex value objects.

- Descriptor name clarity (TTRESG060)
  - Name `SmartEnumMustNotObjectFactoryConstructor` is grammatically off and unclear. The message talks about `HasCorrespondingConstructor = true` on `ObjectFactoryAttribute<T>`.
  - Recommendation: Rename to `SmartEnumMustNotHaveObjectFactoryConstructor` or `SmartEnumMustNotSetHasCorrespondingConstructor` for clarity (keeping existing ID).

- Consistency in terminology
  - Some titles/messages use "Smart Enum" (with space) while others may use "SmartEnum". Choose a single style and apply consistently.
  - Similarly, "key member"/"key" notation should be consistent across messages.

- Placeholder alignment
  - Ensure that for messages using `{0}`, `{1}`, `{2}`, the analyzer reports diagnostics with the correct number and order of arguments. A quick audit is advised, especially for:
    - TTRESG045: message uses `{0}`, `{2}`, `{1}` in a non-sequential order; confirm analyzer argument ordering matches.

- Help links absent
  - No descriptors specify `helpLinkUri` with documentation URLs, which can improve developer experience.
  - Recommendation: Provide `helpLinkUri` (to docs or repo Wiki) for each diagnostic where applicable.

- ID range conventions
  - IDs span multiple ranges: 001–060, 097–104, and 1000. The separate ranges may be intentional; ensure policy is documented and enforced to avoid future collisions.

- Minor style suggestion
  - Consider defining a shared `const string CategoryMain = nameof(ThinktectureRuntimeExtensionsAnalyzer);` and (optionally) `CategoryInternal = nameof(ThinktectureRuntimeExtensionsInternalUsageAnalyzer);` to avoid typos and ease future changes.
