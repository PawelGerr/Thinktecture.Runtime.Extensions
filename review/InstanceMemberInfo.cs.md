Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/InstanceMemberInfo.cs

- Missing constructor argument validation:
  - No checks for typedMemberState, settings, or name. Misuse (reflection or external callers) can lead to NullReferenceException (e.g., Name.GetHashCode()) or invalid instances.
  - Recommendation: Add ArgumentNullException.ThrowIfNull(typedMemberState); ArgumentNullException.ThrowIfNull(settings); ArgumentException.ThrowIfNullOrWhiteSpace(name);.

- Equality/hash fragility due to delegated components:
  - Equality and GetHashCode depend on _typedMemberState and ValueObjectMemberSettings equality/hash implementations. If those types change semantics or use reference equality, InstanceMemberInfo value semantics become unstable.
  - Recommendation: Ensure both ITypedMemberState and ValueObjectMemberSettings are guaranteed to implement consistent value-based equality/hash (e.g., via IEquatable<T> and IHashCodeComputable). Alternatively, compare/hash the primitive identity properties explicitly.

- Inconsistent equality compared to other IMemberState implementations:
  - Unlike KeyMemberState/DefaultMemberState, equality/hash here do not incorporate ArgumentName. Although ArgumentName is derived from Name, other implementations include it, leading to inconsistent equality semantics across IMemberState types and potential future collisions if ArgumentName normalization logic changes.
  - Recommendation: Either include ArgumentName in equality/hash, or standardize across IMemberState implementations and document the chosen approach.

- Mixed error-type detection mechanisms:
  - CreateOrNull uses property/field.Type.Kind == SymbolKind.ErrorType for early return but populates IsErroneous using TypeKind.Error. While both may indicate errors, the dual mechanism is inconsistent and can diverge across Roslyn versions/symbol shapes.
  - Recommendation: Normalize to a single check (prefer TypeKind.Error for types) and keep the early-return and IsErroneous consistent.

- Tri-state return from GetIdentifierLocation:
  - Returns Location.None when a symbol is captured but no identifier syntax is found, and null when symbols were not captured (allowedCaptureSymbols == false). Callers must handle both Location.None and null distinctly. If consumers are not aware, this can cause missed diagnostics.
  - Recommendation: Document the tri-state behavior or consider returning Location.None consistently and expose a separate property indicating whether symbols were captured.

- Potential memory retention when capturing symbols:
  - Storing IFieldSymbol/IPropertySymbol can retain large portions of the compilation graph if allowedCaptureSymbols is true.
  - Recommendation: Keep allowedCaptureSymbols default false (if not already) and audit call sites for necessity. Consider storing only minimal data (e.g., locations) when possible.
