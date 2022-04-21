using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class NewtonsoftJsonValueObjectSourceGeneratorState : ValueObjectSourceGeneratorState, IEquatable<NewtonsoftJsonValueObjectSourceGeneratorState>
{
   public bool HasJsonConverterAttribute { get; }

   public NewtonsoftJsonValueObjectSourceGeneratorState(INamedTypeSymbol type, AttributeData valueObjectAttribute)
      : base(type, valueObjectAttribute)
   {
      HasJsonConverterAttribute = type.HasAttribute("Newtonsoft.Json.JsonConverterAttribute");
   }

   public override bool Equals(object? obj)
   {
      return obj is NewtonsoftJsonValueObjectSourceGeneratorState other && Equals(other);
   }

   public bool Equals(NewtonsoftJsonValueObjectSourceGeneratorState? other)
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
