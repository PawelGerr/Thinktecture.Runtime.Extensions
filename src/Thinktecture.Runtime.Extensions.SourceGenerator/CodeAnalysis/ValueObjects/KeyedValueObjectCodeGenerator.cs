using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class KeyedValueObjectCodeGenerator : SmartEnumAndValueObjectCodeGeneratorBase
{
   private readonly KeyedValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   // The generated factory and validation methods declare fixed 'obj', 'validationError' and 'provider' parameters
   // and locals in the same scope as the user's key-member parameter (for example the partial ValidateFactoryArguments
   // declares both 'validationError' and the key member as parameters). A KeyMemberName that renders to one of these
   // identifiers would otherwise collide (CS0100/CS0136). The comparison uses the RENDERED identifier, not the raw name,
   // because AppendArgumentName drops a leading underscore before a letter (so "_obj" renders to "obj"), and the
   // library's own private-field defaults are underscore-prefixed.
   // The provider parameter is rendered escaped ("@provider") to match SmartEnumCodeGenerator, which renders the
   // same parameter that way. 'obj' and 'validationError' are rendered bare, like the enum generator's fixed
   // 'item' parameter: none of these fixed names can ever be a C# keyword.
   private readonly string _objArgumentName;
   private readonly string _validationErrorArgumentName;
   private readonly string _providerArgumentName;

   public override string CodeGeneratorName => "ValueObject-CodeGenerator";
   public override string FileNameSuffix => ".ValueObject";

   public KeyedValueObjectCodeGenerator(KeyedValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state ?? throw new ArgumentNullException(nameof(state));
      _sb = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));

      var takenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                       {
                          StringBuilderExtensions.RenderArgumentName(_state.KeyMember.ArgumentName)
                       };

      _objArgumentName = StringBuilderExtensions.GetNonCollidingName(takenNames, "obj", "resultObj");
      _validationErrorArgumentName = StringBuilderExtensions.GetNonCollidingName(takenNames, "validationError", "resultValidationError");
      _providerArgumentName = StringBuilderExtensions.GetNonCollidingName(takenNames, "provider", "formatProvider");
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
      var emptyStringYieldsNull = _state.Settings.EmptyStringInFactoryMethodsYieldsNull && _state is { IsReferenceType: true } && _state.KeyMember.IsString();
      var isGeneric = !_state.GenericParameters.IsDefaultOrEmpty || _state.ContainingTypes.Any(ct => !ct.GenericParameters.IsDefaultOrEmpty);

      _sb.GenerateStructLayoutAttributeIfRequired(_state.IsReferenceType, _state.Settings.HasStructLayoutAttribute);

      if (_state is { Settings.SkipFactoryMethods: false } && !isGeneric)
      {
         _sb.Append(@"
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ThinktectureTypeConverter<").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(_state.KeyMember).Append(", ").AppendTypeFullyQualified(_state.ValidationError).Append(">))]");
      }

      _sb.Append(@"
   [global::System.Diagnostics.CodeAnalysis.SuppressMessage(""ThinktectureRuntimeExtensionsAnalyzer"", ""TTRESG1000:Internal Thinktecture.Runtime.Extensions API usage"")]
   ").Append(_state.IsReferenceType ? "sealed " : "readonly ").Append("partial ").AppendTypeKind(_state).Append(" ").Append(_state.Name).AppendGenericTypeParameters(_state).Append(" :");

      if (!_state.Settings.SkipEqualityComparison)
      {
         _sb.Append(@"
      global::System.IEquatable<").AppendTypeFullyQualifiedNullAnnotated(_state).Append(">,");
      }

      _sb.Append(@"
      global::Thinktecture.IKeyedObject<").AppendTypeFullyQualified(_state.KeyMember).Append(@">,
      global::Thinktecture.IConvertible<").AppendTypeFullyQualified(_state.KeyMember).Append(@">,
      global::Thinktecture.Internal.IMetadataOwner");

      if (!_state.Settings.SkipFactoryMethods)
      {
         _sb.Append(@",
      global::Thinktecture.IObjectFactory<").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(_state.KeyMember).Append(", ").AppendTypeFullyQualified(_state.ValidationError).Append(">");
      }

      if (_state.DisallowsDefaultValue)
      {
         _sb.Append(@",
      global::Thinktecture.IDisallowDefaultValue");
      }

      _sb.Append(@"
   {");

      _sb.Append(@"
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
      static global::Thinktecture.Internal.Metadata global::Thinktecture.Internal.IMetadataOwner.Metadata { get; }
         = new global::Thinktecture.Internal.Metadata.Keyed.ValueObject(typeof(").AppendTypeFullyQualified(_state).Append(@"))
         {
            KeyType = typeof(").AppendTypeFullyQualified(_state.KeyMember).Append(@"),
            ValidationErrorType = typeof(").AppendTypeFullyQualified(_state.ValidationError).Append(@"),
            ConvertToKey = static ").AppendTypeFullyQualified(_state.KeyMember).Append(" (").AppendTypeFullyQualified(_state).Append(" item) => item.").AppendIdentifier(_state.KeyMember.Name).Append(@",
            ConvertToKeyExpression = static ").AppendTypeFullyQualified(_state.KeyMember).Append(" (").AppendTypeFullyQualified(_state).Append(" item) => item.").AppendIdentifier(_state.KeyMember.Name).Append(@",
            GetKey = static object (object item) => ((").AppendTypeFullyQualified(_state).Append(")item).").AppendIdentifier(_state.KeyMember.Name).Append(@",
            ConvertFromKey = ").GenerateDelegateConvertFromKey(_state, emptyStringYieldsNull).Append(@",
            ConvertFromKeyExpression = ").GenerateDelegateConvertFromKey(_state, emptyStringYieldsNull).Append(@",
            ConvertFromKeyExpressionViaConstructor = ").GenerateDelegateConvertFromKeyExpressionViaCtor(_state).Append(@",
            TryGetFromKey = ").GenerateDelegateTryGetFromKey(_state).Append(@"
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

      if (!_state.Settings.SkipKeyMember)
         GenerateKeyMember(_sb, _state.KeyMember, false);

      cancellationToken.ThrowIfCancellationRequested();

      if (!_state.Settings.SkipFactoryMethods)
      {
         var allowNullKeyMemberInput = _state.KeyMember.MayBeNull();
         var allowNullOutput = _state.KeyMember.MayBeNull() && _state is { IsReferenceType: true, Settings.NullInFactoryMethodsYieldsNull: true };
         var hasAdditionalFactoryParams = !_state.FactoryValidationAdditionalParameters.IsDefaultOrEmpty;

         GenerateValidateMethod(allowNullKeyMemberInput, allowNullOutput, emptyStringYieldsNull, hasAdditionalFactoryParams);
         GenerateCreateMethod(allowNullOutput, emptyStringYieldsNull);
         GenerateTryCreateMethod(allowNullOutput, emptyStringYieldsNull);

         if (hasAdditionalFactoryParams)
         {
            GenerateValidateCoreMethod();
            GenerateCreateCoreMethod();
         }

         GenerateValidateFactoryArguments();
         GenerateFactoryPostInit();
      }

      GenerateToValue();
      GenerateSafeConversionToKey();
      GenerateUnsafeConversionToKey();

      if (!_state.Settings.SkipFactoryMethods)
         GenerateConversionFromKey(emptyStringYieldsNull);

      cancellationToken.ThrowIfCancellationRequested();

      GenerateConstructor();

      if (!_state.Settings.SkipEqualityComparison)
      {
         GenerateEquals();
         GenerateGetHashCode();
      }

      if (!_state.Settings.SkipToString)
         GenerateToString();

      _sb.Append(@"
   }");
   }

   private void GenerateToValue()
   {
      _sb.Append(@"

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      ").AppendTypeFullyQualified(_state.KeyMember).Append(" global::Thinktecture.IConvertible<").AppendTypeFullyQualified(_state.KeyMember).Append(@">.ToValue()
      {
         return this.").AppendIdentifier(_state.KeyMember.Name).Append(@";
      }");
   }

   private void GenerateSafeConversionToKey()
   {
      var keyMember = _state.KeyMember;

      if (keyMember.IsInterface
          || keyMember.SpecialType == SpecialType.System_Object
          || _state.Settings.ConversionToKeyMemberType == ConversionOperatorsGeneration.None)
         return;

      _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.ConversionToKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion to the type ").AppendMemberTypeForXmlComment(keyMember).Append(@".
      /// </summary>
      /// <param name=""obj"">Object to convert.</param>
      /// <returns>The <see cref=""").AppendIdentifier(keyMember.Name).Append(@"""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static ").AppendConversionOperator(_state.Settings.ConversionToKeyMemberType).Append(" operator ").AppendTypeFullyQualifiedNullable(keyMember).Append("(").AppendTypeFullyQualifiedNullable(_state).Append(@" obj)
      {");

      if (keyMember is { IsTypeParameter: true, IsValueType: false, IsReferenceType: false })
      {
         _sb.Append(@"
         return obj is null ? default(").AppendTypeFullyQualified(keyMember).Append(") : obj.");

         if (_state.IsValueType)
            _sb.Append("Value.");

         _sb.AppendIdentifier(keyMember.Name);
      }
      else
      {
         _sb.Append(@"
         return obj?.").AppendIdentifier(keyMember.Name);
      }

      _sb.Append(@";
      }");

      if (_state.IsReferenceType || keyMember.MayBeNull())
         return;

      // if value object and key member are structs

      _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.ConversionToKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion to the type ").AppendMemberTypeForXmlComment(keyMember).Append(@".
      /// </summary>
      /// <param name=""obj"">Object to convert.</param>
      /// <returns>The <see cref=""").AppendIdentifier(keyMember.Name).Append(@"""/> of provided <paramref name=""obj""/>.</returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static ").AppendConversionOperator(_state.Settings.ConversionToKeyMemberType).Append(" operator ").AppendTypeFullyQualified(keyMember).Append("(").AppendTypeFullyQualified(_state).Append(@" obj)
      {
         return obj.").AppendIdentifier(keyMember.Name).Append(@";
      }");
   }

   private void GenerateUnsafeConversionToKey()
   {
      var keyMember = _state.KeyMember;

      if (keyMember.IsInterface
          || keyMember.SpecialType == SpecialType.System_Object
          || keyMember.MayBeNull()
          || !_state.IsReferenceType
          || _state.Settings.UnsafeConversionToKeyMemberType == ConversionOperatorsGeneration.None)
         return;

      _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.UnsafeConversionToKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion to the type ").AppendMemberTypeForXmlComment(keyMember).Append(@".
      /// </summary>
      /// <param name=""obj"">Object to convert.</param>
      /// <returns>The <see cref=""").AppendIdentifier(keyMember.Name).Append(@"""/> of provided <paramref name=""obj""/>.</returns>
      /// <exception cref=""System.NullReferenceException"">If <paramref name=""obj""/> is <c>null</c>.</exception>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static ").AppendConversionOperator(_state.Settings.UnsafeConversionToKeyMemberType).Append(" operator ").AppendTypeFullyQualified(keyMember).Append("(").AppendTypeFullyQualified(_state).Append(@" obj)
      {
         if(obj is null)
            throw new global::System.NullReferenceException();

         return obj.").AppendIdentifier(keyMember.Name).Append(@";
      }");
   }

   private void GenerateConversionFromKey(bool emptyStringYieldsNull)
   {
      var keyMember = _state.KeyMember;

      if (keyMember.IsInterface
          || keyMember.SpecialType == SpecialType.System_Object
          || _state.Settings.ConversionFromKeyMemberType == ConversionOperatorsGeneration.None)
         return;

      var bothAreReferenceTypes = _state.IsReferenceType && !keyMember.IsValueType; // reference type or generic without "struct" constraint
      var nullableQuestionMark = bothAreReferenceTypes ? "?" : null;

      _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.ConversionFromKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion from the type ").AppendMemberTypeForXmlComment(keyMember).Append(@".
      /// </summary>
      /// <param name=""").AppendArgumentName(keyMember.ArgumentName).Append(@""">Value to convert.</param>
      /// <returns>An instance of ").AppendTypeForXmlComment(_state).Append(".</returns>");

      _sb.Append(@"
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]");

      if (bothAreReferenceTypes && !emptyStringYieldsNull)
      {
         _sb.Append(@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").AppendArgumentName(keyMember.ArgumentName).Append(@""")]");
      }

      _sb.Append(@"
      public static ").AppendConversionOperator(_state.Settings.ConversionFromKeyMemberType).Append(" operator ").AppendTypeFullyQualified(_state).Append(nullableQuestionMark).Append("(").AppendTypeFullyQualified(keyMember).Append(nullableQuestionMark).Append(" ").AppendEscaped(keyMember.ArgumentName).Append(@")
      {");

      if (bothAreReferenceTypes)
      {
         _sb.Append(@"
         if(").AppendEscaped(keyMember.ArgumentName).Append(@" is null)
            return null;
");
      }

      _sb.Append(@"
         return ").AppendTypeFullyQualified(_state).Append(".").Append(_state.Settings.CreateFactoryMethodName).Append("(").AppendEscaped(keyMember.ArgumentName).Append(@");
      }");
   }

   private void GenerateCreateMethod(bool allowNullOutput, bool emptyStringYieldsNull)
   {
      _sb.Append(@"
      /// <summary>
      /// Creates an instance of the type ").AppendTypeForXmlComment(_state).Append(@" from the provided value,
      /// performing validation during the process.
      /// </summary>
      /// <param name=""").AppendArgumentName(_state.KeyMember.ArgumentName).Append(@""">The value to create the object from.</param>
      /// <returns>An instance of ").AppendTypeForXmlComment(_state).Append(@" if validation is successful.</returns>
      /// <exception cref=""System.ComponentModel.DataAnnotations.ValidationException"">
      /// Thrown if the provided value does not pass validation.
      /// </exception>");

      _sb.Append(@"
      ").Append(GENERATED_CODE_ATTRIBUTE);

      // If emptyStringYieldsNull=true then an empty-string-argument (i.e. not null) will lead to null as return value,
      // that's why we cannot use the NotNullIfNotNullAttribute.
      if (allowNullOutput && !emptyStringYieldsNull)
      {
         _sb.Append(@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").AppendArgumentName(_state.KeyMember.ArgumentName).Append(@""")]");
      }

      _sb.Append(@"
      public static ").AppendTypeFullyQualified(_state).Append(allowNullOutput ? "?" : null).Append(" ").Append(_state.Settings.CreateFactoryMethodName).Append("(").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullOutput).Append(@")
      {
         var ").Append(_validationErrorArgumentName).Append(" = Validate(").RenderArgument(_state.KeyMember).Append(", null, ").Append("out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@");

         if (").Append(_validationErrorArgumentName).Append(@" is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(").Append(_validationErrorArgumentName).Append(@".ToString() ?? ""Validation failed."");

         return ").Append(_objArgumentName).Append(allowNullOutput ? null : "!").Append(@";
      }");
   }

   private void GenerateTryCreateMethod(bool allowNullOutput, bool emptyStringYieldsNull)
   {
      // The out value can be null on a successful (true) result when null input yields null (allowNullOutput)
      // or when an empty string yields null (emptyStringYieldsNull). In those cases the out value is not
      // guaranteed to be non-null on true, so the [NotNullWhen(true)] contract must be omitted. This matches
      // the null-success branch emitted by Validate.
      // The first operand is redundant today, because EmptyStringInFactoryMethodsYieldsNull forces
      // NullInFactoryMethodsYieldsNull (AllValueObjectSettings) and a string key member always satisfies
      // MayBeNull(), so emptyStringYieldsNull implies allowNullOutput. It is kept as defense against a change
      // in either fact (for example struct support for EmptyStringInFactoryMethodsYieldsNull), because losing
      // it would silently emit a wrong [NotNullWhen(true)].
      var objMayBeNullOnSuccess = emptyStringYieldsNull || allowNullOutput;

      _sb.Append(@"

      /// <summary>
      /// Tries to create an instance of the type ").AppendTypeForXmlComment(_state).Append(@" based on provided value.
      /// </summary>
      /// <param name=""").AppendArgumentName(_state.KeyMember.ArgumentName).Append(@""">The value to be used for object creation.</param>
      /// <param name=""").Append(_objArgumentName).Append(@""">
      /// When this method returns, contains the created object if the operation succeeded; otherwise, it will be <c>null</c> or default.").AppendNullOnSuccessSentence(objMayBeNullOnSuccess, emptyStringYieldsNull, "when the method returns <c>true</c>").Append(@"
      /// </param>
      /// <returns>
      /// Returns <c>true</c> if the object was successfully created; otherwise, returns <c>false</c>.
      /// </returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullOutput).Append(objMayBeNullOnSuccess ? "," : ", [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]").Append(" out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@")
      {
         return ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(").RenderArgument(_state.KeyMember).Append(", out ").Append(_objArgumentName).Append(@", out _);
      }");

      _sb.Append(@"

      /// <summary>
      /// Tries to create an instance of the type ").AppendTypeForXmlComment(_state).Append(@" based on provided value.
      /// </summary>
      /// <param name=""").AppendArgumentName(_state.KeyMember.ArgumentName).Append(@""">The value to be used for object creation.</param>
      /// <param name=""").Append(_objArgumentName).Append(@""">
      /// When this method returns, contains the created object if the operation succeeded; otherwise, it will be <c>null</c> or default.").AppendNullOnSuccessSentence(objMayBeNullOnSuccess, emptyStringYieldsNull, "when the method returns <c>true</c>").Append(@"
      /// </param>
      /// <param name=""").Append(_validationErrorArgumentName).Append(@""">
      /// When the method returns, contains the validation error if the creation or validation failed; otherwise, it will be <c>null</c>.
      /// </param>
      /// <returns>
      /// Returns <c>true</c> if the object was successfully created; otherwise, returns <c>false</c>.
      /// </returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append(@"(
         ").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullOutput).Append(@",
         ").Append(objMayBeNullOnSuccess ? null : "[global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ").Append("out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@",
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(_validationErrorArgumentName).Append(@")
      {
         ").Append(_validationErrorArgumentName).Append(" = Validate(").RenderArgument(_state.KeyMember).Append(", null, out ").Append(_objArgumentName).Append(@");

         return ").Append(_validationErrorArgumentName).Append(@" is null;
      }");
   }

   private void GenerateValidateMethod(bool allowNullKeyMemberInput, bool allowNullOutput, bool emptyStringYieldsNull, bool hasAdditionalFactoryParams)
   {
      _sb.Append(@"

      /// <summary>
      /// Validates the provided value and attempts to create an instance of ").AppendTypeForXmlComment(_state).Append(@".
      /// </summary>
      /// <param name=""").AppendArgumentName(_state.KeyMember.ArgumentName).Append(@""">The value to validate.</param>
      /// <param name=""").Append(_providerArgumentName).Append(@""">The format provider for parsing or validation, if applicable.</param>
      /// <param name=""").Append(_objArgumentName).Append(@""">When the method returns, contains the created instance of type ").AppendTypeForXmlComment(_state).Append(@" if validation succeeds; otherwise, <c>null</c>.").AppendNullOnSuccessSentence(emptyStringYieldsNull || allowNullOutput, emptyStringYieldsNull, "when validation succeeds").Append(@"</param>
      /// <returns>
      /// A ").AppendTypeFullyQualifiedForXmlComment(_state.ValidationError).Append(@" representing the validation error if validation fails; otherwise, <c>null</c>.
      /// </returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public static ").AppendTypeFullyQualified(_state.ValidationError).Append("? Validate(").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullKeyMemberInput).Append(", global::System.IFormatProvider? ").AppendEscaped(_providerArgumentName).Append(", out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@")
      {");

      if (emptyStringYieldsNull)
      {
         _sb.Append(@"
         if(global::System.String.IsNullOrWhiteSpace(").AppendEscaped(_state.KeyMember.ArgumentName).Append(@"))
         {
            ").Append(_objArgumentName).Append(@" = default;
            return null;
         }
");
      }
      else if (allowNullOutput)
      {
         _sb.Append(@"
         if(").AppendEscaped(_state.KeyMember.ArgumentName).Append(@" is null)
         {
            ").Append(_objArgumentName).Append(@" = default;
            return null;
         }
");
      }
      else if (_state.KeyMember.MayBeNull())
      {
         _sb.Append(@"
         if(").AppendEscaped(_state.KeyMember.ArgumentName).Append(@" is null)
         {
            ").Append(_objArgumentName).Append(@" = default;
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<").AppendTypeFullyQualified(_state.ValidationError).Append(@">(""The argument '").AppendArgumentName(_state.KeyMember.ArgumentName).Append(@"' must not be null."");
         }
");
      }

      if (hasAdditionalFactoryParams)
      {
         // The full validation/construction plumbing lives in the generated ValidateCore building block,
         // which threads the user's additional factory parameters into the hook. The public Validate has
         // no access to those values, so it forwards their defaults (one 'default' per additional parameter).
         _sb.Append(@"
         return ").Append(Constants.Methods.VALIDATE).Append("Core(").RenderArgument(_state.KeyMember).RenderValidateFactoryAdditionalParametersAsDefaultArguments(_state.FactoryValidationAdditionalParameters).Append(", out ").Append(_objArgumentName).Append(@");
      }");
         return;
      }

      _sb.Append(@"
         ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(_validationErrorArgumentName).Append(@" = null;
         ");

      GenerateValidateBody();

      _sb.Append(@"

         return ").Append(_validationErrorArgumentName).Append(@";
      }");
   }

   private void GenerateValidateCoreMethod()
   {
      _sb.Append(@"

      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      private static ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(Constants.Methods.VALIDATE).Append("Core(").RenderArgumentWithType(_state.KeyMember).RenderValidateFactoryAdditionalParametersAsRequiredParameters(_state.FactoryValidationAdditionalParameters).Append(", out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@")
      {
         ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(_validationErrorArgumentName).Append(@" = null;
         ");

      GenerateValidateBody(renderAdditionalParametersAsArguments: true);

      _sb.Append(@"

         return ").Append(_validationErrorArgumentName).Append(@";
      }");
   }

   private void GenerateValidateBody(bool renderAdditionalParametersAsArguments = false)
   {
      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("var ").Append(Constants.Variables.FACTORY_ARGUMENTS_VALIDATION_ERROR).Append(" = ");

      _sb.Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append("(ref ").Append(_validationErrorArgumentName).Append(", ").RenderArgument(_state.KeyMember, "ref ");

      if (renderAdditionalParametersAsArguments)
         _sb.RenderValidateFactoryAdditionalParametersAsArguments(_state.FactoryValidationAdditionalParameters);

      _sb.Append(@");

         if (").Append(_validationErrorArgumentName).Append(@" is null)
         {
            ").Append(_objArgumentName).Append(" = ");

      GenerateConstructCall();

      _sb.Append(@";
            ").Append(_objArgumentName).Append(".").Append(Constants.Methods.FACTORY_POST_INIT).Append("(");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append(Constants.Variables.FACTORY_ARGUMENTS_VALIDATION_ERROR);

      _sb.Append(@");
         }
         else
         {
            ").Append(_objArgumentName).Append(@" = default;
         }");
   }

   private void GenerateCreateCoreMethod()
   {
      _sb.Append(@"

      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      private static ").AppendTypeFullyQualified(_state).Append(" ").Append(_state.Settings.CreateFactoryMethodName).Append("Core(").RenderArgumentWithType(_state.KeyMember).RenderValidateFactoryAdditionalParametersAsRequiredParameters(_state.FactoryValidationAdditionalParameters).Append(@")
      {
         var ").Append(_validationErrorArgumentName).Append(" = ").Append(Constants.Methods.VALIDATE).Append("Core(").RenderArgument(_state.KeyMember).RenderValidateFactoryAdditionalParametersAsArguments(_state.FactoryValidationAdditionalParameters).Append(", out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" ").Append(_objArgumentName).Append(@");

         if (").Append(_validationErrorArgumentName).Append(@" is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(").Append(_validationErrorArgumentName).Append(@".ToString() ?? ""Validation failed."");

         return ").Append(_objArgumentName).Append(@"!;
      }");
   }

   private void GenerateValidateFactoryArguments()
   {
      _sb.Append(@"

      ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("private ");

      _sb.Append("static partial ").Append(_state.FactoryValidationReturnType ?? "void").Append(" ").Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append("(ref ").AppendTypeFullyQualified(_state.ValidationError).Append("? ").Append(_validationErrorArgumentName).Append(", ").RenderArgumentWithType(_state.KeyMember, "ref ").RenderValidateFactoryAdditionalParameters(_state.FactoryValidationAdditionalParameters).Append(");");
   }

   private void GenerateFactoryPostInit()
   {
      _sb.Append(@"

      partial void ").Append(Constants.Methods.FACTORY_POST_INIT).Append("(");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append(_state.FactoryValidationReturnType).Append(" ").Append(Constants.Variables.FACTORY_ARGUMENTS_VALIDATION_ERROR);

      _sb.Append(");");
   }

   private void GenerateConstructCall()
   {
      _sb.Append("new ").AppendTypeFullyQualified(_state).Append("(").RenderArgument(_state.KeyMember).Append(")");
   }

   private void GenerateConstructor()
   {
      _sb.Append(@"

      /// <summary>
      /// Initializes a new instance of the ").AppendTypeForXmlComment(_state).Append(@" type.
      /// </summary>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      ").RenderAccessModifier(_state.Settings.ConstructorAccessModifier).Append(" ").Append(_state.Name).Append("(").RenderArgumentWithType(_state.KeyMember).Append(@")
      {
         ValidateConstructorArguments(").RenderArgument(_state.KeyMember, "ref ").Append(@");

         this.").AppendIdentifier(_state.KeyMember.Name).Append(" = ").AppendEscaped(_state.KeyMember.ArgumentName).Append(@";
      }

      static partial void ValidateConstructorArguments(").RenderArgumentWithType(_state.KeyMember, "ref ").Append(");");
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

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
         if (other is null)
            return false;

         if (global::System.Object.ReferenceEquals(this, other))
            return true;
");
      }

      GenerateKeyMemberEqualityComparison(_sb, _state.KeyMember, _state.Settings.KeyMemberEqualityComparerAccessor);

      _sb.Append(@"
      }");
   }

   private void GenerateGetHashCode()
   {
      _sb.Append(@"

      /// <inheritdoc />
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override int GetHashCode()
      {
         return ");

      if (_state.Settings.KeyMemberEqualityComparerAccessor is not null)
      {
         _sb.Append(_state.Settings.KeyMemberEqualityComparerAccessor).Append(".EqualityComparer.GetHashCode(this.").AppendIdentifier(_state.KeyMember.Name).Append(")");
      }
      else if (_state.KeyMember.IsString())
      {
         _sb.Append("global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(this.").AppendIdentifier(_state.KeyMember.Name).Append(")");
      }
      else if (_state.KeyMember.IsTypeParameter && !_state.KeyMember.IsValueType)
      {
         _sb.Append("this.").AppendIdentifier(_state.KeyMember.Name).Append("?.GetHashCode() ?? 0");
      }
      else
      {
         _sb.Append("this.").AppendIdentifier(_state.KeyMember.Name).Append(".GetHashCode()");
      }

      _sb.Append(@";
      }");
   }

   private void GenerateToString()
   {
      _sb.Append(@"

      /// <inheritdoc />
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override string").Append(_state.KeyMember.IsToStringReturnTypeNullable ? "?" : null).Append(@" ToString()
      {
         return this.").AppendIdentifier(_state.KeyMember.Name).Append(_state.KeyMember.IsTypeParameter && !_state.KeyMember.IsValueType ? "?" : null).Append(@".ToString();
      }");
   }
}

file static class Extensions
{
   /// <summary>
   /// Appends a sentence to the XML doc of the out parameter stating that the value can be null even on
   /// success. Without it the doc would imply that null means failure, which is wrong as soon as null input
   /// (or empty input) yields a null instance.
   /// </summary>
   public static StringBuilder AppendNullOnSuccessSentence(
      this StringBuilder sb,
      bool objMayBeNullOnSuccess,
      bool emptyStringYieldsNull,
      string successPhrase)
   {
      if (!objMayBeNullOnSuccess)
         return sb;

      return sb.Append(" The value can also be <c>null</c> ").Append(successPhrase)
               .Append(emptyStringYieldsNull
                          ? ", because <c>null</c>, empty, or whitespace-only input yields <c>null</c>."
                          : ", because <c>null</c> input yields <c>null</c>.");
   }

   public static StringBuilder GenerateDelegateConvertFromKey(this StringBuilder sb, KeyedValueObjectSourceGeneratorState state, bool emptyStringYieldsNull)
   {
      if (state.Settings.SkipFactoryMethods)
      {
         sb.Append("null");
      }
      else
      {
         var keyMember = state.KeyMember;
         sb.Append("static ").AppendTypeFullyQualified(state, nullable: emptyStringYieldsNull).Append(" (").AppendTypeFullyQualified(keyMember).Append(" ").AppendEscaped(keyMember.ArgumentName).Append(") => ").AppendTypeFullyQualified(state).Append(".").Append(state.Settings.CreateFactoryMethodName).Append("(").AppendEscaped(keyMember.ArgumentName).Append(")");
      }

      return sb;
   }

   public static StringBuilder GenerateDelegateTryGetFromKey(this StringBuilder sb, KeyedValueObjectSourceGeneratorState state)
   {
      if (state.Settings.SkipFactoryMethods)
      {
         sb.Append("null");
      }
      else
      {
         sb.Append(@"
               (object? key,
                out object? obj,
                [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out object? error) =>
               {
                  if (key is ").AppendTypeFullyQualified(state.KeyMember).Append(@" typedKey)
                  {
                     error = ").AppendTypeFullyQualified(state).Append(@".Validate(typedKey, null, out var item);
                     obj = item;

                     return error is null;
                  }

                  obj = null;
                  error = global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<").AppendTypeFullyQualified(state.ValidationError).Append(@">($""There is no value object of type '").AppendTypeMinimallyQualified(state).Append(@"' with the key '{key}'."");

                  return false;
               }");
      }

      return sb;
   }

   public static StringBuilder GenerateDelegateConvertFromKeyExpressionViaCtor(this StringBuilder sb, KeyedValueObjectSourceGeneratorState state)
   {
      var keyMember = state.KeyMember;
      sb.Append("static ").AppendTypeFullyQualified(state).Append(" (").AppendTypeFullyQualified(keyMember).Append(" ").AppendEscaped(keyMember.ArgumentName).Append(") => new ").AppendTypeFullyQualified(state).Append("(").AppendEscaped(keyMember.ArgumentName).Append(")");

      return sb;
   }
}
