using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public sealed class ObjectFactoryState : ITypeFullyQualified, IEquatable<ObjectFactoryState>, IEquatable<ITypeFullyQualified>, IHashCodeComputable
{
   public SpecialType SpecialType { get; }
   public string TypeFullyQualified { get; }
   public SerializationFrameworks UseForSerialization { get; }
   public bool UseWithEntityFramework { get; }
   public bool UseForModelBinding { get; }

   public ObjectFactoryState(
      ITypeSymbol type,
      SerializationFrameworks useForSerialization,
      bool useWithEntityFramework,
      bool useForModelBinding)
   {
      SpecialType = type.SpecialType;
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      UseForSerialization = useForSerialization;
      UseWithEntityFramework = useWithEntityFramework;
      UseForModelBinding = useForModelBinding;
   }

   public override bool Equals(object? obj)
   {
      return Equals(obj as ObjectFactoryState);
   }

   public bool Equals(ObjectFactoryState? other)
   {
      return Equals((ITypeFullyQualified?)other)
             && (int)UseForSerialization == (int)other.UseForSerialization
             && UseWithEntityFramework == other.UseWithEntityFramework
             && UseForModelBinding == other.UseForModelBinding;
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
         hashCode = (hashCode * 397) ^ UseWithEntityFramework.GetHashCode();
         hashCode = (hashCode * 397) ^ UseForModelBinding.GetHashCode();

         return hashCode;
      }
   }
}
