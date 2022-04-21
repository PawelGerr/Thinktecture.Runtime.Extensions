using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class MessagePackEnumSourceGeneratorState : EnumSourceGeneratorStateBase<MessagePackBaseEnumExtension>, IEquatable<MessagePackEnumSourceGeneratorState>
{
   public bool HasMessagePackFormatterAttribute { get; }

   public MessagePackEnumSourceGeneratorState(INamedTypeSymbol type, INamedTypeSymbol enumInterface)
      : base(type, enumInterface)
   {
      HasMessagePackFormatterAttribute = type.HasAttribute("MessagePack.MessagePackFormatterAttribute");
   }

   protected override MessagePackBaseEnumExtension GetBaseEnumExtension(INamedTypeSymbol baseType)
   {
      return new MessagePackBaseEnumExtension(baseType);
   }

   public override bool Equals(object? obj)
   {
      return obj is MessagePackEnumSourceGeneratorState other && Equals(other);
   }

   public bool Equals(MessagePackEnumSourceGeneratorState? other)
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
