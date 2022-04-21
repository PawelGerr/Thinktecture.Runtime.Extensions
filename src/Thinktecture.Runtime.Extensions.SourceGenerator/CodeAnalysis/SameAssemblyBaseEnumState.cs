using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public sealed class SameAssemblyBaseEnumState<TExtension> : IBaseEnumState<TExtension>, IEquatable<SameAssemblyBaseEnumState<TExtension>>
   where TExtension : IEquatable<TExtension>
{
   private readonly EnumSourceGeneratorStateBase<TExtension> _baseEnumState;

   public bool IsSameAssembly => true;

   public string TypeFullyQualified => _baseEnumState.EnumTypeFullyQualified;
   public string TypeMinimallyQualified => _baseEnumState.EnumTypeMinimallyQualified;

   public string? NullableQuestionMark => _baseEnumState.IsReferenceType ? "?" : null;

   public IReadOnlyList<IMemberState> ConstructorArguments { get; }
   public IReadOnlyList<IMemberState> Items { get; }

   public TExtension Extension { get; }
   public EnumSettings Settings => _baseEnumState.Settings;

   public SameAssemblyBaseEnumState(EnumSourceGeneratorStateBase<TExtension> baseEnumState, INamedTypeSymbol baseType, TExtension extension)
   {
      Extension = extension;
      _baseEnumState = baseEnumState;

      ConstructorArguments = GetConstructorArguments(baseEnumState);
      Items = baseType.EnumerateEnumItems().Select(InstanceMemberInfo.CreateFrom).ToList();
   }

   private static List<IMemberState> GetConstructorArguments(EnumSourceGeneratorStateBase<TExtension> baseEnumState)
   {
      var args = new List<IMemberState> { baseEnumState.KeyProperty };

      foreach (var member in baseEnumState.AssignableInstanceFieldsAndProperties)
      {
         var mappedMemberName = member.Settings.MappedMemberName;

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
      return obj is SameAssemblyBaseEnumState<TExtension> other && Equals(other);
   }

   public bool Equals(IBaseEnumState<TExtension>? obj)
   {
      return obj is SameAssemblyBaseEnumState<TExtension> other && Equals(other);
   }

   public bool Equals(SameAssemblyBaseEnumState<TExtension>? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && _baseEnumState.IsReferenceType.Equals(other._baseEnumState.IsReferenceType)
             && Equals(Extension, other.Extension)
             && Settings.Equals(other.Settings)
             && ConstructorArguments.EqualsTo(other.ConstructorArguments)
             && Items.EqualsTo(other.Items);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ _baseEnumState.IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ (Extension?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ ConstructorArguments.ComputeHashCode();
         hashCode = (hashCode * 397) ^ Items.ComputeHashCode();

         return hashCode;
      }
   }
}
