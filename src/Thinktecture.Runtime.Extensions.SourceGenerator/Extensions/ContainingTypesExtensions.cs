using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ContainingTypesExtensions
{
   public static string MakeFullyQualifiedArgumentName(
      this ImmutableArray<ContainingTypeState> containingTypes,
      string typeMemberName,
      int skipLevels,
      StringBuilder sb)
   {
      var originalLen = sb.Length;

      for (var i = skipLevels; i < containingTypes.Length; i++)
      {
         sb.Append(containingTypes[i].Name);
      }

      sb.Append(typeMemberName);

      // Nothing appended -> return empty string.
      if (sb.Length == originalLen)
         return string.Empty;

      var len = sb.Length;

      for (var i = originalLen; i < len; i++)
      {
         var ch = sb[i];

         if (!char.IsLetter(ch))
            continue;

         if (char.IsUpper(ch))
         {
            // Lowercase with look-ahead to preserve the last uppercase before a lowercase (acronyms).
            // Find end of the initial consecutive uppercase run.
            var j = i;
            while (j < len && char.IsUpper(sb[j]))
               j++;

            var nextIsLower = j < len && char.IsLower(sb[j]);

            // Lowercase the first letter
            sb[i] = char.ToLowerInvariant(ch);

            // If the run has more than one char, lowercase the middle portion according to the rule.
            if (j - i > 1)
            {
               var endExclusive = nextIsLower ? j - 1 : j;

               for (var k = i + 1; k < endExclusive; k++)
                  sb[k] = char.ToLowerInvariant(sb[k]);
            }
         }

         // Either we handled the uppercase run or the first letter was already lowercase.
         break;
      }

      var result = sb.ToString(originalLen, len - originalLen);
      sb.Length = originalLen;

      return result;
   }
}
