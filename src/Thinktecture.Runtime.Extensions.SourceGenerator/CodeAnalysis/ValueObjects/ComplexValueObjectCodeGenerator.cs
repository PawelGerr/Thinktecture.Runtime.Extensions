using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectCodeGenerator : SmartEnumAndValueObjectCodeGeneratorBase
{
   private const string _FACTORY_ARGUMENTS_VALIDATION_ERROR = "factoryArgumentsValidationError";
   private const string _FACTORY_POST_INIT = "FactoryPostInit";

   private readonly ComplexValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "ValueObject-CodeGenerator";
   public override string FileNameSuffix => ".ComplexValueObject";

   public ComplexValueObjectCodeGenerator(ComplexValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
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
      _sb.Append(@"
   [global::System.Diagnostics.CodeAnalysis.SuppressMessage(""ThinktectureRuntimeExtensionsAnalyzer"", ""TTRESG1000:Internal Thinktecture.Runtime.Extensions API usage"")]
   ").Append(_state.IsReferenceType ? "sealed " : "readonly ").Append("partial ").AppendTypeKind(_state).Append(" ").Append(_state.Name).Append(" : global::System.IEquatable<").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@">,
      global::System.Numerics.IEqualityOperators<").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(_state).Append(@", bool>,
      global::Thinktecture.Internal.IMetadataOwner");

      if (_state.DisallowsDefaultValue)
      {
         _sb.Append(@",
      global::Thinktecture.IDisallowDefaultValue");
      }

      _sb.Append(@"
   {
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
         };

      private static readonly int _typeHashCode = typeof(").AppendTypeFullyQualified(_state).Append(").GetHashCode();");

      if (_state is { IsReferenceType: false, Settings.AllowDefaultStructs: true })
      {
         _sb.Append(@"

      /// <summary>
      /// Default instance of ").AppendTypeForXmlComment(_state).Append(@".
      /// </summary>
      public static readonly ").AppendTypeFullyQualified(_state).Append(" ").Append(_state.Settings.DefaultInstancePropertyName).Append(" = default;");
      }

      cancellationToken.ThrowIfCancellationRequested();

      if (!_state.Settings.SkipFactoryMethods)
      {
         GenerateValidateMethod();
         GenerateCreateMethod();
         GenerateTryCreateMethod();
         GenerateValidateFactoryArguments();
         GenerateFactoryPostInit();
      }

      cancellationToken.ThrowIfCancellationRequested();

      GenerateConstructor();
      GenerateEqualityOperators();
      GenerateEquals();
      GenerateGetHashCode();

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

      for (var i = 0; i < fieldsAndProperties.Count; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         _sb.Append(@"
      /// <param name=""").Append(memberInfo.ArgumentName).Append(@""">The value to be used for object creation.</param>");
      }

      _sb.Append(@"
      /// <returns>A newly created ").AppendTypeForXmlComment(_state).Append(@" instance.</returns>
      /// <exception cref=""System.ComponentModel.DataAnnotations.ValidationException"">Thrown when the provided values fail validation.</exception>
      public static ").AppendTypeFullyQualified(_state).Append(" ").Append(_state.Settings.CreateFactoryMethodName).Append("(").RenderArgumentsWithType(fieldsAndProperties, prefix: @"
         ", comma: ",").Append(@")
      {
         var validationError = Validate(");

      _sb.RenderArguments(fieldsAndProperties, prefix: @"
            ", comma: ",");

      if (fieldsAndProperties.Count > 0)
         _sb.Append(",");

      _sb.Append(@"
            out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" obj);

         if (validationError is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? ""Validation failed."");

         return obj!;
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

      for (var i = 0; i < fieldsAndProperties.Count; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         _sb.Append(@"
      /// <param name=""").Append(memberInfo.ArgumentName).Append(@""">The value to be used for object creation.</param>");
      }

      _sb.Append(@"
      /// <param name=""obj"">
      /// When this method returns, contains the created instance of type ").AppendTypeForXmlComment(_state).Append(@"
      /// if the operation is successful; otherwise, <c>null</c>.
      /// </param>
      /// <returns>
      /// <c>true</c> if the instance is successfully created; otherwise, <c>false</c>.
      /// </returns>
      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

      _sb.Append(@"
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" obj)
      {
         return ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(");

      _sb.RenderArguments(fieldsAndProperties, prefix: @"
            ", comma: ",");

      if (fieldsAndProperties.Count > 0)
         _sb.Append(",");

      _sb.Append(@"
            out obj,
            out _);
      }");

      _sb.Append(@"

      /// <summary>
      /// Attempts to create an instance of type ").AppendTypeForXmlComment(_state).Append(@"
      /// if the provided values pass validation.
      /// </summary>");

      for (var i = 0; i < fieldsAndProperties.Count; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         _sb.Append(@"
      /// <param name=""").Append(memberInfo.ArgumentName).Append(@""">The value to be used for object creation.</param>");
      }

      _sb.Append(@"
      /// <param name=""obj"">
      /// When this method returns, contains the created instance of type ").AppendTypeForXmlComment(_state).Append(@"
      /// if the operation is successful; otherwise, <c>null</c>.
      /// </param>
      /// <param name=""validationError"">
      /// When this method returns, contains the ").AppendTypeForXmlComment(_state.ValidationError).Append(@"
      /// describing why validation failed, if the operation fails; otherwise, <c>null</c>.
      /// </param>
      /// <returns>
      /// <c>true</c> if the instance is successfully created; otherwise, <c>false</c>.
      /// </returns>
      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

      _sb.Append(@"
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" obj,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out ").AppendTypeFullyQualified(_state.ValidationError).Append(@"? validationError)
      {
         validationError = Validate(");

      _sb.RenderArguments(fieldsAndProperties, prefix: @"
            ", comma: ",");

      if (fieldsAndProperties.Count > 0)
         _sb.Append(",");

      _sb.Append(@"
            out obj);

         return validationError is null;
      }");
   }

   private void GenerateValidateMethod()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      /// <summary>
      /// Validates the values and creates an instance of type ").AppendTypeForXmlComment(_state).Append(@" if validation is successful.
      /// </summary>");

      for (var i = 0; i < fieldsAndProperties.Count; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         _sb.Append(@"
      /// <param name=""").Append(memberInfo.ArgumentName).Append(@""">The value to be used for object creation.</param>");
      }

      _sb.Append(@"
      /// <param name=""obj"">The created object if validation is successful, otherwise <c>null</c>.</param>
      /// <returns>A validation error if validation fails; otherwise, <c>null</c>.</returns>
      public static ").AppendTypeFullyQualified(_state.ValidationError).Append("? Validate(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

      _sb.Append(@"
         out ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" obj)
      {");

      for (var i = 0; i < fieldsAndProperties.Count; i++)
      {
         var memberInfo = fieldsAndProperties[i];

         if (memberInfo.IsReferenceType && memberInfo.NullableAnnotation == NullableAnnotation.NotAnnotated)
         {
            _sb.Append(@"
         if (").AppendEscaped(memberInfo.ArgumentName).Append(@" is null)
         {
            obj = default;
            return ").AppendTypeFullyQualified(_state.ValidationError).Append(@".Create(""The member \""").Append(memberInfo.Name).Append(@"\"" of type \""").AppendTypeMinimallyQualified(_state).Append(@"\"" must not be null."");
         }
");
         }
      }

      _sb.Append(@"
         ").AppendTypeFullyQualified(_state.ValidationError).Append(@"? validationError = null;

         ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("var ").Append(_FACTORY_ARGUMENTS_VALIDATION_ERROR).Append(" = ");

      _sb.Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append(@"(
            ref validationError").RenderArguments(fieldsAndProperties, @"
            ref ", comma: ",", leadingComma: true).Append(@");

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

      _sb.Append("static partial ").Append(_state.FactoryValidationReturnType ?? "void").Append(" ").Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append(@"(
         ref ").AppendTypeFullyQualified(_state.ValidationError).Append("? validationError");

      _sb.RenderArgumentsWithType(fieldsAndProperties, "ref ", comma: @",
         ", leadingComma: true);

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
      public static bool operator !=(").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" obj, ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" other)
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

      /// <summary>
      /// Initializes a new instance of the ").AppendTypeForXmlComment(_state).Append(@" type.
      /// </summary>
      ").RenderAccessModifier(_state.Settings.ConstructorAccessModifier).Append(" ").Append(_state.Name).Append("(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, prefix: @"
         ", comma: ",");

      _sb.Append(@")
      {");

      if (fieldsAndProperties.Count > 0)
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
         this.").Append(memberInfo.Name).Append(" = ").AppendEscaped(memberInfo.ArgumentName).Append(";");
         }
      }

      _sb.Append(@"
      }");

      if (fieldsAndProperties.Count > 0)
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

            if (equalityComparerAccessor != null)
            {
               _sb.Append(equalityComparerAccessor).Append(".EqualityComparer.Equals(this.").Append(member.Name).Append(", other.").Append(member.Name).Append(")");
            }
            else if (member.IsString())
            {
               _sb.Append("global::System.StringComparer.").Append(GetDefaultStringComparer()).Append(".Equals(this.").Append(member.Name).Append(", other.").Append(member.Name).Append(")");
            }
            else
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
      public override int GetHashCode()
      {");

      if (_state.EqualityMembers.Count > 0)
      {
         var useShortForm = _state.EqualityMembers.Count < 8 && _state.EqualityMembers.All(m => m.EqualityComparerAccessor == null && !m.Member.IsString());

         if (useShortForm)
         {
            _sb.Append(@"
         return global::System.HashCode.Combine(
            _typeHashCode");
         }
         else
         {
            _sb.Append(@"
         var hashCode = new global::System.HashCode();
         hashCode.Add(_typeHashCode);");
         }

         for (var i = 0; i < _state.EqualityMembers.Count; i++)
         {
            var (member, equalityComparerAccessor) = _state.EqualityMembers[i];

            if (useShortForm)
            {
               _sb.Append(@",
            this.").Append(member.Name);
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
         return _typeHashCode;");
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

      if (_state.EqualityMembers.Count > 0)
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
         return """).AppendTypeMinimallyQualified(_state).Append(@""";");
      }

      _sb.Append(@"
      }");
   }
}

file static class Extensions
{
   public static StringBuilder AppendAssignableMembersBody(this StringBuilder sb, IReadOnlyList<InstanceMemberInfo> members)
   {
      for (var i = 0; i < members.Count; i++)
      {
         var member = members[i];

         if (i > 0)
            sb.Append(",");

         sb.Append(@"
                                 o.").Append(member.Name);
      }

      return sb;
   }
}
