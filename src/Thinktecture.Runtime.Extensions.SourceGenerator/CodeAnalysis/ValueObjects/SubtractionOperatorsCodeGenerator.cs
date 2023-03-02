using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class SubtractionOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Default = new SubtractionOperatorsCodeGenerator();

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type)
   {
      sb.Append($"global::System.Numerics.ISubtractionOperators<{type.TypeFullyQualified}, {type.TypeFullyQualified}, {type.TypeFullyQualified}>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      sb.Append($@"

      /// <inheritdoc cref=""global::System.Numerics.ISubtractionOperators{{TSelf, TOther, TResult}}.op_Subtraction(TSelf, TOther)"" />
      public static {type.TypeFullyQualified} operator -({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => Create(left.{member.Name} - right.{member.Name});

      /// <inheritdoc cref=""global::System.Numerics.ISubtractionOperators{{TSelf, TOther, TResult}}.op_Subtraction(TSelf, TOther)"" />
      public static {type.TypeFullyQualified} operator checked -({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => Create(checked(left.{member.Name} - right.{member.Name}));");
   }
}
