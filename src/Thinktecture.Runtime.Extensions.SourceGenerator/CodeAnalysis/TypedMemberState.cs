using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class TypedMemberState : IEquatable<TypedMemberState>, ITypedMemberState
{
   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated => IsReferenceType ? TypeFullyQualifiedNullable : TypeFullyQualified;
   public string TypeFullyQualifiedWithNullability => IsReferenceType && NullableAnnotation == NullableAnnotation.Annotated ? TypeFullyQualifiedNullAnnotated : TypeFullyQualified;

   public NullableAnnotation NullableAnnotation { get; }
   public SpecialType SpecialType { get; }
   public bool IsReferenceType { get; }
   public bool IsNullableStruct { get; }
   public bool IsReferenceTypeOrNullableStruct => IsReferenceType || IsNullableStruct;
   public bool IsFormattable { get; }
   public bool IsComparable { get; }
   public bool IsParsable { get; }
   public ImplementedComparisonOperators ComparisonOperators { get; }
   public ImplementedOperators AdditionOperators { get; }
   public ImplementedOperators SubtractionOperators { get; }
   public ImplementedOperators MultiplyOperators { get; }
   public ImplementedOperators DivisionOperators { get; }

   public TypedMemberState(ITypeSymbol type)
   {
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullable = $"{TypeFullyQualified}?";
      IsReferenceType = type.IsReferenceType;
      NullableAnnotation = type.NullableAnnotation;
      SpecialType = type.SpecialType;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

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
               if (customOperator.IsComparisonOperator(type))
                  ComparisonOperators |= ImplementedComparisonOperators.GreaterThan;
               break;

            case "op_GreaterThanOrEqual":
               if (customOperator.IsComparisonOperator(type))
                  ComparisonOperators |= ImplementedComparisonOperators.GreaterThanOrEqual;
               break;

            case "op_LessThan":
               if (customOperator.IsComparisonOperator(type))
                  ComparisonOperators |= ImplementedComparisonOperators.LessThan;
               break;

            case "op_LessThanOrEqual":
               if (customOperator.IsComparisonOperator(type))
                  ComparisonOperators |= ImplementedComparisonOperators.LessThanOrEqual;
               break;

            case "op_Addition":
               if (customOperator.IsArithmeticOperator(type))
                  AdditionOperators |= ImplementedOperators.Default;
               break;

            case "op_CheckedAddition":
               if (customOperator.IsArithmeticOperator(type))
                  AdditionOperators |= ImplementedOperators.Checked;
               break;

            case "op_Subtraction":
               if (customOperator.IsArithmeticOperator(type))
                  SubtractionOperators |= ImplementedOperators.Default;
               break;
            case "op_CheckedSubtraction":
               if (customOperator.IsArithmeticOperator(type))
                  SubtractionOperators |= ImplementedOperators.Checked;
               break;

            case "op_Division":
               if (customOperator.IsArithmeticOperator(type))
                  DivisionOperators |= ImplementedOperators.Default;
               break;

            case "op_CheckedDivision":
               if (customOperator.IsArithmeticOperator(type))
                  DivisionOperators |= ImplementedOperators.Checked;
               break;

            case "op_Multiply":
               if (customOperator.IsArithmeticOperator(type))
                  MultiplyOperators |= ImplementedOperators.Default;
               break;

            case "op_CheckedMultiply":
               if (customOperator.IsArithmeticOperator(type))
                  MultiplyOperators |= ImplementedOperators.Checked;
               break;
         }
      }
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
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualifiedWithNullability == other.TypeFullyQualifiedWithNullability
             && SpecialType == other.SpecialType
             && IsNullableStruct == other.IsNullableStruct
             && IsReferenceType == other.IsReferenceType
             && IsFormattable == other.IsFormattable
             && IsComparable == other.IsComparable
             && IsParsable == other.IsParsable
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
         var hashCode = TypeFullyQualifiedWithNullability.GetHashCode();
         hashCode = (hashCode * 397) ^ SpecialType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsNullableStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ ComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ AdditionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ SubtractionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ MultiplyOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ DivisionOperators.GetHashCode();

         return hashCode;
      }
   }
}
