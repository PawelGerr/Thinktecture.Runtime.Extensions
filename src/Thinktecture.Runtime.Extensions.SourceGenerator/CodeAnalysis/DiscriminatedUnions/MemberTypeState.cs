namespace Thinktecture.CodeAnalysis.DiscriminatedUnions;

public sealed class MemberTypeState : IEquatable<MemberTypeState>, IMemberInformation, ITypeMinimallyQualified, IHashCodeComputable
{
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public string Name { get; }
   public bool IsReferenceType { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }
   public SpecialType SpecialType { get; }
   public bool IsInterface { get; }

   public string ArgumentName { get; }
   public MemberTypeSetting Setting { get; }

   public MemberTypeState(
      string typeName,
      ITypedMemberState typeState,
      MemberTypeSetting setting)
   {
      Name = setting.Name ?? typeName;
      TypeFullyQualified = typeState.TypeFullyQualified;
      TypeMinimallyQualified = typeState.TypeMinimallyQualified;
      IsReferenceType = typeState.IsReferenceType;
      NullableAnnotation = typeState.NullableAnnotation;
      IsNullableStruct = typeState.IsNullableStruct;
      SpecialType = typeState.SpecialType;
      IsInterface = typeState.TypeKind == TypeKind.Interface;

      ArgumentName = Name.MakeArgumentName();
      Setting = setting;
   }

   public static string GetMemberTypeName(INamedTypeSymbol type, ITypedMemberState typeState)
   {
      return typeState.IsNullableStruct ? $"Nullable{type.TypeArguments[0].Name}" : type.Name;
   }

   public static string GetMemberTypeName(IArrayTypeSymbol type)
   {
      return type.ElementType.Name + "Array";
   }

   public override bool Equals(object? obj)
   {
      return obj is MemberTypeState other && Equals(other);
   }

   public bool Equals(MemberTypeState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && SpecialType == other.SpecialType
             && IsInterface == other.IsInterface
             && Setting.Equals(other.Setting);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)SpecialType;
         hashCode = (hashCode * 397) ^ IsInterface.GetHashCode();
         hashCode = (hashCode * 397) ^ Setting.GetHashCode();

         return hashCode;
      }
   }
}
