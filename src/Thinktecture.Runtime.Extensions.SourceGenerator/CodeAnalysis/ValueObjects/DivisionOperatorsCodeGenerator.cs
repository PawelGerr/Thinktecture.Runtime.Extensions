using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class DivisionOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   private static readonly IInterfaceCodeGenerator _default = new DivisionOperatorsCodeGenerator(ImplementedOperators.All, false);
   private static readonly IInterfaceCodeGenerator _defaultWithKeyTypeOverloads = new DivisionOperatorsCodeGenerator(ImplementedOperators.All, true);

   public static bool TryGet(ImplementedOperators keyMemberOperators, OperatorsGeneration operatorsGeneration, [MaybeNullWhen(false)] out IInterfaceCodeGenerator generator)
   {
      switch (operatorsGeneration)
      {
         case OperatorsGeneration.None:
            generator = null;
            return false;
         case OperatorsGeneration.Default:
            generator = keyMemberOperators == ImplementedOperators.All
                           ? _default
                           : new DivisionOperatorsCodeGenerator(keyMemberOperators, false);
            return true;
         case OperatorsGeneration.DefaultWithKeyTypeOverloads:
            generator = keyMemberOperators == ImplementedOperators.All
                           ? _defaultWithKeyTypeOverloads
                           : new DivisionOperatorsCodeGenerator(keyMemberOperators, true);
            return true;
         default:
            throw new ArgumentOutOfRangeException(nameof(operatorsGeneration), operatorsGeneration, "Invalid operations generation.");
      }
   }

   private const string _LEFT_NULL_CHECK = @"global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      ";

   private const string _RIGHT_NULL_CHECK = @"global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      ";

   private readonly ImplementedOperators _keyMemberOperators;
   private readonly bool _withKeyTypeOverloads;

   public string CodeGeneratorName => "DivisionOperators-CodeGenerator";
   public string FileNameSuffix => ".DivisionOperators";

   private DivisionOperatorsCodeGenerator(ImplementedOperators keyMemberOperators, bool withKeyTypeOverloads)
   {
      _keyMemberOperators = keyMemberOperators;
      _withKeyTypeOverloads = withKeyTypeOverloads;
   }

   public void GenerateBaseTypes(StringBuilder sb, InterfaceCodeGeneratorState state)
   {
      if (!_keyMemberOperators.HasOperator(ImplementedOperators.All))
         return;

      sb.Append(@"
   global::System.Numerics.IDivisionOperators<").AppendTypeFullyQualified(state.Type).Append(", ").AppendTypeFullyQualified(state.Type).Append(", ").AppendTypeFullyQualified(state.Type).Append(">");

      if (!_withKeyTypeOverloads)
         return;

      sb.Append(@",
   global::System.Numerics.IDivisionOperators<").AppendTypeFullyQualified(state.Type).Append(", ").AppendTypeFullyQualified(state.KeyMember).Append(", ").AppendTypeFullyQualified(state.Type).Append(">");
   }

   public void GenerateImplementation(StringBuilder sb, InterfaceCodeGeneratorState state)
   {
      var typeLeftNullCheck = state.Type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var typeLightNullCheck = state.Type.IsReferenceType ? _RIGHT_NULL_CHECK : null;
      var needsCastToResultType = state.KeyMember.DoesArithmeticOperationYieldDifferentType();

      if (_keyMemberOperators.HasOperator(ImplementedOperators.Default))
      {
         sb.Append(@"
   /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
   public static ").AppendTypeFullyQualified(state.Type).Append(" operator /(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(typeLightNullCheck).Append("return ").Append(state.CreateFactoryMethodName).Append("(").AppendCast(state.KeyMember, needsCastToResultType).Append("(left.").Append(state.KeyMember.Name).Append(" / right.").Append(state.KeyMember.Name).Append(@"));
   }");
      }

      if (_keyMemberOperators.HasOperator(ImplementedOperators.Checked))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
   public static ").AppendTypeFullyQualified(state.Type).Append(" operator checked /(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(typeLightNullCheck).Append("return ").Append(state.CreateFactoryMethodName).Append("(checked(").AppendCast(state.KeyMember, needsCastToResultType).Append("(left.").Append(state.KeyMember.Name).Append(" / right.").Append(state.KeyMember.Name).Append(@")));
   }");
      }

      if (_withKeyTypeOverloads)
         GenerateOverloadsForKeyType(sb, state, typeLeftNullCheck, typeLightNullCheck, needsCastToResultType);
   }

   private void GenerateOverloadsForKeyType(
      StringBuilder sb,
      InterfaceCodeGeneratorState state,
      string? typeLeftNullCheck,
      string? typeLightNullCheck,
      bool needsCastToResultType)
   {
      var memberLeftNullCheck = state.KeyMember.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var memberRightNullCheck = state.KeyMember.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_keyMemberOperators.HasOperator(ImplementedOperators.Default))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
   public static ").AppendTypeFullyQualified(state.Type).Append(" operator /(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(state.CreateFactoryMethodName).Append("(").AppendCast(state.KeyMember, needsCastToResultType).Append("(left.").Append(state.KeyMember.Name).Append(@" / right));
   }

   /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
   public static ").AppendTypeFullyQualified(state.Type).Append(" operator /(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeLightNullCheck).Append("return ").Append(state.CreateFactoryMethodName).Append("(").AppendCast(state.KeyMember, needsCastToResultType).Append("(left / right.").Append(state.KeyMember.Name).Append(@"));
   }");
      }

      if (_keyMemberOperators.HasOperator(ImplementedOperators.Checked))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
   public static ").AppendTypeFullyQualified(state.Type).Append(" operator checked /(").AppendTypeFullyQualified(state.Type).Append(" left, ").AppendTypeFullyQualified(state.KeyMember).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return ").Append(state.CreateFactoryMethodName).Append("(checked(").AppendCast(state.KeyMember, needsCastToResultType).Append("(left.").Append(state.KeyMember.Name).Append(@" / right)));
   }

   /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
   public static ").AppendTypeFullyQualified(state.Type).Append(" operator checked /(").AppendTypeFullyQualified(state.KeyMember).Append(" left, ").AppendTypeFullyQualified(state.Type).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeLightNullCheck).Append("return ").Append(state.CreateFactoryMethodName).Append("(checked(").AppendCast(state.KeyMember, needsCastToResultType).Append("(left / right.").Append(state.KeyMember.Name).Append(@")));
   }");
      }
   }
}
