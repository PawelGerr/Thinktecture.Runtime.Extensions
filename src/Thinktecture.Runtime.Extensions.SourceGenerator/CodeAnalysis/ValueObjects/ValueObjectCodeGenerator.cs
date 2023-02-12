using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectCodeGenerator : CodeGeneratorBase
{
   private const string _FACTORY_ARGUMENTS_VALIDATION_RESULT = "factoryArgumentsValidationResult";
   private const string _FACTORY_POST_INIT = "FactoryPostInit";

   private readonly ValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string? FileNameSuffix => null;

   public ValueObjectCodeGenerator(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state ?? throw new ArgumentNullException(nameof(state));
      _sb = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));
   }

   public override string Generate()
   {
      _sb.AppendLine(GENERATED_CODE_PREFIX);

      var hasNamespace = _state.Namespace is not null;

      if (hasNamespace)
      {
         _sb.Append(@"
namespace ").Append(_state.Namespace).Append(@"
{");
      }

      var emptyStringYieldsNull = _state.Settings.EmptyStringInFactoryMethodsYieldsNull && _state.IsReferenceType && _state.HasKeyMember && _state.KeyMember.Member.IsString();

      if (_state.HasKeyMember)
         GenerateTypeConverter(_state.KeyMember, emptyStringYieldsNull);

      GenerateValueObject(emptyStringYieldsNull);

      if (hasNamespace)
      {
         _sb.Append(@"
}");
      }

      _sb.Append(@"
");

      return _sb.ToString();
   }

   private void GenerateTypeConverter(EqualityInstanceMemberInfo keyMemberInfo, bool emptyStringYieldsNull)
   {
      var keyMember = keyMemberInfo.Member;

      _sb.Append($@"
   public class {_state.Name}_ValueObjectTypeConverter : global::Thinktecture.ValueObjectTypeConverter<{_state.TypeFullyQualified}, {keyMember.TypeFullyQualified}>
   {{
      /// <inheritdoc />");

      // If emptyStringYieldsNull=true then an empty-string-argument (i.e. not null) will lead to null as return value,
      // that's why we cannot use the NotNullIfNotNullAttribute.
      if (keyMember.IsReferenceType && !emptyStringYieldsNull)
      {
         _sb.Append($@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{keyMember.ArgumentName}"")]");
      }

      _sb.Append($@"
      protected override {(keyMember.IsReferenceType ? _state.TypeFullyQualifiedNullAnnotated : _state.TypeFullyQualified)} ConvertFrom({keyMember.TypeFullyQualifiedNullAnnotated} {keyMember.ArgumentName})
      {{");

      if (keyMember.IsReferenceType)
      {
         _sb.Append($@"
         if({keyMember.ArgumentName} is null)
            return default({_state.TypeFullyQualified});
");
      }

      _sb.Append($@"
         return {_state.TypeFullyQualified}.Create({keyMember.ArgumentName});
      }}

      /// <inheritdoc />
      protected override {keyMember.TypeFullyQualifiedWithNullability} GetKeyValue({_state.TypeFullyQualified} obj)
      {{
         return ({keyMember.TypeFullyQualifiedWithNullability}) obj;
      }}
   }}
");
   }

   private void GenerateValueObject(bool emptyStringYieldsNull)
   {
      var interfaceCodeGenerators = _state.GetInterfaceCodeGenerators();

      if (_state.HasKeyMember)
      {
         _sb.Append($@"
   [global::System.ComponentModel.TypeConverter(typeof({_state.TypeFullyQualified}_ValueObjectTypeConverter))]");
      }

      _sb.Append($@"
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name} : global::System.IEquatable<{_state.TypeFullyQualifiedNullAnnotated}>");

      for (var i = 0; i < interfaceCodeGenerators.Length; i++)
      {
         _sb.Append(", ");

         interfaceCodeGenerators[i].GenerateBaseTypes(_sb, _state);
      }

      if (_state.HasKeyMember)
      {
         _sb.Append($@", global::Thinktecture.IKeyedValueObject<{_state.KeyMember.Member.TypeFullyQualifiedWithNullability}>");

         if (!_state.Settings.SkipFactoryMethods)
         {
            _sb.Append($@", global::Thinktecture.IKeyedValueObject<{_state.TypeFullyQualified}, {_state.KeyMember.Member.TypeFullyQualifiedWithNullability}>");
         }
      }
      else
      {
         _sb.Append(@", global::Thinktecture.IComplexValueObject");
      }

      _sb.Append(@"
   {");

      if (_state.HasKeyMember)
         GenerateModuleInitializer(_state.KeyMember.Member, emptyStringYieldsNull);

      _sb.Append($@"
      private static readonly global::System.Type _type = typeof({_state.TypeFullyQualified});");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"

      public static readonly {_state.TypeFullyQualified} {_state.Settings.DefaultInstancePropertyName} = default;");
      }

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
         GenerateGetKey(_state.KeyMember);
         GenerateImplicitConversionToKey(_state.KeyMember);
         GenerateExplicitConversionToKey(_state.KeyMember);
         GenerateExplicitConversion(_state.KeyMember, emptyStringYieldsNull);
      }
      else
      {
         GenerateGetAssignableMembers();
      }

      GenerateConstructor();
      GenerateEqualityOperators();
      GenerateEquals();
      GenerateGetHashCode();
      GenerateToString();

      if (_state.HasKeyMember)
      {
         for (var i = 0; i < interfaceCodeGenerators.Length; i++)
         {
            interfaceCodeGenerators[i].GenerateImplementation(_sb, _state, _state.KeyMember.Member);
         }
      }

      _sb.Append(@"
   }");
   }

   private void GenerateGetKey(EqualityInstanceMemberInfo keyMember)
   {
      _sb.Append($@"

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      {keyMember.Member.TypeFullyQualified} global::Thinktecture.IKeyedValueObject<{keyMember.Member.TypeFullyQualified}>.GetKey()
      {{
         return this.{keyMember.Member.Name};
      }}");
   }

   private void GenerateModuleInitializer(IMemberState keyMember, bool emptyStringYieldsNull)
   {
      var typeFullyQualified = _state.TypeFullyQualified;
      var nullAnnotatedTypeFullyQualified = emptyStringYieldsNull ? _state.TypeFullyQualifiedNullAnnotated : typeFullyQualified;
      var keyMemberWithoutNullAnnotation = keyMember.IsReferenceType ? keyMember.TypeFullyQualified : keyMember.TypeFullyQualifiedWithNullability;

      _sb.Append($@"
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {{
         var convertFromKey = new global::System.Func<{keyMember.TypeFullyQualifiedWithNullability}, {nullAnnotatedTypeFullyQualified}>({typeFullyQualified}.Create);
         global::System.Linq.Expressions.Expression<global::System.Func<{keyMember.TypeFullyQualifiedWithNullability}, {nullAnnotatedTypeFullyQualified}>> convertFromKeyExpression = static {keyMember.ArgumentName} => {typeFullyQualified}.Create({keyMember.ArgumentName});
         global::System.Linq.Expressions.Expression<global::System.Func<{keyMember.TypeFullyQualifiedWithNullability}, {typeFullyQualified}>> convertFromKeyExpressionViaCtor = static {keyMember.ArgumentName} => new {typeFullyQualified}({keyMember.ArgumentName});

         var convertToKey = new global::System.Func<{typeFullyQualified}, {keyMember.TypeFullyQualifiedWithNullability}>(static item => item.{keyMember.Name});
         global::System.Linq.Expressions.Expression<global::System.Func<{typeFullyQualified}, {keyMember.TypeFullyQualifiedWithNullability}>> convertToKeyExpression = static obj => obj.{keyMember.Name};

         var type = typeof({typeFullyQualified});
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof({keyMemberWithoutNullAnnotation}), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
      }}
");
   }

   private void GenerateImplicitConversionToKey(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;

      _sb.Append($@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""{keyMember.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""{keyMember.Name}""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static implicit operator {keyMember.TypeFullyQualifiedNullable}({(_state.IsReferenceType ? _state.TypeFullyQualifiedNullAnnotated : _state.TypeFullyQualifiedNullable)} obj)
      {{
         return obj?.{keyMember.Name};
      }}");

      if (_state.IsReferenceType || keyMember.IsReferenceType)
         return;

      // if value object and key member are structs

      _sb.Append($@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""{keyMember.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""{keyMember.Name}""/> of provided <paramref name=""obj""/>.</returns>
      public static implicit operator {keyMember.TypeFullyQualified}({_state.TypeFullyQualified} obj)
      {{
         return obj.{keyMember.Name};
      }}");
   }

   private void GenerateExplicitConversionToKey(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;

      if (keyMember.IsReferenceType || !_state.IsReferenceType)
         return;

      _sb.Append($@"

      /// <summary>
      /// Explicit conversion to the type <see cref=""{keyMember.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""{keyMember.Name}""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static explicit operator {keyMember.TypeFullyQualifiedWithNullability}({_state.TypeFullyQualified} obj)
      {{
         if(obj is null)
            throw new global::System.NullReferenceException();

         return obj.{keyMember.Name};
      }}");
   }

   private void GenerateExplicitConversion(EqualityInstanceMemberInfo keyMemberInfo, bool emptyStringYieldsNull)
   {
      var keyMember = keyMemberInfo.Member;
      var bothAreReferenceTypes = _state.IsReferenceType && keyMemberInfo.Member.IsReferenceType;
      var nullableQuestionMark = bothAreReferenceTypes ? "?" : null;

      _sb.Append($@"

      /// <summary>
      /// Explicit conversion from the type <see cref=""{keyMember.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""{keyMember.ArgumentName}"">Value to covert.</param>
      /// <returns>An instance of <see cref=""{_state.TypeMinimallyQualified}""/>.</returns>");

      if (bothAreReferenceTypes && !emptyStringYieldsNull)
      {
         _sb.Append($@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{keyMember.ArgumentName}"")]");
      }

      _sb.Append($@"
      public static explicit operator {_state.TypeFullyQualified}{nullableQuestionMark}({keyMember.TypeFullyQualified}{nullableQuestionMark} {keyMember.ArgumentName})
      {{");

      if (bothAreReferenceTypes)
      {
         _sb.Append($@"
         if({keyMember.ArgumentName} is null)
            return null;
");
      }

      _sb.Append($@"
         return {_state.TypeFullyQualified}.Create({keyMember.ArgumentName});
      }}");
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
         _sb.Append($@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{_state.KeyMember!.Member.ArgumentName}"")]");
      }

      _sb.Append($@"
      public static {_state.TypeFullyQualified}{(allowNullOutput ? "?" : null)} Create(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, useNullableTypes: allowNullOutput);

      _sb.Append(@")
      {
         var validationResult = Validate(");

      _sb.RenderArguments(fieldsAndProperties);

      if (fieldsAndProperties.Count > 0)
         _sb.Append(", ");

      _sb.Append($@"out {_state.TypeFullyQualifiedNullAnnotated} obj);

         if (validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationResult!.ErrorMessage ?? ""Validation failed."");

         return obj{(allowNullOutput ? null : "!")};
      }}");
   }

   private void GenerateTryCreateMethod(bool allowNullOutput, bool emptyStringYieldsNull)
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      public static bool TryCreate(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true, useNullableTypes: allowNullOutput);

      _sb.Append($@"
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out {_state.TypeFullyQualifiedNullAnnotated} obj)
      {{
         var validationResult = Validate(");

      _sb.RenderArguments(fieldsAndProperties);

      if (fieldsAndProperties.Count > 0)
         _sb.Append(", ");

      _sb.Append(@"out obj);

         return validationResult == global::System.ComponentModel.DataAnnotations.ValidationResult.Success;
      }");
   }

   private void GenerateValidateMethod(bool allowNullKeyMemberInput, bool allowNullOutput, bool emptyStringYieldsNull)
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      public static global::System.ComponentModel.DataAnnotations.ValidationResult? Validate(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true, useNullableTypes: allowNullKeyMemberInput);

      _sb.Append($@"
         [global::System.Diagnostics.CodeAnalysis.MaybeNull] out {_state.TypeFullyQualifiedNullAnnotated} obj)
      {{");

      if (_state.HasKeyMember)
      {
         if (emptyStringYieldsNull)
         {
            _sb.Append($@"
         if(global::System.String.IsNullOrWhiteSpace({_state.KeyMember.Member.ArgumentName}))
         {{
            obj = default;
            return global::System.ComponentModel.DataAnnotations.ValidationResult.Success;
         }}
");
         }
         else if (allowNullOutput)
         {
            _sb.Append($@"
         if({_state.KeyMember.Member.ArgumentName} is null)
         {{
            obj = default;
            return global::System.ComponentModel.DataAnnotations.ValidationResult.Success;
         }}
");
         }
         else if (_state.KeyMember.Member.IsReferenceType)
         {
            _sb.Append($@"
         if({_state.KeyMember.Member.ArgumentName} is null)
         {{
            obj = default;
            return new global::System.ComponentModel.DataAnnotations.ValidationResult(""The argument '{_state.KeyMember.Member.ArgumentName}' must not be null."");
         }}
");
         }
      }

      _sb.Append(@"
         var validationResult = global::System.ComponentModel.DataAnnotations.ValidationResult.Success;
         ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("var ").Append(_FACTORY_ARGUMENTS_VALIDATION_RESULT).Append(" = ");

      _sb.Append(@"ValidateFactoryArguments(ref validationResult");

      _sb.RenderArguments(fieldsAndProperties, "ref ", true);

      _sb.Append(@");

         if (validationResult == global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
         {
            obj = ");

      GenerateConstructCall();

      _sb.Append(@";
            obj.").Append(_FACTORY_POST_INIT).Append("(");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append(_FACTORY_ARGUMENTS_VALIDATION_RESULT);

      _sb.Append(@");
         }
         else
         {
            obj = default;
         }

         return validationResult;
      }");
   }

   private void GenerateValidateFactoryArguments()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("private ");

      _sb.Append("static partial ").Append(_state.FactoryValidationReturnType ?? "void").Append(" ValidateFactoryArguments(ref global::System.ComponentModel.DataAnnotations.ValidationResult? validationResult");

      _sb.RenderArgumentsWithType(fieldsAndProperties, "ref ", leadingComma: true);

      _sb.Append(@");");
   }

   private void GenerateFactoryPostInit()
   {
      _sb.Append(@"

      partial void ").Append(_FACTORY_POST_INIT).Append("(");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append(_state.FactoryValidationReturnType).Append(" ").Append(_FACTORY_ARGUMENTS_VALIDATION_RESULT);

      _sb.Append(@");");
   }

   private void GenerateConstructCall()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append($@"new {_state.TypeFullyQualified}(");
      _sb.RenderArguments(fieldsAndProperties);

      _sb.Append(@")");
   }

   private void GenerateEqualityOperators()
   {
      _sb.Append($@"

      /// <summary>
      /// Compares to instances of <see cref=""{_state.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==({_state.TypeFullyQualifiedNullAnnotated} obj, {_state.TypeFullyQualifiedNullAnnotated} other)
      {{");

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
         if (obj is null)
            return other is null;
");
      }

      _sb.Append($@"
         return obj.Equals(other);
      }}

      /// <summary>
      /// Compares to instances of <see cref=""{_state.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=({_state.TypeFullyQualifiedNullAnnotated} obj, {_state.TypeFullyQualifiedNullAnnotated} other)
      {{
         return !(obj == other);
      }}");
   }

   private void GenerateGetAssignableMembers()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append($@"

      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::System.Reflection.MemberInfo>> _assignableMembers
         = new(() =>
               {{
                  global::System.Linq.Expressions.Expression<global::System.Func<{_state.Name}, object>> action = o => new
                                                                                                                     {{");

      for (var i = 0; i < fieldsAndProperties.Count; i++)
      {
         if (i > 0)
            _sb.Append(",");

         var memberInfo = fieldsAndProperties[i];
         _sb.Append($@"
                                                                                                                        o.{memberInfo.Name}");
      }

      _sb.Append(@"
                                                                                                                     };

               var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

               foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
               {
                  members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
               }

               return members.AsReadOnly();
            }, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      static global::System.Collections.Generic.IReadOnlyList<global::System.Reflection.MemberInfo> global::Thinktecture.IComplexValueObject.GetAssignableMembers()
      {
         return _assignableMembers.Value;
      }");
   }

   private void GenerateConstructor()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append($@"

      private {_state.Name}(");

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
            _sb.Append($@"
         this.{memberInfo.Name} = {memberInfo.ArgumentName};");
         }
      }

      _sb.Append(@"
      }");

      if (fieldsAndProperties.Count > 0)
      {
         _sb.Append(@"

      static partial void ValidateConstructorArguments(");

         _sb.RenderArgumentsWithType(fieldsAndProperties, "ref ");

         _sb.Append(@");");
      }
   }

   private void GenerateEquals()
   {
      _sb.Append($@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {{
         return other is {_state.TypeFullyQualified} obj && Equals(obj);
      }}

      /// <inheritdoc />
      public bool Equals({_state.TypeFullyQualifiedNullAnnotated} other)
      {{");

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
         if (other is null)
            return false;

         if (!global::System.Object.ReferenceEquals(GetType(), other.GetType()))
            return false;

         if (global::System.Object.ReferenceEquals(this, other))
            return true;
");
      }

      if (_state.EqualityMembers.Count > 0)
      {
         for (var i = 0; i < _state.EqualityMembers.Count; i++)
         {
            var (member, equalityComparer) = _state.EqualityMembers[i];

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

            if (equalityComparer == null)
            {
               if (member.IsReferenceType)
               {
                  _sb.Append($@"(this.{member.Name} is null ? other.{member.Name} is null : this.{member.Name}.Equals(other.{member.Name}))");
               }
               else
               {
                  _sb.Append($@"this.{member.Name}.Equals(other.{member.Name})");
               }
            }
            else
            {
               _sb.Append($@"{equalityComparer}.Equals(this.{member.Name}, other.{member.Name})");
            }
         }

         _sb.Append(@";");
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
         var useShortForm = _state.EqualityMembers.Count <= 8 && _state.EqualityMembers.All(m => m.EqualityComparer == null);

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
            var (member, equalityComparer) = _state.EqualityMembers[i];

            if (useShortForm)
            {
               if (i > 0)
               {
                  _sb.Append(@",
            ");
               }

               _sb.Append($@"this.{member.Name}");
            }
            else
            {
               _sb.Append($@"
         hashCode.Add(this.{member.Name}");

               if (equalityComparer is not null)
                  _sb.Append($@", {equalityComparer}");

               _sb.Append(@");");
            }
         }

         if (useShortForm)
         {
            _sb.Append(@");");
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
      public override string? ToString()
      {");

      if (_state.HasKeyMember)
      {
         var keyMember = _state.KeyMember.Member;
         var nullConditionalOperator = keyMember.IsReferenceType ? "?" : null;

         _sb.Append($@"
         return this.{keyMember.Name}{nullConditionalOperator}.ToString();");
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

            _sb.Append($@" {member.Name} = {{this.{member.Name}}}");
         }

         _sb.Append(" }}\";");
      }
      else
      {
         _sb.Append($@"
         return ""{_state.TypeMinimallyQualified}"";");
      }

      _sb.Append(@"
      }");
   }
}
