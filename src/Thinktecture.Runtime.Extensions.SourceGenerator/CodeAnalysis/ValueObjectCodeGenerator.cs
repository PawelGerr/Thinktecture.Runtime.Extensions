using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class ValueObjectCodeGenerator
{
   private readonly ValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public ValueObjectCodeGenerator(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state ?? throw new ArgumentNullException(nameof(state));
      _sb = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));
   }

   public string Generate()
   {
      _sb.AppendLine(ThinktectureSourceGeneratorBase.GENERATED_CODE_PREFIX);

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
   public class {_state.Type.Name}_ValueObjectTypeConverter : global::Thinktecture.ValueObjectTypeConverter<{_state.TypeFullyQualified}, {keyMember.TypeFullyQualified}>
   {{
      /// <inheritdoc />");

      if (keyMember.Type.IsReferenceType)
      {
         _sb.Append($@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{keyMember.ArgumentName}"")]");
      }

      _sb.Append($@"
      protected override {_state.TypeFullyQualified}{(keyMember.Type.IsReferenceType ? _state.NullableQuestionMark : null)} ConvertFrom({keyMember.TypeFullyQualified}{keyMember.NullableQuestionMark} {keyMember.ArgumentName})
      {{");

      if (keyMember.Type.IsReferenceType)
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
      protected override {keyMember.TypeFullyQualified} GetKeyValue({_state.TypeFullyQualified} obj)
      {{
         return ({keyMember.TypeFullyQualified}) obj;
      }}
   }}
");
   }

   private void GenerateValueObject()
   {
      var isFormattable = _state.HasKeyMember && _state.KeyMember.Member.Type.IsFormattable();
      var isComparable = !_state.SkipCompareTo && _state.HasKeyMember && (_state.KeyMember.Member.Type.IsComparable() || _state.KeyMember.Comparer is not null);

      _sb.Append(@"
   [global::Thinktecture.Internal.ValueObjectConstructor(");

      for (var i = 0; i < _state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         if (i != 0)
            _sb.Append(", ");

         _sb.Append($@"nameof({_state.AssignableInstanceFieldsAndProperties[i].Identifier})");
      }

      _sb.Append(")]");

      if (_state.HasKeyMember)
      {
         _sb.Append($@"
   [global::Thinktecture.Internal.KeyedValueObject]
   [global::System.ComponentModel.TypeConverter(typeof({_state.TypeFullyQualified}_ValueObjectTypeConverter))]");
      }

      _sb.Append($@"
   partial {(_state.Type.IsValueType ? "struct" : "class")} {_state.Type.Name} : global::System.IEquatable<{_state.TypeFullyQualified}{_state.NullableQuestionMark}>");

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
         var convertFromKey = new global::System.Func<{keyMember.TypeFullyQualified}, {_state.TypeFullyQualified}>({_state.TypeFullyQualified}.Create);
         global::System.Linq.Expressions.Expression<global::System.Func<{keyMember.TypeFullyQualified}, {_state.TypeFullyQualified}>> convertFromKeyExpression = static {keyMember.ArgumentName} => new {_state.TypeFullyQualified}({keyMember.ArgumentName});

         var convertToKey = new global::System.Func<{_state.TypeFullyQualified}, {keyMember.TypeFullyQualified}>(static item => item.{keyMember.Identifier});
         global::System.Linq.Expressions.Expression<global::System.Func<{_state.TypeFullyQualified}, {keyMember.TypeFullyQualified}>> convertToKeyExpression = static obj => obj.{keyMember.Identifier};

         var tryCreate = new global::Thinktecture.Internal.Validate<{_state.TypeFullyQualified}, {_state.KeyMember.Member.TypeFullyQualified}>({_state.TypeFullyQualified}.TryCreate);

         var type = typeof({_state.TypeFullyQualified});
         var metadata = new global::Thinktecture.Internal.ValueObjectMetadata(type, typeof({keyMember.TypeFullyQualified}), false, convertFromKey, convertFromKeyExpression, convertToKey, convertToKeyExpression, tryCreate);

         global::Thinktecture.Internal.ValueObjectMetadataLookup.AddMetadata(type, metadata);
      }}
");
      }

      _sb.Append($@"
      private static readonly global::System.Type _type = typeof({_state.TypeFullyQualified});");

      if (_state.Type.IsValueType)
      {
         _sb.Append($@"

      public static readonly {_state.TypeFullyQualified} Empty = default;");
      }

      if (!_state.SkipFactoryMethods)
      {
         var allowNullKeyMember = _state.HasKeyMember && _state.KeyMember.Member.Type.IsReferenceType && _state.Type.IsReferenceType && _state.NullInFactoryMethodsYieldsNull;

         GenerateCreateMethod(allowNullKeyMember);
         GenerateTryCreateMethod(allowNullKeyMember);
         GenerateValidateFactoryArguments();
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
      var returnTypeNullableQuestionMark = _state.Type.IsReferenceType ? "?" : keyMember.NullableQuestionMark;

      _sb.Append($@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""{keyMember.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""{keyMember.Identifier}""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static implicit operator {keyMember.TypeFullyQualified}{returnTypeNullableQuestionMark}({_state.TypeFullyQualified}{_state.NullableQuestionMark} obj)
      {{");

      if (_state.Type.IsReferenceType)
      {
         _sb.Append($@"
         return obj is null ? null : obj.{keyMember.Identifier};");
      }
      else
      {
         _sb.Append($@"
         return obj.{keyMember.Identifier};");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateExplicitConversionToKey(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;

      if (keyMember.Type.IsReferenceType || _state.Type.IsValueType)
         return;

      _sb.Append($@"

      /// <summary>
      /// Explicit conversion to the type <see cref=""{keyMember.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""{keyMember.Identifier}""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""obj"")]
      public static explicit operator {keyMember.TypeFullyQualified}({_state.TypeFullyQualified} obj)
      {{
         if(obj is null)
            throw new global::System.NullReferenceException();

         return obj.{keyMember.Identifier};
      }}");
   }

   private void GenerateExplicitConversion(EqualityInstanceMemberInfo keyMemberInfo)
   {
      var keyMember = keyMemberInfo.Member;
      var bothAreReferenceTypes = _state.Type.IsReferenceType && keyMemberInfo.Member.Type.IsReferenceType;
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
         ValidateFactoryArguments(ref validationResult");

      _sb.RenderArguments(fieldsAndProperties, "ref ", true);

      _sb.Append(@");

         if(validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationResult!.ErrorMessage ?? ""Validation failed."");

         return ");

      GenerateConstructCall();

      _sb.Append(@";
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
         ValidateFactoryArguments(ref validationResult");

      _sb.RenderArguments(fieldsAndProperties, "ref ", true);

      _sb.Append(@");

         obj = validationResult == global::System.ComponentModel.DataAnnotations.ValidationResult.Success
               ? ");

      GenerateConstructCall();

      _sb.Append(@"
               : default;

         return validationResult;
      }");
   }

   private void GenerateValidateFactoryArguments()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      static partial void ValidateFactoryArguments(ref global::System.ComponentModel.DataAnnotations.ValidationResult? validationResult");

      _sb.RenderArgumentsWithType(fieldsAndProperties, "ref ", leadingComma: true);

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

      if (_state.Type.IsReferenceType)
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

      private {_state.Type.Name}(");

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
         this.{memberInfo.Identifier} = {memberInfo.ArgumentName};");
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

      if (_state.Type.IsReferenceType)
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
            var (member, equalityComparer, _) = _state.EqualityMembers[i];

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
               if (member.Type.IsReferenceType)
               {
                  _sb.Append($@"(this.{member.Identifier} is null ? other.{member.Identifier} is null : this.{member.Identifier}.Equals(other.{member.Identifier}))");
               }
               else
               {
                  _sb.Append($@"this.{member.Identifier}.Equals(other.{member.Identifier})");
               }
            }
            else
            {
               _sb.Append($@"{equalityComparer}.Equals(this.{member.Identifier}, other.{member.Identifier})");
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
            var (member, equalityComparer, _) = _state.EqualityMembers[i];

            if (useShortForm)
            {
               if (i > 0)
               {
                  _sb.Append(@",
            ");
               }

               _sb.Append($@"this.{member.Identifier}");
            }
            else
            {
               _sb.Append($@"
         hashCode.Add(this.{member.Identifier}");

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
         return this.{keyMember.Identifier}{keyMember.NullableQuestionMark}.ToString();");
      }
      else if (_state.EqualityMembers.Count > 0)
      {
         _sb.Append(@"
         return $""{{");

         for (var i = 0; i < _state.EqualityMembers.Count; i++)
         {
            var (member, _, _) = _state.EqualityMembers[i];

            if (i > 0)
               _sb.Append(',');

            _sb.Append($@" {member.Identifier} = {{this.{member.Identifier}}}");
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
         return this.{keyMember.Identifier}.ToString(format, formatProvider);
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

      if (_state.Type.IsReferenceType)
      {
         _sb.Append(@"
         if(obj is null)
            return 1;
");
      }

      if (comparer is null)
      {
         if (keyMember.Type.IsReferenceType)
         {
            _sb.Append($@"
         if(this.{keyMember.Identifier} is null)
            return obj.{keyMember.Identifier} is null ? 0 : -1;
");
         }

         _sb.Append($@"
         return this.{keyMember.Identifier}.CompareTo(obj.{keyMember.Identifier});");
      }
      else
      {
         _sb.Append($@"
         return {comparer}.Compare(this.{keyMember.Identifier}, obj.{keyMember.Identifier});");
      }

      _sb.Append(@"
      }");
   }
}
