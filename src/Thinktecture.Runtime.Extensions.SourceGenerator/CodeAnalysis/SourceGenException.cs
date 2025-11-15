using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

public readonly record struct SourceGenException(
   string Message,
   Exception Exception,
   TypeDeclarationSyntax Node);
