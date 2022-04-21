using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class JsonBaseEnumExtension : IEquatable<JsonBaseEnumExtension>
{
   public bool HasValueObjectJsonConverterFactory { get; }

   public JsonBaseEnumExtension(INamedTypeSymbol baseType)
   {
      HasValueObjectJsonConverterFactory = baseType.GetTypeMembers("ValueObjectJsonConverterFactory").Any();
   }

   public override bool Equals(object? obj)
   {
      return obj is JsonBaseEnumExtension other && Equals(other);
   }

   public bool Equals(JsonBaseEnumExtension? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return HasValueObjectJsonConverterFactory == other.HasValueObjectJsonConverterFactory;
   }

   public override int GetHashCode()
   {
      return HasValueObjectJsonConverterFactory.GetHashCode();
   }
}
