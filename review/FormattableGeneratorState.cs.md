Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/FormattableGeneratorState.cs

- Default(struct) produces invalid null state that breaks GetHashCode:
  - As a readonly struct, `default(FormattableGeneratorState)` yields `Type == null`, `KeyMember == null`, and `CreateFactoryMethodName == null`. `GetHashCode()` dereferences all three via:
    - `TypeInformationComparer.Instance.GetHashCode(Type)` (Type null → NRE)
    - `MemberInformationComparer.Instance.GetHashCode(KeyMember)` (KeyMember null → NRE)
    - `CreateFactoryMethodName.GetHashCode()` (null → NRE)
  - Recommendation: Prefer a class to avoid invalid default, or add `IsDefault/IsValid` semantics and guard usage in `GetHashCode/Equals`. At minimum, null-guard each field in `GetHashCode`.

- Missing constructor argument validation:
  - No checks for `type`, `keyMember`, or `createFactoryMethodName`. Misuse/reflection can create invalid instances causing runtime failures (e.g., in hashing).
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);`, `ArgumentNullException.ThrowIfNull(keyMember);`, and `ArgumentException.ThrowIfNullOrWhiteSpace(createFactoryMethodName);`.

- Invariants between flags not enforced:
  - `SkipIFormattable` and `IsKeyMemberFormattable` imply constraints (e.g., don’t emit IFormattable when `SkipIFormattable == true`; ensure key member is formattable when emitting). The state does not validate incompatible combinations.
  - Recommendation: Validate or document required combinations to prevent inconsistent states entering generators.

- String equality/hash semantics implicit:
  - Equality uses `==` and hash uses `string.GetHashCode()` for `CreateFactoryMethodName`. While fine for in-memory use, ordinal nature is implicit and can differ across runtimes; leading/trailing whitespace or casing differences can also cause surprising inequality.
  - Recommendation: Normalize (trim/canonicalize) `CreateFactoryMethodName` on construction. If deterministic hashing across processes is ever required, compute over a canonical form and document ordinal semantics.

- Consistency with project conventions:
  - Many CodeAnalysis state types implement `IHashCodeComputable` to standardize hashing; this type does not.
  - Recommendation: Implement `IHashCodeComputable` for consistency and testability.

- Operator overloads redundancy:
  - `==`/`!=` simply delegate to `Equals`. Acceptable but increases surface without added guarantees.
  - Recommendation: Keep if ergonomics desired; else rely on `Equals` only.
