namespace Thinktecture.CodeAnalysis;

public class EqualityInstanceMemberInfo : IEquatable<EqualityInstanceMemberInfo>
{
   public InstanceMemberInfo Member { get; }
   public string? EqualityComparer { get; }
   public string? Comparer { get; }

   public EqualityInstanceMemberInfo(
      InstanceMemberInfo member,
      string? equalityComparer,
      string? comparer)
   {
      Member = member;
      EqualityComparer = AdjustEqualityComparer(member, equalityComparer);
      Comparer = comparer;
   }

   private static string? AdjustEqualityComparer(InstanceMemberInfo member, string? equalityComparer)
   {
      if (equalityComparer is null)
         return null;

      if (member.IsString())
         return AdjustStringComparer(equalityComparer);

      return equalityComparer;
   }

   private static string AdjustStringComparer(string comparer)
   {
      return comparer switch
      {
         nameof(StringComparer.Ordinal) => "global::System.StringComparer.Ordinal",
         nameof(StringComparer.OrdinalIgnoreCase) => "global::System.StringComparer.OrdinalIgnoreCase",
         nameof(StringComparer.InvariantCulture) => "global::System.StringComparer.InvariantCulture",
         nameof(StringComparer.InvariantCultureIgnoreCase) => "global::System.StringComparer.InvariantCultureIgnoreCase",
         nameof(StringComparer.CurrentCulture) => "global::System.StringComparer.CurrentCulture",
         nameof(StringComparer.CurrentCultureIgnoreCase) => "global::System.StringComparer.CurrentCultureIgnoreCase",
         _ => comparer
      };
   }

   public void Deconstruct(out InstanceMemberInfo member, out string? equalityComparer)
   {
      member = Member;
      equalityComparer = EqualityComparer;
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
             && EqualityComparer == other.EqualityComparer
             && Comparer == other.Comparer;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Member.GetHashCode();
         hashCode = (hashCode * 397) ^ (EqualityComparer?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ (Comparer?.GetHashCode() ?? 0);

         return hashCode;
      }
   }
}
