using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public sealed class DesiredFactory : ITypeFullyQualified, IEquatable<DesiredFactory>, IEquatable<ITypeFullyQualified>, IHashCodeComputable
{
   public SpecialType SpecialType { get; }
   public string TypeFullyQualified { get; }
   public SerializationFrameworks UseForSerialization { get; }

   public DesiredFactory(ITypeSymbol type, SerializationFrameworks useForSerialization)
   {
      SpecialType = type.SpecialType;
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      UseForSerialization = useForSerialization;
   }

   public override bool Equals(object? obj)
   {
      return Equals(obj as DesiredFactory);
   }

   public bool Equals(DesiredFactory? other)
   {
      return Equals((ITypeFullyQualified?)other)
             && (int)UseForSerialization == (int)other.UseForSerialization;
   }

   public bool Equals([NotNullWhen(true)] ITypeFullyQualified? other)
   {
      if (ReferenceEquals(null, other))
         return false;

      return TypeFullyQualified == other.TypeFullyQualified;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)UseForSerialization;

         return hashCode;
      }
   }
}
