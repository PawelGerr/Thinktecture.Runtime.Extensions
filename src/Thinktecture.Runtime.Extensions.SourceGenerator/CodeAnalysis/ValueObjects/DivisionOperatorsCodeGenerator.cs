using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class DivisionOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   private static readonly IInterfaceCodeGenerator _default = new DivisionOperatorsCodeGenerator(false);
   private static readonly IInterfaceCodeGenerator _defaultWithKeyTypeOverloads = new DivisionOperatorsCodeGenerator(true);

   public static bool TryGet(OperatorsGeneration operatorsGeneration, [MaybeNullWhen(false)] out IInterfaceCodeGenerator generator)
   {
      switch (operatorsGeneration)
      {
         case OperatorsGeneration.None:
            generator = null;
            return false;
         case OperatorsGeneration.Default:
            generator = _default;
            return true;
         case OperatorsGeneration.DefaultWithKeyTypeOverloads:
            generator = _defaultWithKeyTypeOverloads;
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

   private DivisionOperatorsCodeGenerator(bool withKeyTypeOverloads)
   {
      _withKeyTypeOverloads = withKeyTypeOverloads;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      sb.Append(@"
      global::System.Numerics.IDivisionOperators<").Append(type.TypeFullyQualified).Append(", ").Append(type.TypeFullyQualified).Append(", ").Append(type.TypeFullyQualified).Append(">");

      if (_withKeyTypeOverloads)
      {
         sb.Append(@",
      global::System.Numerics.IDivisionOperators<").Append(type.TypeFullyQualified).Append(", ").Append(keyMember.TypeFullyQualified).Append(", ").Append(type.TypeFullyQualified).Append(">");
      }
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      var typeLeftNullCheck = type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var typeLightNullCheck = type.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      sb.Append(@"

      /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
      public static ").Append(type.TypeFullyQualified).Append(" operator /(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
      {
         ").Append(typeLeftNullCheck).Append(typeLightNullCheck).Append("return Create(left.").Append(keyMember.Name).Append(" / right.").Append(keyMember.Name).Append(@");
      }

      /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
      public static ").Append(type.TypeFullyQualified).Append(" operator checked /(").Append(type.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
      {
         ").Append(typeLeftNullCheck).Append(typeLightNullCheck).Append("return Create(checked(left.").Append(keyMember.Name).Append(" / right.").Append(keyMember.Name).Append(@"));
      }");

      if (_withKeyTypeOverloads)
      {
         var memberLeftNullCheck = keyMember.IsReferenceType ? _LEFT_NULL_CHECK : null;
         var memberRightNullCheck = keyMember.IsReferenceType ? _RIGHT_NULL_CHECK : null;

         sb.Append(@"

      /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
      public static ").Append(type.TypeFullyQualified).Append(" operator /(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
      {
         ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return Create(left.").Append(keyMember.Name).Append(@" / right);
      }

      /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
      public static ").Append(type.TypeFullyQualified).Append(" operator /(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
      {
         ").Append(memberLeftNullCheck).Append(typeLightNullCheck).Append("return Create(left / right.").Append(keyMember.Name).Append(@");
      }

      /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
      public static ").Append(type.TypeFullyQualified).Append(" operator checked /(").Append(type.TypeFullyQualified).Append(" left, ").Append(keyMember.TypeFullyQualified).Append(@" right)
      {
         ").Append(typeLeftNullCheck).Append(memberRightNullCheck).Append("return Create(checked(left.").Append(keyMember.Name).Append(@" / right));
      }

      /// <inheritdoc cref=""global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"" />
      public static ").Append(type.TypeFullyQualified).Append(" operator checked /(").Append(keyMember.TypeFullyQualified).Append(" left, ").Append(type.TypeFullyQualified).Append(@" right)
      {
         ").Append(memberLeftNullCheck).Append(typeLightNullCheck).Append("return Create(checked(left / right.").Append(keyMember.Name).Append(@"));
      }");
      }
   }
}
