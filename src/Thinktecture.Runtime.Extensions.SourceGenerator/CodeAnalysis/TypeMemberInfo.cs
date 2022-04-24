using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public readonly struct TypeMemberInfo : IEquatable<TypeMemberInfo>
{
   public bool HasJsonConverterFactory { get; }
   public bool HasNewtonsoftJsonConverter { get; }
   public bool HasMessagePackFormatter { get; }

   public TypeMemberInfo(INamedTypeSymbol type)
   {
      HasJsonConverterFactory = default;
      HasNewtonsoftJsonConverter = default;
      HasMessagePackFormatter = default;

      foreach (var innerType in type.GetTypeMembers())
      {
         if (innerType.Name == "ValueObjectJsonConverterFactory")
         {
            HasJsonConverterFactory = true;
         }
         else if (innerType.Name == "ValueObjectNewtonsoftJsonConverter")
         {
            HasNewtonsoftJsonConverter = true;
         }
         else if (innerType.Name == "ValueObjectMessagePackFormatter")
         {
            HasMessagePackFormatter = true;
         }
      }
   }

   public override bool Equals(object? obj)
   {
      return obj is TypeMemberInfo other && Equals(other);
   }

   public bool Equals(TypeMemberInfo other)
   {
      return HasJsonConverterFactory == other.HasJsonConverterFactory
             && HasNewtonsoftJsonConverter == other.HasNewtonsoftJsonConverter
             && HasMessagePackFormatter == other.HasMessagePackFormatter;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = HasJsonConverterFactory.GetHashCode();
         hashCode = (hashCode * 397) ^ HasNewtonsoftJsonConverter.GetHashCode();
         hashCode = (hashCode * 397) ^ HasMessagePackFormatter.GetHashCode();

         return hashCode;
      }
   }
}
