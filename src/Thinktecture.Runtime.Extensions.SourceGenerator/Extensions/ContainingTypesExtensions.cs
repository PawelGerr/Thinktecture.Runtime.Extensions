using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ContainingTypesExtensions
{
   public static string MakeFullyQualifiedArgumentName(
      this IReadOnlyList<ContainingTypeState> containingTypes,
      string typeMemberName,
      bool skipRootContainingType,
      StringBuilder sb)
   {
      var originalLen = sb.Length;

      for (var i = skipRootContainingType ? 1 : 0; i < containingTypes.Count; i++)
         sb.Append(containingTypes[i].Name);

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
            sb[i] = char.ToLowerInvariant(ch);

            // Lowercase the rest of the initial consecutive uppercase run.
            for (var j = i + 1; j < len && char.IsUpper(sb[j]); j++)
            {
               sb[j] = char.ToLowerInvariant(sb[j]);
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
