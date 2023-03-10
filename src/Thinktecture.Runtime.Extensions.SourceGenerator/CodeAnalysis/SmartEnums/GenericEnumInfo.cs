using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public class GenericEnumInfo : IEquatable<GenericEnumInfo>
{
   public string? Namespace { get; }
   public string EnumTypeName { get; }
   public string EnumTypeFullyQualified { get; }
   public string GenericEnumTypeFullyQualified { get; }

   public GenericEnumInfo(INamedTypeSymbol enumType, INamedTypeSymbol genericEnumType)
   {
      Namespace = enumType.ContainingNamespace?.IsGlobalNamespace == true ? null : enumType.ContainingNamespace?.ToString();
      EnumTypeName = enumType.Name;
      EnumTypeFullyQualified = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      GenericEnumTypeFullyQualified = genericEnumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
   }

   public bool Equals(GenericEnumInfo? other)
   {
      if (other is null)
         return false;

      return EnumTypeFullyQualified.Equals(other.EnumTypeFullyQualified)
             && GenericEnumTypeFullyQualified.Equals(other.GenericEnumTypeFullyQualified);
   }

   public override bool Equals(object? obj)
   {
      return obj is GenericEnumInfo other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (EnumTypeFullyQualified.GetHashCode() * 397) ^ GenericEnumTypeFullyQualified.GetHashCode();
      }
   }
}
