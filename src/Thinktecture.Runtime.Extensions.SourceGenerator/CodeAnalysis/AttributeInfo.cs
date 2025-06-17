namespace Thinktecture.CodeAnalysis;

public readonly struct AttributeInfo : IEquatable<AttributeInfo>
{
   public bool HasStructLayoutAttribute { get; }
   public bool HasJsonConverterAttribute { get; }
   public bool HasNewtonsoftJsonConverterAttribute { get; }
   public bool HasMessagePackFormatterAttribute { get; }
   public ImmutableArray<ObjectFactoryState> ObjectFactories { get; }
   public ValidationErrorState ValidationError { get; }
   public string? KeyMemberComparerAccessor { get; }
   public string? KeyMemberEqualityComparerAccessor { get; }

   private AttributeInfo(
      bool hasStructLayoutAttribute,
      bool hasJsonConverterAttribute,
      bool hasNewtonsoftJsonConverterAttribute,
      bool hasMessagePackFormatterAttribute,
      ImmutableArray<ObjectFactoryState> objectFactories,
      ValidationErrorState validationError,
      string? keyMemberComparerAccessor,
      string? keyMemberEqualityComparerAccessor)
   {
      HasStructLayoutAttribute = hasStructLayoutAttribute;
      HasJsonConverterAttribute = hasJsonConverterAttribute;
      HasNewtonsoftJsonConverterAttribute = hasNewtonsoftJsonConverterAttribute;
      HasMessagePackFormatterAttribute = hasMessagePackFormatterAttribute;
      ObjectFactories = objectFactories;
      ValidationError = validationError;
      KeyMemberComparerAccessor = keyMemberComparerAccessor;
      KeyMemberEqualityComparerAccessor = keyMemberEqualityComparerAccessor;
   }

   public static string? TryCreate(
      INamedTypeSymbol type,
      out AttributeInfo info)
   {
      return TryCreate(type, out info, out _);
   }

   public static string? TryCreate(
      INamedTypeSymbol type,
      out AttributeInfo info,
      out AttributeData? thinktectureComponentAttribute)
   {
      var hasStructLayoutAttribute = false;
      var hasJsonConverterAttribute = false;
      var hasNewtonsoftJsonConverterAttribute = false;
      var hasMessagePackFormatterAttribute = false;
      var validationError = ValidationErrorState.Default;
      var valueObjectFactories = ImmutableArray<ObjectFactoryState>.Empty;
      string? keyMemberComparerAccessor = null;
      string? keyMemberEqualityComparerAccessor = null;
      thinktectureComponentAttribute = null;
      var numberOfSourceGenAttributes = 0;

      foreach (var attribute in type.GetAttributes())
      {
         if (attribute.AttributeClass is not { } attributeClass || attributeClass.TypeKind == TypeKind.Error)
            continue;

         if (attribute.AttributeClass.IsStructLayoutAttribute())
         {
            hasStructLayoutAttribute = true;
         }
         else if (attribute.AttributeClass.IsJsonConverterAttribute())
         {
            hasJsonConverterAttribute = true;
         }
         else if (attribute.AttributeClass.IsNewtonsoftJsonConverterAttribute())
         {
            hasNewtonsoftJsonConverterAttribute = true;
         }
         else if (attribute.AttributeClass.IsMessagePackFormatterAttribute())
         {
            hasMessagePackFormatterAttribute = true;
         }
         else if (attribute.AttributeClass.IsObjectFactoryAttribute())
         {
            var useForSerialization = attribute.FindUseForSerialization();
            var useWithEntityFramework = attribute.FindUseWithEntityFramework();
            var desiredFactory = new ObjectFactoryState(attribute.AttributeClass.TypeArguments[0], useForSerialization, useWithEntityFramework);

            valueObjectFactories = valueObjectFactories.RemoveAll(static (f, fullTypeName) => f.TypeFullyQualified == fullTypeName, desiredFactory.TypeFullyQualified);
            valueObjectFactories = valueObjectFactories.Add(desiredFactory);
         }
         else if (attribute.AttributeClass.IsValidationErrorAttribute())
         {
            validationError = new ValidationErrorState(attribute.AttributeClass.TypeArguments[0].ToFullyQualifiedDisplayString());
         }
         else if (attribute.AttributeClass.IsKeyMemberComparerAttribute())
         {
            keyMemberComparerAccessor = attribute.AttributeClass.TypeArguments[0].ToFullyQualifiedDisplayString();
         }
         else if (attribute.AttributeClass.IsKeyMemberEqualityComparerAttribute())
         {
            keyMemberEqualityComparerAccessor = attribute.AttributeClass.TypeArguments[0].ToFullyQualifiedDisplayString();
         }
         else if (attribute.AttributeClass.IsThinktectureComponentAttribute())
         {
            ++numberOfSourceGenAttributes;
            thinktectureComponentAttribute = attribute;
         }

         if (numberOfSourceGenAttributes > 1)
         {
            info = default;
            return "Multiple ValueObject/SmartEnum/Union-attributes found";
         }
      }

      info = new AttributeInfo(hasStructLayoutAttribute,
                               hasJsonConverterAttribute,
                               hasNewtonsoftJsonConverterAttribute,
                               hasMessagePackFormatterAttribute,
                               valueObjectFactories,
                               validationError,
                               keyMemberComparerAccessor,
                               keyMemberEqualityComparerAccessor);
      return null;
   }

   public override bool Equals(object? obj)
   {
      return obj is AttributeInfo other && Equals(other);
   }

   public bool Equals(AttributeInfo other)
   {
      return HasStructLayoutAttribute == other.HasStructLayoutAttribute
             && HasJsonConverterAttribute == other.HasJsonConverterAttribute
             && HasNewtonsoftJsonConverterAttribute == other.HasNewtonsoftJsonConverterAttribute
             && HasMessagePackFormatterAttribute == other.HasMessagePackFormatterAttribute
             && ObjectFactories.SequenceEqual(other.ObjectFactories)
             && ValidationError.Equals(other.ValidationError)
             && KeyMemberComparerAccessor == other.KeyMemberComparerAccessor
             && KeyMemberEqualityComparerAccessor == other.KeyMemberEqualityComparerAccessor;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = HasStructLayoutAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ HasJsonConverterAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ HasNewtonsoftJsonConverterAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ HasMessagePackFormatterAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ ObjectFactories.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ (KeyMemberComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ (KeyMemberEqualityComparerAccessor?.GetHashCode() ?? 0);

         return hashCode;
      }
   }
}
