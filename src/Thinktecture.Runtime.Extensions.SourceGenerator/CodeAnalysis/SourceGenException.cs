using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

public readonly record struct SourceGenException(Exception Exception, TypeDeclarationSyntax Node);
