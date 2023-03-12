using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

public record struct SourceGenError(string Message, TypeDeclarationSyntax Node);
