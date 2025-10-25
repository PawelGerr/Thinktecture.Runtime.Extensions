namespace Thinktecture.CodeAnalysis;

public sealed class TypedMemberState : IEquatable<TypedMemberState>, ITypedMemberState
{
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }

   public NullableAnnotation NullableAnnotation { get; }
   public SpecialType SpecialType { get; }
   public TypeKind TypeKind { get; }
   public bool IsReferenceType { get; }
   public bool IsStruct { get; }
   public bool IsTypeParameter => TypeKind == TypeKind.TypeParameter;
   public bool IsNullableStruct { get; }
   public bool IsReferenceTypeOrNullableStruct => IsReferenceType || IsNullableStruct || (IsTypeParameter && !IsStruct);
   public bool IsFormattable { get; }
   public bool IsComparable { get; }
   public bool IsParsable { get; }
   public bool IsToStringReturnTypeNullable { get; }
   public ImplementedComparisonOperators ComparisonOperators { get; }
   public ImplementedOperators AdditionOperators { get; }
   public ImplementedOperators SubtractionOperators { get; }
   public ImplementedOperators MultiplyOperators { get; }
   public ImplementedOperators DivisionOperators { get; }

   public TypedMemberState(ITypeSymbol type)
   {
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;
      IsStruct = type.IsValueType;
      NullableAnnotation = type.NullableAnnotation;
      SpecialType = type.SpecialType;
      TypeKind = type.TypeKind;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
                         || type is { IsValueType: true, NullableAnnotation: NullableAnnotation.Annotated };
      IsToStringReturnTypeNullable = IsToStringNullable(type);

      // Check for implemented interfaces
      foreach (var @interface in type.AllInterfaces)
      {
         if (@interface.IsFormattableInterface())
         {
            IsFormattable = true;
         }
         else if (@interface.IsComparableInterface(type))
         {
            IsComparable = true;
         }
         else if (@interface.IsParsableInterface(type))
         {
            IsParsable = true;
         }
         else if (@interface.IsIAdditionOperators(type))
         {
            AdditionOperators = ImplementedOperators.All;
         }
         else if (@interface.IsISubtractionOperators(type))
         {
            SubtractionOperators = ImplementedOperators.All;
         }
         else if (@interface.IsIMultiplyOperators(type))
         {
            MultiplyOperators = ImplementedOperators.All;
         }
         else if (@interface.IsIDivisionOperators(type))
         {
            DivisionOperators = ImplementedOperators.All;
         }
         else if (@interface.IsIComparisonOperators(type))
         {
            ComparisonOperators = ImplementedComparisonOperators.All;
         }
      }

      // If the operator-interfaces are not implemented then check for user-defined operators
      var members = type.GetMembers();

      for (var i = 0; i < members.Length; i++)
      {
         if (members[i] is not IMethodSymbol { MethodKind: MethodKind.UserDefinedOperator } customOperator)
            continue;

         switch (customOperator.Name)
         {
            case "op_GreaterThan":
               if (!ComparisonOperators.HasOperator(ImplementedComparisonOperators.GreaterThan) && customOperator.IsComparisonOperator(type))
                  ComparisonOperators |= ImplementedComparisonOperators.GreaterThan;
               break;

            case "op_GreaterThanOrEqual":
               if (!ComparisonOperators.HasOperator(ImplementedComparisonOperators.GreaterThanOrEqual) && customOperator.IsComparisonOperator(type))
                  ComparisonOperators |= ImplementedComparisonOperators.GreaterThanOrEqual;
               break;

            case "op_LessThan":
               if (!ComparisonOperators.HasOperator(ImplementedComparisonOperators.LessThan) && customOperator.IsComparisonOperator(type))
                  ComparisonOperators |= ImplementedComparisonOperators.LessThan;
               break;

            case "op_LessThanOrEqual":
               if (!ComparisonOperators.HasOperator(ImplementedComparisonOperators.LessThanOrEqual) && customOperator.IsComparisonOperator(type))
                  ComparisonOperators |= ImplementedComparisonOperators.LessThanOrEqual;
               break;

            case "op_Addition":
               if (!AdditionOperators.HasOperator(ImplementedOperators.Default) && customOperator.IsArithmeticOperator(type))
                  AdditionOperators |= ImplementedOperators.Default;
               break;

            case "op_CheckedAddition":
               if (!AdditionOperators.HasOperator(ImplementedOperators.Checked) && customOperator.IsArithmeticOperator(type))
                  AdditionOperators |= ImplementedOperators.Checked;
               break;

            case "op_Subtraction":
               if (!SubtractionOperators.HasOperator(ImplementedOperators.Default) && customOperator.IsArithmeticOperator(type))
                  SubtractionOperators |= ImplementedOperators.Default;
               break;
            case "op_CheckedSubtraction":
               if (!SubtractionOperators.HasOperator(ImplementedOperators.Checked) && customOperator.IsArithmeticOperator(type))
                  SubtractionOperators |= ImplementedOperators.Checked;
               break;

            case "op_Division":
               if (!DivisionOperators.HasOperator(ImplementedOperators.Default) && customOperator.IsArithmeticOperator(type))
                  DivisionOperators |= ImplementedOperators.Default;
               break;

            case "op_CheckedDivision":
               if (!DivisionOperators.HasOperator(ImplementedOperators.Checked) && customOperator.IsArithmeticOperator(type))
                  DivisionOperators |= ImplementedOperators.Checked;
               break;

            case "op_Multiply":
               if (!MultiplyOperators.HasOperator(ImplementedOperators.Default) && customOperator.IsArithmeticOperator(type))
                  MultiplyOperators |= ImplementedOperators.Default;
               break;

            case "op_CheckedMultiply":
               if (!MultiplyOperators.HasOperator(ImplementedOperators.Checked) && customOperator.IsArithmeticOperator(type))
                  MultiplyOperators |= ImplementedOperators.Checked;
               break;
         }
      }
   }

   private static bool IsToStringNullable(ITypeSymbol type)
   {
      var array = type.GetMembers("ToString");

      for (var i = 0; i < array.Length; i++)
      {
         var member = array[i];

         if (member is not IMethodSymbol toString)
            continue;

         return toString.ReturnNullableAnnotation == NullableAnnotation.Annotated;
      }

      return true;
   }

   public override bool Equals(object? obj)
   {
      return obj is TypedMemberState other && Equals(other);
   }

   public bool Equals(ITypedMemberState? obj)
   {
      return obj is TypedMemberState other && Equals(other);
   }

   public bool Equals(TypedMemberState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && SpecialType == other.SpecialType
             && TypeKind == other.TypeKind
             && IsNullableStruct == other.IsNullableStruct
             && IsReferenceType == other.IsReferenceType
             && IsStruct == other.IsStruct
             && IsFormattable == other.IsFormattable
             && IsComparable == other.IsComparable
             && IsParsable == other.IsParsable
             && IsToStringReturnTypeNullable == other.IsToStringReturnTypeNullable
             && ComparisonOperators == other.ComparisonOperators
             && AdditionOperators == other.AdditionOperators
             && SubtractionOperators == other.SubtractionOperators
             && MultiplyOperators == other.MultiplyOperators
             && DivisionOperators == other.DivisionOperators;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)SpecialType;
         hashCode = (hashCode * 397) ^ (int)TypeKind;
         hashCode = (hashCode * 397) ^ IsNullableStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ IsFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsToStringReturnTypeNullable.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)ComparisonOperators;
         hashCode = (hashCode * 397) ^ (int)AdditionOperators;
         hashCode = (hashCode * 397) ^ (int)SubtractionOperators;
         hashCode = (hashCode * 397) ^ (int)MultiplyOperators;
         hashCode = (hashCode * 397) ^ (int)DivisionOperators;

         return hashCode;
      }
   }
}
