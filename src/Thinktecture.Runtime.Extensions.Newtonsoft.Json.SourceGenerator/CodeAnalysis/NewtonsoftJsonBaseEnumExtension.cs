using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class NewtonsoftJsonBaseEnumExtension : IEquatable<NewtonsoftJsonBaseEnumExtension>
{
   public bool HasValueObjectNewtonsoftJsonConverter { get; }

   public NewtonsoftJsonBaseEnumExtension(INamedTypeSymbol baseType)
   {
      HasValueObjectNewtonsoftJsonConverter = baseType.GetTypeMembers("ValueObjectNewtonsoftJsonConverter").Any();
   }

   public override bool Equals(object? obj)
   {
      return obj is NewtonsoftJsonBaseEnumExtension other && Equals(other);
   }

   public bool Equals(NewtonsoftJsonBaseEnumExtension? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return HasValueObjectNewtonsoftJsonConverter == other.HasValueObjectNewtonsoftJsonConverter;
   }

   public override int GetHashCode()
   {
      return HasValueObjectNewtonsoftJsonConverter.GetHashCode();
   }
}
