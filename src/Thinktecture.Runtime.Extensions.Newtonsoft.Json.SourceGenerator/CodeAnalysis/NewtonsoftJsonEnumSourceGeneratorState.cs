using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class NewtonsoftJsonEnumSourceGeneratorState : EnumSourceGeneratorStateBase<NewtonsoftJsonBaseEnumExtension>, IEquatable<NewtonsoftJsonEnumSourceGeneratorState>
{
   public bool HasJsonConverterAttribute { get; }

   public NewtonsoftJsonEnumSourceGeneratorState(INamedTypeSymbol type, INamedTypeSymbol enumInterface)
      : base(type, enumInterface)
   {
      HasJsonConverterAttribute = type.HasAttribute("Newtonsoft.Json.JsonConverterAttribute");
   }

   protected override NewtonsoftJsonBaseEnumExtension GetBaseEnumExtension(INamedTypeSymbol baseType)
   {
      return new NewtonsoftJsonBaseEnumExtension(baseType);
   }

   public override bool Equals(object? obj)
   {
      return obj is NewtonsoftJsonEnumSourceGeneratorState other && Equals(other);
   }

   public bool Equals(NewtonsoftJsonEnumSourceGeneratorState? other)
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
