using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class EqualityComparisonOperatorsCodeGenerator : IInterfaceCodeGenerator<EqualityComparisonOperatorsGeneratorState>
{
   private static readonly IInterfaceCodeGenerator<EqualityComparisonOperatorsGeneratorState> _default = new EqualityComparisonOperatorsCodeGenerator(false, null);
   private static readonly IInterfaceCodeGenerator<EqualityComparisonOperatorsGeneratorState> _defaultWithKeyTypeOverloads = new EqualityComparisonOperatorsCodeGenerator(true, null);

   public static bool TryGet(
      OperatorsGeneration operatorsGeneration,
      ComparerInfo? equalityComparer,
      [MaybeNullWhen(false)] out IInterfaceCodeGenerator<EqualityComparisonOperatorsGeneratorState> generator)
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

   public void GenerateBaseTypes(StringBuilder sb, EqualityComparisonOperatorsGeneratorState state)
   {
      sb.Append(@"
   global::System.Numerics.IEqualityOperators<").AppendTypeFullyQualified(state.Type).Append(", ").AppendTypeFullyQualified(state.Type).Append(", bool>");

      if (!_withKeyTypeOverloads || state.KeyMember is null)
         return;

      sb.Append(@",
   global::System.Numerics.IEqualityOperators<").AppendTypeFullyQualified(state.Type).Append(", ").AppendTypeFullyQualified(state.KeyMember).Append(", bool>");
   }

   public void GenerateImplementation(StringBuilder sb, EqualityComparisonOperatorsGeneratorState state)
   {
      GenerateUsingEquals(sb, state);

      if (_withKeyTypeOverloads)
         GenerateKeyOverloads(sb, state);
   }

   private static void GenerateUsingEquals(StringBuilder sb, EqualityComparisonOperatorsGeneratorState state)
   {
      sb.Append(@"
      /// <summary>
      /// Compares two instances of ").AppendTypeForXmlComment(state.Type).Append(@".
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==(").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(" obj, ").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(@" other)
      {");

      if (state.Type.IsReferenceType)
      {
         if (state.Type.IsEqualWithReferenceEquality)
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
      /// Compares two instances of ").AppendTypeForXmlComment(state.Type).Append(@".
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=(").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(" obj, ").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(@" other)
      {
         return !(obj == other);
      }");
   }

   private void GenerateKeyOverloads(
      StringBuilder sb,
      EqualityComparisonOperatorsGeneratorState state)
   {
      if (state.KeyMember is null)
         return;

      sb.Append(@"

      private static bool Equals(").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(" obj, ").AppendTypeFullyQualifiedNullAnnotated(state.KeyMember).Append(@" value)
      {");

      if (state.Type.IsReferenceType)
      {
         if (state.KeyMember.IsReferenceType)
         {
            sb.Append(@"
         if (obj is null)
            return value is null;

         if(value is null)
            return false;
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

      if (_equalityComparer != null)
      {
         sb.Append(_equalityComparer.Value.Comparer);

         if (_equalityComparer.Value.IsAccessor)
            sb.Append(".EqualityComparer");

         sb.Append(".Equals(obj.").Append(state.KeyMember.Name).Append(", value)");
      }
      else if (state.KeyMember.IsString())
      {
         sb.Append("global::System.StringComparer.OrdinalIgnoreCase.Equals(obj.").Append(state.KeyMember.Name).Append(", value)");
      }
      else if (state.KeyMember.IsReferenceType)
      {
         sb.Append("obj.").Append(state.KeyMember.Name).Append(" is null ? value").Append(" is null : obj.").Append(state.KeyMember.Name).Append(".Equals(value").Append(")");
      }
      else
      {
         sb.Append("obj.").Append(state.KeyMember.Name).Append(".Equals(value)");
      }

      sb.Append(@";
      }

      /// <summary>
      /// Compares an instance of ").AppendTypeForXmlComment(state.Type).Append(" with ").AppendTypeForXmlComment(state.KeyMember).Append(@".
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""value"">Value to compare with.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==(").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(" obj, ").AppendTypeFullyQualifiedNullAnnotated(state.KeyMember).Append(@" value)
      {
         return Equals(obj, value);
      }

      /// <summary>
      /// Compares an instance of ").AppendTypeForXmlComment(state.Type).Append(" with ").AppendTypeForXmlComment(state.KeyMember).Append(@".
      /// </summary>
      /// <param name=""value"">Value to compare.</param>
      /// <param name=""obj"">Instance to compare with.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==(").AppendTypeFullyQualifiedNullAnnotated(state.KeyMember).Append(" value, ").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(@" obj)
      {
         return Equals(obj, value);
      }

      /// <summary>
      /// Compares an instance of ").AppendTypeForXmlComment(state.Type).Append(@" with ").AppendTypeForXmlComment(state.KeyMember).Append(@".
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""value"">Value to compare with.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=(").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(" obj, ").AppendTypeFullyQualifiedNullAnnotated(state.KeyMember).Append(@" value)
      {
         return !(obj == value);
      }

      /// <summary>
      /// Compares an instance of ").AppendTypeForXmlComment(state.KeyMember).Append(" with ").AppendTypeForXmlComment(state.Type).Append(@".
      /// </summary>
      /// <param name=""value"">Value to compare.</param>
      /// <param name=""obj"">Instance to compare with.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=(").AppendTypeFullyQualifiedNullAnnotated(state.KeyMember).Append(" value, ").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(@" obj)
      {
         return !(obj == value);
      }");
   }
}
