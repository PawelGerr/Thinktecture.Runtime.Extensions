using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class KeyedValueObjectCodeGenerator : SmartEnumAndValueObjectCodeGeneratorBase
{
   private const string _FACTORY_ARGUMENTS_VALIDATION_ERROR = "factoryArgumentsValidationError";
   private const string _FACTORY_POST_INIT = "FactoryPostInit";

   private readonly KeyedValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "ValueObject-CodeGenerator";
   public override string FileNameSuffix => ".ValueObject";

   public KeyedValueObjectCodeGenerator(KeyedValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state ?? throw new ArgumentNullException(nameof(state));
      _sb = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));
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

      _sb.GenerateStructLayoutAttributeIfRequired(_state.IsReferenceType, _state.Settings.HasStructLayoutAttribute);

      if (_state is { Settings.SkipFactoryMethods: false })
      {
         _sb.Append(@"
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ThinktectureTypeConverter<").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(_state.KeyMember).Append(", ").AppendTypeFullyQualified(_state.ValidationError).Append(">))]");
      }

      _sb.Append(@"
   [global::System.Diagnostics.CodeAnalysis.SuppressMessage(""ThinktectureRuntimeExtensionsAnalyzer"", ""TTRESG1000:Internal Thinktecture.Runtime.Extensions API usage"")]
   ").Append(_state.IsReferenceType ? "sealed " : "readonly ").Append("partial ").AppendTypeKind(_state).Append(" ").Append(_state.Name).Append(" : global::System.IEquatable<").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@">,
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
      static global::Thinktecture.Internal.Metadata global::Thinktecture.Internal.IMetadataOwner.Metadata { get; } = new global::Thinktecture.Internal.Metadata.Keyed.ValueObject
      {
         Type = typeof(").AppendTypeFullyQualified(_state).Append(@"),
         KeyType = typeof(").AppendTypeFullyQualified(_state.KeyMember).Append(@"),
         ValidationErrorType = typeof(").AppendTypeFullyQualified(_state.ValidationError).Append(@"),
         ConvertToKey = static ").AppendTypeFullyQualified(_state.KeyMember).Append(" (").AppendTypeFullyQualified(_state).Append(" item) => item.").Append(_state.KeyMember.Name).Append(@",
         ConvertToKeyExpression = static ").AppendTypeFullyQualified(_state.KeyMember).Append(" (").AppendTypeFullyQualified(_state).Append(" item) => item.").Append(_state.KeyMember.Name).Append(@",
         GetKey = static object (object item) => ((").AppendTypeFullyQualified(_state).Append(")item).").Append(_state.KeyMember.Name).Append(@",
         ConvertFromKey = ").GenerateDelegateConvertFromKey(_state).Append(@",
         ConvertFromKeyExpression = ").GenerateDelegateConvertFromKey(_state).Append(@",
         ConvertFromKeyExpressionViaConstructor = ").GenerateDelegateConvertFromKeyExpressionViaCtor(_state).Append(@",
         TryGetFromKey = ").GenerateDelegateTryGetFromKey(_state).Append(@"
      };

      private static readonly int _typeHashCode = typeof(").AppendTypeFullyQualified(_state).Append(").GetHashCode();");

      if (_state is { IsReferenceType: false, Settings.AllowDefaultStructs: true })
      {
         _sb.Append(@"

      public static readonly ").AppendTypeFullyQualified(_state).Append(" ").Append(_state.Settings.DefaultInstancePropertyName).Append(" = default;");
      }

      if (!_state.Settings.SkipKeyMember)
         GenerateKeyMember(_sb, _state.KeyMember, false);

      cancellationToken.ThrowIfCancellationRequested();

      if (!_state.Settings.SkipFactoryMethods)
      {
         var allowNullKeyMemberInput = _state.KeyMember.IsReferenceType;
         var allowNullOutput = _state.KeyMember.IsReferenceType && _state is { IsReferenceType: true, Settings.NullInFactoryMethodsYieldsNull: true };

         GenerateValidateMethod(allowNullKeyMemberInput, allowNullOutput, emptyStringYieldsNull);
         GenerateCreateMethod(allowNullOutput, emptyStringYieldsNull);
         GenerateTryCreateMethod(allowNullOutput, emptyStringYieldsNull);
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
      GenerateEquals();
      GenerateGetHashCode();

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
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      ").AppendTypeFullyQualified(_state.KeyMember).Append(" global::Thinktecture.IConvertible<").AppendTypeFullyQualified(_state.KeyMember).Append(@">.ToValue()
      {
         return this.").Append(_state.KeyMember.Name).Append(@";
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
      /// ").Append(_state.Settings.ConversionToKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion to the type ").AppendTypeForXmlComment(keyMember).Append(@".
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""").Append(keyMember.Name).Append(@"""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static ").AppendConversionOperator(_state.Settings.ConversionToKeyMemberType).Append(" operator ").AppendTypeFullyQualifiedNullable(keyMember).Append("(").AppendTypeFullyQualifiedNullable(_state).Append(@" obj)
      {
         return obj?.").Append(keyMember.Name).Append(@";
      }");

      if (_state.IsReferenceType || keyMember.IsReferenceType)
         return;

      // if value object and key member are structs

      _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.ConversionToKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion to the type ").AppendTypeForXmlComment(keyMember).Append(@".
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""").Append(keyMember.Name).Append(@"""/> of provided <paramref name=""obj""/>.</returns>
      public static ").AppendConversionOperator(_state.Settings.ConversionToKeyMemberType).Append(" operator ").AppendTypeFullyQualified(keyMember).Append("(").AppendTypeFullyQualified(_state).Append(@" obj)
      {
         return obj.").Append(keyMember.Name).Append(@";
      }");
   }

   private void GenerateUnsafeConversionToKey()
   {
      var keyMember = _state.KeyMember;

      if (keyMember.IsInterface
          || keyMember.SpecialType == SpecialType.System_Object
          || keyMember.IsReferenceType
          || !_state.IsReferenceType
          || _state.Settings.UnsafeConversionToKeyMemberType == ConversionOperatorsGeneration.None)
         return;

      _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.UnsafeConversionToKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion to the type ").AppendTypeForXmlComment(keyMember).Append(@".
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""").Append(keyMember.Name).Append(@"""/> of provided <paramref name=""obj""/>.</returns>
      /// <exception cref=""System.NullReferenceException"">If <paramref name=""obj""/> is <c>null</c>.</exception>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static ").AppendConversionOperator(_state.Settings.UnsafeConversionToKeyMemberType).Append(" operator ").AppendTypeFullyQualified(keyMember).Append("(").AppendTypeFullyQualified(_state).Append(@" obj)
      {
         if(obj is null)
            throw new global::System.NullReferenceException();

         return obj.").Append(keyMember.Name).Append(@";
      }");
   }

   private void GenerateConversionFromKey(bool emptyStringYieldsNull)
   {
      var keyMember = _state.KeyMember;

      if (keyMember.IsInterface
          || keyMember.SpecialType == SpecialType.System_Object
          || _state.Settings.ConversionFromKeyMemberType == ConversionOperatorsGeneration.None)
         return;

      var bothAreReferenceTypes = _state.IsReferenceType && keyMember.IsReferenceType;
      var nullableQuestionMark = bothAreReferenceTypes ? "?" : null;

      _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.ConversionFromKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion from the type ").AppendTypeForXmlComment(keyMember).Append(@".
      /// </summary>
      /// <param name=""").Append(keyMember.ArgumentName).Append(@""">Value to covert.</param>
      /// <returns>An instance of ").AppendTypeForXmlComment(_state).Append(".</returns>");

      if (bothAreReferenceTypes && !emptyStringYieldsNull)
      {
         _sb.Append(@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").Append(keyMember.ArgumentName).Append(@""")]");
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
");

      // If emptyStringYieldsNull=true then an empty-string-argument (i.e. not null) will lead to null as return value,
      // that's why we cannot use the NotNullIfNotNullAttribute.
      if (allowNullOutput && !emptyStringYieldsNull)
      {
         _sb.Append(@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").Append(_state.KeyMember.ArgumentName).Append(@""")]");
      }

      _sb.Append(@"
      public static ").AppendTypeFullyQualified(_state).Append(allowNullOutput ? "?" : null).Append(" ").Append(_state.Settings.CreateFactoryMethodName).Append("(").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullOutput).Append(@")
      {
         var validationError = Validate(").RenderArgument(_state.KeyMember).Append(", null, ").Append("out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" obj);

         if (validationError is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? ""Validation failed."");

         return obj").Append(allowNullOutput ? null : "!").Append(@";
      }");
   }

   private void GenerateTryCreateMethod(bool allowNullOutput, bool emptyStringYieldsNull)
   {
      _sb.Append(@"

      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullOutput).Append(emptyStringYieldsNull ? "," : ", [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]").Append(" out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" obj)
      {
         return ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(").RenderArgument(_state.KeyMember).Append(@", out obj, out _);
      }");

      _sb.Append(@"

      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append(@"(
         ").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullOutput).Append(@",
         ").Append(emptyStringYieldsNull ? null : "[global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ").Append("out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" obj,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out ").AppendTypeFullyQualified(_state.ValidationError).Append(@"? validationError)
      {
         validationError = Validate(").RenderArgument(_state.KeyMember).Append(@", null, out obj);

         return validationError is null;
      }");
   }

   private void GenerateValidateMethod(bool allowNullKeyMemberInput, bool allowNullOutput, bool emptyStringYieldsNull)
   {
      var providerArgumentName = _state.KeyMember.ArgumentName == "provider" ? "formatProvider" : "provider";

      _sb.Append(@"

      public static ").AppendTypeFullyQualified(_state.ValidationError).Append("? Validate(").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullKeyMemberInput).Append(", global::System.IFormatProvider? ").AppendEscaped(providerArgumentName).Append(", out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" obj)
      {");

      if (emptyStringYieldsNull)
      {
         _sb.Append(@"
         if(global::System.String.IsNullOrWhiteSpace(").AppendEscaped(_state.KeyMember.ArgumentName).Append(@"))
         {
            obj = default;
            return null;
         }
");
      }
      else if (allowNullOutput)
      {
         _sb.Append(@"
         if(").AppendEscaped(_state.KeyMember.ArgumentName).Append(@" is null)
         {
            obj = default;
            return null;
         }
");
      }
      else if (_state.KeyMember.IsReferenceType)
      {
         _sb.Append(@"
         if(").AppendEscaped(_state.KeyMember.ArgumentName).Append(@" is null)
         {
            obj = default;
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<").AppendTypeFullyQualified(_state.ValidationError).Append(@">(""The argument '").Append(_state.KeyMember.ArgumentName).Append(@"' must not be null."");
         }
");
      }

      _sb.Append(@"
         ").AppendTypeFullyQualified(_state.ValidationError).Append(@"? validationError = null;
         ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("var ").Append(_FACTORY_ARGUMENTS_VALIDATION_ERROR).Append(" = ");

      _sb.Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append("(ref validationError, ").RenderArgument(_state.KeyMember, "ref ").Append(@");

         if (validationError is null)
         {
            obj = ");

      GenerateConstructCall();

      _sb.Append(@";
            obj.").Append(_FACTORY_POST_INIT).Append("(");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append(_FACTORY_ARGUMENTS_VALIDATION_ERROR);

      _sb.Append(@");
         }
         else
         {
            obj = default;
         }

         return validationError;
      }");
   }

   private void GenerateValidateFactoryArguments()
   {
      _sb.Append(@"

      ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("private ");

      _sb.Append("static partial ").Append(_state.FactoryValidationReturnType ?? "void").Append(" ").Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append("(ref ").AppendTypeFullyQualified(_state.ValidationError).Append("? validationError, ").RenderArgumentWithType(_state.KeyMember, "ref ").Append(");");
   }

   private void GenerateFactoryPostInit()
   {
      _sb.Append(@"

      partial void ").Append(_FACTORY_POST_INIT).Append("(");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append(_state.FactoryValidationReturnType).Append(" ").Append(_FACTORY_ARGUMENTS_VALIDATION_ERROR);

      _sb.Append(");");
   }

   private void GenerateConstructCall()
   {
      _sb.Append("new ").AppendTypeFullyQualified(_state).Append("(").RenderArgument(_state.KeyMember).Append(")");
   }

   private void GenerateConstructor()
   {
      _sb.Append(@"

      ").RenderAccessModifier(_state.Settings.ConstructorAccessModifier).Append(" ").Append(_state.Name).Append("(").RenderArgumentWithType(_state.KeyMember).Append(@")
      {
         ValidateConstructorArguments(").RenderArgument(_state.KeyMember, "ref ").Append(@");

         this.").Append(_state.KeyMember.Name).Append(" = ").AppendEscaped(_state.KeyMember.ArgumentName).Append(@";
      }

      static partial void ValidateConstructorArguments(").RenderArgumentWithType(_state.KeyMember, "ref ").Append(");");
   }

   private void GenerateEquals()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {
         return other is ").AppendTypeFullyQualified(_state).Append(@" obj && Equals(obj);
      }

      /// <inheritdoc />
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
      public override int GetHashCode()
      {
         return global::System.HashCode.Combine(_typeHashCode, ");

      if (_state.Settings.KeyMemberEqualityComparerAccessor is not null)
      {
         _sb.Append(_state.Settings.KeyMemberEqualityComparerAccessor).Append(".EqualityComparer.GetHashCode(this.").Append(_state.KeyMember.Name).Append(")");
      }
      else if (_state.KeyMember.IsString())
      {
         _sb.Append("global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(this.").Append(_state.KeyMember.Name).Append(")");
      }
      else
      {
         _sb.Append("this.").Append(_state.KeyMember.Name);
      }

      _sb.Append(@");
      }");
   }

   private void GenerateToString()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override string").Append(_state.KeyMember.IsToStringReturnTypeNullable ? "?" : null).Append(@" ToString()
      {
         return this.").Append(_state.KeyMember.Name).Append(@".ToString();
      }");
   }
}

file static class Extensions
{
   public static StringBuilder GenerateDelegateConvertFromKey(this StringBuilder sb, KeyedValueObjectSourceGeneratorState state)
   {
      if (state.Settings.SkipFactoryMethods)
      {
         sb.Append("null");
      }
      else
      {
         var keyMember = state.KeyMember;
         sb.Append("static ").AppendTypeFullyQualified(state, nullable: state.Settings.EmptyStringInFactoryMethodsYieldsNull).Append(" (").AppendTypeFullyQualified(keyMember).Append(" ").AppendEscaped(keyMember.ArgumentName).Append(") => ").AppendTypeFullyQualified(state).Append(".").Append(state.Settings.CreateFactoryMethodName).Append("(").AppendEscaped(keyMember.ArgumentName).Append(")");
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
             [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(true)] out object error) =>
            {
               error = ").AppendTypeFullyQualified(state).Append(".Validate(key is ").AppendTypeFullyQualified(state.KeyMember).Append(@" typedKey ? typedKey : default, null, out var item);
               obj = item;

               return error is null;
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
