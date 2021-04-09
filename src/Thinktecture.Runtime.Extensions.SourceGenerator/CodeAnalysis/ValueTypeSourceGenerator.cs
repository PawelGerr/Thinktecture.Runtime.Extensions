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
   public class {_state.Type.Name}_ValueTypeConverter : Thinktecture.ValueTypeConverter<{_state.Type.Name}, {keyMember.Type}>
   {{
      /// <inheritdoc />");

         if (keyMember.Type.IsReferenceType)
         {
            _sb.Append($@"
      [return: NotNullIfNotNull(""{keyMember.ArgumentName}"")]");
         }

         _sb.Append($@"
      protected override {_state.Type.Name}{(keyMember.Type.IsReferenceType ? _state.NullableQuestionMark : null)} ConvertFrom({nullableKeyType} {keyMember.ArgumentName})
      {{");

         if (keyMember.Type.IsReferenceType)
         {
            _sb.Append($@"
         if({keyMember.ArgumentName} is null)
            return default({_state.Type.Name});
");
         }

         _sb.Append($@"
         return {_state.Type.Name}.Create({keyMember.ArgumentName});
      }}

      /// <inheritdoc />
      protected override {keyMember.Type} GetKeyValue({_state.Type.Name} obj)
      {{
         return ({keyMember.Type}) obj;
      }}
   }}
");
      }

      private void GenerateValueType()
      {
         var isFormattable = _state.HasKeyMember && _state.KeyMember.Member.Type.IsFormattable();
         var isComparable = !_state.SkipCompareTo && _state.HasKeyMember && _state.KeyMember.Member.Type.IsComparable();

         _sb.Append(@"
   [Thinktecture.Internal.ValueTypeConstructor(");

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
   [Thinktecture.Internal.KeyedValueType]
   [System.ComponentModel.TypeConverter(typeof({_state.Type.Name}_ValueTypeConverter))]");
         }

         _sb.Append($@"
   partial {(_state.Type.IsValueType ? "struct" : "class")} {_state.Type.Name} : System.IEquatable<{_state.Type.Name}{_state.NullableQuestionMark}>");

         if (isFormattable)
            _sb.Append(", System.IFormattable");

         if (isComparable)
            _sb.Append($", System.IComparable, System.IComparable<{_state.Type.Name}>");

         _sb.Append($@"
   {{");

         if (_state.HasKeyMember)
         {
            var keyMember = _state.KeyMember.Member;

            _sb.Append($@"
      [System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {{
         var convertFromKey = new Func<{keyMember.Type}, {_state.Type.Name}>({_state.Type.Name}.Create);
         Expression<Func<{keyMember.Type}, {_state.Type.Name}>> convertFromKeyExpression = {keyMember.ArgumentName} => new {_state.Type.Name}({keyMember.ArgumentName});

         var convertToKey = new Func<{_state.Type.Name}, {keyMember.Type}>(item => item.{keyMember.Identifier});
         Expression<Func<{_state.Type.Name}, {keyMember.Type}>> convertToKeyExpression = obj => obj.{keyMember.Identifier};

         var tryCreate = new Thinktecture.Internal.Validate<{_state.Type.Name}, {_state.KeyMember.Member.Type}>({_state.Type.Name}.TryCreate);

         var type = typeof({_state.Type.Name});
         var metadata = new Thinktecture.Internal.ValueTypeMetadata(type, typeof({keyMember.Type}), false, convertFromKey, convertFromKeyExpression, convertToKey, convertToKeyExpression, tryCreate);

         Thinktecture.Internal.ValueTypeMetadataLookup.AddMetadata(type, metadata);
      }}
");
         }

         _sb.Append($@"
      private static readonly Type _type = typeof({_state.Type.Name});");

         if (_state.Type.IsValueType)
         {
            _sb.Append($@"

      public static readonly {_state.Type.Name} Empty = default;");
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
      public static implicit operator {keyMember.Type}{returnTypeNullableQuestionMark}({_state.Type.Name}{_state.NullableQuestionMark} obj)
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
      public static explicit operator {keyMember.Type}({_state.Type.Name} obj)
      {{
         if(obj is null)
            throw new NullReferenceException();

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
      /// Explicit conversion from the type <see cref=""{keyMember.Type}""/>.
      /// </summary>
      /// <param name=""{keyMember.ArgumentName}"">Value to covert.</param>
      /// <returns>An instance of <see cref=""{_state.Type.Name}""/>.</returns>");

         if (bothAreReferenceTypes)
         {
            _sb.Append($@"
      [return: NotNullIfNotNull(""{keyMember.ArgumentName}"")]");
         }

         _sb.Append($@"
      public static explicit operator {_state.Type.Name}{nullableQuestionMark}({keyMember.Type}{nullableQuestionMark} {keyMember.ArgumentName})
      {{");

         if (bothAreReferenceTypes)
         {
            _sb.Append($@"
         if({keyMember.ArgumentName} is null)
            return null;
");
         }

         _sb.Append($@"
         return {_state.Type.Name}.Create({keyMember.ArgumentName});
      }}");
      }

      private void GenerateCreateMethod(bool allowNullKeyMember)
      {
         var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

         _sb.Append($@"
");

         if (allowNullKeyMember)
         {
            _sb.Append($@"
      [return: NotNullIfNotNull(""{_state.KeyMember!.Member.ArgumentName}"")]");
         }

         _sb.Append($@"
      public static {_state.Type.Name}{(allowNullKeyMember ? "?" : null)} Create(");

         _sb.RenderArgumentsWithType(fieldsAndProperties, useNullableTypes: allowNullKeyMember);

         _sb.Append($@")
      {{");

         if (allowNullKeyMember)
         {
            _sb.Append($@"
         if({_state.KeyMember!.Member.ArgumentName} is null)
            return default;
");
         }

         _sb.Append($@"
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

      private void GenerateTryCreateMethod(bool allowNullKeyMember)
      {
         var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

         _sb.Append($@"

      public static ValidationResult? TryCreate(");

         _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true, useNullableTypes: allowNullKeyMember);

         _sb.Append($@"
         [MaybeNull] out {_state.Type.Name}{_state.NullableQuestionMark} obj)
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

         _sb.Append($@"
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

         _sb.Append($@"new {_state.Type.Name}(");
         _sb.RenderArguments(fieldsAndProperties);
         _sb.Append($@")");
      }

      private void GenerateEqualityOperators()
      {
         _sb.Append($@"

      /// <summary>
      /// Compares to instances of <see cref=""{_state.Type.Name}""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==({_state.Type.Name}{_state.NullableQuestionMark} obj, {_state.Type.Name}{_state.NullableQuestionMark} other)
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
      /// Compares to instances of <see cref=""{_state.Type.Name}""/>.
      /// </summary>
      /// <param name=""obj"">Instance to compare.</param>
      /// <param name=""other"">Another instance to compare.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=({_state.Type.Name}{_state.NullableQuestionMark} obj, {_state.Type.Name}{_state.NullableQuestionMark} other)
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

      private {_state.Type.Name}(");

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
         return other is {_state.Type.Name} obj && Equals(obj);
      }}

      /// <inheritdoc />
      public bool Equals({_state.Type.Name}{_state.NullableQuestionMark} other)
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
               var (member, equalityComparer, _) = _state.EqualityMembers[i];

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
         return ""{_state.Type.Name}"";");
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
         var comparer = keyMemberInfo.Comparer;

         _sb.Append($@"

      /// <inheritdoc />
      public int CompareTo(object? obj)
      {{
         if(obj is null)
            return 1;

         if(obj is not {_state.Type.Name} valueType)
            throw new ArgumentException(""Argument must be of type '{_state.Type.Name}'."", nameof(obj));

         return this.CompareTo(valueType);
      }}

      /// <inheritdoc />
      public int CompareTo({_state.Type.Name}{_state.NullableQuestionMark} obj)
      {{");

         if (_state.Type.IsReferenceType)
         {
            _sb.Append($@"
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

         _sb.Append($@"
      }}");
      }
   }
}
