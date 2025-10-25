Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumSourceGenerator.cs

1) Candidate filtering excludes structs and records (potentially unintended)
- Details: IsSmartEnumCandidate filters only ClassDeclarationSyntax and excludes generics. This ignores StructDeclarationSyntax and RecordDeclarationSyntax entirely.
- Impact: If SmartEnums are intended to support struct or record declarations (as suggested by repo-wide guidance that types may be class/struct), such annotated types will be silently ignored by the generator (no code generated).
- Recommendation:
  - If SmartEnums are class-only by design: add a diagnostic that informs the user when [SmartEnum] is applied to non-class declarations to avoid silent no-op.
  - If structs/records should be supported: extend predicate to include StructDeclarationSyntax and/or RecordDeclarationSyntax (for record class) and ensure downstream logic supports these forms.

2) Silent skip for invalid keyed configurations provides no diagnostic
- Details: In GetSourceGenContextOrNull (keyed path), certain invalid/misconfigured cases return null and thus completely skip codegen, without reporting a SourceGenError:
  - keyMemberType.NullableAnnotation == Annotated (e.g., SmartEnum<string?>)
  - attributeType.Arity != 1 (unexpected, but would also silently skip)
  - keyMemberType.TypeKind == Error (attribute resolves but type arg is erroneous)
- Impact: Users receive no generator-side feedback and might miss analyzer diagnostics (if analyzers are disabled or not present in a specific context). This leads to non-obvious “nothing generated” behavior.
- Recommendation: Convert these early-return null cases into actionable diagnostics via SourceGenError (e.g., “Key type must be non-nullable” with location tds). This improves UX and aligns with other validation branches that already emit SourceGenError.

3) Mixed keyed and keyless attributes on the same type not explicitly rejected by generator
- Details: The generator runs two independent pipelines (keyed and keyless). The “more than 1 ‘SmartEnum’” check uses context.Attributes.Length > 1 within each pipeline. If a type is (incorrectly) decorated with both the keyed and the keyless attributes, each pipeline will see only one attribute and won’t emit the “more than 1” error.
- Impact: Misconfiguration may proceed further than intended until later checks, potentially resulting in confusing outcomes.
- Recommendation: Either:
  - Add a cross-attribute check (e.g., via a shared symbol pass) to detect presence of both keyed and keyless variants and report a diagnostic; or
  - Ensure the analyzer reliably catches this scenario (and consider adding a targeted diagnostic if missing).

4) Potentially noisy logging for serializer participants
- Details: InitializeSerializerGenerators logs an Information entry per referenced module detection (“will participate in code generation”). In larger solutions with multiple references, this can duplicate messages.
- Impact: Log noise, making it harder to spot relevant issues.
- Recommendation: Log once per distinct detected factory (after Distinct), or downgrade to Debug for repetitive detections.
