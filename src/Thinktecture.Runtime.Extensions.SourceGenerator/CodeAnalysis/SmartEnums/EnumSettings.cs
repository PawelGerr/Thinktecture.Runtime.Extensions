using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class EnumSettings : IEquatable<EnumSettings>
{
   public string? KeyPropertyName { get; }

   public EnumSettings(AttributeData? attribute)
   {
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

      return KeyPropertyName == other.KeyPropertyName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return KeyPropertyName?.GetHashCode() ?? 0;
      }
   }
}
