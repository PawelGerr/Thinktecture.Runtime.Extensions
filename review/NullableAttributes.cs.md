# NullableAttributes.cs – Review

Summary:
- Purpose: Provide shim implementations of selected System.Diagnostics.CodeAnalysis nullability attributes for environments targeting netstandard2.0 so the source generator code can compile and annotate generated code consistently.
- Scope: Internal, non-public attribute types in the System.Diagnostics.CodeAnalysis namespace, conditionally compiled only when NETSTANDARD2_0 is defined.
- Context: The SourceGenerator project targets netstandard2.0; the repo globally sets LangVersion to 13.0, enabling newer C# syntax used in this file.

Findings:
- Warnings:
  - [Conditional compilation scope] The guard is limited to NETSTANDARD2_0. If this project ever multi-targets frameworks that also lack these attributes (e.g., netstandard2.1), the shims will not be compiled, potentially breaking builds or code-gen that relies on them.
  - [Empty type declaration syntax] AllowNullAttribute is declared using the empty type declaration form (“internal sealed class AllowNullAttribute : Attribute;”). This requires modern C# (C# 13) and is inconsistent with other attributes in the same file that use braces. This is fine given LangVersion=13.0 at the repo level, but is fragile if LangVersion is ever lowered.
  - [Shim coverage completeness] Only a subset of BCL nullability attributes are provided (AllowNull, NotNullWhen, MaybeNullWhen, MemberNotNullWhen, MemberNotNull, NotNullIfNotNull). If the generator starts emitting/consuming other related attributes (e.g., DisallowNull, MaybeNull, NotNull), additional shims may be needed for netstandard2.0 builds.
  - [Consumer availability vs. analyzer assembly] These shims exist inside the analyzer/source generator assembly. They are not automatically available in the consumer’s compilation unless the generator emits the attributes’ source into the user project when missing. Ensure the generator either:
    - Uses BCL attributes when available, or
    - Emits attribute source conditionally into the user compilation when targeting TFMs lacking them.
  - [C# feature usage] The file uses file-scoped namespaces and collection expressions (Members = [member];), which require C# 10 and C# 12 respectively. With LangVersion=13.0 this compiles, but it establishes a high compiler baseline for this project.

- Errors:
  - No errors found in the current context.

Suggestions & Fixes:
- Future-proof conditional compilation:
  - Expand the guard to include other TFMs lacking these attributes if you ever multi-target. For example:
    - Option A (explicit): #if NETSTANDARD2_0 || NETSTANDARD2_1
    - Option B (broader): #if NETSTANDARD2_0 || NETSTANDARD2_1 || NETFRAMEWORK
  - Alternatively, use a positive guard for known presence and default to shims otherwise, but keep clarity in mind.
- Normalize empty type declaration:
  - For consistency and resilience if LangVersion changes, consider changing AllowNullAttribute to a brace-based empty type:
    - internal sealed class AllowNullAttribute : Attribute {}
- Consider completeness:
  - If the generator emits or plans to emit additional nullability attributes (DisallowNull, MaybeNull, NotNull, DoesNotReturn, DoesNotReturnIf, etc.), add matching shims in this file under the same guard.
- Document intent:
  - Add a short XML doc comment at the top of the file explaining that these are conditional shims to support builds targeting TFMs that don’t provide the nullability attributes.
- Validate code-gen paths:
  - Review the code generation to ensure generated sources reference BCL attributes when available, and that for netstandard2.0 consumers you either omit those attributes or inject minimal compatible definitions into the user compilation (similar to AnnotationsSourceGenerator).

References:
- Source file: src/Thinktecture.Runtime.Extensions.SourceGenerator/NullableAttributes.cs
- Project file (TFM: netstandard2.0): src/Thinktecture.Runtime.Extensions.SourceGenerator/Thinktecture.Runtime.Extensions.SourceGenerator.csproj
- Global repo settings (LangVersion=13.0, Nullable enabled): Directory.Build.props
- Src-level props: src/Directory.Build.props
- Related shim: src/Thinktecture.Runtime.Extensions.SourceGenerator/IsExternalInit.cs
- Example of generator emitting attribute source: src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/Annotations/AnnotationsSourceGenerator.cs
- Repo conventions and rules: CLAUDE.md
- EditorConfig in src: src/.editorconfig

Notes:
- AttributeUsage values appear aligned with BCL definitions:
  - NotNullWhenAttribute/MaybeNullWhenAttribute target parameters.
  - MemberNotNull(When) target methods/properties, AllowMultiple=true, Inherited=false.
  - NotNullIfNotNullAttribute targets parameter/property/return, AllowMultiple=true.
- Classes are sealed and internal, aligning with analyzer guidance (e.g., CA1852 severity=warning in src/.editorconfig).
- Using the System.Diagnostics.CodeAnalysis namespace with internal visibility avoids leaking shims into public API while matching the expected names for in-assembly references.
- The current single-targeting (netstandard2.0) means the shims will always be compiled in this project, which is appropriate for now. Revisit if multi-targeting is introduced.

Outcome:
- No blocking issues. The file fulfills its purpose as a conditional shim provider for nullable-related attributes under netstandard2.0. Apply the suggestions above to improve future-proofing and consistency.
