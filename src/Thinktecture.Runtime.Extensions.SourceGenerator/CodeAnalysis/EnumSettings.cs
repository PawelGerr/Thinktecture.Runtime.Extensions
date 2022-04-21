using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class EnumSettings : IEquatable<EnumSettings>
{
   public string? KeyComparer { get; }
   public string? KeyPropertyName { get; }
   public bool IsExtensible { get; }

   public EnumSettings(AttributeData? attribute)
   {
      KeyComparer = attribute?.FindKeyComparer().TrimAndNullify();
      KeyPropertyName = attribute?.FindKeyPropertyName().TrimAndNullify();
      IsExtensible = attribute?.IsExtensible() ?? false;
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

      return KeyComparer == other.KeyComparer
             && KeyPropertyName == other.KeyPropertyName
             && IsExtensible == other.IsExtensible;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = KeyComparer?.GetHashCode() ?? 0;
         hashCode = (hashCode * 397) ^ (KeyPropertyName?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ IsExtensible.GetHashCode();

         return hashCode;
      }
   }
}
