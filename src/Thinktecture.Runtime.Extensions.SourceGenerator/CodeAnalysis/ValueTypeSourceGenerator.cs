using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   public class ValueTypeSourceGenerator
   {
      private readonly ValueTypeSourceGeneratorState _state;
      private readonly StringBuilder _sb;

      public ValueTypeSourceGenerator(ValueTypeSourceGeneratorState state)
      {
         _state = state ?? throw new ArgumentNullException(nameof(state));
         _sb = new StringBuilder();
      }

      public string Generate()
      {
         _sb.Clear();
         _sb.Append($@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using Thinktecture;

{(String.IsNullOrWhiteSpace(_state.Namespace) ? null : $"namespace {_state.Namespace}")}
{{");
         if (_state.HasKeyMember)
            GenerateTypeConverter(_state.KeyMember);

         GenerateValueType();

         _sb.Append($@"
}}
");

         return _sb.ToString();
      }

      private void GenerateTypeConverter(EqualityInstanceMemberInfo keyMemberInfo)
      {
         var keyMember = keyMemberInfo.Member;
         var nullableKeyType = keyMember.Type.WithNullableAnnotation(NullableAnnotation.Annotated);

         _sb.Append($@"
   public class {_state.TypeIdentifier}_ValueTypeConverter : Thinktecture.ValueTypeConverter<{_state.TypeIdentifier}, {keyMember.Type}>
   {{
      /// <inheritdoc />");

         if (keyMember.Type.IsReferenceType)
         {
            _sb.Append($@"
      [return: NotNullIfNotNull(""{keyMember.ArgumentName}"")]");
         }

         _sb.Append($@"
      protected override {_state.TypeIdentifier}{(keyMember.Type.IsReferenceType ? _state.NullableQuestionMark : null)} ConvertFrom({nullableKeyType} {keyMember.ArgumentName})
      {{");

         if (keyMember.Type.IsReferenceType)
         {
            _sb.Append($@"
         if({keyMember.ArgumentName} is null)
            return default({_state.TypeIdentifier});
");
         }

         _sb.Append($@"
         return {_state.TypeIdentifier}.Create({keyMember.ArgumentName});
      }}

      /// <inheritdoc />
      protected override {keyMember.Type} GetKeyValue({_state.TypeIdentifier} obj)
      {{
         return ({keyMember.Type}) obj;
      }}
   }}
");
      }

      private void GenerateValueType()
      {
         var isFormattable = _state.HasKeyMember && _state.KeyMember.Member.Type.IsFormattable();
         var isComparable = _state.HasKeyMember && _state.KeyMember.Member.Type.IsComparable();

         if (_state.HasKeyMember)
         {
            _sb.Append($@"
   [System.ComponentModel.TypeConverter(typeof({_state.TypeIdentifier}_ValueTypeConverter))]");
         }

         _sb.Append($@"
   partial {(_state.Type.IsValueType ? "struct" : "class")} {_state.TypeIdentifier} : System.IEquatable<{_state.TypeIdentifier}{_state.NullableQuestionMark}>");

         if (isFormattable)
            _sb.Append(", System.IFormattable");

         if (isComparable)
            _sb.Append($", System.IComparable, System.IComparable<{_state.TypeIdentifier}>");

         _sb.Append($@"
   {{");

         if (_state.HasKeyMember)
         {
            var keyMember = _state.KeyMember.Member;

            _sb.Append($@"
      [System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {{
         var convertFromKey = new Func<{keyMember.Type}, {_state.TypeIdentifier}>({_state.TypeIdentifier}.Create);
         Expression<Func<{keyMember.Type}, {_state.TypeIdentifier}>> convertFromKeyExpression = {keyMember.ArgumentName} => {_state.TypeIdentifier}.Create({keyMember.ArgumentName});

         var convertToKey = new Func<{_state.TypeIdentifier}, {keyMember.Type}>(item => item.{keyMember.Identifier});
         Expression<Func<{_state.TypeIdentifier}, {keyMember.Type}>> convertToKeyExpression = obj => obj.{keyMember.Identifier};

         var tryCreate = new Thinktecture.Internal.Validate<{_state.TypeIdentifier}, {_state.KeyMember.Member.Type}>({_state.TypeIdentifier}.TryCreate);

         var type = typeof({_state.TypeIdentifier});
         var metadata = new ValueTypeMetadata(type, typeof({keyMember.Type}), false, convertFromKey, convertFromKeyExpression, convertToKey, convertToKeyExpression, tryCreate);

         ValueTypeMetadataLookup.AddMetadata(type, metadata);
      }}
");
         }

         _sb.Append($@"
      private static readonly Type _type = typeof({_state.TypeIdentifier});");

         if (_state.Type.IsValueType)
         {
            _sb.Append($@"

      public static readonly {_state.TypeIdentifier} Empty = default;");
         }

         if (!_state.SkipFactoryMethods)
         {
            GenerateCreateMethod();
            GenerateTryCreateMethod();
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

         _sb.Append($@"
   }}");
      }

      private void GenerateImplicitConversionToKey(EqualityInstanceMemberInfo keyMemberInfo)
      {
         var keyMember = keyMemberInfo.Member;
         var returnTypeNullableQuestionMark = _state.Type.IsReferenceType ? "?" : keyMember.NullableQuestionMark;

         _sb.Append($@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""{keyMember.Type}""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""{keyMember.Identifier}""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: NotNullIfNotNull(""obj"")]
      public static implicit operator {keyMember.Type}{returnTypeNullableQuestionMark}({_state.TypeIdentifier}{_state.NullableQuestionMark} obj)
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

         _sb.Append($@"
      }}");
      }

      private void GenerateExplicitConversionToKey(EqualityInstanceMemberInfo keyMemberInfo)
      {
         var keyMember = keyMemberInfo.Member;

         if (keyMember.Type.IsReferenceType || _state.Type.IsValueType)
            return;

         _sb.Append($@"

      /// <summary>
      /// Explicit conversion to the type <see cref=""{keyMember.Type}""/>.
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>The <see cref=""{keyMember.Identifier}""/> of provided <paramref name=""obj""/> or <c>default</c> if <paramref name=""obj""/> is <c>null</c>.</returns>
      [return: NotNullIfNotNull(""obj"")]
      public static explicit operator {keyMember.Type}({_state.TypeIdentifier} obj)
      {{
         if(obj is null)
            throw new NullReferenceException();

         return obj.{keyMember.Identifier};
      }}");
      }

      private void GenerateExplicitConversion(EqualityInstanceMemberInfo keyMemberInfo)
      {
         var keyMember = keyMemberInfo.Member;

         _sb.Append($@"

      /// <summary>
      /// Explicit conversion from the type <see cref=""{keyMember.Type}""/>.
      /// </summary>
      /// <param name=""{keyMember.ArgumentName}"">Value to covert.</param>
      /// <returns>An instance of <see cref=""{_state.TypeIdentifier}""/>.</returns>
      public static explicit operator {_state.TypeIdentifier}({keyMember.Type} {keyMember.ArgumentName})
      {{
         return {_state.TypeIdentifier}.Create({keyMember.ArgumentName});
      }}");
      }

      private void GenerateCreateMethod()
      {
         var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

         _sb.Append($@"

      public static {_state.TypeIdentifier} Create(");

         _sb.RenderArgumentsWithType(fieldsAndProperties);

         _sb.Append($@")
      {{
         var validationResult = ValidationResult.Success;
         ValidateFactoryArguments(ref validationResult");

         _sb.RenderArguments(fieldsAndProperties, "ref ", true);

         _sb.Append($@");

         if(validationResult != ValidationResult.Success)
            throw new ValidationException(validationResult!.ErrorMessage ?? ""Validation failed."");

         return ");

         GenerateConstructCall();

         _sb.Append($@";
      }}");
      }

      private void GenerateTryCreateMethod()
      {
         var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

         _sb.Append($@"

      public static ValidationResult? TryCreate(");

         _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

         _sb.Append($@"
         [MaybeNull] out {_state.TypeIdentifier}{_state.NullableQuestionMark} obj)
      {{
         var validationResult = ValidationResult.Success;
         ValidateFactoryArguments(ref validationResult");

         _sb.RenderArguments(fieldsAndProperties, "ref ", true);

         _sb.Append($@");

         obj = validationResult == ValidationResult.Success
               ? ");

         GenerateConstructCall();

         _sb.Append($@"
               : default;

         return validationResult;
      }}");
      }

      private void GenerateValidateFactoryArguments()
      {
         var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

         _sb.Append($@"

      static partial void ValidateFactoryArguments(ref ValidationResult? validationResult");

         _sb.RenderArgumentsWithType(fieldsAndProperties, "ref ", leadingComma: true);

         _sb.Append($@");");
      }

      private void GenerateConstructCall()
      {
         var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

         _sb.Append($@"new {_state.TypeIdentifier}(");
         _sb.RenderArguments(fieldsAndProperties);
         _sb.Append($@")");
      }

      private void GenerateEqualityOperators()
      {
         _sb.Append($@"

      /// <summary>
      /// Compares to instances of <see cref=""{_state.TypeIdentifier}""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==({_state.TypeIdentifier}{_state.NullableQuestionMark} obj, {_state.TypeIdentifier}{_state.NullableQuestionMark} other)
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
      /// Compares to instances of <see cref=""{_state.TypeIdentifier}""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=({_state.TypeIdentifier}{_state.NullableQuestionMark} obj, {_state.TypeIdentifier}{_state.NullableQuestionMark} other)
      {{
         return !(obj == other);
      }}");
      }

      private void GenerateConstructor()
      {
         var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

         if (fieldsAndProperties.Count == 0)
            return;

         _sb.Append($@"

      private {_state.TypeIdentifier}(");

         _sb.RenderArgumentsWithType(fieldsAndProperties);

         _sb.Append($@")
      {{");

         _sb.Append($@"
         ValidateConstructorArguments(");

         _sb.RenderArguments(fieldsAndProperties, "ref ");

         _sb.Append($@");
");

         foreach (var memberInfo in fieldsAndProperties)
         {
            _sb.Append($@"
         this.{memberInfo.Identifier} = {memberInfo.ArgumentName};");
         }

         _sb.Append($@"
      }}");

         _sb.Append($@"

      static partial void ValidateConstructorArguments(");

         _sb.RenderArgumentsWithType(fieldsAndProperties, "ref ");

         _sb.Append($@");");
      }

      private void GenerateEquals()
      {
         _sb.Append($@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {{
         return other is {_state.TypeIdentifier} obj && Equals(obj);
      }}

      /// <inheritdoc />
      public bool Equals({_state.TypeIdentifier}{_state.NullableQuestionMark} other)
      {{");

         if (_state.Type.IsReferenceType)
         {
            _sb.Append(@"
         if (other is null)
            return false;

         if (!ReferenceEquals(GetType(), other.GetType()))
            return false;

         if (ReferenceEquals(this, other))
            return true;
");
         }

         if (_state.EqualityMembers.Count > 0)
         {
            for (var i = 0; i < _state.EqualityMembers.Count; i++)
            {
               var (member, comparer) = _state.EqualityMembers[i];

               if (i == 0)
               {
                  _sb.Append($@"
         return ");
               }
               else
               {
                  _sb.Append($@"
             && ");
               }

               if (comparer == null)
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
                  _sb.Append($@"{comparer}.Equals(this.{member.Identifier}, other.{member.Identifier})");
               }
            }

            _sb.Append($@";");
         }
         else
         {
            _sb.Append($@"
         return true;");
         }

         _sb.Append($@"
      }}");
      }

      private void GenerateGetHashCode()
      {
         _sb.Append($@"

      /// <inheritdoc />
      public override int GetHashCode()
      {{");

         if (_state.EqualityMembers.Count > 0)
         {
            var useShortForm = _state.EqualityMembers.Count <= 8 && _state.EqualityMembers.All(m => m.EqualityComparer == null);

            if (useShortForm)
            {
               _sb.Append($@"
         return HashCode.Combine(");
            }
            else
            {
               _sb.Append($@"
         var hashCode = new HashCode();");
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

                  _sb.Append($@"this.{member.Identifier}");
               }
               else
               {
                  _sb.Append($@"
         hashCode.Add(this.{member.Identifier}");

                  if (equalityComparer is not null)
                     _sb.Append($@", {equalityComparer}");

                  _sb.Append($@");");
               }
            }

            if (useShortForm)
            {
               _sb.Append($@");");
            }
            else
            {
               _sb.Append($@"
         return hashCode.ToHashCode();");
            }
         }
         else
         {
            _sb.Append($@"
         return _type.GetHashCode();");
         }

         _sb.Append($@"
      }}");
      }

      private void GenerateToString()
      {
         _sb.Append($@"

      /// <inheritdoc />
      public override string? ToString()
      {{");

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
               var (member, _) = _state.EqualityMembers[i];

               if (i > 0)
                  _sb.Append(',');

               _sb.Append($@" {member.Identifier} = {{this.{member.Identifier}}}");
            }

            _sb.Append(" }}\";");
         }
         else
         {
            _sb.Append($@"
         return ""{_state.TypeIdentifier}"";");
         }

         _sb.Append($@"
      }}");
      }

      private void GenerateToStringFormat(EqualityInstanceMemberInfo keyMemberInfo)
      {
         var keyMember = keyMemberInfo.Member;

         _sb.Append($@"

      /// <inheritdoc />
      public string ToString(string? format, IFormatProvider? formatProvider = null)
      {{
         return this.{keyMember.Identifier}.ToString(format, formatProvider);
      }}");
      }

      private void GenerateCompareTo(EqualityInstanceMemberInfo keyMemberInfo)
      {
         var keyMember = keyMemberInfo.Member;

         _sb.Append($@"

      /// <inheritdoc />
      public int CompareTo(object? obj)
      {{
         if(obj is null)
            return 1;

         if(obj is not {_state.TypeIdentifier} valueType)
            throw new ArgumentException(""Argument must be of type '{_state.TypeIdentifier}'."", nameof(obj));

         return this.CompareTo(valueType);
      }}

      /// <inheritdoc />
      public int CompareTo({_state.TypeIdentifier}{_state.NullableQuestionMark} obj)
      {{");

         if (_state.Type.IsReferenceType)
         {
            _sb.Append($@"
         if(obj is null)
            return 1;
");
         }

         if (keyMember.Type.IsReferenceType)
         {
            _sb.Append($@"
         if(this.{keyMember.Identifier} is null)
            return obj.{keyMember.Identifier} is null ? 0 : -1;
");
         }

         _sb.Append($@"
         return this.{keyMember.Identifier}.CompareTo(obj.{keyMember.Identifier});
      }}");
      }
   }
}
