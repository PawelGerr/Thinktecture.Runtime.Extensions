- Errors
  - Missing namespace imports for Roslyn types. The file uses IPropertySymbol and SyntaxToken but only imports Microsoft.CodeAnalysis.CSharp.Syntax. Unless there is a project-wide global using for Microsoft.CodeAnalysis, this will not compile. Add:
    - using Microsoft.CodeAnalysis;
  - Reliance on implicit usings for CancellationToken. If ImplicitUsings is disabled or changed, the signature won&#39;t compile. Be explicit:
    - using System.Threading;

- Warnings
  - Assumes the first DeclaringSyntaxReference is a PropertyDeclarationSyntax. While properties typically have a single syntax reference, being defensive can prevent future issues. Prefer iterating all references and returning the first PropertyDeclarationSyntax:
    ```
    foreach (var sr in property.DeclaringSyntaxReferences)
    {
        if (sr.GetSyntax(cancellationToken) is PropertyDeclarationSyntax p)
            return p.Identifier;
    }
    return null;
    ```
  - Edge cases not handled: For properties originating from non-PropertyDeclarationSyntax nodes (e.g., record positional parameters producing synthesized properties), this method will always return null. If callers expect identifiers for such cases, consider also handling ParameterSyntax (returning its Identifier) or documenting the limitation.
