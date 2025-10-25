namespace Thinktecture.CodeAnalysis;

public readonly struct ValidationErrorState
   : ITypeFullyQualified, IEquatable<ValidationErrorState>, IHashCodeComputable
{
   public static readonly ValidationErrorState Default = new("global::Thinktecture.ValidationError");

   public string TypeFullyQualified { get; }

   public ValidationErrorState(string typeFullyQualified)
   {
      TypeFullyQualified = typeFullyQualified;
   }

   public override bool Equals(object? obj)
   {
      return obj is ValidationErrorState state && Equals(state);
   }

   public bool Equals(ValidationErrorState other)
   {
      return TypeFullyQualified == other.TypeFullyQualified;
   }

   public override int GetHashCode()
   {
      return TypeFullyQualified.GetHashCode();
   }

   public override string ToString()
   {
      return TypeFullyQualified;
   }

   public static bool operator ==(ValidationErrorState left, ValidationErrorState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(ValidationErrorState left, ValidationErrorState right)
   {
      return !(left == right);
   }
}
