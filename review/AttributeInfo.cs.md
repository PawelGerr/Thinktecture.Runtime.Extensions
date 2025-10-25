- Warning: Potential reliance on implicit/global usings. The file uses types and extensions without explicit using directives:
  - INamedTypeSymbol, AttributeData, TypeKind (Microsoft.CodeAnalysis)
  - ImmutableArray<T> (System.Collections.Immutable)
  - SequenceEqual (System.Linq)
  - IEquatable<T> (System)
  If implicit or global usings are not configured, this will not compile. Mitigation: add explicit usings (e.g., using Microsoft.CodeAnalysis; using System.Collections.Immutable; using System.Linq; using System;).

- Warning: Unsafe indexing into generic type arguments. Multiple branches access attribute.AttributeClass.TypeArguments[0] without verifying arity. If an attribute is malformed/misapplied (no generic type argument), this will throw. Mitigation: guard via TypeArguments.Length >= 1 (or ensure the Is*Attribute() extension methods already guarantee arity and document that contract).
