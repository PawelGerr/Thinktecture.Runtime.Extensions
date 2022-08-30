using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public class ValueObjectCodeGenerator : CodeGeneratorBase
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

      if (_state.HasKeyMember)
         GenerateTypeConverter(_state.KeyMember);

      GenerateValueObject();

      if (hasNamespace)
      {
         _sb.Append(@"
}");
      }

      _sb.Append(@"
");

      return _sb.ToString();
   }

   private void GenerateTypeConverter(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;

      _sb.Append($@"
   public class {_state.Name}_ValueObjectTypeConverter : global::Thinktecture.ValueObjectTypeConverter<{_state.TypeFullyQualified}, {keyMember.TypeFullyQualifiedWithNullability}>
   {{
      /// <inheritdoc />");

      if (keyMember.IsReferenceType)
      {
         _sb.Append($@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{keyMember.ArgumentName}"")]");
      }

      _sb.Append($@"
      protected override {_state.TypeFullyQualified}{(keyMember.IsReferenceType ? _state.NullableQuestionMark : null)} ConvertFrom({keyMember.TypeFullyQualified}{keyMember.NullableQuestionMark} {keyMember.ArgumentName})
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

   private void GenerateValueObject()
   {
      var isFormattable = _state.HasKeyMember && _state.KeyMember.Member.IsFormattable;
      var isComparable = !_state.Settings.SkipCompareTo && _state.HasKeyMember && (_state.KeyMember.Member.IsComparable || _state.KeyMember.Comparer is not null);

      _sb.Append(@"
   [global::Thinktecture.Internal.ValueObjectConstructor(");

      for (var i = 0; i < _state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         if (i != 0)
            _sb.Append(", ");

         _sb.Append($@"nameof({_state.AssignableInstanceFieldsAndProperties[i].Name})");
      }

      _sb.Append(")]");

      if (_state.HasKeyMember)
      {
         _sb.Append($@"
   [global::Thinktecture.Internal.KeyedValueObject]
   [global::System.ComponentModel.TypeConverter(typeof({_state.TypeFullyQualified}_ValueObjectTypeConverter))]");
      }

      _sb.Append($@"
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name} : global::System.IEquatable<{_state.TypeFullyQualified}{_state.NullableQuestionMark}>");

      if (isFormattable)
         _sb.Append(", global::System.IFormattable");

      if (isComparable)
         _sb.Append($", global::System.IComparable, global::System.IComparable<{_state.TypeFullyQualified}>");

      _sb.Append(@"
   {");

      if (_state.HasKeyMember)
      {
         var keyMember = _state.KeyMember.Member;

         _sb.Append($@"
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {{
         var convertFromKey = new global::System.Func<{keyMember.TypeFullyQualifiedWithNullability}, {_state.TypeFullyQualified}>({_state.TypeFullyQualified}.Create);
         global::System.Linq.Expressions.Expression<global::System.Func<{keyMember.TypeFullyQualifiedWithNullability}, {_state.TypeFullyQualified}>> convertFromKeyExpression = static {keyMember.ArgumentName} => new {_state.TypeFullyQualified}({keyMember.ArgumentName});

         var convertToKey = new global::System.Func<{_state.TypeFullyQualified}, {keyMember.TypeFullyQualifiedWithNullability}>(static item => item.{keyMember.Name});
         global::System.Linq.Expressions.Expression<global::System.Func<{_state.TypeFullyQualified}, {keyMember.TypeFullyQualifiedWithNullability}>> convertToKeyExpression = static obj => obj.{keyMember.Name};

         var tryCreate = new global::Thinktecture.Internal.Validate<{_state.TypeFullyQualified}, {_state.KeyMember.Member.TypeFullyQualifiedWithNullability}>({_state.TypeFullyQualified}.TryCreate);

         var type = typeof({_state.TypeFullyQualified});
         var metadata = new global::Thinktecture.Internal.ValueObjectMetadata(type, typeof({keyMember.TypeFullyQualifiedWithNullability}), false, false, convertFromKey, convertFromKeyExpression, convertToKey, convertToKeyExpression, tryCreate);

         global::Thinktecture.Internal.ValueObjectMetadataLookup.AddMetadata(type, metadata);
      }}
");
      }

      _sb.Append($@"
      private static readonly global::System.Type _type = typeof({_state.TypeFullyQualified});");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"

      public static readonly {_state.TypeFullyQualified} Empty = default;");
      }

      if (!_state.Settings.SkipFactoryMethods)
      {
         var allowNullKeyMember = _state.HasKeyMember && _state.KeyMember.Member.IsReferenceType && _state.IsReferenceType && _state.Settings.NullInFactoryMethodsYieldsNull;

         GenerateCreateMethod(allowNullKeyMember);
         GenerateTryCreateMethod(allowNullKeyMember);
         GenerateValidateFactoryArguments();
         GenerateFactoryPostInit();
      }

      if (_state.HasKeyMember)
      {
         GenerateImplicitConversionToKey(_state.KeyMember);
         GenerateExplicitConversionToKey(_state.KeyMember);
         GenerateExplicitConversion(_state.KeyMember);
      }

      GenerateConstructor();
      GenerateEqualityOperators();
      GenerateEquals();
      GenerateGetHashCode();
      GenerateToString();

      if (_state.HasKeyMember)
      {
         if (isFormattable)
            GenerateToStringFormat(_state.KeyMember);

         if (isComparable)
            GenerateCompareTo(_state.KeyMember);
      }

      _sb.Append(@"
   }");
   }

   private void GenerateImplicitConversionToKey(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;
      var returnTypeNullableQuestionMark = _state.IsReferenceType ? "?" : keyMember.NullableQuestionMark;

      _sb.Append($@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""{keyMember.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""{keyMember.Name}""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static implicit operator {keyMember.TypeFullyQualified}{returnTypeNullableQuestionMark}({_state.TypeFullyQualified}{_state.NullableQuestionMark} obj)
      {{");

      if (_state.IsReferenceType)
      {
         _sb.Append($@"
         return obj is null ? null : obj.{keyMember.Name};");
      }
      else
      {
         _sb.Append($@"
         return obj.{keyMember.Name};");
      }

      _sb.Append(@"
      }");
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

   private void GenerateExplicitConversion(EqualityInstanceMemberInfo keyMemberInfo)
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

      if (bothAreReferenceTypes)
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

   private void GenerateCreateMethod(bool allowNullKeyMember)
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"
");

      if (allowNullKeyMember)
      {
         _sb.Append($@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{_state.KeyMember!.Member.ArgumentName}"")]");
      }

      _sb.Append($@"
      public static {_state.TypeFullyQualified}{(allowNullKeyMember ? "?" : null)} Create(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, useNullableTypes: allowNullKeyMember);

      _sb.Append(@")
      {");

      if (allowNullKeyMember)
      {
         _sb.Append($@"
         if({_state.KeyMember!.Member.ArgumentName} is null)
            return default;
");
      }

      _sb.Append(@"
         var validationResult = global::System.ComponentModel.DataAnnotations.ValidationResult.Success;
         ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("var ").Append(_FACTORY_ARGUMENTS_VALIDATION_RESULT).Append(" = ");

      _sb.Append(@"ValidateFactoryArguments(ref validationResult");

      _sb.RenderArguments(fieldsAndProperties, "ref ", true);

      _sb.Append(@");

         if(validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationResult!.ErrorMessage ?? ""Validation failed."");

         var obj = ");

      GenerateConstructCall();

      _sb.Append(@";
         obj.").Append(_FACTORY_POST_INIT).Append("(");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append(_FACTORY_ARGUMENTS_VALIDATION_RESULT);

      _sb.Append(@");

         return obj;
      }");
   }

   private void GenerateTryCreateMethod(bool allowNullKeyMember)
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      public static global::System.ComponentModel.DataAnnotations.ValidationResult? TryCreate(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true, useNullableTypes: allowNullKeyMember);

      _sb.Append($@"
         [global::System.Diagnostics.CodeAnalysis.MaybeNull] out {_state.TypeFullyQualified}{_state.NullableQuestionMark} obj)
      {{");

      if (allowNullKeyMember)
      {
         _sb.Append($@"
         if({_state.KeyMember!.Member.ArgumentName} is null)
         {{
            obj = default;
            return null;
         }}
");
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
      public static bool operator ==({_state.TypeFullyQualified}{_state.NullableQuestionMark} obj, {_state.TypeFullyQualified}{_state.NullableQuestionMark} other)
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
      public static bool operator !=({_state.TypeFullyQualified}{_state.NullableQuestionMark} obj, {_state.TypeFullyQualified}{_state.NullableQuestionMark} other)
      {{
         return !(obj == other);
      }}");
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
      public bool Equals({_state.TypeFullyQualified}{_state.NullableQuestionMark} other)
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

         _sb.Append($@"
         return this.{keyMember.Name}{keyMember.NullableQuestionMark}.ToString();");
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

   private void GenerateToStringFormat(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;

      _sb.Append($@"

      /// <inheritdoc />
      public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)
      {{
         return this.{keyMember.Name}.ToString(format, formatProvider);
      }}");
   }

   private void GenerateCompareTo(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;
      var comparer = keyMemberInfo.Comparer;

      _sb.Append($@"

      /// <inheritdoc />
      public int CompareTo(object? obj)
      {{
         if(obj is null)
            return 1;

         if(obj is not {_state.TypeFullyQualified} valueObject)
            throw new global::System.ArgumentException(""Argument must be of type \""{_state.TypeMinimallyQualified}\""."", nameof(obj));

         return this.CompareTo(valueObject);
      }}

      /// <inheritdoc />
      public int CompareTo({_state.TypeFullyQualified}{_state.NullableQuestionMark} obj)
      {{");

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
         if(obj is null)
            return 1;
");
      }

      if (comparer is null)
      {
         if (keyMember.IsReferenceType)
         {
            _sb.Append($@"
         if(this.{keyMember.Name} is null)
            return obj.{keyMember.Name} is null ? 0 : -1;
");
         }

         _sb.Append($@"
         return this.{keyMember.Name}.CompareTo(obj.{keyMember.Name});");
      }
      else
      {
         _sb.Append($@"
         return {comparer}.Compare(this.{keyMember.Name}, obj.{keyMember.Name});");
      }

      _sb.Append(@"
      }");
   }
}
