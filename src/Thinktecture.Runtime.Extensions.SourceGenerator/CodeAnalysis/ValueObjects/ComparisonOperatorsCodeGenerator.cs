using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComparisonOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Default = new ComparisonOperatorsCodeGenerator();

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type)
   {
      sb.Append($"global::System.Numerics.IComparisonOperators<{type.TypeFullyQualified}, {type.TypeFullyQualified}, bool>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      sb.Append($@"

      /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{{TSelf, TOther, TResult}}.op_LessThan(TSelf, TOther)"" />
      public static bool operator <({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => left.{member.Name} < right.{member.Name};

      /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{{TSelf, TOther, TResult}}.op_LessThanOrEqual(TSelf, TOther)"" />
      public static bool operator <=({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => left.{member.Name} <= right.{member.Name};

      /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{{TSelf, TOther, TResult}}.op_GreaterThan(TSelf, TOther)"" />
      public static bool operator >({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => left.{member.Name} > right.{member.Name};

      /// <inheritdoc cref=""global::System.Numerics.IComparisonOperators{{TSelf, TOther, TResult}}.op_GreaterThanOrEqual(TSelf, TOther)"" />
      public static bool operator >=({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => left.{member.Name} >= right.{member.Name};");
   }
}
