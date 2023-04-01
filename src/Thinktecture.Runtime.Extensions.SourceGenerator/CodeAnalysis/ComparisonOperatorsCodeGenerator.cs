using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ComparisonOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   private static readonly IInterfaceCodeGenerator _default = new ComparisonOperatorsCodeGenerator(false, ImplementedComparisonOperators.All, null);
   private static readonly IInterfaceCodeGenerator _defaultWithKeyTypeOverloads = new ComparisonOperatorsCodeGenerator(true, ImplementedComparisonOperators.All, null);

   public static bool TryGet(
      ImplementedComparisonOperators keyMemberOperators,
      OperatorsGeneration operatorsGeneration,
      string? comparerAccessor,
      [MaybeNullWhen(false)] out IInterfaceCodeGenerator generator)
   {
      switch (operatorsGeneration)
      {
         case OperatorsGeneration.None:
            generator = null;
            return false;
         case OperatorsGeneration.Default:
            generator = comparerAccessor is null && keyMemberOperators == ImplementedComparisonOperators.All
                           ? _default
                           : new ComparisonOperatorsCodeGenerator(false, keyMemberOperators, comparerAccessor);
            return true;
         case OperatorsGeneration.DefaultWithKeyTypeOverloads:
            generator = comparerAccessor is null && keyMemberOperators == ImplementedComparisonOperators.All
                           ? _defaultWithKeyTypeOverloads
                           : new ComparisonOperatorsCodeGenerator(true, keyMemberOperators, comparerAccessor);
            return true;
         default:
            throw new ArgumentOutOfRangeException(nameof(operatorsGeneration), operatorsGeneration, "Invalid operations generation.");
      }
   }

   private const string _LEFT_NULL_CHECK = @"global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      ";

   private const string _RIGHT_NULL_CHECK = @"global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      ";

   private readonly bool _withKeyTypeOverloads;
   private readonly ImplementedComparisonOperators _keyMemberOperators;
   private readonly string? _comparerAccessor;

   public string CodeGeneratorName => "ComparisonOperators-CodeGenerator";
   public string FileNameSuffix => ".ComparisonOperators";

   private ComparisonOperatorsCodeGenerator(
      bool withKeyTypeOverloads,
      ImplementedComparisonOperators keyMemberOperators,
      string? comparerAccessor)
   {
      _withKeyTypeOverloads = withKeyTypeOverloads;
      _keyMemberOperators = keyMemberOperators;
      _comparerAccessor = comparerAccessor;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      // If we have a custom comparer then we don't care whether the key member has the operators or not
      if (!_keyMemberOperators.HasOperator(ImplementedComparisonOperators.All) && _comparerAccessor is null)
         return;

      sb.Append(@"
   global::System.Numerics.IComparisonOperators<").Append(type.TypeFullyQualified).Append(", ").Append(type.TypeFullyQualified).Append(", bool>");

      if (!_withKeyTypeOverloads)
         return;

      sb.Append(@",
   global::System.Numerics.IComparisonOperators<").Append(type.TypeFullyQualified).Append(", ").Append(keyMember.TypeFullyQualified).Append(", bool>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      var leftNullCheck = type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var rightNullCheck = type.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_comparerAccessor is null)
      {
         GenerateUsingOperators(sb, type, keyMember, _keyMemberOperators, leftNullCheck, rightNullCheck);
      }
      else
      {
         GenerateUsingComparer(sb, type, keyMember, _comparerAccessor, leftNullCheck, rightNullCheck);
      }

      if (_withKeyTypeOverloads)
         GenerateOverloadsForKeyType(sb, type, keyMember);
   }

   private static void GenerateUsingOperators(
      StringBuilder sb,
      ITypeInformation type,
      IMemberInformation keyMember,
      ImplementedComparisonOperators keyMemberOperators,
      string? leftNullCheck,
      string? rightNullCheck)
   {
      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.LessThan))
      {
         sb.Append(@"
   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(keyMember.Name).Append(" < right.").Append(keyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.LessThanOrEqual))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(keyMember.Name).Append(" <= right.").Append(keyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.GreaterThan))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(keyMember.Name).Append(" > right.").Append(keyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.GreaterThanOrEqual))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(keyMember.Name).Append(" >= right.").Append(keyMember.Name).Append(@";
   }");
      }
   }

   private static void GenerateUsingComparer(
      StringBuilder sb,
      ITypeInformation type,
      IMemberInformation keyMember,
      string comparerAccessor,
      string? leftNullCheck,
      string? rightNullCheck)
   {
      sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(", right.").Append(keyMember.Name).Append(@") < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(", right.").Append(keyMember.Name).Append(@") <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(", right.").Append(keyMember.Name).Append(@") > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(", right.").Append(keyMember.Name).Append(@") >= 0;
   }");
   }

   private void GenerateOverloadsForKeyType(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      var typeLeftNullCheck = type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var typeRightNullCheck = type.IsReferenceType ? _RIGHT_NULL_CHECK : null;
      var memberLeftNullCheck = keyMember.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var memberRightNullCheck = keyMember.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_comparerAccessor is null)
      {
         GenerateKeyOverloadsUsingOperators(sb, type, keyMember, _keyMemberOperators, typeLeftNullCheck, memberRightNullCheck, memberLeftNullCheck, typeRightNullCheck);
      }
      else
      {
         GenerateKeyOverloadsUsingComparer(sb, type, keyMember, _comparerAccessor, typeLeftNullCheck, memberRightNullCheck, memberLeftNullCheck, typeRightNullCheck);
      }
   }

   private static void GenerateKeyOverloadsUsingOperators(
      StringBuilder sb,
      ITypeInformation type,
      IMemberInformation keyMember,
      ImplementedComparisonOperators keyMemberOperators,
      string? typeLeftNullCheck,
      string? memberRightNullCheck,
      string? memberLeftNullCheck,
      string? typeRightNullCheck)
   {
      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.LessThan))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(keyMember.Name).Append(@" < right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left < right.").Append(keyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.LessThanOrEqual))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(keyMember.Name).Append(@" <= right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left <= right.").Append(keyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.GreaterThan))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(keyMember.Name).Append(@" > right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left > right.").Append(keyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.GreaterThanOrEqual))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(keyMember.Name).Append(@" >= right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left >= right.").Append(keyMember.Name).Append(@";
   }");
      }
   }

   private static void GenerateKeyOverloadsUsingComparer(
      StringBuilder sb,
      ITypeInformation type,
      IMemberInformation keyMember,
      string comparerAccessor,
      string? typeLeftNullCheck,
      string? memberRightNullCheck,
      string? memberLeftNullCheck,
      string? typeRightNullCheck)
   {
      sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(@", right) < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left, right.").Append(keyMember.Name).Append(@") < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(@", right) <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left, right.").Append(keyMember.Name).Append(@") <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(@", right) > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left, right.").Append(keyMember.Name).Append(@") > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(@", right) >= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left, right.").Append(keyMember.Name).Append(@") >= 0;
   }");
   }
}
