using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class EqualityInstanceMemberInfo
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
      EqualityComparer = AdjustEqualityComparer(member.Type, equalityComparer);
      Comparer = comparer;
   }

   private static string? AdjustEqualityComparer(ITypeSymbol memberType, string? equalityComparer)
   {
      if (equalityComparer is null)
         return null;

      if (memberType.IsString())
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

   public void Deconstruct(out InstanceMemberInfo member, out string? equalityComparer, out string? comparer)
   {
      member = Member;
      equalityComparer = EqualityComparer;
      comparer = Comparer;
   }
}
