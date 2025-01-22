namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class AllValueObjectSettings : IEquatable<AllValueObjectSettings>, IKeyMemberSettings
{
   public ValueObjectAccessModifier KeyMemberAccessModifier { get; }
   public ValueObjectMemberKind KeyMemberKind { get; }
   public string KeyMemberName { get; }
   public bool SkipKeyMember { get; }
   public bool SkipFactoryMethods { get; }
   public ValueObjectAccessModifier ConstructorAccessModifier { get; }
   public string CreateFactoryMethodName { get; }
   public string TryCreateFactoryMethodName { get; }
   public bool EmptyStringInFactoryMethodsYieldsNull { get; }
   public bool NullInFactoryMethodsYieldsNull { get; }
   public bool SkipIComparable { get; }
   public bool SkipIParsable { get; }
   public bool SkipIFormattable { get; }
   public bool SkipToString { get; }
   public OperatorsGeneration AdditionOperators { get; }
   public OperatorsGeneration SubtractionOperators { get; }
   public OperatorsGeneration MultiplyOperators { get; }
   public OperatorsGeneration DivisionOperators { get; }
   public OperatorsGeneration ComparisonOperators { get; }
   public OperatorsGeneration EqualityComparisonOperators { get; }
   public string DefaultInstancePropertyName { get; }
   public bool AllowDefaultStructs { get; }
   public ConversionOperatorsGeneration ConversionFromKeyMemberType { get; }
   public ConversionOperatorsGeneration UnsafeConversionToKeyMemberType { get; }
   public ConversionOperatorsGeneration ConversionToKeyMemberType { get; }

   public AllValueObjectSettings(AttributeData valueObjectAttribute)
   {
      KeyMemberAccessModifier = valueObjectAttribute.FindKeyMemberAccessModifier() ?? Constants.ValueObject.DEFAULT_KEY_MEMBER_ACCESS_MODIFIER;
      KeyMemberKind = valueObjectAttribute.FindKeyMemberKind() ?? Constants.ValueObject.DEFAULT_KEY_MEMBER_KIND;
      KeyMemberName = valueObjectAttribute.FindKeyMemberName() ?? Helper.GetDefaultValueObjectKeyMemberName(KeyMemberAccessModifier, KeyMemberKind);
      SkipKeyMember = valueObjectAttribute.FindSkipKeyMember() ?? false;
      SkipFactoryMethods = valueObjectAttribute.FindSkipFactoryMethods() ?? false;
      ConstructorAccessModifier = valueObjectAttribute.FindConstructorAccessModifier() ?? Constants.ValueObject.DEFAULT_CONSTRUCTOR_ACCESS_MODIFIER;
      CreateFactoryMethodName = valueObjectAttribute.FindCreateFactoryMethodName() ?? "Create";
      TryCreateFactoryMethodName = valueObjectAttribute.FindTryCreateFactoryMethodName() ?? "TryCreate";
      EmptyStringInFactoryMethodsYieldsNull = valueObjectAttribute.FindEmptyStringInFactoryMethodsYieldsNull() ?? false;
      NullInFactoryMethodsYieldsNull = EmptyStringInFactoryMethodsYieldsNull || (valueObjectAttribute.FindNullInFactoryMethodsYieldsNull() ?? false);
      SkipIComparable = valueObjectAttribute.FindSkipIComparable() ?? false;
      SkipIParsable = SkipFactoryMethods || (valueObjectAttribute.FindSkipIParsable() ?? false);
      SkipIFormattable = valueObjectAttribute.FindSkipIFormattable() ?? false;
      SkipToString = valueObjectAttribute.FindSkipToString() ?? false;
      AdditionOperators = SkipFactoryMethods ? OperatorsGeneration.None : valueObjectAttribute.FindAdditionOperators();
      SubtractionOperators = SkipFactoryMethods ? OperatorsGeneration.None : valueObjectAttribute.FindSubtractionOperators();
      MultiplyOperators = SkipFactoryMethods ? OperatorsGeneration.None : valueObjectAttribute.FindMultiplyOperators();
      DivisionOperators = SkipFactoryMethods ? OperatorsGeneration.None : valueObjectAttribute.FindDivisionOperators();
      EqualityComparisonOperators = valueObjectAttribute.FindEqualityComparisonOperators();
      ComparisonOperators = valueObjectAttribute.FindComparisonOperators();
      DefaultInstancePropertyName = valueObjectAttribute.FindDefaultInstancePropertyName() ?? "Empty";
      AllowDefaultStructs = valueObjectAttribute.FindAllowDefaultStructs();
      ConversionToKeyMemberType = valueObjectAttribute.FindConversionToKeyMemberType() ?? ConversionOperatorsGeneration.Implicit;
      UnsafeConversionToKeyMemberType = valueObjectAttribute.FindUnsafeConversionToKeyMemberType() ?? ConversionOperatorsGeneration.Explicit;
      ConversionFromKeyMemberType = valueObjectAttribute.FindConversionFromKeyMemberType() ?? ConversionOperatorsGeneration.Explicit;

      // Comparison operators depend on the equality comparison operators
      if (ComparisonOperators > EqualityComparisonOperators)
         EqualityComparisonOperators = ComparisonOperators;
   }

   public override bool Equals(object? obj)
   {
      return obj is AllValueObjectSettings other && Equals(other);
   }

   public bool Equals(AllValueObjectSettings? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return KeyMemberAccessModifier == other.KeyMemberAccessModifier
             && KeyMemberKind == other.KeyMemberKind
             && KeyMemberName == other.KeyMemberName
             && SkipKeyMember == other.SkipKeyMember
             && SkipFactoryMethods == other.SkipFactoryMethods
             && ConstructorAccessModifier == other.ConstructorAccessModifier
             && CreateFactoryMethodName == other.CreateFactoryMethodName
             && TryCreateFactoryMethodName == other.TryCreateFactoryMethodName
             && EmptyStringInFactoryMethodsYieldsNull == other.EmptyStringInFactoryMethodsYieldsNull
             && NullInFactoryMethodsYieldsNull == other.NullInFactoryMethodsYieldsNull
             && SkipIComparable == other.SkipIComparable
             && SkipIParsable == other.SkipIParsable
             && SkipIFormattable == other.SkipIFormattable
             && SkipToString == other.SkipToString
             && AdditionOperators == other.AdditionOperators
             && SubtractionOperators == other.SubtractionOperators
             && MultiplyOperators == other.MultiplyOperators
             && DivisionOperators == other.DivisionOperators
             && ComparisonOperators == other.ComparisonOperators
             && EqualityComparisonOperators == other.EqualityComparisonOperators
             && DefaultInstancePropertyName == other.DefaultInstancePropertyName
             && AllowDefaultStructs == other.AllowDefaultStructs
             && ConversionToKeyMemberType == other.ConversionToKeyMemberType
             && UnsafeConversionToKeyMemberType == other.UnsafeConversionToKeyMemberType
             && ConversionFromKeyMemberType == other.ConversionFromKeyMemberType;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = (int)KeyMemberAccessModifier;
         hashCode = (hashCode * 397) ^ (int)KeyMemberKind;
         hashCode = (hashCode * 397) ^ KeyMemberName.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipKeyMember.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipFactoryMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ ConstructorAccessModifier.GetHashCode();
         hashCode = (hashCode * 397) ^ CreateFactoryMethodName.GetHashCode();
         hashCode = (hashCode * 397) ^ TryCreateFactoryMethodName.GetHashCode();
         hashCode = (hashCode * 397) ^ EmptyStringInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ NullInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)AdditionOperators;
         hashCode = (hashCode * 397) ^ (int)SubtractionOperators;
         hashCode = (hashCode * 397) ^ (int)MultiplyOperators;
         hashCode = (hashCode * 397) ^ (int)DivisionOperators;
         hashCode = (hashCode * 397) ^ (int)ComparisonOperators;
         hashCode = (hashCode * 397) ^ (int)EqualityComparisonOperators;
         hashCode = (hashCode * 397) ^ DefaultInstancePropertyName.GetHashCode();
         hashCode = (hashCode * 397) ^ AllowDefaultStructs.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)ConversionToKeyMemberType;
         hashCode = (hashCode * 397) ^ (int)UnsafeConversionToKeyMemberType;
         hashCode = (hashCode * 397) ^ (int)ConversionFromKeyMemberType;

         return hashCode;
      }
   }
}
