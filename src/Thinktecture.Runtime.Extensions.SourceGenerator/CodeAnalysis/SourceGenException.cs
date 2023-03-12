using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

public record struct SourceGenException(Exception Exception, TypeDeclarationSyntax Node);
