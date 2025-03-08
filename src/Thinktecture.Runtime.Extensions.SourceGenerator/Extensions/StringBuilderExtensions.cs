using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class StringBuilderExtensions
{
   public static void GenerateStructLayoutAttributeIfRequired(this StringBuilder sb, bool isReferenceType, bool hasStructLayoutAttribute)
   {
      if (!isReferenceType && !hasStructLayoutAttribute)
      {
         sb.Append(@"
   [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]");
      }
   }

   public static StringBuilder AppendConversionOperator(
      this StringBuilder sb,
      ConversionOperatorsGeneration operatorsGeneration)
   {
      switch (operatorsGeneration)
      {
         case ConversionOperatorsGeneration.None:
            break;
         case ConversionOperatorsGeneration.Implicit:
            sb.Append("implicit");
            break;
         case ConversionOperatorsGeneration.Explicit:
            sb.Append("explicit");
            break;
      }

      return sb;
   }

   public static StringBuilder RenderAccessModifier(
      this StringBuilder sb,
      ValueObjectAccessModifier accessModifier)
   {
      switch (accessModifier)
      {
         case ValueObjectAccessModifier.Private:
            sb.Append("private");
            break;
         case ValueObjectAccessModifier.Protected:
            sb.Append("protected");
            break;
         case ValueObjectAccessModifier.Internal:
            sb.Append("internal");
            break;
         case ValueObjectAccessModifier.Public:
            sb.Append("public");
            break;
         case ValueObjectAccessModifier.PrivateProtected:
            sb.Append("private protected");
            break;
         case ValueObjectAccessModifier.ProtectedInternal:
            sb.Append("protected internal");
            break;
      }

      return sb;
   }

   public static StringBuilder AppendAccessModifier(
      this StringBuilder sb,
      UnionConstructorAccessModifier accessModifier)
   {
      switch (accessModifier)
      {
         case UnionConstructorAccessModifier.Private:
            sb.Append("private");
            break;
         case UnionConstructorAccessModifier.Internal:
            sb.Append("internal");
            break;
         case UnionConstructorAccessModifier.Public:
            sb.Append("public");
            break;
      }

      return sb;
   }

   public static StringBuilder AppendAccessibility(
      this StringBuilder sb,
      Accessibility accessibility)
   {
      switch (accessibility)
      {
         case Accessibility.Private:
            sb.Append("private");
            break;
         case Accessibility.ProtectedAndInternal:
            sb.Append("protected internal");
            break;
         case Accessibility.Protected:
            sb.Append("protected");
            break;
         case Accessibility.Internal:
            sb.Append("internal");
            break;
         case Accessibility.Public:
            sb.Append("public");
            break;
      }

      return sb;
   }

   public static StringBuilder RenderArguments(
      this StringBuilder sb,
      IReadOnlyList<InstanceMemberInfo> members,
      string? prefix = null,
      string? comma = ", ",
      bool leadingComma = false)
   {
      for (var i = 0; i < members.Count; i++)
      {
         if (leadingComma || i > 0)
            sb.Append(comma);

         sb.RenderArgument(members[i], prefix);
      }

      return sb;
   }

   public static StringBuilder RenderArgument(
      this StringBuilder sb,
      IMemberState member,
      string? prefix = null)
   {
      return sb.Append(prefix).AppendEscaped(member.ArgumentName);
   }

   public static StringBuilder RenderArgumentsWithType(
      this StringBuilder sb,
      IReadOnlyList<InstanceMemberInfo> members,
      string? prefix = null,
      string comma = ", ",
      bool leadingComma = false,
      bool trailingComma = false,
      bool useNullableTypes = false,
      bool addAllowNullNotNullCombi = false)
   {
      for (var i = 0; i < members.Count; i++)
      {
         if (leadingComma || i > 0)
            sb.Append(comma);

         sb.RenderArgumentWithType(members[i], prefix, useNullableTypes, addAllowNullNotNullCombi);
      }

      if (trailingComma && members.Count > 0)
         sb.Append(comma);

      return sb;
   }

   public static StringBuilder RenderArgumentWithType(
      this StringBuilder sb,
      IMemberState member,
      string? prefix = null,
      bool useNullableTypes = false,
      bool addAllowNullNotNullCombi = false)
   {
      if (addAllowNullNotNullCombi && member.IsReferenceType && member.NullableAnnotation != NullableAnnotation.Annotated)
         sb.Append("[global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ");

      sb.Append(prefix).AppendTypeFullyQualified(member);

      if (useNullableTypes && !member.IsNullableStruct)
         sb.Append("?");

      return sb.Append(' ').AppendEscaped(member.ArgumentName);
   }

   public static StringBuilder AppendEscaped(
      this StringBuilder sb,
      string argName)
   {
      return sb.Append("@").Append(argName);
   }

   public static StringBuilder AppendCast(
      this StringBuilder sb,
      ITypeFullyQualified type,
      bool condition = true)
   {
      if (condition)
         sb.Append("(").AppendTypeFullyQualified(type).Append(")");

      return sb;
   }

   public static StringBuilder AppendTypeForXmlComment(
      this StringBuilder sb,
      ITypeInformation type,
      (string Text, string? Separator)? suffix = null)
   {
      return sb.AppendTypeForXmlComment(type.TypeMinimallyQualified,
                                        suffix);
   }

   public static StringBuilder AppendTypeForXmlComment(
      this StringBuilder sb,
      ITypeFullyQualified type,
      (string Text, string? Separator)? suffix = null)
   {
      return sb.AppendTypeForXmlComment(type.TypeFullyQualified, suffix);
   }

   private static StringBuilder AppendTypeForXmlComment(
      this StringBuilder sb,
      string qualifiedTypeName,
      (string Text, string? Separator)? suffix)
   {
      var hasProblematicCharsForXmlDocs = qualifiedTypeName.IndexOfAny(['<', '[', '?']) > -1;

      if (hasProblematicCharsForXmlDocs)
      {
         sb.Append("<c>").Append(qualifiedTypeName.Replace("<", "&lt;").Replace(">", "&gt;"));
      }
      else
      {
         sb.Append(@"<see cref=""").Append(qualifiedTypeName);
      }

      if (suffix is not null)
         sb.Append(suffix.Value.Separator).Append(suffix.Value.Text);

      return sb.Append(hasProblematicCharsForXmlDocs ? "</c>" : @"""/>");
   }

   public static StringBuilder AppendTypeMinimallyQualified(
      this StringBuilder sb,
      ITypeMinimallyQualified type)
   {
      return sb.Append(type.TypeMinimallyQualified);
   }

   public static StringBuilder AppendTypeFullyQualified(
      this StringBuilder sb,
      ITypeFullyQualified type)
   {
      return sb.Append(type.TypeFullyQualified);
   }

   public static StringBuilder AppendTypeFullyQualified(
      this StringBuilder sb,
      ITypeInformationWithNullability type,
      bool nullable)
   {
      return nullable
                ? sb.AppendTypeFullyQualifiedNullable(type)
                : sb.AppendTypeFullyQualified(type);
   }

   public static StringBuilder AppendTypeFullyQualifiedWithoutNullAnnotation(
      this StringBuilder sb,
      ITypeInformationWithNullability type)
   {
      sb.AppendTypeFullyQualified(type);

      if (type is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated })
         sb.Length -= 1;

      return sb;
   }

   public static StringBuilder AppendTypeFullyQualifiedNullAnnotated(
      this StringBuilder sb,
      ITypeInformationWithNullability type)
   {
      sb.AppendTypeFullyQualified(type);

      if (type.IsReferenceType && type.NullableAnnotation != NullableAnnotation.Annotated)
         sb.Append("?");

      return sb;
   }

   public static StringBuilder AppendTypeFullyQualifiedNullable(
      this StringBuilder sb,
      ITypeInformationWithNullability type)
   {
      sb.Append(type.TypeFullyQualified);

      if ((type.IsReferenceType && type.NullableAnnotation != NullableAnnotation.Annotated) || type is { IsReferenceType: false, IsNullableStruct: false })
         sb.Append("?");

      return sb;
   }

   public static StringBuilder RenderContainingTypesStart(
      this StringBuilder sb,
      IReadOnlyList<ContainingTypeState> containingTypes)
   {
      for (var i = 0; i < containingTypes.Count; i++)
      {
         var containingType = containingTypes[i];
         var typeKind = containingType.IsRecord
                           ? containingType.IsReferenceType ? "record " : "record struct "
                           : containingType.IsReferenceType ? "class " : "struct ";

         sb.Append(@"
partial ").Append(typeKind).Append(containingType.Name).Append(@"
{");
      }

      return sb;
   }

   public static StringBuilder RenderContainingTypesEnd(
      this StringBuilder sb,
      IReadOnlyList<ContainingTypeState> containingTypes)
   {
      for (var i = 0; i < containingTypes.Count; i++)
      {
         sb.Append(@"
}");
      }

      return sb;
   }

   public static StringBuilder AppendRefKindParameterPrefix(
      this StringBuilder sb,
      RefKind refKind)
   {
      var kind = refKind switch
      {
         RefKind.Out => "out ",
         RefKind.Ref => "ref ",
         RefKind.In => "in ",
         RefKind.RefReadOnlyParameter => "ref readonly ",
         _ => null
      };

      return sb.Append(kind);
   }

   public static StringBuilder AppendRefKindArgumentPrefix(
      this StringBuilder sb,
      RefKind refKind)
   {
      var kind = refKind switch
      {
         RefKind.Out => "out ",
         RefKind.Ref => "ref ",
         RefKind.In => "in ",
         RefKind.RefReadOnlyParameter => "in ",
         _ => null
      };

      return sb.Append(kind);
   }

   public static StringBuilder AppendDelegateType(
      this StringBuilder sb,
      DelegateMethodState method)
   {
      if (method.NeedsCustomDelegate())
      {
         return method.DelegateName is not null
                   ? sb.Append(method.DelegateName)
                   : sb.Append(method.MethodName).Append("Delegate");
      }

      // Use standard delegates if no reference parameters
      var isFunc = method.ReturnType is not null;

      if (isFunc)
      {
         sb.Append("global::System.Func");
      }
      else
      {
         sb.Append("global::System.Action");
      }

      if (method.Parameters.Count == 0)
      {
         return isFunc
                   ? sb.Append("<").Append(method.ReturnType).Append(">")
                   : sb;
      }

      sb.Append("<");

      for (var i = 0; i < method.Parameters.Count; i++)
      {
         if (i > 0)
            sb.Append(", ");

         sb.Append(method.Parameters[i].Type);
      }

      if (isFunc)
         sb.Append(", ").Append(method.ReturnType);

      return sb.Append(">");
   }

   public static StringBuilder AppendGenericTypeParameters(
      this StringBuilder sb,
      IReadOnlyList<GenericTypeParameterState> typeParameters)
   {
      if (typeParameters.Count <= 0)
         return sb;

      sb.Append("<");

      for (var i = 0; i < typeParameters.Count; i++)
      {
         if (i > 0)
            sb.Append(", ");

         sb.Append(typeParameters[i].Name);
      }

      return sb.Append(">");
   }

   public static StringBuilder AppendGenericConstraints(
      this StringBuilder sb,
      IReadOnlyList<string> constraints)
   {
      if (constraints.Count <= 0)
         return sb;

      for (var i = 0; i < constraints.Count; i++)
      {
         if (i > 0)
            sb.Append(", ");

         sb.Append(constraints[i]);
      }

      return sb;
   }
}
