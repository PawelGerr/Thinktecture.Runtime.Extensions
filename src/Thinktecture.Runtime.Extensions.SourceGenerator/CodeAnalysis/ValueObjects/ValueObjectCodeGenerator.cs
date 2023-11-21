using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectCodeGenerator : CodeGeneratorBase
{
   private const string _FACTORY_ARGUMENTS_VALIDATION_ERROR = "factoryArgumentsValidationError";
   private const string _FACTORY_POST_INIT = "FactoryPostInit";

   private readonly ValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "ValueObject-CodeGenerator";
   public override string? FileNameSuffix => null;

   public ValueObjectCodeGenerator(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
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

      var emptyStringYieldsNull = _state.Settings.EmptyStringInFactoryMethodsYieldsNull && _state is { IsReferenceType: true, HasKeyMember: true } && _state.KeyMember.Member.IsString();

      GenerateValueObject(emptyStringYieldsNull, cancellationToken);

      if (hasNamespace)
      {
         _sb.Append(@"
}");
      }

      _sb.Append(@"
");
   }

   private void GenerateValueObject(bool emptyStringYieldsNull, CancellationToken cancellationToken)
   {
      if (_state is { HasKeyMember: true, Settings.SkipFactoryMethods: false })
      {
         _sb.Append(@"
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<").Append(_state.TypeFullyQualified).Append(", ").Append(_state.KeyMember.Member.TypeFullyQualified).Append(", ").Append(_state.ValidationError.TypeFullyQualified).Append(">))]");
      }

      _sb.Append(@"
   partial ").Append(_state.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Name).Append(" : global::System.IEquatable<").Append(_state.TypeFullyQualifiedNullAnnotated).Append(">");

      if (_state.HasKeyMember)
      {
         _sb.Append(@",
      global::Thinktecture.IKeyedValueObject,
      global::Thinktecture.IValueObjectConvertable<").Append(_state.KeyMember.Member.TypeFullyQualified).Append(">");

         if (!_state.Settings.SkipFactoryMethods)
         {
            _sb.Append(@",
      global::Thinktecture.IValueObjectFactory<").Append(_state.TypeFullyQualified).Append(", ").Append(_state.KeyMember.Member.TypeFullyQualified).Append(", ").Append(_state.ValidationError.TypeFullyQualified).Append(">");
         }
      }
      else
      {
         _sb.Append(@",
      global::System.Numerics.IEqualityOperators<").Append(_state.TypeFullyQualified).Append(", ").Append(_state.TypeFullyQualified).Append(@", bool>,
      global::Thinktecture.IComplexValueObject");
      }

      foreach (var desiredFactory in _state.Settings.DesiredFactories)
      {
         if (_state is { HasKeyMember: true, Settings.SkipFactoryMethods: false } && desiredFactory.Equals(_state.KeyMember.Member))
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

      if (_state.HasKeyMember)
      {
         GenerateModuleInitializerForKeyedValueObject(_state.KeyMember.Member, emptyStringYieldsNull);
      }
      else
      {
         GenerateModuleInitializerForComplexValueObject();
      }

      _sb.Append(@"
      private static readonly global::System.Type _type = typeof(").Append(_state.TypeFullyQualified).Append(");");

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"

      public static readonly ").Append(_state.TypeFullyQualified).Append(" ").Append(_state.Settings.DefaultInstancePropertyName).Append(" = default;");
      }

      cancellationToken.ThrowIfCancellationRequested();

      if (!_state.Settings.SkipFactoryMethods)
      {
         var allowNullKeyMemberInput = _state.HasKeyMember && _state.KeyMember.Member.IsReferenceType;
         var allowNullOutput = _state.HasKeyMember && _state.KeyMember.Member.IsReferenceType && _state.IsReferenceType && _state.Settings.NullInFactoryMethodsYieldsNull;

         GenerateValidateMethod(allowNullKeyMemberInput, allowNullOutput, emptyStringYieldsNull);
         GenerateCreateMethod(allowNullOutput, emptyStringYieldsNull);
         GenerateTryCreateMethod(allowNullOutput, emptyStringYieldsNull);
         GenerateValidateFactoryArguments();
         GenerateFactoryPostInit();
      }

      if (_state.HasKeyMember)
      {
         GenerateToValue(_state.KeyMember);
         GenerateImplicitConversionToKey(_state.KeyMember);
         GenerateExplicitConversionToKey(_state.KeyMember);

         if (!_state.Settings.SkipFactoryMethods)
            GenerateExplicitConversion(_state.KeyMember, emptyStringYieldsNull);
      }

      cancellationToken.ThrowIfCancellationRequested();

      GenerateConstructor();

      // Keyed value object get their equality operators from EqualityComparisonOperatorsCodeGenerator
      if (!_state.HasKeyMember)
         GenerateEqualityOperators();

      GenerateEquals();
      GenerateGetHashCode();

      if (!_state.Settings.SkipToString)
         GenerateToString();

      GenerateValidationErrorFactory();

      _sb.Append(@"
   }");
   }

   private void GenerateToValue(EqualityInstanceMemberInfo keyMember)
   {
      _sb.Append(@"

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      ").Append(keyMember.Member.TypeFullyQualified).Append(" global::Thinktecture.IValueObjectConvertable<").Append(keyMember.Member.TypeFullyQualified).Append(@">.ToValue()
      {
         return this.").Append(keyMember.Member.Name).Append(@";
      }");
   }

   private void GenerateModuleInitializerForKeyedValueObject(IMemberState keyMember, bool emptyStringYieldsNull)
   {
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
         _sb.Append("new (").Append(typeFullyQualified).Append(".Create)");
      }

      _sb.Append(@";
         global::System.Linq.Expressions.Expression<global::System.Func<").Append(keyMember.TypeFullyQualifiedWithNullability).Append(", ").Append(nullAnnotatedTypeFullyQualified).Append(">>").Append(_state.Settings.SkipFactoryMethods ? "?" : null).Append(" convertFromKeyExpression = ");

      if (_state.Settings.SkipFactoryMethods)
      {
         _sb.Append("null");
      }
      else
      {
         _sb.Append("static ").Append(keyMember.ArgumentName.Escaped).Append(" => ").Append(typeFullyQualified).Append(".Create(").Append(keyMember.ArgumentName.Escaped).Append(")");
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

   private void GenerateModuleInitializerForComplexValueObject()
   {
      _sb.Append(@"
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         global::System.Linq.Expressions.Expression<global::System.Func<").Append(_state.Name).Append(@", object>> action = o => new
                                                                                                            {");

      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      for (var i = 0; i < fieldsAndProperties.Count; i++)
      {
         if (i > 0)
            _sb.Append(",");

         var memberInfo = fieldsAndProperties[i];
         _sb.Append(@"
                                                                                                               o.").Append(memberInfo.Name);
      }

      _sb.Append(@"
                                                                                                            };

         var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

         foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
         {
            members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
         }

         var type = typeof(").Append(_state.TypeFullyQualified).Append(@");
         var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

         global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
      }
");
   }

   private void GenerateImplicitConversionToKey(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;

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

   private void GenerateExplicitConversionToKey(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;

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

   private void GenerateExplicitConversion(EqualityInstanceMemberInfo keyMemberInfo, bool emptyStringYieldsNull)
   {
      var keyMember = keyMemberInfo.Member;
      var bothAreReferenceTypes = _state.IsReferenceType && keyMemberInfo.Member.IsReferenceType;
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
         return ").Append(_state.TypeFullyQualified).Append(".Create(").Append(keyMember.ArgumentName.Escaped).Append(@");
      }");
   }

   private void GenerateCreateMethod(bool allowNullOutput, bool emptyStringYieldsNull)
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"
");

      // If emptyStringYieldsNull=true then an empty-string-argument (i.e. not null) will lead to null as return value,
      // that's why we cannot use the NotNullIfNotNullAttribute.
      if (allowNullOutput && !emptyStringYieldsNull)
      {
         _sb.Append(@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").Append(_state.KeyMember!.Member.ArgumentName.Escaped).Append(@""")]");
      }

      _sb.Append(@"
      public static ").Append(_state.TypeFullyQualified).Append(allowNullOutput ? "?" : null).Append(" Create(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, useNullableTypes: allowNullOutput);

      _sb.Append(@")
      {
         var validationError = Validate(");

      _sb.RenderArguments(fieldsAndProperties);

      if (fieldsAndProperties.Count > 0)
         _sb.Append(", ");

      if (_state.HasKeyMember)
         _sb.Append("null, "); // IFormatProvider

      _sb.Append("out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj);

         if (validationError is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? ""Validation failed."");

         return obj").Append(allowNullOutput ? null : "!").Append(@";
      }");
   }

   private void GenerateTryCreateMethod(bool allowNullOutput, bool emptyStringYieldsNull)
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      public static bool TryCreate(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true, useNullableTypes: allowNullOutput);

      _sb.Append(@"
         ").Append(emptyStringYieldsNull ? null : "[global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ").Append("out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj)
      {
         var validationError = Validate(");

      _sb.RenderArguments(fieldsAndProperties);

      if (fieldsAndProperties.Count > 0)
         _sb.Append(", ");

      if (_state.HasKeyMember)
         _sb.Append("null, "); // IFormatProvider

      _sb.Append(@"out obj);

         return validationError is null;
      }");
   }

   private void GenerateValidateMethod(bool allowNullKeyMemberInput, bool allowNullOutput, bool emptyStringYieldsNull)
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      public static ").Append(_state.ValidationError.TypeFullyQualified).Append("? Validate(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true, useNullableTypes: allowNullKeyMemberInput);

      if (_state.HasKeyMember)
      {
         var providerArgumentName = _state.KeyMember.Member.ArgumentName.Escaped == "provider" ? "formatProvider" : "provider";
         _sb.Append(@"
         global::System.IFormatProvider? ").Append(providerArgumentName).Append(",");
      }

      _sb.Append(@"
         out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj)
      {");

      if (_state.HasKeyMember)
      {
         if (emptyStringYieldsNull)
         {
            _sb.Append(@"
         if(global::System.String.IsNullOrWhiteSpace(").Append(_state.KeyMember.Member.ArgumentName.Escaped).Append(@"))
         {
            obj = default;
            return null;
         }
");
         }
         else if (allowNullOutput)
         {
            _sb.Append(@"
         if(").Append(_state.KeyMember.Member.ArgumentName.Escaped).Append(@" is null)
         {
            obj = default;
            return null;
         }
");
         }
         else if (_state.KeyMember.Member.IsReferenceType)
         {
            _sb.Append(@"
         if(").Append(_state.KeyMember.Member.ArgumentName.Escaped).Append(@" is null)
         {
            obj = default;
            return CreateValidationError<").Append(_state.ValidationError.TypeFullyQualified).Append(@">(""The argument '").Append(_state.KeyMember.Member.ArgumentName.Escaped).Append(@"' must not be null."");
         }
");
         }
      }

      _sb.Append(@"
         ").Append(_state.ValidationError.TypeFullyQualified).Append(@"? validationError = null;
         ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("var ").Append(_FACTORY_ARGUMENTS_VALIDATION_ERROR).Append(" = ");

      _sb.Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append("(ref validationError");

      _sb.RenderArguments(fieldsAndProperties, "ref ", true);

      _sb.Append(@");

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
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("private ");

      _sb.Append("static partial ").Append(_state.FactoryValidationReturnType ?? "void").Append(" ").Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append("(ref ").Append(_state.ValidationError.TypeFullyQualified).Append("? validationError");

      _sb.RenderArgumentsWithType(fieldsAndProperties, "ref ", leadingComma: true, addAllowNullNotNullCombi: true);

      _sb.Append(");");
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
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append("new ").Append(_state.TypeFullyQualified).Append("(");
      _sb.RenderArguments(fieldsAndProperties);

      _sb.Append(")");
   }

   private void GenerateEqualityOperators()
   {
      _sb.Append(@"

      /// <summary>
      /// Compares to instances of <see cref=""").Append(_state.TypeMinimallyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==(").Append(_state.TypeFullyQualifiedNullAnnotated).Append(" obj, ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" other)
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
      /// Compares to instances of <see cref=""").Append(_state.TypeMinimallyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=(").Append(_state.TypeFullyQualifiedNullAnnotated).Append(" obj, ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" other)
      {
         return !(obj == other);
      }");
   }

   private void GenerateConstructor()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      var isStructDefaultCtor = !_state.IsReferenceType && fieldsAndProperties.Count == 0;

      if (isStructDefaultCtor)
         return;

      _sb.Append(@"

      private ").Append(_state.Name).Append("(");

      _sb.RenderArgumentsWithType(fieldsAndProperties);

      _sb.Append(@")
      {");

      if (fieldsAndProperties.Count > 0)
      {
         _sb.Append(@"
         ValidateConstructorArguments(");

         _sb.RenderArguments(fieldsAndProperties, "ref ");

         _sb.Append(@");
");

         foreach (var memberInfo in fieldsAndProperties)
         {
            _sb.Append(@"
         this.").Append(memberInfo.Name).Append(" = ").Append(memberInfo.ArgumentName.Escaped).Append(";");
         }
      }

      _sb.Append(@"
      }");

      if (fieldsAndProperties.Count > 0)
      {
         _sb.Append(@"

      static partial void ValidateConstructorArguments(");

         _sb.RenderArgumentsWithType(fieldsAndProperties, "ref ");

         _sb.Append(");");
      }
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

      if (_state.EqualityMembers.Count > 0)
      {
         for (var i = 0; i < _state.EqualityMembers.Count; i++)
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

            if (equalityComparerAccessor == null)
            {
               if (member.IsReferenceType)
               {
                  _sb.Append("(this.").Append(member.Name).Append(" is null ? other.").Append(member.Name).Append(" is null : this.").Append(member.Name).Append(".Equals(other.").Append(member.Name).Append("))");
               }
               else
               {
                  _sb.Append("this.").Append(member.Name).Append(".Equals(other.").Append(member.Name).Append(")");
               }
            }
            else
            {
               _sb.Append(equalityComparerAccessor).Append(".EqualityComparer.Equals(this.").Append(member.Name).Append(", other.").Append(member.Name).Append(")");
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

   private void GenerateGetHashCode()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override int GetHashCode()
      {");

      if (_state.EqualityMembers.Count > 0)
      {
         var useShortForm = _state.EqualityMembers.Count <= 8 && _state.EqualityMembers.All(m => m.EqualityComparerAccessor == null);

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

         for (var i = 0; i < _state.EqualityMembers.Count; i++)
         {
            var (member, equalityComparerAccessor) = _state.EqualityMembers[i];

            if (useShortForm)
            {
               if (i > 0)
               {
                  _sb.Append(@",
            ");
               }

               _sb.Append("this.").Append(member.Name);
            }
            else
            {
               _sb.Append(@"
         hashCode.Add(this.").Append(member.Name);

               if (equalityComparerAccessor is not null)
               {
                  _sb.Append(", ").Append(equalityComparerAccessor).Append(".EqualityComparer");

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
         return _type.GetHashCode();");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateToString()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override string ToString()
      {");

      if (_state.HasKeyMember)
      {
         var keyMember = _state.KeyMember.Member;

         _sb.Append(@"
         return this.").Append(keyMember.Name).Append(".ToString();");
      }
      else if (_state.EqualityMembers.Count > 0)
      {
         _sb.Append(@"
         return $""{{");

         for (var i = 0; i < _state.EqualityMembers.Count; i++)
         {
            var member = _state.EqualityMembers[i].Member;

            if (i > 0)
               _sb.Append(',');

            _sb.Append(" ").Append(member.Name).Append(" = {this.").Append(member.Name).Append("}");
         }

         _sb.Append(" }}\";");
      }
      else
      {
         _sb.Append(@"
         return """).Append(_state.TypeMinimallyQualified).Append(@""";");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateValidationErrorFactory()
   {
      _sb.Append(@"

      private static TError CreateValidationError<TError>(string message)
         where TError : class, global::Thinktecture.IValidationError<TError>
      {
         return TError.Create(message);
      }");
   }
}
