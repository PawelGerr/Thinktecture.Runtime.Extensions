using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class EqualityComparisonOperatorsCodeGenerator : IInterfaceCodeGenerator
{
   private static readonly IInterfaceCodeGenerator _default = new EqualityComparisonOperatorsCodeGenerator(false, null);
   private static readonly IInterfaceCodeGenerator _defaultWithKeyTypeOverloads = new EqualityComparisonOperatorsCodeGenerator(true, null);

   public static bool TryGet(
      OperatorsGeneration operatorsGeneration,
      ComparerInfo? equalityComparer,
      [MaybeNullWhen(false)] out IInterfaceCodeGenerator generator)
   {
      switch (operatorsGeneration)
      {
         case OperatorsGeneration.None:
            generator = null;
            return false;
         case OperatorsGeneration.Default:
            generator = equalityComparer is null
                           ? _default
                           : new EqualityComparisonOperatorsCodeGenerator(false, equalityComparer);
            return true;
         case OperatorsGeneration.DefaultWithKeyTypeOverloads:
            generator = equalityComparer is null
                           ? _defaultWithKeyTypeOverloads
                           : new EqualityComparisonOperatorsCodeGenerator(true, equalityComparer);
            return true;
         default:
            throw new ArgumentOutOfRangeException(nameof(operatorsGeneration), operatorsGeneration, "Invalid operations generation.");
      }
   }

   private readonly bool _withKeyTypeOverloads;
   private readonly ComparerInfo? _equalityComparer;

   public string CodeGeneratorName => "EqualityComparisonOperators-CodeGenerator";
   public string FileNameSuffix => ".EqualityComparisonOperators";

   private EqualityComparisonOperatorsCodeGenerator(
      bool withKeyTypeOverloads,
      ComparerInfo? equalityComparer)
   {
      _withKeyTypeOverloads = withKeyTypeOverloads;
      _equalityComparer = equalityComparer;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      sb.Append(@"
   global::System.Numerics.IEqualityOperators<").Append(type.TypeFullyQualified).Append(", ").Append(type.TypeFullyQualified).Append(", bool>");

      if (!_withKeyTypeOverloads)
         return;

      sb.Append(@",
   global::System.Numerics.IEqualityOperators<").Append(type.TypeFullyQualified).Append(", ").Append(keyMember.TypeFullyQualified).Append(", bool>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      GenerateUsingEquals(sb, type);

      if (_withKeyTypeOverloads)
         GenerateKeyOverloads(sb, type, keyMember);
   }

   private static void GenerateUsingEquals(StringBuilder sb, ITypeInformation type)
   {
      sb.Append(@"
      /// <summary>
      /// Compares two instances of <see cref=""").Append(type.TypeMinimallyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==(").Append(type.TypeFullyQualifiedNullAnnotated).Append(" obj, ").Append(type.TypeFullyQualifiedNullAnnotated).Append(@" other)
      {");

      if (type.IsReferenceType)
      {
         if (type.IsEqualWithReferenceEquality)
         {
            sb.Append(@"
         return global::System.Object.ReferenceEquals(obj, other);");
         }
         else
         {
            sb.Append(@"
         if (obj is null)
            return other is null;

         return obj.Equals(other);");
         }
      }
      else
      {
         sb.Append(@"
         return obj.Equals(other);");
      }

      sb.Append(@"
      }

      /// <summary>
      /// Compares two instances of <see cref=""").Append(type.TypeMinimallyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=(").Append(type.TypeFullyQualifiedNullAnnotated).Append(" obj, ").Append(type.TypeFullyQualifiedNullAnnotated).Append(@" other)
      {
         return !(obj == other);
      }");
   }

   private void GenerateKeyOverloads(
      StringBuilder sb,
      ITypeInformation type,
      IMemberInformation keyMember)
   {
      sb.Append(@"

      private static bool Equals(").Append(type.TypeFullyQualifiedNullAnnotated).Append(" obj, ").Append(keyMember.TypeFullyQualified).Append(@" value)
      {");

      if (type.IsReferenceType)
      {
         if (keyMember.IsReferenceType)
         {
            sb.Append(@"
         if (obj is null)
            return value is null;
");
         }
         else
         {
            sb.Append(@"
         if (obj is null)
            return false;
");
         }
      }

      sb.Append(@"
         return ");

      if (_equalityComparer == null)
      {
         if (keyMember.IsReferenceType)
         {
            sb.Append("obj.").Append(keyMember.Name).Append(" is null ? value").Append(" is null : obj.").Append(keyMember.Name).Append(".Equals(value").Append(")");
         }
         else
         {
            sb.Append("obj.").Append(keyMember.Name).Append(".Equals(value)");
         }
      }
      else
      {
         sb.Append(_equalityComparer.Value.Comparer);

         if (_equalityComparer.Value.IsAccessor)
            sb.Append(".EqualityComparer");

         sb.Append(".Equals(obj.").Append(keyMember.Name).Append(", value)");
      }

      sb.Append(@";
      }

      /// <summary>
      /// Compares an instance of <see cref=""").Append(type.TypeMinimallyQualified).Append(@"""/> with <see cref=""").Append(keyMember.TypeFullyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""value"">Value to compare with.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==(").Append(type.TypeFullyQualifiedNullAnnotated).Append(" obj, ").Append(keyMember.TypeFullyQualified).Append(@" value)
      {
         return Equals(obj, value);
      }

      /// <summary>
      /// Compares an instance of <see cref=""").Append(type.TypeMinimallyQualified).Append(@"""/> with <see cref=""").Append(keyMember.TypeFullyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""value"">Value to compare.</param>
      /// <param name=""obj"">Instance to compare with.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==(").Append(keyMember.TypeFullyQualified).Append(" value, ").Append(type.TypeFullyQualifiedNullAnnotated).Append(@" obj)
      {
         return Equals(obj, value);
      }

      /// <summary>
      /// Compares an instance of <see cref=""").Append(type.TypeMinimallyQualified).Append(@"""/> with <see cref=""").Append(keyMember.TypeFullyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""value"">Value to compare with.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=(").Append(type.TypeFullyQualifiedNullAnnotated).Append(" obj, ").Append(keyMember.TypeFullyQualified).Append(@" value)
      {
         return !(obj == value);
      }

      /// <summary>
      /// Compares an instance of <see cref=""").Append(keyMember.TypeFullyQualified).Append(@"""/> with <see cref=""").Append(type.TypeMinimallyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""value"">Value to compare.</param>
      /// <param name=""obj"">Instance to compare with.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=(").Append(keyMember.TypeFullyQualified).Append(" value, ").Append(type.TypeFullyQualifiedNullAnnotated).Append(@" obj)
      {
         return !(obj == value);
      }");
   }
}
