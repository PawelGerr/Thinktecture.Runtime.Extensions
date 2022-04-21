using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class MessagePackBaseEnumExtension : IEquatable<MessagePackBaseEnumExtension>
{
   public bool HasValueObjectMessagePackFormatter { get; }

   public MessagePackBaseEnumExtension(INamedTypeSymbol baseType)
   {
      HasValueObjectMessagePackFormatter = baseType.GetTypeMembers("ValueObjectMessagePackFormatter").Any();
   }

   public override bool Equals(object? obj)
   {
      return obj is MessagePackBaseEnumExtension other && Equals(other);
   }

   public bool Equals(MessagePackBaseEnumExtension? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return HasValueObjectMessagePackFormatter == other.HasValueObjectMessagePackFormatter;
   }

   public override int GetHashCode()
   {
      return HasValueObjectMessagePackFormatter.GetHashCode();
   }
}
