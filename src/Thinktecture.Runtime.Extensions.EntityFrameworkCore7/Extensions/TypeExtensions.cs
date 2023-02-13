using System.Reflection;
using Thinktecture.Internal;

namespace Thinktecture;

internal static class TypeExtensions
{
   public static bool TryGetAssignableMembers(this Type type, out IReadOnlyList<MemberInfo> members)
   {
      var metadata = ComplexValueObjectMetadataLookup.Find(type);

      if (metadata is null)
      {
         members = Array.Empty<MemberInfo>();
         return false;
      }

      members = metadata.AssignableMembers;
      return true;
   }
}
