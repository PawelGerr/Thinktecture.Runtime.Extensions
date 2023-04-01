using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class MultiplyOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   private static readonly IInterfaceCodeGenerator _default = new MultiplyOperatorsCodeGenerator(ImplementedOperators.All, false);
   private static readonly IInterfaceCodeGenerator _defaultWithKeyTypeOverloads = new MultiplyOperatorsCodeGenerator(ImplementedOperators.All, true);

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
                           : new MultiplyOperatorsCodeGenerator(keyMemberOperators, false);
            return true;
         case OperatorsGeneration.DefaultWithKeyTypeOverloads:
            generator = keyMemberOperators == ImplementedOperators.All
                           ? _defaultWithKeyTypeOverloads
                           : new MultiplyOperatorsCodeGenerator(keyMemberOperators, true);
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

   public string CodeGeneratorName => "MultiplyOperators-CodeGenerator";
   public string FileNameSuffix => ".MultiplyOperators";

   private MultiplyOperatorsCodeGenerator(ImplementedOperators keyMemberOperators, bool withKeyTypeOverloads)
   {
      _keyMemberOperators = keyMemberOperators;
      _withKeyTypeOverloads = withKeyTypeOverloads;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      if (!_keyMemberOperators.HasOperator(ImplementedOperators.All))
         return;

      sb.Append(@"
   global::System.Numerics.IMultiplyOperators<").Append(type.TypeFullyQualified).Append(", ").Append(type.TypeFullyQualified).Append(", ").Append(type.TypeFullyQualified).Append(">");

      if (!_withKeyTypeOverloads)
         return;

      sb.Append(@",
   global::System.Numerics.IMultiplyOperators<").Append(type.TypeFullyQualified).Append(", ").Append(keyMember.TypeFullyQualified).Append(", ").Append(type.TypeFullyQualified).Append(">");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      var typeLeftNullCheck = type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var typeLightNullCheck = type.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_keyMemberOperators.HasOperator(ImplementedOperators.Default))
      {
         sb.Append(@"
   /// <inheritdoc cref=""global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)"" />
   public static ").Append(type.TypeFullyQualified).Append(" operator *(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(typeLightNullCheck).Append("return Create(left.").Append(keyMember.Name).Append(" * right.").Append(keyMember.Name).Append(@");
   }");
      }

      if (_keyMemberOperators.HasOperator(ImplementedOperators.Checked))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)"" />
   public static ").Append(type.TypeFullyQualified).Append(" operator checked *(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(typeLightNullCheck).Append("return Create(checked(left.").Append(keyMember.Name).Append(" * right.").Append(keyMember.Name).Append(@"));
   }");
      }

      if (_withKeyTypeOverloads)
         GenerateOverloadsForKeyType(sb, type, keyMember, typeLeftNullCheck, typeLightNullCheck);
   }

   private void GenerateOverloadsForKeyType(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember, string? typeLeftNullCheck, string? typeLightNullCheck)
   {
      var memberLeftNullCheck = keyMember.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var memberRightNullCheck = keyMember.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      if (_keyMemberOperators.HasOperator(ImplementedOperators.Default))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)"" />
   public static ").Append(type.TypeFullyQualified).Append(" operator *(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return Create(left.").Append(keyMember.Name).Append(@" * right);
   }

   /// <inheritdoc cref=""global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)"" />
   public static ").Append(type.TypeFullyQualified).Append(" operator *(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeLightNullCheck).Append("return Create(left * right.").Append(keyMember.Name).Append(@");
   }");
      }

      if (_keyMemberOperators.HasOperator(ImplementedOperators.Checked))
      {
         sb.Append(@"

   /// <inheritdoc cref=""global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)"" />
   public static ").Append(type.TypeFullyQualified).Append(" operator checked *(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
   {
      ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return Create(checked(left.").Append(keyMember.Name).Append(@" * right));
   }

   /// <inheritdoc cref=""global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)"" />
   public static ").Append(type.TypeFullyQualified).Append(" operator checked *(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
   {
      ").Append(memberLeftNullCheck).Append(typeLightNullCheck).Append("return Create(checked(left * right.").Append(keyMember.Name).Append(@"));
   }");
      }
   }
}
