using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ComparisonOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   private static readonly IInterfaceCodeGenerator _default = new ComparisonOperatorsCodeGenerator(false, null);
   private static readonly IInterfaceCodeGenerator _defaultWithKeyTypeOverloads = new ComparisonOperatorsCodeGenerator(true, null);

   public static bool TryGet(
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
            generator = comparerAccessor is null ? _default : new ComparisonOperatorsCodeGenerator(false, comparerAccessor);
            return true;
         case OperatorsGeneration.DefaultWithKeyTypeOverloads:
            generator = comparerAccessor is null ? _defaultWithKeyTypeOverloads : new ComparisonOperatorsCodeGenerator(true, comparerAccessor);
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
   private readonly string? _comparerAccessor;

   public string FileNameSuffix => ".ComparisonOperators";

   private ComparisonOperatorsCodeGenerator(bool withKeyTypeOverloads, string? comparerAccessor)
   {
      _withKeyTypeOverloads = withKeyTypeOverloads;
      _comparerAccessor = comparerAccessor;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      sb.Append(@"
   global::System.Numerics.IComparisonOperators<").Append(type.TypeFullyQualified).Append(", ").Append(type.TypeFullyQualified).Append(", bool>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      var leftNullCheck = type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var rightNullCheck = type.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_comparerAccessor is null)
      {
         sb.Append(@"
   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(keyMember.Name).Append(" < right.").Append(keyMember.Name).Append(@";
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(keyMember.Name).Append(" <= right.").Append(keyMember.Name).Append(@";
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(keyMember.Name).Append(" > right.").Append(keyMember.Name).Append(@";
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(keyMember.Name).Append(" >= right.").Append(keyMember.Name).Append(@";
   }");
      }
      else
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(", right.").Append(keyMember.Name).Append(@") < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(", right.").Append(keyMember.Name).Append(@") <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(", right.").Append(keyMember.Name).Append(@") > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(", right.").Append(keyMember.Name).Append(@") >= 0;
   }");
      }

      if (_withKeyTypeOverloads)
         GenerateOverloadsForKeyType(sb, type, keyMember);
   }

   private void GenerateOverloadsForKeyType(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      var typeLeftNullCheck = type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var typeRightNullCheck = type.IsReferenceType ? _RIGHT_NULL_CHECK : null;
      var memberLeftNullCheck = keyMember.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var memberRightNullCheck = keyMember.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_comparerAccessor is null)
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
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(keyMember.Name).Append(@" <= right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left <= right.").Append(keyMember.Name).Append(@";
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(keyMember.Name).Append(@" > right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left > right.").Append(keyMember.Name).Append(@";
   }

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
      else
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(@", right) < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left, right.").Append(keyMember.Name).Append(@") < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(@", right) <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left, right.").Append(keyMember.Name).Append(@") <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(@", right) > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left, right.").Append(keyMember.Name).Append(@") > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left.").Append(keyMember.Name).Append(@", right) >= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(_comparerAccessor).Append(".Comparer.Compare(left, right.").Append(keyMember.Name).Append(@") >= 0;
   }");
      }
   }
}
