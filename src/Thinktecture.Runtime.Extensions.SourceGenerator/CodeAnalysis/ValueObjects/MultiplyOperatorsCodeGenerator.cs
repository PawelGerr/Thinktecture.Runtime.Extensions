using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class MultiplyOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Default = new MultiplyOperatorsCodeGenerator();

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type)
   {
      sb.Append($"global::System.Numerics.IMultiplyOperators<{type.TypeFullyQualified}, {type.TypeFullyQualified}, {type.TypeFullyQualified}>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      sb.Append($@"

      /// <inheritdoc cref=""global::System.Numerics.IMultiplyOperators{{TSelf, TOther, TResult}}.op_Multiply(TSelf, TOther)"" />
      public static {type.TypeFullyQualified} operator *({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => Create(left.{member.Name} * right.{member.Name});

      /// <inheritdoc cref=""global::System.Numerics.IMultiplyOperators{{TSelf, TOther, TResult}}.op_Multiply(TSelf, TOther)"" />
      public static {type.TypeFullyQualified} operator checked *({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => Create(checked(left.{member.Name} * right.{member.Name}));");
   }
}
