using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

public readonly record struct SourceGenError(string Message, TypeDeclarationSyntax Node);
