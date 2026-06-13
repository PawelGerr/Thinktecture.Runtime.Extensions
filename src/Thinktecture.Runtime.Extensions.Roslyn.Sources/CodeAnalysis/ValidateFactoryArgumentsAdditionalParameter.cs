namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Represents an additional trailing parameter declared by the user on the
/// <c>ValidateFactoryArguments</c> partial method of a value object. The source generator reproduces
/// these parameters on the defining declaration it emits, synthesizing the default value on its own
/// (<c>= null</c> for reference types, <c>= default</c> for value types).
/// </summary>
public readonly struct ValidateFactoryArgumentsAdditionalParameter(
   string name,
   string typeFullyQualifiedWithNullability,
   bool isReferenceType)
   : IEquatable<ValidateFactoryArgumentsAdditionalParameter>, IHashCodeComputable
{
   /// <summary>
   /// The raw identifier of the parameter (must be escaped with <c>@</c> at emit time).
   /// </summary>
   public string Name { get; } = name;

   /// <summary>
   /// The fully qualified type (<c>global::</c>-prefixed) with the nullable annotation baked in.
   /// Reference types are always emitted as nullable so the synthesized <c>= null</c> default is valid.
   /// </summary>
   public string TypeFullyQualifiedWithNullability { get; } = typeFullyQualifiedWithNullability;

   /// <summary>
   /// Selects the synthesized default at emit time: <c>= null</c> for reference types,
   /// <c>= default</c> for value types (including <c>Nullable&lt;T&gt;</c>).
   /// </summary>
   public bool IsReferenceType { get; } = isReferenceType;

   public override bool Equals(object? obj)
   {
      return obj is ValidateFactoryArgumentsAdditionalParameter other && Equals(other);
   }

   public bool Equals(ValidateFactoryArgumentsAdditionalParameter other)
   {
      return Name == other.Name
             && TypeFullyQualifiedWithNullability == other.TypeFullyQualifiedWithNullability
             && IsReferenceType == other.IsReferenceType;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Name.GetHashCode();
         hashCode = (hashCode * 397) ^ TypeFullyQualifiedWithNullability.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         return hashCode;
      }
   }

   public static bool operator ==(ValidateFactoryArgumentsAdditionalParameter left, ValidateFactoryArgumentsAdditionalParameter right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(ValidateFactoryArgumentsAdditionalParameter left, ValidateFactoryArgumentsAdditionalParameter right)
   {
      return !(left == right);
   }
}
