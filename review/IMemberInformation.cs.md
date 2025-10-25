IMemberInformation.cs – Issues

Errors:
- Unqualified type reference: SpecialType is used without namespace qualification or a using directive. Unless there is a project-wide/global using for Microsoft.CodeAnalysis, this will not compile.
  Suggested fix: add `using Microsoft.CodeAnalysis;` or fully qualify the type as `Microsoft.CodeAnalysis.SpecialType`.

Warnings:
- Interface visibility: The interface is public and references Roslyn types (Microsoft.CodeAnalysis). If this abstraction is only used internally within the source generator, consider making it `internal` to avoid leaking Roslyn types in the public surface and to reduce API surface area.
