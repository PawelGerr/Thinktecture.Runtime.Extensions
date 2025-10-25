Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ParsableCodeGenerator.cs

- Invalid use of ref struct type as a generic argument:
  - Under NET9_0_OR_GREATER, `Validate<T>(ReadOnlySpan<char> ...) where T : IObjectFactory<TType, ReadOnlySpan<char>, ValidationError>` uses `ReadOnlySpan<char>` (a ref struct) as a generic type argument. C# disallows ref struct types as generic type arguments for interfaces/classes.
  - Recommendation: Avoid `ReadOnlySpan<char>` as a generic parameter in `IObjectFactory<...>`. Provide span-specific non-generic overloads or an alternate factory interface designed for spans (e.g., a static method on the target type or a separate non-generic contract taking spans).

- Potential null path for key type in Validate signature:
  - `var keyType = isKeyTypeString || state.HasStringBasedValidateMethod ? "string" : state.KeyMember?.TypeFullyQualified;`
  - If `state.KeyMember` is null and `HasStringBasedValidateMethod` is false, `keyType` becomes null, producing invalid generated code for the `Validate<T>` method signature.
  - Recommendation: Ensure `keyType` is always resolved to a valid type name (guard with null checks and/or fail early), or assert that `KeyMember` is present for non-string-based scenarios.

- String literal for error message not escaped:
  - In `Parse`/`Parse(ReadOnlySpan<char>)`, the message builds `"... \"" + state.Type.Name + "\"."` by injecting `state.Type.Name` directly into the emitted source string literal. If the type name contains characters needing escaping (e.g., quotes, backticks, generics), the generated code can be malformed or the runtime message incorrect.
  - Recommendation: Use a helper to emit an escaped string literal, or prefer compile-time `nameof`/`typeof`-based messaging inside generated code (e.g., $"Unable to parse \"{typeof(T).Name}\".").

- ISpanParsable support limited to enums only:
  - Span-based `Parse/TryParse` and ISpanParsable are added only when `_isForEnum` and key type is string. String-keyed value objects with string-based validation could also benefit from span parsing for performance.
  - Recommendation: Consider enabling span-based parsing for string-keyed value objects (or behind a setting) to achieve parity.

- Base type list comma/style robustness:
  - `GenerateBaseTypes` appends `, ISpanParsable<T>` under `#if NET9_0_OR_GREATER`. While generally safe, ensure the generated base type list and preceding line breaks don’t introduce formatting issues (e.g., dangling commas if the conditional block toggles).
  - Recommendation: Keep comma and spacing generation centralized to avoid trailing/leading comma pitfalls under conditional compilation.

- Message composition uses ToString with potential null:
  - The thrown `FormatException` message uses `validationError?.ToString()` with null-coalescing fallback. While safe, consider ensuring `ValidationError` has a reliable message (or centralize message creation) to avoid vague fallbacks.

- Minor style consistency:
  - Hardcoded `FileNameSuffix` and `CodeGeneratorName` strings are fine but should be consistent with other generators’ naming conventions. Ensure downstream naming logic doesn’t rely on transient formatting differences (e.g., hyphen vs space).

- Missing cancellation propagation:
  - Methods don’t take or check a `CancellationToken`. If code generation can be long-running or iterative, consider threading through cancellation from the orchestrator (only if your generator infrastructure supports it).

- Conditional symbol requirement:
  - The file assumes `NET9_0_OR_GREATER` is defined appropriately when targeting net9+. Ensure multi-targeting defines the symbol to avoid mismatches between API usage and target frameworks.
