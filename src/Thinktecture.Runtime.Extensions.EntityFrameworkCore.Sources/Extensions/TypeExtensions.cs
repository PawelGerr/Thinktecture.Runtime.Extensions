using System.Reflection;
using Thinktecture.Internal;

namespace Thinktecture;

internal static class TypeExtensions
{
   public static bool TryGetAssignableMembers(this Type type, out IReadOnlyList<MemberInfo> members)
   {
      var metadata = ComplexValueObjectMetadataLookup.Find(type);

      members = metadata?.AssignableMembers ?? Array.Empty<MemberInfo>();
      return metadata is not null;
   }
}
