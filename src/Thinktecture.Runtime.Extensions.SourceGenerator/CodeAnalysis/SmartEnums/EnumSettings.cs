using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class EnumSettings : IEquatable<EnumSettings>
{
   public string? KeyEqualityComparer { get; }
   public string? KeyPropertyName { get; }

   public EnumSettings(AttributeData? attribute)
   {
      KeyEqualityComparer = attribute?.FindKeyEqualityComparer().TrimAndNullify();
      KeyPropertyName = attribute?.FindKeyPropertyName().TrimAndNullify();
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(EnumSettings? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return KeyEqualityComparer == other.KeyEqualityComparer
             && KeyPropertyName == other.KeyPropertyName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = KeyEqualityComparer?.GetHashCode() ?? 0;
         hashCode = (hashCode * 397) ^ (KeyPropertyName?.GetHashCode() ?? 0);

         return hashCode;
      }
   }
}
