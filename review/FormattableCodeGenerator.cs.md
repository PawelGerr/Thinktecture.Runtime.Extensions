Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/FormattableCodeGenerator.cs

- Risk of InvalidCastException at runtime:
  - The implementation casts the key member to `IFormattable`: `((global::System.IFormattable)this.<KeyMember>).ToString(...)`.
  - If the key member type doesn’t implement `IFormattable` (or is nullable and currently null), this will throw at runtime.
  - Recommendation: Ensure emission is gated only when the key member is known to implement `IFormattable`, or generate a safer call path (e.g., `FormattableString.Invariant(...)` or use `ToString()` with provider only when supported). If null is possible, consider null-checks or clearly document non-null invariant.

- Optional parameter on interface implementation:
  - The generated method is `public string ToString(string? format, IFormatProvider? formatProvider = null)`. `IFormattable.ToString` has no default value for `formatProvider`.
  - While default parameter values are not part of the signature and thus compile, they can be misleading when implementing interfaces and may cause inconsistent call sites.
  - Recommendation: Omit the default value to exactly mirror the interface signature: `public string ToString(string? format, IFormatProvider? formatProvider)`.

- Missing ISpanFormattable support:
  - Many BCL primitives implement `ISpanFormattable` for allocation-free formatting. The generator doesn’t surface it even if the key member supports it.
  - Recommendation: Under appropriate target frameworks, optionally emit `ISpanFormattable` implementation and forward to the key member’s span-based `TryFormat` for performance parity.

- Nullability handling of key member:
  - For reference-type key members, if the value can be null, `((IFormattable)this.Key).ToString(...)` will throw.
  - Recommendation: Either rely on a non-null invariant for the key or generate a null-safe fallback (e.g., return empty or `this.Key?.ToString(...) ?? string.Empty`) depending on desired semantics; at minimum, document the assumption.

- Base type emission relies on upstream gating:
  - `GenerateBaseTypes` always appends `IFormattable` without checking state flags. If pipeline gating fails, consumer types may end up claiming `IFormattable` though the key member cannot format.
  - Recommendation: Keep gating centralized in the pipeline, or add a defensive check in the generator if state carries “is key member formattable” info.

- Naming/style consistency:
  - `CodeGeneratorName`/`FileNameSuffix` appear consistent, but ensure they align with conventions used elsewhere (hyphenation/casing), especially if tooling relies on these identifiers.
