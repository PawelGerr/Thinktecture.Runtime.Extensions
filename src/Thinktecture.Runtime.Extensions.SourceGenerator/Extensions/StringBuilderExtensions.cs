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
      AccessModifier accessModifier)
   {
      switch (accessModifier)
      {
         case AccessModifier.Private:
            sb.Append("private");
            break;
         case AccessModifier.Protected:
            sb.Append("protected");
            break;
         case AccessModifier.Internal:
            sb.Append("internal");
            break;
         case AccessModifier.Public:
            sb.Append("public");
            break;
         case AccessModifier.PrivateProtected:
            sb.Append("private protected");
            break;
         case AccessModifier.ProtectedInternal:
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
         case Accessibility.NotApplicable:
            break;
         case Accessibility.Private:
            sb.Append("private");
            break;
         case Accessibility.ProtectedAndInternal:
            sb.Append("private protected");
            break;
         case Accessibility.Protected:
            sb.Append("protected");
            break;
         case Accessibility.Internal:
            sb.Append("internal");
            break;
         case Accessibility.ProtectedOrInternal:
            sb.Append("protected internal");
            break;
         case Accessibility.Public:
            sb.Append("public");
            break;
      }

      return sb;
   }

   public static StringBuilder RenderArguments(
      this StringBuilder sb,
      ImmutableArray<InstanceMemberInfo> members,
      string? prefix = null,
      string? comma = ", ",
      bool leadingComma = false)
   {
      for (var i = 0; i < members.Length; i++)
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
      ImmutableArray<InstanceMemberInfo> members,
      string? prefix = null,
      string comma = ", ",
      bool leadingComma = false,
      bool trailingComma = false,
      bool useNullableTypes = false)
   {
      for (var i = 0; i < members.Length; i++)
      {
         if (leadingComma || i > 0)
            sb.Append(comma);

         sb.RenderArgumentWithType(members[i], prefix, useNullableTypes);
      }

      if (trailingComma && members.Length > 0)
         sb.Append(comma);

      return sb;
   }

   public static StringBuilder RenderArgumentWithType(
      this StringBuilder sb,
      IMemberState member,
      string? prefix = null,
      bool useNullableTypes = false)
   {
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

   public static StringBuilder AppendEscaped(
      this StringBuilder sb,
      ArgumentName argName)
   {
      return sb.Append("@").AppendArgumentName(argName);
   }

   public static StringBuilder AppendArgumentName(
      this StringBuilder sb,
      ArgumentName argName)
   {
      var name = argName.Name;

      if (argName.RenderAsIs)
      {
         sb.Append(name);
         return sb;
      }

      if (name.Length == 0)
         return sb;

      var index = 0;

      // Handle leading underscores:
      // - If the name starts with '_' and is followed by a letter, drop all leading underscores.
      // - Otherwise, keep underscores (e.g., "_", "__", or "__1").
      if (name[0] == '_')
      {
         var j = 0;

         while (j < name.Length && name[j] == '_')
         {
            j++;
         }

         if (j < name.Length && Char.IsLetter(name[j]))
         {
            // Drop underscores if immediately followed by a letter
            index = j;
         }
         else
         {
            // Keep underscores as-is and continue processing after them
            sb.Append(name, 0, j);
            index = j;
         }
      }

      // Keep other non-letter prefixes (e.g., digits, '$') verbatim.
      while (index < name.Length && !Char.IsLetter(name[index]))
      {
         sb.Append(name[index]);
         index++;
      }

      // No letters found, we're done.
      if (index >= name.Length)
         return sb;

      // If the first letter is already lowercase, append the remainder as-is.
      if (Char.IsLower(name[index]))
      {
         sb.Append(name, index, name.Length - index);
         return sb;
      }

      // Lowercase with look-ahead to preserve the last uppercase before a lowercase (acronyms).
      // Detect initial consecutive uppercase run starting at 'index'.
      var runEnd = index;

      while (runEnd < name.Length && char.IsUpper(name[runEnd]))
      {
         runEnd++;
      }

      var runLen = runEnd - index;

      if (runLen == 1)
      {
         // Single uppercase followed by non-uppercase -> just lowercase the first letter.
         sb.Append(char.ToLowerInvariant(name[index]));
      }
      else
      {
         // Multi-letter uppercase run.
         var nextIsLower = runEnd < name.Length && char.IsLower(name[runEnd]);

         // Lowercase the first letter.
         sb.Append(char.ToLowerInvariant(name[index]));

         // Lowercase subsequent uppercase letters only while the next char is also uppercase.
         var endExclusive = nextIsLower ? runEnd - 1 : runEnd;

         for (var i2 = index + 1; i2 < endExclusive; i2++)
         {
            sb.Append(char.ToLowerInvariant(name[i2]));
         }

         // Preserve the last uppercase letter that precedes a lowercase (e.g., URLValue -> urlValue).
         if (nextIsLower)
         {
            sb.Append(name[runEnd - 1]);
         }
      }

      // Append the remainder.
      if (runEnd < name.Length)
         sb.Append(name, runEnd, name.Length - runEnd);

      return sb;
   }

   public static StringBuilder AppendBackingFieldName(
      this StringBuilder sb,
      BackingFieldName fieldName)
   {
      var name = fieldName.Name;
      var start = sb.Length;

      if (string.IsNullOrEmpty(name))
      {
         // nothing to append
      }
      else if (name.Length == 1)
      {
         if (name[0] == '_')
         {
            sb.Append(name);
         }
         else
         {
            sb.Append('_').Append(char.ToLowerInvariant(name[0]));
         }
      }
      else
      {
         // Determine leading underscores to preserve (do not add more),
         // but ensure at least one underscore if none exist.
         var underscoreCount = 0;

         while (underscoreCount < name.Length && name[underscoreCount] == '_')
         {
            underscoreCount++;
         }

         if (underscoreCount > 0)
         {
            sb.Append(name, 0, underscoreCount);
         }
         else
         {
            sb.Append('_');
         }

         var remainderIndex = underscoreCount;

         if (remainderIndex >= name.Length)
         {
            // only underscores -> prefix already appended
         }
         else
         {
            // If the remainder starts with non-letter (e.g., digit), keep non-letter prefix,
            // then camelize the first letter segment after that.
            if (!char.IsLetter(name[remainderIndex]))
            {
               // Find first letter
               var index = remainderIndex;

               while (index < name.Length && !char.IsLetter(name[index]))
               {
                  index++;
               }

               if (index == name.Length)
               {
                  // No letters at all -> keep as-is
                  sb.Append(name, remainderIndex, name.Length - remainderIndex);
               }
               else
               {
                  // Append non-letter prefix as-is
                  sb.Append(name, remainderIndex, index - remainderIndex);
                  // Camelize the letter part
                  AppendCamelize(sb, name, index, name.Length - index);
               }
            }
            else
            {
               // Starts with a letter -> camelize the entire remainder
               AppendCamelize(sb, name, remainderIndex, name.Length - remainderIndex);
            }
         }
      }

      // Special case: if the rendered backing field name equals the PropertyName exactly,
      // then prefix the result with an additional underscore to avoid collision.
      var propName = fieldName.PropertyName;
      var writtenLength = sb.Length - start;

      if (propName.Length == writtenLength)
      {
         var equals = true;

         for (var i = 0; i < writtenLength; i++)
         {
            if (sb[start + i] != propName[i])
            {
               equals = false;
               break;
            }
         }

         if (equals)
         {
            sb.Insert(start, "_");
         }
      }

      return sb;

      // Helper that camelizes a string segment starting at a letter, with acronym handling,
      // and appends the result to the provided StringBuilder.
      static void AppendCamelize(StringBuilder sb, string s, int startIndex, int length)
      {
         if (length <= 0)
            return;

         var character = s[startIndex];

         // If first is not a letter, keep as-is
         if (!char.IsLetter(character))
         {
            sb.Append(s, startIndex, length);
            return;
         }

         if (length == 1)
         {
            sb.Append(char.ToLowerInvariant(character));
            return;
         }

         // If second is lowercase -> PascalCase -> lower only first char
         if (char.IsLower(s[startIndex + 1]))
         {
            sb.Append(char.ToLowerInvariant(character));
            sb.Append(s, startIndex + 1, length - 1);
            return;
         }

         // Compute length of initial uppercase run
         var run = 1;
         var end = startIndex + length;

         var i = startIndex + 1;

         while (i < end && char.IsUpper(s[i]))
         {
            run++;
            i++;
         }

         if (run == length)
         {
            // Entire segment is uppercase -> lowercase all
            for (var j = startIndex; j < end; j++)
            {
               sb.Append(char.ToLowerInvariant(s[j]));
            }

            return;
         }

         // Multi-letter uppercase run with a following non-uppercase character.
         // Apply look-ahead rule: lowercase first letter, then lowercase subsequent
         // uppercase letters only while the next character is also uppercase.
         var nextIsLower = run < length && char.IsLower(s[startIndex + run]);

         // Lowercase the first letter.
         sb.Append(char.ToLowerInvariant(s[startIndex]));

         if (run > 1)
         {
            var endExclusive = nextIsLower ? startIndex + run - 1 : startIndex + run;

            for (var j = startIndex + 1; j < endExclusive; j++)
            {
               sb.Append(char.ToLowerInvariant(s[j]));
            }

            // Preserve the last uppercase letter of the run if followed by a lowercase (e.g., URLValue -> urlValue).
            if (nextIsLower)
               sb.Append(s[startIndex + run - 1]);
         }

         // Append the remainder of the segment.
         if (run < length)
         {
            sb.Append(s, startIndex + run, length - run);
         }
      }
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

   public static StringBuilder AppendMemberTypeForXmlComment<T>(
      this StringBuilder sb,
      T member,
      (string Text, string? Separator)? suffix = null)
      where T : IMemberState
   {
      return sb.AppendTypeForXmlComment(member.TypeFullyQualified,
                                        member.IsTypeParameter,
                                        suffix);
   }

   public static StringBuilder AppendTypeForXmlComment(
      this StringBuilder sb,
      ITypeInformation type,
      (string Text, string? Separator)? suffix = null)
   {
      return sb.AppendTypeForXmlComment(type.ContainingTypes.IsDefaultOrEmpty ? type.TypeMinimallyQualified : type.TypeFullyQualified,
                                        false,
                                        suffix);
   }

   public static StringBuilder AppendTypeFullyQualifiedForXmlComment<T>(
      this StringBuilder sb,
      T type,
      (string Text, string? Separator)? suffix = null)
      where T : ITypeFullyQualified
   {
      return sb.AppendTypeForXmlComment(type.TypeFullyQualified, false, suffix);
   }

   private static StringBuilder AppendTypeForXmlComment(
      this StringBuilder sb,
      string qualifiedTypeName,
      bool isGenericTypeParameter,
      (string Text, string? Separator)? suffix)
   {
      if (isGenericTypeParameter)
      {
         sb.Append("<typeparamref name=\"").Append(qualifiedTypeName).Append("\"/>");
         return sb;
      }

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

   public static StringBuilder AppendTypeFullyQualified<T>(
      this StringBuilder sb,
      T type)
      where T : ITypeFullyQualified
   {
      return sb.Append(type.TypeFullyQualified);
   }

   public static StringBuilder AppendTypeFullyQualified<T>(
      this StringBuilder sb,
      T type,
      ImmutableArray<ContainingTypeState> containingTypes)
      where T : INamespaceAndName
   {
      sb.Append("global::");

      if (type.Namespace is not null)
      {
         sb.Append(type.Namespace).Append('.');
      }

      foreach (var containingType in containingTypes)
      {
         sb.Append(containingType.Name).Append('.');
      }

      return sb.Append(type.Name);
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

      if ((type.IsReferenceType || type is { IsTypeParameter: true, IsValueType: false }) && type.NullableAnnotation != NullableAnnotation.Annotated)
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
      ImmutableArray<ContainingTypeState> containingTypes)
   {
      for (var i = 0; i < containingTypes.Length; i++)
      {
         var containingType = containingTypes[i];
         var typeKind = containingType.IsRecord
                           ? containingType.IsReferenceType ? "record " : "record struct "
                           : containingType.IsReferenceType ? "class " : "struct ";

         sb.Append(@"
partial ").Append(typeKind).Append(containingType.Name).AppendGenericTypeParameters(containingType).Append(@"
{");
      }

      return sb;
   }

   public static StringBuilder RenderContainingTypesEnd(
      this StringBuilder sb,
      ImmutableArray<ContainingTypeState> containingTypes)
   {
      for (var i = 0; i < containingTypes.Length; i++)
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

      if (method.Parameters.Length == 0)
      {
         return isFunc
                   ? sb.Append("<").Append(method.ReturnType).Append(">")
                   : sb;
      }

      sb.Append("<");

      for (var i = 0; i < method.Parameters.Length; i++)
      {
         if (i > 0)
            sb.Append(", ");

         sb.Append(method.Parameters[i].Type);
      }

      if (isFunc)
         sb.Append(", ").Append(method.ReturnType);

      return sb.Append(">");
   }

   public static StringBuilder AppendGenericTypeParameters<T>(
      this StringBuilder sb,
      T type,
      bool constructOpenGeneric = false)
      where T : IHasGenerics
   {
      if (type.GenericParameters.IsDefaultOrEmpty)
         return sb;

      sb.Append("<");

      for (var i = 0; i < type.GenericParameters.Length; i++)
      {
         if (i > 0)
            sb.Append(constructOpenGeneric ? "," : ", ");

         if (!constructOpenGeneric)
            sb.Append(type.GenericParameters[i].Name);
      }

      return sb.Append(">");
   }

   public static StringBuilder AppendGenericConstraints(
      this StringBuilder sb,
      IHasGenerics type,
      string prefix)
   {
      if (type.GenericParameters.IsDefaultOrEmpty)
         return sb;

      for (var i = 0; i < type.GenericParameters.Length; i++)
      {
         var parameter = type.GenericParameters[i];

         if (parameter.Constraints.IsDefaultOrEmpty)
            continue;

         sb.Append(@"
").Append(prefix).Append("where ").Append(parameter.Name).Append(" : ").AppendGenericConstraints(parameter.Constraints);
      }

      return sb;
   }

   public static StringBuilder AppendGenericConstraints(
      this StringBuilder sb,
      ImmutableArray<string> constraints)
   {
      if (constraints.IsDefaultOrEmpty)
         return sb;

      for (var i = 0; i < constraints.Length; i++)
      {
         if (i > 0)
            sb.Append(", ");

         sb.Append(constraints[i]);
      }

      return sb;
   }

   public static StringBuilder AppendTypeKind<T>(
      this StringBuilder sb,
      T type)
      where T : ITypeKindInformation
   {
      if (type.IsReferenceType)
      {
         sb.Append(type.IsRecord ? "record" : "class");
      }
      else
      {
         sb.Append(type.IsRecord ? "record struct" : "struct");
      }

      return sb;
   }
}
