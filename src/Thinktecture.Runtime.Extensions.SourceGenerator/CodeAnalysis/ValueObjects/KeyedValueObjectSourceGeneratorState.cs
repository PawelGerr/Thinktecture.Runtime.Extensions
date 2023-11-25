namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class KeyedValueObjectSourceGeneratorState : ITypeInformation, IEquatable<KeyedValueObjectSourceGeneratorState>
{
   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated => IsReferenceType ? TypeFullyQualifiedNullable : TypeFullyQualified;
   public string TypeMinimallyQualified { get; }
   public bool IsEqualWithReferenceEquality => false;

   public string? Namespace { get; }
   public string Name { get; }
   public bool IsReferenceType { get; }

   public string? FactoryValidationReturnType { get; }

   public KeyMemberState KeyMember { get; }
   public ValidationErrorState ValidationError { get; }
   public ValueObjectSettings Settings { get; }

   public KeyedValueObjectSourceGeneratorState(
      INamedTypeSymbol type,
      KeyMemberState keyMember,
      ValidationErrorState validationError,
      ValueObjectSettings settings)
   {
      KeyMember = keyMember;
      ValidationError = validationError;
      Settings = settings;
      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullable = $"{TypeFullyQualified}?";
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;

      var nonIgnoredMembers = type.GetNonIgnoredMembers();

      for (var i = 0; i < nonIgnoredMembers.Length; i++)
      {
         var member = nonIgnoredMembers[i];

         if (member.IsValidateFactoryArgumentsImplementation(out var method) && method.ReturnType.SpecialType != SpecialType.System_Void)
         {
            FactoryValidationReturnType = method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            if (method.ReturnType.NullableAnnotation == NullableAnnotation.Annotated)
               FactoryValidationReturnType += "?";
         }
      }
   }

   public override bool Equals(object? obj)
   {
      return obj is KeyedValueObjectSourceGeneratorState other && Equals(other);
   }

   public bool Equals(KeyedValueObjectSourceGeneratorState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && FactoryValidationReturnType == other.FactoryValidationReturnType
             && KeyMember.Equals(other.KeyMember)
             && ValidationError.Equals(other.ValidationError)
             && Settings.Equals(other.Settings);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ FactoryValidationReturnType?.GetHashCode() ?? 0;
         hashCode = (hashCode * 397) ^ KeyMember.GetHashCode();
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();

         return hashCode;
      }
   }
}
