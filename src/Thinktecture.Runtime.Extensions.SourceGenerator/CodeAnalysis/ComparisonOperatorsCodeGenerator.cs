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

   private const string _LEFT_NULL_CHECK = @"global::System.ArgumentNullException.ThrowIfNull(left);
      ";

   private const string _RIGHT_NULL_CHECK = @"global::System.ArgumentNullException.ThrowIfNull(right);
      ";

   private readonly bool _withKeyTypeOverloads;
   private readonly ImplementedComparisonOperators _keyMemberOperators;
   private readonly string? _comparerAccessor;

   public string CodeGeneratorName => "ComparisonOperators-CodeGenerator";
   public string FileNameSuffix => ".ComparisonOperators";
   public bool CanAppendColon => true;

   private ComparisonOperatorsCodeGenerator(
      bool withKeyTypeOverloads,
      ImplementedComparisonOperators keyMemberOperators,
      string? comparerAccessor)
   {
      _withKeyTypeOverloads = withKeyTypeOverloads;
      _keyMemberOperators = keyMemberOperators;
      _comparerAccessor = comparerAccessor;
   }

   public void GenerateBaseTypes(StringBuilder sb, InterfaceCodeGeneratorState state)
   {
      // If we have a custom comparer then we don't care whether the key member has the operators or not
      if (!_keyMemberOperators.HasOperator(ImplementedComparisonOperators.All) && _comparerAccessor is null && !state.KeyMember.IsString())
         return;

      sb.Append(@"
   global::System.Numerics.IComparisonOperators<").AppendTypeFullyQualified(state.Type).Append(", ").AppendTypeFullyQualified(state.Type).Append(", bool>");

      if (!_withKeyTypeOverloads)
         return;

      sb.Append(@",
   global::System.Numerics.IComparisonOperators<").AppendTypeFullyQualified(state.Type).Append(", ").AppendTypeFullyQualified(state.KeyMember).Append(", bool>");
   }

   public void GenerateImplementation(StringBuilder sb, InterfaceCodeGeneratorState state)
   {
      var leftNullCheck = state.Type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var rightNullCheck = state.Type.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_comparerAccessor is not null)
      {
         GenerateUsingComparerAccessor(sb, state, _comparerAccessor, leftNullCheck, rightNullCheck);
      }
      else if (state.KeyMember.IsString())
      {
         GenerateUsingOrdinalIgnoreCaseComparer(sb, state, leftNullCheck, rightNullCheck);
      }
      else
      {
         GenerateUsingOperators(sb, state, _keyMemberOperators, leftNullCheck, rightNullCheck);
      }

      if (_withKeyTypeOverloads)
         GenerateOverloadsForKeyType(sb, state);
   }

   private static void GenerateUsingOperators(
      StringBuilder sb,
      InterfaceCodeGeneratorState state,
      ImplementedComparisonOperators keyMemberOperators,
      string? leftNullCheck,
      string? rightNullCheck)
   {
      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.LessThan))
      {
         sb.Append(@"
   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(state.KeyMember.Name).Append(" < right.").Append(state.KeyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.LessThanOrEqual))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(state.KeyMember.Name).Append(" <= right.").Append(state.KeyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.GreaterThan))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(state.KeyMember.Name).Append(" > right.").Append(state.KeyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.GreaterThanOrEqual))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return left.").Append(state.KeyMember.Name).Append(" >= right.").Append(state.KeyMember.Name).Append(@";
   }");
      }
   }

   private static void GenerateUsingComparerAccessor(
      StringBuilder sb,
      InterfaceCodeGeneratorState state,
      string comparerAccessor,
      string? leftNullCheck,
      string? rightNullCheck)
   {
      sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(state.KeyMember.Name).Append(", right.").Append(state.KeyMember.Name).Append(@") < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(state.KeyMember.Name).Append(", right.").Append(state.KeyMember.Name).Append(@") <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(state.KeyMember.Name).Append(", right.").Append(state.KeyMember.Name).Append(@") > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(state.KeyMember.Name).Append(", right.").Append(state.KeyMember.Name).Append(@") >= 0;
   }");
   }

   private static void GenerateUsingOrdinalIgnoreCaseComparer(
      StringBuilder sb,
      InterfaceCodeGeneratorState state,
      string? leftNullCheck,
      string? rightNullCheck)
   {
      sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.").Append(state.KeyMember.Name).Append(", right.").Append(state.KeyMember.Name).Append(@") < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.").Append(state.KeyMember.Name).Append(", right.").Append(state.KeyMember.Name).Append(@") <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.").Append(state.KeyMember.Name).Append(", right.").Append(state.KeyMember.Name).Append(@") > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(leftNullCheck).Append(rightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.").Append(state.KeyMember.Name).Append(", right.").Append(state.KeyMember.Name).Append(@") >= 0;
   }");
   }

   private void GenerateOverloadsForKeyType(StringBuilder sb, InterfaceCodeGeneratorState state)
   {
      var typeLeftNullCheck = state.Type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var typeRightNullCheck = state.Type.IsReferenceType ? _RIGHT_NULL_CHECK : null;
      var memberLeftNullCheck = state.KeyMember.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var memberRightNullCheck = state.KeyMember.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_comparerAccessor is not null)
      {
         GenerateKeyOverloadsUsingComparerAccessor(sb, state, _comparerAccessor, typeLeftNullCheck, memberRightNullCheck, memberLeftNullCheck, typeRightNullCheck);
      }
      else if (state.KeyMember.IsString())
      {
         GenerateKeyOverloadsUsingOrdinalIgnoreCaseComparer(sb, state, typeLeftNullCheck, memberRightNullCheck, memberLeftNullCheck, typeRightNullCheck);
      }
      else
      {
         GenerateKeyOverloadsUsingOperators(sb, state, _keyMemberOperators, typeLeftNullCheck, memberRightNullCheck, memberLeftNullCheck, typeRightNullCheck);
      }
   }

   private static void GenerateKeyOverloadsUsingOperators(
      StringBuilder sb,
      InterfaceCodeGeneratorState state,
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
   public static bool operator <(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(state.KeyMember.Name).Append(@" < right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left < right.").Append(state.KeyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.LessThanOrEqual))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(state.KeyMember.Name).Append(@" <= right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left <= right.").Append(state.KeyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.GreaterThan))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(state.KeyMember.Name).Append(@" > right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left > right.").Append(state.KeyMember.Name).Append(@";
   }");
      }

      if (keyMemberOperators.HasOperator(ImplementedComparisonOperators.GreaterThanOrEqual))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return left.").Append(state.KeyMember.Name).Append(@" >= right;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return left >= right.").Append(state.KeyMember.Name).Append(@";
   }");
      }
   }

   private static void GenerateKeyOverloadsUsingComparerAccessor(
      StringBuilder sb,
      InterfaceCodeGeneratorState state,
      string comparerAccessor,
      string? typeLeftNullCheck,
      string? memberRightNullCheck,
      string? memberLeftNullCheck,
      string? typeRightNullCheck)
   {
      sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(state.KeyMember.Name).Append(@", right) < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left, right.").Append(state.KeyMember.Name).Append(@") < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(state.KeyMember.Name).Append(@", right) <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left, right.").Append(state.KeyMember.Name).Append(@") <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(state.KeyMember.Name).Append(@", right) > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left, right.").Append(state.KeyMember.Name).Append(@") > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left.").Append(state.KeyMember.Name).Append(@", right) >= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return ").Append(comparerAccessor).Append(".Comparer.Compare(left, right.").Append(state.KeyMember.Name).Append(@") >= 0;
   }");
   }

   private static void GenerateKeyOverloadsUsingOrdinalIgnoreCaseComparer(
      StringBuilder sb,
      InterfaceCodeGeneratorState state,
      string? typeLeftNullCheck,
      string? memberRightNullCheck,
      string? memberLeftNullCheck,
      string? typeRightNullCheck)
   {
      sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.").Append(state.KeyMember.Name).Append(@", right) < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"" />
   public static bool operator <(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left, right.").Append(state.KeyMember.Name).Append(@") < 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.").Append(state.KeyMember.Name).Append(@", right) <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"" />
   public static bool operator <=(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left, right.").Append(state.KeyMember.Name).Append(@") <= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.").Append(state.KeyMember.Name).Append(@", right) > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"" />
   public static bool operator >(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left, right.").Append(state.KeyMember.Name).Append(@") > 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.").Append(state.KeyMember.Name).Append(@", right) >= 0;
   }

   /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"" />
   public static bool operator >=(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeRightNullCheck).Append("return global::System.StringComparer.OrdinalIgnoreCase.Compare(left, right.").Append(state.KeyMember.Name).Append(@") >= 0;
   }");
   }
}
