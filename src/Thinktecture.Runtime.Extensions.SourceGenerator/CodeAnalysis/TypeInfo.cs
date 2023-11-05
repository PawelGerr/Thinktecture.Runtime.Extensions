using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public sealed class TypeInfo : ITypeFullyQualified, IEquatable<TypeInfo>, IEquatable<ITypeFullyQualified>
{
   public SpecialType SpecialType { get; }
   public string TypeFullyQualified { get; }

   public TypeInfo(ITypeSymbol type)
   {
      SpecialType = type.SpecialType;
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
   }

   public override bool Equals(object? obj)
   {
      return Equals(obj as TypeInfo);
   }

   public bool Equals(TypeInfo? other)
   {
      return Equals((ITypeFullyQualified?)other);
   }

   public bool Equals(ITypeFullyQualified? other)
   {
      if (ReferenceEquals(null, other))
         return false;

      return TypeFullyQualified == other.TypeFullyQualified;
   }

   public override int GetHashCode()
   {
      return TypeFullyQualified.GetHashCode();
   }
}
