namespace Thinktecture.CodeAnalysis;

public sealed class EqualityInstanceMemberInfo : IEquatable<EqualityInstanceMemberInfo>, IHashCodeComputable
{
   public InstanceMemberInfo Member { get; }
   public string? EqualityComparerAccessor { get; }

   public EqualityInstanceMemberInfo(
      InstanceMemberInfo member,
      string? equalityComparerAccessor)
   {
      Member = member;
      EqualityComparerAccessor = equalityComparerAccessor;
   }

   public void Deconstruct(out InstanceMemberInfo member, out string? equalityComparer)
   {
      member = Member;
      equalityComparer = EqualityComparerAccessor;
   }

   public override bool Equals(object? obj)
   {
      return obj is EqualityInstanceMemberInfo other && Equals(other);
   }

   public bool Equals(EqualityInstanceMemberInfo? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return Member.Equals(other.Member)
             && EqualityComparerAccessor == other.EqualityComparerAccessor;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Member.GetHashCode();
         hashCode = (hashCode * 397) ^ (EqualityComparerAccessor?.GetHashCode() ?? 0);

         return hashCode;
      }
   }
}
