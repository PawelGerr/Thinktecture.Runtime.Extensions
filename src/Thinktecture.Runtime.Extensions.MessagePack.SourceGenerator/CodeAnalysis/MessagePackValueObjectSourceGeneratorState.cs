using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class MessagePackValueObjectSourceGeneratorState : ValueObjectSourceGeneratorState, IEquatable<MessagePackValueObjectSourceGeneratorState>
{
   public bool HasMessagePackFormatterAttribute { get; }

   public MessagePackValueObjectSourceGeneratorState(INamedTypeSymbol type, AttributeData valueObjectAttribute)
      : base(type, valueObjectAttribute)
   {
      HasMessagePackFormatterAttribute = type.HasAttribute("MessagePack.MessagePackFormatterAttribute");
   }

   public override bool Equals(object? obj)
   {
      return obj is MessagePackValueObjectSourceGeneratorState other && Equals(other);
   }

   public bool Equals(MessagePackValueObjectSourceGeneratorState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return base.Equals(other) && HasMessagePackFormatterAttribute == other.HasMessagePackFormatterAttribute;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (base.GetHashCode() * 397) ^ HasMessagePackFormatterAttribute.GetHashCode();
      }
   }
}
