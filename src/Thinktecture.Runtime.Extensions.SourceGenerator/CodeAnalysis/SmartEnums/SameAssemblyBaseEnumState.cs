using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SameAssemblyBaseEnumState : IBaseEnumState, IEquatable<SameAssemblyBaseEnumState>
{
   private readonly INamedTypeSymbol _type;

   public bool IsSameAssembly => true;

   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }

   public string? NullableQuestionMark => _type.IsReferenceType ? "?" : null;

   public IReadOnlyList<IMemberState> ConstructorArguments { get; }
   public IReadOnlyList<IMemberState> Items { get; }

   public EnumSettings Settings { get; }
   public TypeMemberInfo InnerTypesInfo { get; }

   public SameAssemblyBaseEnumState(INamedTypeSymbol type, ITypeSymbol keyType)
   {
      _type = type;

      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      Settings = new EnumSettings(type.FindEnumGenerationAttribute());
      InnerTypesInfo = new TypeMemberInfo(type);

      ConstructorArguments = GetConstructorArguments(type, Settings.CreateKeyProperty(keyType));
      Items = type.EnumerateEnumItems().Select(InstanceMemberInfo.CreateFrom).ToList();
   }

   private static List<IMemberState> GetConstructorArguments(INamedTypeSymbol type, IMemberState keyProperty)
   {
      var args = new List<IMemberState> { keyProperty };
      var assignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true);

      foreach (var member in assignableInstanceFieldsAndProperties)
      {
         var mappedMemberName = member.EnumMemberSettings.MappedMemberName;

         if (mappedMemberName is not null)
         {
            args.Add(member.CreateSymbolState(mappedMemberName, member.IsStatic));
         }
         else
         {
            args.Add(member);
         }
      }

      return args;
   }

   public override bool Equals(object? obj)
   {
      return obj is SameAssemblyBaseEnumState other && Equals(other);
   }

   public bool Equals(IBaseEnumState? obj)
   {
      return obj is SameAssemblyBaseEnumState other && Equals(other);
   }

   public bool Equals(SameAssemblyBaseEnumState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && _type.IsReferenceType.Equals(other._type.IsReferenceType)
             && InnerTypesInfo.Equals(other.InnerTypesInfo)
             && Settings.Equals(other.Settings)
             && ConstructorArguments.EqualsTo(other.ConstructorArguments)
             && Items.EqualsTo(other.Items);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ _type.IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ InnerTypesInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ ConstructorArguments.ComputeHashCode();
         hashCode = (hashCode * 397) ^ Items.ComputeHashCode();

         return hashCode;
      }
   }
}
