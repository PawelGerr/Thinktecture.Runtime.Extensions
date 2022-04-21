using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class JsonEnumSourceGeneratorState : EnumSourceGeneratorStateBase<JsonBaseEnumExtension>, IEquatable<JsonEnumSourceGeneratorState>
{
   public bool HasJsonConverterAttribute { get; }

   public JsonEnumSourceGeneratorState(INamedTypeSymbol type, INamedTypeSymbol enumInterface)
      : base(type, enumInterface)
   {
      HasJsonConverterAttribute = type.HasAttribute("System.Text.Json.Serialization.JsonConverterAttribute");
   }

   protected override JsonBaseEnumExtension GetBaseEnumExtension(INamedTypeSymbol baseType)
   {
      return new JsonBaseEnumExtension(baseType);
   }

   public override bool Equals(object? obj)
   {
      return obj is JsonEnumSourceGeneratorState other && Equals(other);
   }

   public bool Equals(JsonEnumSourceGeneratorState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return base.Equals(other) && HasJsonConverterAttribute == other.HasJsonConverterAttribute;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (base.GetHashCode() * 397) ^ HasJsonConverterAttribute.GetHashCode();
      }
   }
}
