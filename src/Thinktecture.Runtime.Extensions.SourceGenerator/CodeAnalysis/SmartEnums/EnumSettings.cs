using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class EnumSettings : IEquatable<EnumSettings>
{
   public string? KeyPropertyName { get; }
   public bool SkipToString { get; }

   public EnumSettings(AttributeData? attribute)
   {
      KeyPropertyName = attribute?.FindKeyPropertyName().TrimAndNullify();
      SkipToString = attribute?.FindSkipToString() ?? false;
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

      return KeyPropertyName == other.KeyPropertyName
             && SkipToString == other.SkipToString;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = KeyPropertyName?.GetHashCode() ?? 0;
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();

         return hashCode;
      }
   }
}
