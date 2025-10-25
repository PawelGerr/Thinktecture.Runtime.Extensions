Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectSourceGenerator.cs

1) Wrong diagnostic/error message text (copy/paste)
- Location: GetKeyedSourceGenContextOrNull and GetComplexSourceGenContextOrNull
- Details: When TypedMemberStateFactoryProvider.GetFactoryOrNull returns null, the returned SourceGenError reads: "Could not fetch type information for code generation of a smart enum". This file handles value objects, not smart enums.
- Impact: Misleading diagnostics during generator failures; confuses users and maintainers.
- Suggested fix: Change message to "value object" in both methods.

2) Silent skip on nullable key type (missing diagnostic)
- Location: GetKeyedSourceGenContextOrNull
- Code: if (keyMemberType.NullableAnnotation == NullableAnnotation.Annotated) return null;
- Details: Generator silently bails out for [ValueObject<TKey?>], producing no source and no diagnostic from this path.
- Impact: Users see no generated code without a clear reason from the generator path; relies on separate analyzer coverage.
- Suggested fix: Return a SourceGenError with a precise message (e.g., "Key member type must be non-nullable") instead of null so InitializeErrorReporting can surface a diagnostic. Alternatively, wire an explicit diagnostic here mirroring the analyzer rule.

3) Potential null usage risk in complex serializer path
- Location: InitializeSerializerGenerators (complex overload)
- Code: Creates KeyedSerializerGeneratorState with keyMember: null for complex value objects when any ObjectFactory.UseForSerialization != None:
  var serializerState = new KeyedSerializerGeneratorState(state.State, null, state.AttributeInfo, state.Settings.SerializationFrameworks);
- Details: Factories iterate these states and call Factory.MustGenerateCode(tuple.State) and later GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory). If any serializer factory assumes a non-null KeyMember for KeyedSerializerGeneratorState, this risks NREs or incorrect logic.
- Impact: Potential runtime exceptions in the generator depending on factory implementations; fragile coupling (complex using a "keyed" state with null key).
- Suggested fix: Either:
  a) Ensure all IValueObjectSerializerCodeGeneratorFactory implementations defensively handle null KeyMember for complex types; or
  b) Introduce a dedicated state for "object-factory-based" complex serialization to avoid reusing KeyedSerializerGeneratorState with null KeyMember; or
  c) Guard before enqueuing these states with a factory capability check specific to object-factory-from-string flow.

Notes:
- Other flows (options/logging, distinct set handling, exception routing, and incremental composition) look consistent with the codebase patterns.
