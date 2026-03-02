namespace Thinktecture.CodeAnalysis;

[Flags]
public enum PropertyDeclarationSyntaxKind
{
   Implementation = 1 << 0,
   DeclarationOnly = 1 << 1,

   All = Implementation | DeclarationOnly
}
