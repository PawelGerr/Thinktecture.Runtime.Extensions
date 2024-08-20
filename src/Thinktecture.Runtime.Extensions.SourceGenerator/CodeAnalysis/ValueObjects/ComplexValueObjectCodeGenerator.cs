using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectCodeGenerator : SmartEnumAndValueObjectCodeGeneratorBase
{
   private const string _FACTORY_ARGUMENTS_VALIDATION_ERROR = "factoryArgumentsValidationError";
   private const string _FACTORY_POST_INIT = "FactoryPostInit";

   private readonly ComplexValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "ValueObject-CodeGenerator";
   public override string? FileNameSuffix => null;

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
      _sb.Append(@"
   ").Append(_state.IsReferenceType ? "sealed " : "readonly ").Append("partial ").Append(_state.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Name).Append(" : global::System.IEquatable<").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@">,
      global::System.Numerics.IEqualityOperators<").Append(_state.TypeFullyQualified).Append(", ").Append(_state.TypeFullyQualified).Append(@", bool>,
      global::Thinktecture.IComplexValueObject");

      foreach (var desiredFactory in _state.Settings.DesiredFactories)
      {
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

      GenerateModuleInitializerForComplexValueObject();

      _sb.Append(@"
      private static readonly int _typeHashCode = typeof(").Append(_state.TypeFullyQualified).Append(").GetHashCode();");

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"

      public static readonly ").Append(_state.TypeFullyQualified).Append(" ").Append(_state.Settings.DefaultInstancePropertyName).Append(" = default;");
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

   private void GenerateCreateMethod()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      public static ").Append(_state.TypeFullyQualified).Append(" ").Append(_state.Settings.CreateFactoryMethodName).Append("(").RenderArgumentsWithType(fieldsAndProperties).Append(@")
      {
         var validationError = Validate(");

      _sb.RenderArguments(fieldsAndProperties);

      if (fieldsAndProperties.Count > 0)
         _sb.Append(", ");

      _sb.Append("out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj);

         if (validationError is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? ""Validation failed."");

         return obj!;
      }");
   }

   private void GenerateTryCreateMethod()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

      _sb.Append(@"
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj)
      {
         return ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(");

      _sb.RenderArguments(fieldsAndProperties);

      if (fieldsAndProperties.Count > 0)
         _sb.Append(", ");

      _sb.Append(@"out obj, out _);
      }");

      _sb.Append(@"

      public static bool ").Append(_state.Settings.TryCreateFactoryMethodName).Append("(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

      _sb.Append(@"
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out ").Append(_state.ValidationError.TypeFullyQualified).Append(@"? validationError)
      {
         validationError = Validate(");

      _sb.RenderArguments(fieldsAndProperties);

      if (fieldsAndProperties.Count > 0)
         _sb.Append(", ");

      _sb.Append(@"out obj);

         return validationError is null;
      }");
   }

   private void GenerateValidateMethod()
   {
      var fieldsAndProperties = _state.AssignableInstanceFieldsAndProperties;

      _sb.Append(@"

      public static ").Append(_state.ValidationError.TypeFullyQualified).Append("? Validate(");

      _sb.RenderArgumentsWithType(fieldsAndProperties, @"
         ", ",", trailingComma: true);

      _sb.Append(@"
         out ").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" obj)
      {
         ").Append(_state.ValidationError.TypeFullyQualified).Append(@"? validationError = null;
         ");

      if (_state.FactoryValidationReturnType is not null)
         _sb.Append("var ").Append(_FACTORY_ARGUMENTS_VALIDATION_ERROR).Append(" = ");

      _sb.Append(Constants.Methods.VALIDATE_FACTORY_ARGUMENTS).Append("(ref validationError").RenderArguments(fieldsAndProperties, "ref ", true).Append(@");

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

      ").RenderAccessModifier(_state.Settings.ConstructorAccessModifier).Append(" ").Append(_state.Name).Append("(");

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

            if (equalityComparerAccessor != null)
            {
               _sb.Append(equalityComparerAccessor).Append(".EqualityComparer.Equals(this.").Append(member.Name).Append(", other.").Append(member.Name).Append(")");
            }
            else if (member.IsString())
            {
               _sb.Append("global::System.StringComparer.OrdinalIgnoreCase.Equals(this.").Append(member.Name).Append(", other.").Append(member.Name).Append(")");
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

   private void GenerateGetHashCode()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override int GetHashCode()
      {");

      if (_state.EqualityMembers.Count > 0)
      {
         var useShortForm = _state.EqualityMembers.Count < 8 && _state.EqualityMembers.All(m => m.EqualityComparerAccessor == null);

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
                  _sb.Append(", global::System.StringComparer.OrdinalIgnoreCase");

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
         return """).Append(_state.TypeMinimallyQualified).Append(@""";");
      }

      _sb.Append(@"
      }");
   }
}
