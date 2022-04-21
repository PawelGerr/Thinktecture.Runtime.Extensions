using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class JsonValueObjectSourceGeneratorState : ValueObjectSourceGeneratorState, IEquatable<JsonValueObjectSourceGeneratorState>
{
   public bool HasJsonConverterAttribute { get; }

   public JsonValueObjectSourceGeneratorState(INamedTypeSymbol type, AttributeData valueObjectAttribute)
      : base(type, valueObjectAttribute)
   {
      HasJsonConverterAttribute = type.HasAttribute("System.Text.Json.Serialization.JsonConverterAttribute");
   }

   public override bool Equals(object? obj)
   {
      return obj is JsonValueObjectSourceGeneratorState other && Equals(other);
   }

   public bool Equals(JsonValueObjectSourceGeneratorState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return base.Equals(other)
             && HasJsonConverterAttribute == other.HasJsonConverterAttribute;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (base.GetHashCode() * 397) ^ HasJsonConverterAttribute.GetHashCode();
      }
   }
}
