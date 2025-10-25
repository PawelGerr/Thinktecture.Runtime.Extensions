# Review: src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/LogLevel.cs

Status: Reviewed

Summary:
Defines a logging level enum in Thinktecture.Logging:
- Trace = 0
- Debug = 1
- Information = 2
- Warning = 3
- Error = 4
- None = 5

This resembles Microsoft.Extensions.Logging.LogLevel but notably omits Critical and assigns None = 5, which diverges from the MEL values.

Strengths:
- Explicit numeric assignments prevent accidental reordering changes and make comparisons predictable.
- Compact set of levels suitable for source-generator diagnostics/logging without external dependencies.
- Namespace Thinktecture.Logging avoids direct type collisions with Microsoft.Extensions.Logging.LogLevel.

Issues, Risks, and Suggestions:

1) Divergence from Microsoft.Extensions.Logging.LogLevel (potential mapping bugs)
- MEL defines: Trace=0, Debug=1, Information=2, Warning=3, Error=4, Critical=5, None=6.
- Current enum omits Critical and sets None=5. Any adapter that forwards to MEL or expects parity will mis-map (e.g., None=5 would be treated as Critical in MEL).
Recommendations:
- If MEL parity is desirable, update to:
  - Trace=0, Debug=1, Information=2, Warning=3, Error=4, Critical=5, None=6
- If parity is not intended (simplified model), ensure every sink/bridge has an explicit switch mapping and add tests verifying correct translation to MEL/Serilog/etc.

2) Missing “Critical” severity (semantics coverage)
- Without Critical, consumers cannot differentiate between Error and highest-severity failures. Consider adding Critical=5 for richer filtering and to align with common logging ecosystems.

3) API surface and usage scope
- If this enum is internal to the source generator’s own logging infrastructure, consider making it internal to avoid public API drift and confusion with MEL’s LogLevel.
- If it’s part of public options, document divergence from MEL’s values and mapping guidance.

4) XML documentation (recommended)
- Add XML docs clarifying intent and typical use:
  - The meaning of None (disable logging).
  - Whether parity with MEL is intended or not.
  - Guidance for mapping to external logging frameworks.

5) Name ambiguity in consuming code
- With both Thinktecture.Logging and Microsoft.Extensions.Logging in scope, “LogLevel” can be ambiguous. Documentation should suggest using namespace qualification or using aliases in consumer code if both enums are referenced.

6) Forward compatibility
- If you later add Critical to align with MEL, treat it as a breaking change if the enum is public (binary compat concerns). Decide now whether to align or keep the simplified set.

Overall Assessment:
- Minimal and explicit enum is fine for in-house logging, but the deviation from MEL (missing Critical and None=5) is a common foot-gun for adapters. Either align with MEL values or guarantee robust mapping and documentation to prevent misinterpretation by downstream sinks.
