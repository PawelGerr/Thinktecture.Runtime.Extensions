# Review: CodeAnalysis/SerializationFrameworks.cs

Issues found (errors/warnings only):

1) Logging/diagnostics ambiguity from composite alias
- Location: enum member Json
- Problem: With [Flags], defining a named composite (Json = SystemTextJson | NewtonsoftJson) causes Enum.ToString() to return "Json" (single name) instead of "SystemTextJson, NewtonsoftJson" when the value matches the composite exactly. This can reduce clarity in logs/diagnostics and can surprise code that expects decomposition.
- Recommendation: Either:
  - Keep the alias but do not rely on Enum.ToString() for logs; provide a helper to render individual flags, or
  - Rename to a more explicit name like AnyJson/JsonBoth to signal it’s a group, or
  - Remove the composite alias if it’s not needed as an input/output value.

2) Cross-name inconsistency/ambiguity with “Json”
- Location: enum member Json vs Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_JSON
- Problem: The enum’s Json groups both JSON frameworks, but the module constant THINKTECTURE_RUNTIME_EXTENSIONS_JSON typically refers only to the System.Text.Json-based assembly. The shared “Json” wording can be misread, making code reviews and searches error-prone.
- Recommendation: Align naming across the codebase. Options:
  - Rename the enum composite to AnyJson/JsonBoth, or
  - Rename the module constant to THINKTECTURE_RUNTIME_EXTENSIONS_SYSTEMTEXTJSON for precision, or
  - Add comments clarifying the difference.

3) Maintenance risk: “All” requires manual updates
- Location: enum member All
- Problem: When adding new frameworks, All must be updated manually, otherwise it silently stops representing all flags.
- Recommendation: Add tests that assert All equals the bitwise OR of all defined single-flag members. This prevents regressions if a new flag is added without updating All.
