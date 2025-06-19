using System.Reflection;
using Thinktecture.Internal;

namespace Thinktecture;

internal static class TypeExtensions
{
   public static bool TryGetAssignableMembers(this Type type, out IReadOnlyList<MemberInfo> members)
   {
      var metadata = MetadataLookup.Find(type) as Metadata.ComplexValueObject;

      members = metadata?.AssignableMembers ?? [];
      return metadata is not null;
   }

   public static ConversionMetadata? FindMetadataForValueConverter(this Type type)
   {
      return MetadataLookup.FindMetadataForConversion(
         type,
         f => f.UseWithEntityFramework,
         _ => true);
   }
}
