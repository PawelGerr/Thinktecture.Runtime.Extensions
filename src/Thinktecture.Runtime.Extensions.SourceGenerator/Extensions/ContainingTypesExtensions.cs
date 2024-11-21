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
      var sbLength = sb.Length;

      for (var i = skipRootContainingType ? 1 : 0; i < containingTypes.Count; i++)
      {
         var containingType = containingTypes[i];
         sb.Append(containingType.Name);
      }

      sb.Append(typeMemberName);

      var firstChar = sb[sbLength];

      sb[sbLength] = firstChar == '_'
                        ? Char.ToLowerInvariant(sb[sbLength + 1])
                        : Char.ToLowerInvariant(firstChar);

      var argName = sb.ToString(sbLength, sb.Length - sbLength);
      sb.Length = sbLength;

      return argName;
   }
}
