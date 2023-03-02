using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class DivisionOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Default = new DivisionOperatorsCodeGenerator();

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type)
   {
      sb.Append($"global::System.Numerics.IDivisionOperators<{type.TypeFullyQualified}, {type.TypeFullyQualified}, {type.TypeFullyQualified}>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      sb.Append($@"

      /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{{TSelf, TOther, TResult}}.op_Division(TSelf, TOther)"" />
      public static {type.TypeFullyQualified} operator /({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => Create(left.{member.Name} / right.{member.Name});

      /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{{TSelf, TOther, TResult}}.op_Division(TSelf, TOther)"" />
      public static {type.TypeFullyQualified} operator checked /({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => Create(checked(left.{member.Name} / right.{member.Name}));");
   }
}
