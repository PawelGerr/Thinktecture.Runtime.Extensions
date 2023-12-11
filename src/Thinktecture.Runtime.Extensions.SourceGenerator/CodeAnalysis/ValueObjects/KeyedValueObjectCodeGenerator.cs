using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class KeyedValueObjectCodeGenerator : SmartEnumAndValueObjectCodeGeneratorBase
{
   private const string _FACTORY_ARGUMENTS_VALIDATION_ERROR = "factoryArgumentsValidationError";
   private const string _FACTORY_POST_INIT = "FactoryPostInit";

   private readonly KeyedValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "ValueObject-CodeGenerator";
   public override string? FileNameSuffix => null;

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

      GenerateValueObject(cancellationToken);

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
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<").Append(_state.TypeFullyQualified).Append(", ").Append(_state.KeyMember.TypeFullyQualified).Append(", ").Append(_state.ValidationError.TypeFullyQualified).Append(">))]");
      }

      _sb.Append(@"
   partial ").Append(_state.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Name).Append(" : global::System.IEquatable<").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@">,
      global::Thinktecture.IKeyedValueObject<").Append(_state.KeyMember.TypeFullyQualified).Append(@">,
      global::Thinktecture.IValueObjectConvertable<").Append(_state.KeyMember.TypeFullyQualified).Append(">");

      if (!_state.Settings.SkipFactoryMethods)
      {
         _sb.Append(@",
      global::Thinktecture.IValueObjectFactory<").Append(_state.TypeFullyQualified).Append(", ").Append(_state.KeyMember.TypeFullyQualified).Append(", ").Append(_state.ValidationError.TypeFullyQualified).Append(">");
      }

      foreach (var desiredFactory in _state.Settings.DesiredFactories)
      {
         if (_state is { Settings.SkipFactoryMethods: false } && desiredFactory.Equals(_state.KeyMember))
            continue;

         _sb.Append(@",
      global::Thinktecture.IValueObjectFactory<").Append(_state.TypeFullyQualified).Append(", ").Append(desiredFactory.TypeFullyQualified).Append(", ").Append(_state.ValidationError.TypeFullyQualified).Append(">");

         if (desiredFactory.UseForSerialization != SerializationFrameworks.None)
         {
            _sb.Append(@",
      global::Thinktecture.IValueObjectConvertable<").Append(desiredFactory.TypeFullyQualified).Append(">");
         }
      }

      _sb.Append(@"
   {");

      GenerateModuleInitializerForKeyedValueObject(emptyStringYieldsNull);

      _sb.Append(@"
      private static readonly int _typeHashCode = typeof(").Append(_state.TypeFullyQualified).Append(").GetHashCode();");

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"

      public static readonly ").Append(_state.TypeFullyQualified).Append(" ").Append(_state.Settings.DefaultInstancePropertyName).Append(" = default;");
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
      GenerateImplicitConversionToKey();
      GenerateExplicitConversionToKey();

      if (!_state.Settings.SkipFactoryMethods)
         GenerateExplicitConversion(emptyStringYieldsNull);

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
      ").Append(_state.KeyMember.TypeFullyQualified).Append(" global::Thinktecture.IValueObjectConvertable<").Append(_state.KeyMember.TypeFullyQualified).Append(@">.ToValue()
      {
         return this.").Append(_state.KeyMember.Name).Append(@";
      }");
   }

   private void GenerateModuleInitializerForKeyedValueObject(bool emptyStringYieldsNull)
   {
      var keyMember = _state.KeyMember;
      var typeFullyQualified = _state.TypeFullyQualified;
      var nullAnnotatedTypeFullyQualified = emptyStringYieldsNull ? _state.TypeFullyQualifiedNullAnnotated : typeFullyQualified;
      var keyMemberWithoutNullAnnotation = keyMember.IsReferenceType ? keyMember.TypeFullyQualified : keyMember.TypeFullyQualifiedWithNullability;

      _sb.Append(@"
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         global::System.Func<").Append(keyMember.TypeFullyQualifiedWithNullability).Append(", ").Append(nullAnnotatedTypeFullyQualified).Append(">").Append(_state.Settings.SkipFactoryMethods ? "?" : null).Append(" convertFromKey = ");

      if (_state.Settings.SkipFactoryMethods)
      {
         _sb.Append("null");
      }
      else
      {
         _sb.Append("new (").Append(typeFullyQualified).Append(".").Append(_state.Settings.CreateFactoryMethodName).Append(")");
      }

      _sb.Append(@";
         global::System.Linq.Expressions.Expression<global::System.Func<").Append(keyMember.TypeFullyQualifiedWithNullability).Append(", ").Append(nullAnnotatedTypeFullyQualified).Append(">>").Append(_state.Settings.SkipFactoryMethods ? "?" : null).Append(" convertFromKeyExpression = ");

      if (_state.Settings.SkipFactoryMethods)
      {
         _sb.Append("null");
      }
      else
      {
         _sb.Append("static ").Append(keyMember.ArgumentName.Escaped).Append(" => ").Append(typeFullyQualified).Append(".").Append(_state.Settings.CreateFactoryMethodName).Append("(").Append(keyMember.ArgumentName.Escaped).Append(")");
      }

      _sb.Append(@";
         global::System.Linq.Expressions.Expression<global::System.Func<").Append(keyMember.TypeFullyQualifiedWithNullability).Append(", ").Append(typeFullyQualified).Append(">> convertFromKeyExpressionViaCtor = static ").Append(keyMember.ArgumentName.Escaped).Append(" => new ").Append(typeFullyQualified).Append("(").Append(keyMember.ArgumentName.Escaped).Append(@");

         var convertToKey = new global::System.Func<").Append(typeFullyQualified).Append(", ").Append(keyMember.TypeFullyQualifiedWithNullability).Append(">(static item => item.").Append(keyMember.Name).Append(@");
         global::System.Linq.Expressions.Expression<global::System.Func<").Append(typeFullyQualified).Append(", ").Append(keyMember.TypeFullyQualifiedWithNullability).Append(">> convertToKeyExpression = static obj => obj.").Append(keyMember.Name).Append(@";

         var type = typeof(").Append(typeFullyQualified).Append(@");
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(").Append(keyMemberWithoutNullAnnotation).Append(@"), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
      }
");
   }

   private void GenerateImplicitConversionToKey()
   {
      var keyMember = _state.KeyMember;

      _sb.Append(@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""").Append(keyMember.TypeFullyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""").Append(keyMember.Name).Append(@"""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static implicit operator ").Append(keyMember.TypeFullyQualifiedNullable).Append("(").Append(_state.IsReferenceType ? _state.TypeFullyQualifiedNullAnnotated : _state.TypeFullyQualifiedNullable).Append(@" obj)
      {
         return obj?.").Append(keyMember.Name).Append(@";
      }");

      if (_state.IsReferenceType || keyMember.IsReferenceType)
         return;

      // if value object and key member are structs

      _sb.Append(@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""").Append(keyMember.TypeFullyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""").Append(keyMember.Name).Append(@"""/> of provided <paramref name=""obj""/>.</returns>
      public static implicit operator ").Append(keyMember.TypeFullyQualified).Append("(").Append(_state.TypeFullyQualified).Append(@" obj)
      {
         return obj.").Append(keyMember.Name).Append(@";
      }");
   }

   private void GenerateExplicitConversionToKey()
   {
      var keyMember = _state.KeyMember;

      if (keyMember.IsReferenceType || !_state.IsReferenceType)
         return;

      _sb.Append(@"

      /// <summary>
      /// Explicit conversion to the type <see cref=""").Append(keyMember.TypeFullyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""").Append(keyMember.Name).Append(@"""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static explicit operator ").Append(keyMember.TypeFullyQualifiedWithNullability).Append("(").Append(_state.TypeFullyQualified).Append(@" obj)
      {
         if(obj is null)
            throw new global::System.NullReferenceException();

         return obj.").Append(keyMember.Name).Append(@";
      }");
   }

   private void GenerateExplicitConversion(bool emptyStringYieldsNull)
   {
      var keyMember = _state.KeyMember;
      var bothAreReferenceTypes = _state.IsReferenceType && keyMember.IsReferenceType;
      var nullableQuestionMark = bothAreReferenceTypes ? "?" : null;

      _sb.Append(@"

      /// <summary>
      /// Explicit conversion from the type <see cref=""").Append(keyMember.TypeFullyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""").Append(keyMember.ArgumentName.Raw).Append(@""">Value to covert.</param>
      /// <returns>An instance of <see cref=""").Append(_state.TypeMinimallyQualified).Append(@"""/>.</returns>");

      if (bothAreReferenceTypes && !emptyStringYieldsNull)
      {
         _sb.Append(@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").Append(keyMember.ArgumentName.Escaped).Append(@""")]");
      }

      _sb.Append(@"
      public static explicit operator ").Append(_state.TypeFullyQualified).Append(nullableQuestionMark).Append("(").Append(keyMember.TypeFullyQualified).Append(nullableQuestionMark).Append(" ").Append(keyMember.ArgumentName.Escaped).Append(@")
      {");

      if (bothAreReferenceTypes)
      {
         _sb.Append(@"
         if(").Append(keyMember.ArgumentName.Escaped).Append(@" is null)
            return null;
");
      }

      _sb.Append(@"
         return ").Append(_state.TypeFullyQualified).Append(".").Append(_state.Settings.CreateFactoryMethodName).Append("(").Append(keyMember.ArgumentName.Escaped).Append(@");
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
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").Append(_state.KeyMember.ArgumentName.Escaped).Append(@""")]");
      }

      _sb.Append(@"
      public static ").Append(_state.TypeFullyQualified).Append(allowNullOutput ? "?" : null).Append(" ").Append(_state.Settings.CreateFactoryMethodName).Append("(").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullOutput).Append(@")
      {
         var validationError = Validate(").RenderArgument(_state.KeyMember).Append(", null, ").Append("out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj);

         if (validationError is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? ""Validation failed."");

         return obj").Append(allowNullOutput ? null : "!").Append(@";
      }");
   }

   private void GenerateTryCreateMethod(bool allowNullOutput, bool emptyStringYieldsNull)
   {
      _sb.Append(@"

      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullOutput).Append(emptyStringYieldsNull ? "," : ", [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]").Append(" out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj)
      {
         var validationError = Validate(").RenderArgument(_state.KeyMember).Append(@", null, out obj);

         return validationError is null;
      }");
   }

   private void GenerateValidateMethod(bool allowNullKeyMemberInput, bool allowNullOutput, bool emptyStringYieldsNull)
   {
      var providerArgumentName = _state.KeyMember.ArgumentName.Escaped == "provider" ? "formatProvider" : "provider";

      _sb.Append(@"

      public static ").Append(_state.ValidationError.TypeFullyQualified).Append("? Validate(").RenderArgumentWithType(_state.KeyMember, useNullableTypes: allowNullKeyMemberInput).Append(", global::System.IFormatProvider? ").Append(providerArgumentName).Append(", out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj)
      {");

      if (emptyStringYieldsNull)
      {
         _sb.Append(@"
         if(global::System.String.IsNullOrWhiteSpace(").Append(_state.KeyMember.ArgumentName.Escaped).Append(@"))
         {
            obj = default;
            return null;
         }
");
      }
      else if (allowNullOutput)
      {
         _sb.Append(@"
         if(").Append(_state.KeyMember.ArgumentName.Escaped).Append(@" is null)
         {
            obj = default;
            return null;
         }
");
      }
      else if (_state.KeyMember.IsReferenceType)
      {
         _sb.Append(@"
         if(").Append(_state.KeyMember.ArgumentName.Escaped).Append(@" is null)
         {
            obj = default;
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<").Append(_state.ValidationError.TypeFullyQualified).Append(@">(""The argument '").Append(_state.KeyMember.ArgumentName.Escaped).Append(@"' must not be null."");
         }
");
      }

      _sb.Append(@"
         ").Append(_state.ValidationError.TypeFullyQualified).Append(@"? validationError = null;
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

      _sb.Append("static partial ").Append(_state.FactoryValidationReturnType ?? "void").Append(" ").Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append("(ref ").Append(_state.ValidationError.TypeFullyQualified).Append("? validationError, ").RenderArgumentWithType(_state.KeyMember, "ref ", addAllowNullNotNullCombi: true).Append(");");
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
      _sb.Append("new ").Append(_state.TypeFullyQualified).Append("(").RenderArgument(_state.KeyMember).Append(")");
   }

   private void GenerateConstructor()
   {
      _sb.Append(@"

      ").RenderAccessModifier(_state.Settings.ConstructorAccessModifier).Append(" ").Append(_state.Name).Append("(").RenderArgumentWithType(_state.KeyMember).Append(@")
      {
         ValidateConstructorArguments(").RenderArgument(_state.KeyMember, "ref ").Append(@");

         this.").Append(_state.KeyMember.Name).Append(" = ").Append(_state.KeyMember.ArgumentName.Escaped).Append(@";
      }

      static partial void ValidateConstructorArguments(").RenderArgumentWithType(_state.KeyMember, "ref ").Append(");");
   }

   private void GenerateEquals()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {
         return other is ").Append(_state.TypeFullyQualified).Append(@" obj && Equals(obj);
      }

      /// <inheritdoc />
      public bool Equals(").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" other)
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
      public override string ToString()
      {
         return this.").Append(_state.KeyMember.Name).Append(@".ToString();
      }");
   }
}
