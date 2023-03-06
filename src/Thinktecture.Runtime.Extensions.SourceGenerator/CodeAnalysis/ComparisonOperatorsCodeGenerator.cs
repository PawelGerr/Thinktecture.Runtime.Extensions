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

   private ComparisonOperatorsCodeGenerator(bool withKeyTypeOverloads, string? comparerAccessor)
   {
      _withKeyTypeOverloads = withKeyTypeOverloads;
      _comparerAccessor = comparerAccessor;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      sb.Append(@$"
      global::System.Numerics.IComparisonOperators<{type.TypeFullyQualified}, {type.TypeFullyQualified}, bool>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      var leftNullCheck = type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var rightNullCheck = type.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_comparerAccessor is null)
      {
         sb.Append($$"""


      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
      public static bool operator <({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{leftNullCheck}}{{rightNullCheck}}return left.{{keyMember.Name}} < right.{{keyMember.Name}};
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
      public static bool operator <=({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{leftNullCheck}}{{rightNullCheck}}return left.{{keyMember.Name}} <= right.{{keyMember.Name}};
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
      public static bool operator >({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{leftNullCheck}}{{rightNullCheck}}return left.{{keyMember.Name}} > right.{{keyMember.Name}};
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
      public static bool operator >=({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{leftNullCheck}}{{rightNullCheck}}return left.{{keyMember.Name}} >= right.{{keyMember.Name}};
      }
""");
      }
      else
      {
         sb.Append($$"""


      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
      public static bool operator <({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{leftNullCheck}}{{rightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left.{{keyMember.Name}}, right.{{keyMember.Name}}) < 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
      public static bool operator <=({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{leftNullCheck}}{{rightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left.{{keyMember.Name}}, right.{{keyMember.Name}}) <= 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
      public static bool operator >({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{leftNullCheck}}{{rightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left.{{keyMember.Name}}, right.{{keyMember.Name}}) > 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
      public static bool operator >=({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{leftNullCheck}}{{rightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left.{{keyMember.Name}}, right.{{keyMember.Name}}) >= 0;
      }
""");
      }

      if (_withKeyTypeOverloads)
         GenerateOverloadsForKeyType(sb, type, keyMember);
   }

   private void GenerateOverloadsForKeyType(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      var typeLeftNullCheck = type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var typeRightNullCheck = type.IsReferenceType ? _RIGHT_NULL_CHECK : null;
      var memberLeftNullCheck = keyMember.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var memberRightNullCheck = keyMember.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_comparerAccessor is null)
      {
         sb.Append($$"""


      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
      public static bool operator <({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return left.{{keyMember.Name}} < right;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
      public static bool operator <({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeRightNullCheck}}return left < right.{{keyMember.Name}};
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
      public static bool operator <=({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return left.{{keyMember.Name}} <= right;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
      public static bool operator <=({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeRightNullCheck}}return left <= right.{{keyMember.Name}};
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
      public static bool operator >({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return left.{{keyMember.Name}} > right;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
      public static bool operator >({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeRightNullCheck}}return left > right.{{keyMember.Name}};
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
      public static bool operator >=({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return left.{{keyMember.Name}} >= right;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
      public static bool operator >=({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeRightNullCheck}}return left >= right.{{keyMember.Name}};
      }
""");
      }
      else
      {
         sb.Append($$"""


      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
      public static bool operator <({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left.{{keyMember.Name}}, right) < 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
      public static bool operator <({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeRightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left, right.{{keyMember.Name}}) < 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
      public static bool operator <=({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left.{{keyMember.Name}}, right) <= 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
      public static bool operator <=({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeRightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left, right.{{keyMember.Name}}) <= 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
      public static bool operator >({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left.{{keyMember.Name}}, right) > 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
      public static bool operator >({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeRightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left, right.{{keyMember.Name}}) > 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
      public static bool operator >=({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left.{{keyMember.Name}}, right) >= 0;
      }

      /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
      public static bool operator >=({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeRightNullCheck}}return {{_comparerAccessor}}.Comparer.Compare(left, right.{{keyMember.Name}}) >= 0;
      }
""");
      }
   }
}
