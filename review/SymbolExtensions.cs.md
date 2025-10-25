Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SymbolExtensions.cs

Errors
- None found.

Warnings
1) ValidateFactoryArguments detection may yield false positives by matching only name and static modifier.
   - Code: member is { IsStatic: true, Name: Constants.Methods.VALIDATE_FACTORY_ARGUMENTS } and IMethodSymbol methodSymbol
   - Risk: Any unrelated static method named "ValidateFactoryArguments" (with arbitrary return type/parameters) would be treated as a match here.
   - Context: This helper returns true and exposes the method via out IMethodSymbol for downstream checks. If additional signature validation is not guaranteed elsewhere, this could misidentify methods.
   - Mitigation: Either (a) validate expected signature here (e.g., return type, parameters, accessibility), or (b) assert that downstream code always performs strict signature checks before relying on this identification.

2) Reliance on project-level global using for Microsoft.CodeAnalysis.
   - The file lacks an explicit using Microsoft.CodeAnalysis; and depends on the csproj&#39;s <Using Include="Microsoft.CodeAnalysis" /> to compile.
   - Impact: If the global using is removed/changed, or this file is moved to a different project without that global using, compilation will fail.
   - Mitigation: Consider adding an explicit using Microsoft.CodeAnalysis; to reduce coupling to project configuration (optional if the global using is a deliberate convention).
