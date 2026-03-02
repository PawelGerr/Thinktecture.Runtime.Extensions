namespace Thinktecture.CodeAnalysis;

public readonly record struct DerivedTypeInfo(
   INamedTypeSymbol Type,
   INamedTypeSymbol TypeDef,
   int Level);
