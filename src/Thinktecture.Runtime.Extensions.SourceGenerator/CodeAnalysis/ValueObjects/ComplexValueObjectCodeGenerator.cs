using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectCodeGenerator : SmartEnumAndValueObjectCodeGeneratorBase
{
   private readonly ComplexValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   // The generated factory and validation methods declare fixed 'obj', 'validationError' and
   // 'factoryArgumentsValidationError' parameters and locals in the same scope as the members' parameters
   // (for example the partial ValidateFactoryArguments declares 'validationError' and every member as parameters).
   // A member name that renders to one of these identifiers would otherwise collide (CS0100/CS0128/CS0136).
   // These names are part of the user-visible signatures, so they keep their friendly form and are renamed only
   // on collision. The comparison uses the RENDERED identifier, not the raw member name, because
   // AppendArgumentName drops a leading underscore before a letter (so "_Obj" renders to "obj").
   private readonly string _objArgumentName;
   private readonly string _validationErrorArgumentName;
   private readonly string _factoryArgumentsValidationErrorArgumentName;

   public override string CodeGeneratorName => "ValueObject-CodeGenerator";
   public override string FileNameSuffix => ".ComplexValueObject";

   public ComplexValueObjectCodeGenerator(ComplexValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state ?? throw new ArgumentNullException(nameof(state));
      _sb = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));

      var members = _state.AssignableInstanceFieldsAndProperties;
      var renderedMemberNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

      for (var i = 0; i < members.Length; i++)
      {
         renderedMemberNames.Add(StringBuilderExtensions.RenderArgumentName(members[i].ArgumentName));
      }

      _objArgumentName = StringBuilderExtensions.GetNonCollidingName(renderedMemberNames, "obj", "resultObj");
      _validationErrorArgumentName = StringBuilderExtensions.GetNonCollidingName(renderedMemberNames, "validationError", "resultValidationError");
      _factoryArgumentsValidationErrorArgumentName = StringBuilderExtensions.GetNonCollidingName(renderedMemberNames, Constants.Variables.FACTORY_ARGUMENTS_VALIDATION_ERROR, "resultFactoryArgumentsValidationError");
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      _sb.AppendLine(GENERATED_CODE_PREFIX);

      var hasNamespace = _state.Namespace is not null;

      if (hasNamespace)
      {
         _sb.Append(@"
namespace ").Append(_state.Namespace).Append(@"
{");
      }

      _sb.RenderContainingTypesStart(_state.ContainingTypes);

      GenerateValueObject(cancellationToken);

      _sb.RenderContainingTypesEnd(_state.ContainingTypes);

      if (hasNamespace)
      {
         _sb.Append(@"
}");
      }

      _sb.Append(@"
");
   }

   private void GenerateValueObject(CancellationToken cancellationToken)
   {
      _sb.Append(@"
   [global::System.Diagnostics.CodeAnalysis.SuppressMessage(""ThinktectureRuntimeExtensionsAnalyzer"", ""TTRESG1000:Internal Thinktecture.Runtime.Extensions API usage"")]
   ").Append(_state.IsReferenceType ? "sealed " : "readonly ").Append("partial ").AppendTypeKind(_state).Append(" ").Append(_state.Name).AppendGenericTypeParameters(_state).Append(" :");

      if (!_state.Settings.SkipEqualityComparison)
      {
         _sb.Append(@"
      global::System.IEquatable<").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@">,
      global::System.Numerics.IEqualityOperators<").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(_state).Append(", bool>,");
      }

      _sb.Append(@"
      global::Thinktecture.Internal.IMetadataOwner");

      if (_state.DisallowsDefaultValue)
      {
         _sb.Append(@",
      global::Thinktecture.IDisallowDefaultValue");
      }

      _sb.Append(@"
   {
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
      static global::Thinktecture.Internal.Metadata global::Thinktecture.Internal.IMetadataOwner.Metadata { get; }
         = new global::Thinktecture.Internal.Metadata.ComplexValueObject(typeof(").AppendTypeFullyQualified(_state).Append(@"))
         {
            AssignableMembers = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>(
                  global::System.Linq.Enumerable.Select(
                     ((global::System.Linq.Expressions.NewExpression)
                        ((global::System.Linq.Expressions.Expression<global::System.Func<").AppendTypeFullyQualified(_state).Append(@", object>>)
                           (object (").AppendTypeFullyQualified(_state).Append(@" o) => new
                              {").AppendAssignableMembersBody(_state.AssignableInstanceFieldsAndProperties).Append(@"
                              })).Body).Arguments,
                     arg => ((global::System.Linq.Expressions.MemberExpression)arg).Member)
               )
               .AsReadOnly()
         };");

      if (_state is { IsValueType: true, DisallowsDefaultValue: false })
      {
         _sb.Append(@"

      /// <summary>
      /// Default instance of ").AppendTypeForXmlComment(_state).Append(@".
      /// </summary>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public static readonly ").AppendTypeFullyQualified(_state).Append(" ").Append(_state.Settings.DefaultInstancePropertyName).Append(" = default;");
      }

      cancellationToken.ThrowIfCancellationRequested();

      if (!_state.Settings.SkipFactoryMethods)
      {
         var hasAdditionalFactoryParams = !_state.FactoryValidationAdditionalParameters.IsDefaultOrEmpty;

         GenerateValidateMethod(hasAdditionalFactoryParams);
         GenerateCreateMethod();
         GenerateTryCreateMethod();

         if (hasAdditionalFactoryParams)
         {
            GenerateValidateCoreMethod();
            GenerateCreateCoreMethod();
         }

         GenerateValidateFactoryArguments();
         GenerateFactoryPostInit();
      }

      cancellationToken.ThrowIfCancellationRequested();

      GenerateConstructor();

      if (!_state.Settings.SkipEqualityComparison)
      {
         GenerateEqualityOperators();
         GenerateEquals();
         GenerateGetHashCode();
      }

      if (!_state.Settings.SkipToString)
         GenerateToString();

      _sb.Append(@"
   }");
   }

   private void GenerateCreateMethod()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      /// <summary>
      /// Creates an instance of the ").AppendTypeForXmlComment(_state).Append(@" type if the provided values pass validation.
      /// </summary>");

      for (var i = 0; i < fieldsAndProperties.Length; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         _sb.Append(@"
      /// <param name=""").AppendArgumentName(memberInfo.ArgumentName).Append(@""">").Append(memberInfo.Name).Append("</param>");
      }

      _sb.Append(@"
      /// <returns>A newly created ").AppendTypeForXmlComment(_state).Append(@" instance.</returns>
      /// <exception cref=""System.ComponentModel.DataAnnotations.ValidationException"">Thrown when the provided values fail validation.</exception>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public static ").AppendTypeFullyQualified(_state).Append(" ").Append(_state.Settings.CreateFactoryMethodName).Append("(").RenderArgumentsWithType(fieldsAndProperties, prefix: @"
         ", comma: ",").Append(@")
      {
         var ").Append(_validationErrorArgumentName).Append(" = Validate(");

      _sb.RenderArguments(fieldsAndProperties, prefix: @"
            ", comma: ",");

      if (fieldsAndProperties.Length > 0)
         _sb.Append(",");

      _sb.Append(@"
            out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@");

         if (").Append(_validationErrorArgumentName).Append(@" is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(").Append(_validationErrorArgumentName).Append(@".ToString() ?? ""Validation failed."");

         return ").Append(_objArgumentName).Append(@"!;
      }");
   }

   private void GenerateTryCreateMethod()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      /// <summary>
      /// Attempts to create an instance of type ").AppendTypeForXmlComment(_state).Append(@"
      /// if the provided values pass validation.
      /// </summary>");

      for (var i = 0; i < fieldsAndProperties.Length; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         _sb.Append(@"
      /// <param name=""").AppendArgumentName(memberInfo.ArgumentName).Append(@""">").Append(memberInfo.Name).Append("</param>");
      }

      _sb.Append(@"
      /// <param name=""").Append(_objArgumentName).Append(@""">
      /// When this method returns, contains the created instance of type ").AppendTypeForXmlComment(_state).Append(@"
      /// if the operation is successful; otherwise, <c>null</c>.
      /// </param>
      /// <returns>
      /// <c>true</c> if the instance is successfully created; otherwise, <c>false</c>.
      /// </returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

      _sb.Append(@"
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@")
      {
         return ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(");

      _sb.RenderArguments(fieldsAndProperties, prefix: @"
            ", comma: ",");

      if (fieldsAndProperties.Length > 0)
         _sb.Append(",");

      _sb.Append(@"
            out ").Append(_objArgumentName).Append(@",
            out _);
      }");

      _sb.Append(@"

      /// <summary>
      /// Attempts to create an instance of type ").AppendTypeForXmlComment(_state).Append(@"
      /// if the provided values pass validation.
      /// </summary>");

      for (var i = 0; i < fieldsAndProperties.Length; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         _sb.Append(@"
      /// <param name=""").AppendArgumentName(memberInfo.ArgumentName).Append(@""">").Append(memberInfo.Name).Append("</param>");
      }

      _sb.Append(@"
      /// <param name=""").Append(_objArgumentName).Append(@""">
      /// When this method returns, contains the created instance of type ").AppendTypeForXmlComment(_state).Append(@"
      /// if the operation is successful; otherwise, <c>null</c>.
      /// </param>
      /// <param name=""").Append(_validationErrorArgumentName).Append(@""">
      /// When this method returns, contains the ").AppendTypeFullyQualifiedForXmlComment(_state.ValidationError).Append(@"
      /// describing why validation failed, if the operation fails; otherwise, <c>null</c>.
      /// </param>
      /// <returns>
      /// <c>true</c> if the instance is successfully created; otherwise, <c>false</c>.
      /// </returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

      _sb.Append(@"
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@",
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(_validationErrorArgumentName).Append(@")
      {
         ").Append(_validationErrorArgumentName).Append(" = Validate(");

      _sb.RenderArguments(fieldsAndProperties, prefix: @"
            ", comma: ",");

      if (fieldsAndProperties.Length > 0)
         _sb.Append(",");

      _sb.Append(@"
            out ").Append(_objArgumentName).Append(@");

         return ").Append(_validationErrorArgumentName).Append(@" is null;
      }");
   }

   private void GenerateValidateMethod(bool hasAdditionalFactoryParams)
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      /// <summary>
      /// Validates the values and creates an instance of type ").AppendTypeForXmlComment(_state).Append(@" if validation is successful.
      /// </summary>");

      for (var i = 0; i < fieldsAndProperties.Length; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         _sb.Append(@"
      /// <param name=""").AppendArgumentName(memberInfo.ArgumentName).Append(@""">").Append(memberInfo.Name).Append("</param>");
      }

      _sb.Append(@"
      /// <param name=""").Append(_objArgumentName).Append(@""">The created object if validation is successful, otherwise <c>null</c>.</param>
      /// <returns>A validation error if validation fails; otherwise, <c>null</c>.</returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public static ").AppendTypeFullyQualified(_state.ValidationError).Append("? Validate(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

      _sb.Append(@"
         out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@")
      {");

      for (var i = 0; i < fieldsAndProperties.Length; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         if (memberInfo.IsReferenceType && memberInfo.NullableAnnotation == NullableAnnotation.NotAnnotated)
         {
            _sb.Append(@"
         if (").AppendEscaped(memberInfo.ArgumentName).Append(@" is null)
         {
            ").Append(_objArgumentName).Append(@" = default;
            return ").AppendTypeFullyQualified(_state.ValidationError).Append(@".Create(""The member \""").Append(memberInfo.Name).Append(@"\"" of type \""").AppendTypeMinimallyQualified(_state).Append(@"\"" must not be null."");
         }
");
         }
      }

      if (hasAdditionalFactoryParams)
      {
         // The full validation/construction plumbing lives in the generated ValidateCore building block,
         // which threads the user's additional factory parameters into the hook. The public Validate has
         // no access to those values, so it forwards their defaults (one 'default' per additional parameter).
         _sb.Append(@"
         return ").Append(Constants.Methods.VALIDATE).Append("Core(");

         _sb.RenderArguments(fieldsAndProperties, prefix: @"
            ", comma: ",");

         _sb.RenderValidateFactoryAdditionalParametersAsDefaultArguments(
            _state.FactoryValidationAdditionalParameters,
            firstSeparator: fieldsAndProperties.Length > 0 ? @",
            " : @"
            ",
            separator: @",
            ");

         _sb.Append(@",
            out ").Append(_objArgumentName).Append(@");
      }");
         return;
      }

      _sb.Append(@"
         ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(_validationErrorArgumentName).Append(@" = null;

         ");

      GenerateValidateBody(renderAdditionalParametersAsArguments: false);

      _sb.Append(@"

         return ").Append(_validationErrorArgumentName).Append(@";
      }");
   }

   private void GenerateValidateCoreMethod()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      private static ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(Constants.Methods.VALIDATE).Append("Core(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, prefix: @"
         ", comma: ",");

      _sb.RenderValidateFactoryAdditionalParametersAsRequiredParameters(
         _state.FactoryValidationAdditionalParameters,
         firstSeparator: fieldsAndProperties.Length > 0 ? @",
         " : @"
         ",
         separator: @",
         ");

      _sb.Append(@",
         out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@")
      {
         ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(_validationErrorArgumentName).Append(@" = null;

         ");

      GenerateValidateBody(renderAdditionalParametersAsArguments: true);

      _sb.Append(@"

         return ").Append(_validationErrorArgumentName).Append(@";
      }");
   }

   private void GenerateValidateBody(bool renderAdditionalParametersAsArguments)
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("var ").Append(_factoryArgumentsValidationErrorArgumentName).Append(" = ");

      _sb.Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append(@"(
            ref ").Append(_validationErrorArgumentName).RenderArguments(fieldsAndProperties, @"
            ref ", comma: ",", leadingComma: true);

      if (renderAdditionalParametersAsArguments)
      {
         // 'ref validationError' (and possibly members) always precede the extras here, so both the first
         // and subsequent separators carry the inter-element comma.
         _sb.RenderValidateFactoryAdditionalParametersAsArguments(
            _state.FactoryValidationAdditionalParameters,
            firstSeparator: @",
            ",
            separator: @",
            ");
      }

      _sb.Append(@");

         if (").Append(_validationErrorArgumentName).Append(@" is null)
         {
            ").Append(_objArgumentName).Append(" = ");

      GenerateConstructCall();

      _sb.Append(@";
            ").Append(_objArgumentName).Append(".").Append(Constants.Methods.FACTORY_POST_INIT).Append("(");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append(_factoryArgumentsValidationErrorArgumentName);

      _sb.Append(@");
         }
         else
         {
            ").Append(_objArgumentName).Append(@" = default;
         }");
   }

   private void GenerateCreateCoreMethod()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      private static ").AppendTypeFullyQualified(_state).Append(" ").Append(_state.Settings.CreateFactoryMethodName).Append("Core(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, prefix: @"
         ", comma: ",");

      _sb.RenderValidateFactoryAdditionalParametersAsRequiredParameters(
         _state.FactoryValidationAdditionalParameters,
         firstSeparator: fieldsAndProperties.Length > 0 ? @",
         " : @"
         ",
         separator: @",
         ");

      _sb.Append(@")
      {
         var ").Append(_validationErrorArgumentName).Append(" = ").Append(Constants.Methods.VALIDATE).Append("Core(");

      _sb.RenderArguments(fieldsAndProperties, prefix: @"
            ", comma: ",");

      _sb.RenderValidateFactoryAdditionalParametersAsArguments(
         _state.FactoryValidationAdditionalParameters,
         firstSeparator: fieldsAndProperties.Length > 0 ? @",
            " : @"
            ",
         separator: @",
            ");

      _sb.Append(@",
            out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@");

         if (").Append(_validationErrorArgumentName).Append(@" is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(").Append(_validationErrorArgumentName).Append(@".ToString() ?? ""Validation failed."");

         return ").Append(_objArgumentName).Append(@"!;
      }");
   }

   private void GenerateValidateFactoryArguments()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("private ");

      _sb.Append("static partial ").Append(_state.FactoryValidationReturnType ?? "void").Append(" ").Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append(@"(
         ref ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(_validationErrorArgumentName);

      _sb.RenderArgumentsWithType(fieldsAndProperties, "ref ", comma: @",
         ", leadingComma: true);

      _sb.RenderValidateFactoryAdditionalParameters(_state.FactoryValidationAdditionalParameters, @",
         ");

      _sb.Append(");");
   }

   private void GenerateFactoryPostInit()
   {
      _sb.Append(@"

      partial void ").Append(Constants.Methods.FACTORY_POST_INIT).Append("(");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append(_state.FactoryValidationReturnType).Append(" ").Append(_factoryArgumentsValidationErrorArgumentName);

      _sb.Append(");");
   }

   private void GenerateConstructCall()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append("new ").AppendTypeFullyQualified(_state).Append("(");
      _sb.RenderArguments(fieldsAndProperties, prefix: @"
               ", comma: ",");

      _sb.Append(")");
   }

   private void GenerateEqualityOperators()
   {
      _sb.Append(@"

      /// <summary>
      /// Compares two instances of ").AppendTypeForXmlComment(_state).Append(@".
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static bool operator ==(").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" obj, ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" other)
      {");

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
         if (obj is null)
            return other is null;
");
      }

      _sb.Append(@"
         return obj.Equals(other);
      }

      /// <summary>
      /// Compares two instances of ").AppendTypeForXmlComment(_state).Append(@".
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static bool operator !=(").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" obj, ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" other)
      {
         return !(obj == other);
      }");
   }

   private void GenerateConstructor()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      var isStructDefaultCtor = !_state.IsReferenceType && fieldsAndProperties.Length == 0;

      if (isStructDefaultCtor)
         return;

      _sb.Append(@"

      /// <summary>
      /// Initializes a new instance of the ").AppendTypeForXmlComment(_state).Append(@" type.
      /// </summary>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      ").RenderAccessModifier(_state.Settings.ConstructorAccessModifier).Append(" ").Append(_state.Name).Append("(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, prefix: @"
         ", comma: ",");

      _sb.Append(@")
      {");

      if (fieldsAndProperties.Length > 0)
      {
         _sb.Append(@"
         ValidateConstructorArguments(");

         _sb.RenderArguments(fieldsAndProperties, @"
            ref ", comma: ",");

         _sb.Append(@");
");

         foreach (var memberInfo in fieldsAndProperties)
         {
            _sb.Append(@"
         this.").AppendIdentifier(memberInfo.Name).Append(" = ").AppendEscaped(memberInfo.ArgumentName).Append(";");
         }
      }

      _sb.Append(@"
      }");

      if (fieldsAndProperties.Length > 0)
      {
         _sb.Append(@"

      static partial void ValidateConstructorArguments(");

         _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ref ", comma: ",");

         _sb.Append(");");
      }
   }

   private void GenerateEquals()
   {
      _sb.Append(@"

      /// <inheritdoc />
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override bool Equals(object? other)
      {
         return other is ").AppendTypeFullyQualified(_state).Append(@" obj && Equals(obj);
      }

      /// <inheritdoc />
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public bool Equals(").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" other)
      {");

      if (_state.IsReferenceType || _state is { IsTypeParameter: true, IsValueType: false })
      {
         _sb.Append(@"
         if (other is null)
            return false;

         if (global::System.Object.ReferenceEquals(this, other))
            return true;
");
      }

      if (_state.EqualityMembers.Length > 0)
      {
         for (var i = 0; i < _state.EqualityMembers.Length; i++)
         {
            var (member, equalityComparerAccessor) = _state.EqualityMembers[i];

            if (i == 0)
            {
               _sb.Append(@"
         return ");
            }
            else
            {
               _sb.Append(@"
             && ");
            }

            if (equalityComparerAccessor != null)
            {
               _sb.Append(equalityComparerAccessor).Append(".EqualityComparer.Equals(this.").AppendIdentifier(member.Name).Append(", other.").AppendIdentifier(member.Name).Append(")");
            }
            else if (member.IsString())
            {
               _sb.Append("global::System.StringComparer.").Append(GetDefaultStringComparer()).Append(".Equals(this.").AppendIdentifier(member.Name).Append(", other.").AppendIdentifier(member.Name).Append(")");
            }
            else
            {
               if (member.IsReferenceType || member is { IsTypeParameter: true, IsValueType: false })
               {
                  _sb.Append("(this.").AppendIdentifier(member.Name).Append(" is null ? other.").AppendIdentifier(member.Name).Append(" is null : this.").AppendIdentifier(member.Name).Append(".Equals(other.").AppendIdentifier(member.Name).Append("))");
               }
               else
               {
                  _sb.Append("this.").AppendIdentifier(member.Name).Append(".Equals(other.").AppendIdentifier(member.Name).Append(")");
               }
            }
         }

         _sb.Append(";");
      }
      else
      {
         _sb.Append(@"
         return true;");
      }

      _sb.Append(@"
      }");
   }

   private string GetDefaultStringComparer()
   {
      var comparer = _state.Settings.DefaultStringComparison switch
      {
         StringComparison.Ordinal => "Ordinal",
         StringComparison.OrdinalIgnoreCase => "OrdinalIgnoreCase",
         StringComparison.CurrentCulture => "CurrentCulture",
         StringComparison.CurrentCultureIgnoreCase => "CurrentCultureIgnoreCase",
         StringComparison.InvariantCulture => "InvariantCulture",
         StringComparison.InvariantCultureIgnoreCase => "InvariantCultureIgnoreCase",
         _ => "OrdinalIgnoreCase"
      };
      return comparer;
   }

   private void GenerateGetHashCode()
   {
      _sb.Append(@"

      /// <inheritdoc />
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override int GetHashCode()
      {");

      if (_state.EqualityMembers.Length > 0)
      {
         var useShortForm = _state.EqualityMembers.Length < 8 && _state.EqualityMembers.All(m => m.EqualityComparerAccessor == null && !m.Member.IsString());

         if (useShortForm)
         {
            _sb.Append(@"
         return global::System.HashCode.Combine(");
         }
         else
         {
            _sb.Append(@"
         var hashCode = new global::System.HashCode();");
         }

         for (var i = 0; i < _state.EqualityMembers.Length; i++)
         {
            var (member, equalityComparerAccessor) = _state.EqualityMembers[i];

            if (useShortForm)
            {
               if (i > 0)
                  _sb.Append(",");

               _sb.Append(@"
            this.").AppendIdentifier(member.Name);
            }
            else
            {
               _sb.Append(@"
         hashCode.Add(this.").AppendIdentifier(member.Name);

               if (equalityComparerAccessor is not null)
               {
                  _sb.Append(", ").Append(equalityComparerAccessor).Append(".EqualityComparer");

                  if (member is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated })
                     _sb.Append("!");
               }
               else if (member.IsString())
               {
                  _sb.Append(", global::System.StringComparer.").Append(GetDefaultStringComparer());

                  if (member is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated })
                     _sb.Append("!");
               }

               _sb.Append(");");
            }
         }

         if (useShortForm)
         {
            _sb.Append(");");
         }
         else
         {
            _sb.Append(@"
         return hashCode.ToHashCode();");
         }
      }
      else
      {
         _sb.Append(@"
         return 0;");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateToString()
   {
      _sb.Append(@"

      /// <inheritdoc />
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override string ToString()
      {");

      if (_state.EqualityMembers.Length > 0)
      {
         _sb.Append(@"
         return $""{{");

         for (var i = 0; i < _state.EqualityMembers.Length; i++)
         {
            var member = _state.EqualityMembers[i].Member;

            if (i > 0)
               _sb.Append(',');

            _sb.Append(" ").Append(member.Name).Append(" = {this.").AppendIdentifier(member.Name).Append("}");
         }

         _sb.Append(" }}\";");
      }
      else
      {
         _sb.Append(@"
         return """).AppendTypeMinimallyQualified(_state).Append(@""";");
      }

      _sb.Append(@"
      }");
   }
}

file static class Extensions
{
   public static StringBuilder AppendAssignableMembersBody(this StringBuilder sb, ImmutableArray<InstanceMemberInfo> members)
   {
      for (var i = 0; i < members.Length; i++)
      {
         var member = members[i];

         if (i > 0)
            sb.Append(",");

         sb.Append(@"
                                 o.").AppendIdentifier(member.Name);
      }

      return sb;
   }
}
