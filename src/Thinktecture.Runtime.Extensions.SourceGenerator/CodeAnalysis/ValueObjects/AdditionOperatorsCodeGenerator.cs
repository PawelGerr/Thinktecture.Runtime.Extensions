using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class AdditionOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Default = new AdditionOperatorsCodeGenerator();

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type)
   {
      sb.Append($"global::System.Numerics.IAdditionOperators<{type.TypeFullyQualified}, {type.TypeFullyQualified}, {type.TypeFullyQualified}>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      sb.Append($@"

      /// <inheritdoc cref=""global::System.Numerics.IAdditionOperators{{TSelf, TOther, TResult}}.op_Addition(TSelf, TOther)"" />
      public static {type.TypeFullyQualified} operator +({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => Create(left.{member.Name} + right.{member.Name});

      /// <inheritdoc cref=""global::System.Numerics.IAdditionOperators{{TSelf, TOther, TResult}}.op_Addition(TSelf, TOther)"" />
      public static {type.TypeFullyQualified} operator checked +({type.TypeFullyQualified} left, {type.TypeFullyQualified} right) => Create(checked(left.{member.Name} + right.{member.Name}));");
   }
}
