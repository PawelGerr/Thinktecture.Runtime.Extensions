using System;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   public class EqualityInstanceMemberInfo
   {
      public InstanceMemberInfo Member { get; }
      public string? EqualityComparer { get; }

      public EqualityInstanceMemberInfo(
         InstanceMemberInfo member,
         string? equalityComparer)
      {
         Member = member;
         EqualityComparer = AdjustEqualityComparer(member.Type, equalityComparer);
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
            nameof(StringComparer.Ordinal) => "StringComparer.Ordinal",
            nameof(StringComparer.OrdinalIgnoreCase) => "StringComparer.OrdinalIgnoreCase",
            nameof(StringComparer.InvariantCulture) => "StringComparer.InvariantCulture",
            nameof(StringComparer.InvariantCultureIgnoreCase) => "StringComparer.InvariantCultureIgnoreCase",
            nameof(StringComparer.CurrentCulture) => "StringComparer.CurrentCulture",
            nameof(StringComparer.CurrentCultureIgnoreCase) => "StringComparer.CurrentCultureIgnoreCase",
            _ => comparer
         };
      }

      public void Deconstruct(out InstanceMemberInfo member, out string? equalityComparer)
      {
         member = Member;
         equalityComparer = EqualityComparer;
      }
   }
}
