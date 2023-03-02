namespace Thinktecture.CodeAnalysis;

public sealed class EqualityInstanceMemberInfo : IEquatable<EqualityInstanceMemberInfo>
{
   public InstanceMemberInfo Member { get; }
   public string? EqualityComparerAccessor { get; }
   public string? ComparerAccessor { get; }

   public EqualityInstanceMemberInfo(
      InstanceMemberInfo member,
      string? equalityComparerAccessor,
      string? comparerAccessor)
   {
      Member = member;
      EqualityComparerAccessor = equalityComparerAccessor;
      ComparerAccessor = comparerAccessor;
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
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return Member.Equals(other.Member)
             && EqualityComparerAccessor == other.EqualityComparerAccessor
             && ComparerAccessor == other.ComparerAccessor;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Member.GetHashCode();
         hashCode = (hashCode * 397) ^ (EqualityComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ (ComparerAccessor?.GetHashCode() ?? 0);

         return hashCode;
      }
   }
}
