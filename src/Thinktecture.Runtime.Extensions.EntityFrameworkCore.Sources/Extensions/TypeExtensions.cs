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
         // ReadOnlySpan<char>-based object factories are excluded because a ref struct cannot be used as the
         // generic provider-value argument of ThinktectureValueConverter, which mirrors the source generator and
         // lets the conversion fall back to the key-based metadata.
         f => f.ValueType != typeof(ReadOnlySpan<char>) && f.UseWithEntityFramework,
         _ => true);
   }
}
