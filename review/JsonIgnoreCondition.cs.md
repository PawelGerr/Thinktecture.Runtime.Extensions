# Review: src/Thinktecture.Runtime.Extensions.SourceGenerator/Json/JsonIgnoreCondition.cs

Status: Reviewed

Summary:
Defines a JsonIgnoreCondition enum in the Thinktecture.Json namespace mirroring System.Text.Json.Serialization.JsonIgnoreCondition values:
- Never
- Always
- WhenWritingDefault
- WhenWritingNull

This likely decouples the source generator from a hard dependency on System.Text.Json at compile time while preserving equivalent semantics.

Strengths:
- Names and order match the BCL JsonIgnoreCondition (Never=0, Always=1, WhenWritingDefault=2, WhenWritingNull=3), preserving expected semantics.
- Minimal and clear, enabling the generator to reason about JSON ignore behavior without importing STJ.
- Public enum allows sharing across internal generator components without additional wrappers.

Issues, Risks, and Suggestions:

1) Make numeric values explicit to guarantee parity with BCL
- Relying on declaration order is fine, but adding explicit assignments prevents accidental reordering from changing numeric values.
Suggested:
```
public enum JsonIgnoreCondition
{
    Never = 0,
    Always = 1,
    WhenWritingDefault = 2,
    WhenWritingNull = 3
}
```

2) Namespace divergence from System.Text.Json
- Being in Thinktecture.Json prevents accidental type confusion with STJ’s enum, which is good. However, any bridging code that translates from this enum to STJ (or Newtonsoft/MessagePack equivalents) needs a well-defined mapping.
- Suggest documenting the mapping strategy (e.g., extension method or switch) in a central place to avoid drift.

3) API surface and versioning
- If this enum is only for generator-internal use, consider making it internal to avoid leaking a pseudo-STJ type to library consumers.
- If kept public (e.g., used in attributes or user-facing options), add XML docs clarifying that this mirrors STJ semantics and may need updates when STJ evolves.

4) Forward compatibility with new STJ values
- STJ may add new values in future releases. Ensure any switch statements over this enum include a default path or are audited when updating STJ support.
- Consider adding tests validating mapping equivalence to STJ’s enum where available.

5) Underlying type (optional)
- Default underlying type is int. If you have strict size constraints (rare here), you could set : byte to reduce metadata size. Only consider before the API ships; changing underlying type later is a breaking change.

6) XML documentation (recommended)
- Document each value’s behavior relative to JSON serialization:
  - Never: Never ignore.
  - Always: Always ignore.
  - WhenWritingDefault: Ignore default(T) values.
  - WhenWritingNull: Ignore nulls.
- Helps consumers and enforces consistent usage in the codebase.

Overall Assessment:
- Correct and minimal mirror of STJ’s JsonIgnoreCondition. Add explicit numeric values, consider internal visibility if not user-facing, and document mapping/semantics. Keep an eye on STJ changes to maintain parity.
