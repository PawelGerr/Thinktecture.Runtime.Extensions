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
      sb.Append(@$"
      global::System.Numerics.IDivisionOperators<{type.TypeFullyQualified}, {type.TypeFullyQualified}, {type.TypeFullyQualified}>");

      if (_withKeyTypeOverloads)
      {
         sb.Append(@$",
      global::System.Numerics.IDivisionOperators<{type.TypeFullyQualified}, {keyMember.TypeFullyQualified}, {type.TypeFullyQualified}>");
      }
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      var typeLeftNullCheck = type.IsReferenceType ? _LEFT_NULL_CHECK : null;
      var typeLightNullCheck = type.IsReferenceType ? _RIGHT_NULL_CHECK : null;

      sb.Append($$"""


      /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
      public static {{type.TypeFullyQualified}} operator /({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{typeLightNullCheck}}return Create(left.{{keyMember.Name}} / right.{{keyMember.Name}});
      }

      /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
      public static {{type.TypeFullyQualified}} operator checked /({{type.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{typeLightNullCheck}}return Create(checked(left.{{keyMember.Name}} / right.{{keyMember.Name}}));
      }
""");

      if (_withKeyTypeOverloads)
      {
         var memberLeftNullCheck = keyMember.IsReferenceType ? _LEFT_NULL_CHECK : null;
         var memberRightNullCheck = keyMember.IsReferenceType ? _RIGHT_NULL_CHECK : null;

         sb.Append($$"""


      /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
      public static {{type.TypeFullyQualified}} operator /({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return Create(left.{{keyMember.Name}} / right);
      }

      /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
      public static {{type.TypeFullyQualified}} operator /({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeLightNullCheck}}return Create(left / right.{{keyMember.Name}});
      }

      /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
      public static {{type.TypeFullyQualified}} operator checked /({{type.TypeFullyQualified}} left, {{keyMember.TypeFullyQualified}} right)
      {
         {{typeLeftNullCheck}}{{memberRightNullCheck}}return Create(checked(left.{{keyMember.Name}} / right));
      }

      /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
      public static {{type.TypeFullyQualified}} operator checked /({{keyMember.TypeFullyQualified}} left, {{type.TypeFullyQualified}} right)
      {
         {{memberLeftNullCheck}}{{typeLightNullCheck}}return Create(checked(left / right.{{keyMember.Name}}));
      }
""");
      }
   }
}
