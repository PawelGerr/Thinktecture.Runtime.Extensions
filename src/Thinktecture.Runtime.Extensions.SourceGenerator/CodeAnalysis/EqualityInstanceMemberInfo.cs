using System;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   public class EqualityInstanceMemberInfo
   {
      public InstanceMemberInfo Member { get; }
      public string? Comparer { get; }

      public EqualityInstanceMemberInfo(
         InstanceMemberInfo member,
         string? comparer)
      {
         Member = member;
         Comparer = AdjustComparer(member.Type, comparer);
      }

      private static string? AdjustComparer(ITypeSymbol memberType, string? comparer)
      {
         if (comparer is null)
            return null;

         if (memberType.IsString())
            return AdjustStringComparer(comparer);

         return comparer;
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

      public void Deconstruct(out InstanceMemberInfo member, out string? comparer)
      {
         member = Member;
         comparer = Comparer;
      }
   }
}
