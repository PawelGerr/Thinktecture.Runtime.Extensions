using System.Text;

namespace Thinktecture.CodeAnalysis.AdHocUnions;

public class AdHocUnionCodeGenerator : CodeGeneratorBase
{
   public override string CodeGeneratorName => "AdHocUnion-CodeGenerator";
   public override string? FileNameSuffix => null;

   private readonly AdHocUnionSourceGenState _state;
   private readonly StringBuilder _sb;

   public AdHocUnionCodeGenerator(
      AdHocUnionSourceGenState state,
      StringBuilder sb)
   {
      _state = state;
      _sb = sb;
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

      GenerateUnion(cancellationToken);

      _sb.RenderContainingTypesEnd(_state.ContainingTypes);

      if (hasNamespace)
      {
         _sb.Append(@"
}");
      }

      _sb.Append(@"
");
   }

   private void GenerateUnion(CancellationToken cancellationToken)
   {
      _sb.GenerateStructLayoutAttributeIfRequired(_state.IsReferenceType, _state.Settings.HasStructLayoutAttribute);

      _sb.Append(@"
   ");

      _sb.Append(_state.IsReferenceType ? "sealed " : "readonly ").Append("partial ").Append(_state.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Name);

      if (!_state.IsRefStruct)
      {
         _sb.Append(@" :
      global::System.IEquatable<").AppendTypeFullyQualified(_state).Append(@">,
      global::System.Numerics.IEqualityOperators<").AppendTypeFullyQualified(_state).Append(@", ").AppendTypeFullyQualified(_state).Append(", bool>");
      }

      _sb.Append(@"
   {
      private static readonly int _typeHashCode = typeof(").AppendTypeFullyQualified(_state).Append(@").GetHashCode();

      private readonly int _valueIndex;
"); // index is 1-based

      GenerateMemberTypeFieldsAndProps();
      GenerateRawValueGetter();
      GenerateConstructors();
      GenerateFactoriesForTypeDuplicates();

      cancellationToken.ThrowIfCancellationRequested();

      if (_state.Settings.SwitchMethods != SwitchMapMethodsGeneration.None)
      {
         GenerateSwitchForAction(false, false);

         if (_state.Settings.SwitchMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateSwitchForAction(false, true);

         GenerateSwitchForAction(true, false);

         if (_state.Settings.SwitchMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateSwitchForAction(true, true);

         GenerateSwitchForFunc(false, false);

         if (_state.Settings.SwitchMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateSwitchForFunc(false, true);

         GenerateSwitchForFunc(true, false);

         if (_state.Settings.SwitchMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateSwitchForFunc(true, true);
      }

      if (_state.Settings.MapMethods != SwitchMapMethodsGeneration.None)
      {
         GenerateMap(false);

         if (_state.Settings.MapMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateMap(true);
      }

      if (!_state.Settings.SkipImplicitConversionFromValue)
         GenerateImplicitConversions();

      GenerateExplicitConversions();
      GenerateEqualityOperators();
      GenerateEquals();
      GenerateGetHashCode();

      if (!_state.Settings.SkipToString)
         GenerateToString();

      _sb.Append(@"
   }");
   }

   private void GenerateImplicitConversions()
   {
      foreach (var memberType in _state.MemberTypes)
      {
         if (memberType.IsInterface || memberType.TypeDuplicateIndex is not null)
            continue;

         _sb.Append(@"

      /// <summary>
      /// Implicit conversion from type ").AppendTypeForXmlComment(memberType).Append(@".
      /// </summary>
      /// <param name=""").Append(memberType.ArgumentName).Append(@""">Value to covert from.</param>
      /// <returns>A new instance of ").AppendTypeForXmlComment(_state).Append(@" converted from <paramref name=""").Append(memberType.ArgumentName).Append(@"""/>.</returns>
      public static implicit operator ").AppendTypeFullyQualified(_state).Append("(").AppendTypeFullyQualified(memberType).Append(" ").AppendEscaped(memberType.ArgumentName).Append(@")
      {
         return new ").AppendTypeFullyQualified(_state).Append("(").AppendEscaped(memberType.ArgumentName).Append(@");
      }");
      }
   }

   private void GenerateExplicitConversions()
   {
      foreach (var memberType in _state.MemberTypes)
      {
         if (memberType.IsInterface || memberType.TypeDuplicateIndex is not null)
            continue;

         _sb.Append(@"

      /// <summary>
      /// Implicit conversion to type ").AppendTypeForXmlComment(memberType).Append(@".
      /// </summary>
      /// <param name=""obj"">Object to covert.</param>
      /// <returns>Inner value of type ").AppendTypeForXmlComment(memberType).Append(@".</returns>
      /// <exception cref=""System.InvalidOperationException"">If the inner value is not a ").AppendTypeForXmlComment(memberType).Append(@".</exception>
      public static explicit operator ").AppendTypeFullyQualified(memberType).Append("(").AppendTypeFullyQualified(_state).Append(@" obj)
      {
         return obj.As").Append(memberType.Name).Append(@";
      }");
      }
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

   private void GenerateToString()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override string? ToString()
      {
         return this._valueIndex switch
         {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
            0 => throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values.""),");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
            ").Append(i + 1).Append(" => this._").Append(memberType.BackingFieldName);

         if (memberType.SpecialType != SpecialType.System_String)
         {
            if (memberType.IsReferenceType)
               _sb.Append("?");

            _sb.Append(".ToString()");
         }

         _sb.Append(",");
      }

      _sb.Append(@"
            _ => throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."")
         };
      }");
   }

   private void GenerateGetHashCode()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override int GetHashCode()
      {
         return this._valueIndex switch
         {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
            0 => throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values.""),");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
            ").Append(i + 1).Append(" => ");

         _sb.Append("global::System.HashCode.Combine(").AppendTypeFullyQualified(_state).Append("._typeHashCode, this._").Append(memberType.BackingFieldName);

         if (memberType.IsReferenceType)
            _sb.Append("?");

         _sb.Append(".GetHashCode(");

         if (memberType.SpecialType == SpecialType.System_String)
            _sb.Append("global::System.StringComparison.").Append(Enum.GetName(typeof(StringComparison), _state.Settings.DefaultStringComparison));

         _sb.Append(")");

         if (memberType.IsReferenceType)
            _sb.Append(" ?? 0");

         _sb.Append("),");
      }

      _sb.Append(@"
            _ => throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."")
         };
      }");
   }

   private void GenerateEquals()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {");

      if (_state.IsRefStruct)
      {
         _sb.Append(@"
         return false;");
      }
      else
      {
         _sb.Append(@"
         return other is ").AppendTypeFullyQualified(_state).Append(" obj && Equals(obj);");
      }

      _sb.Append(@"
      }

      /// <inheritdoc />
      public bool Equals(").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" other)
      {");

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
         if (other is null)
            return false;

         if (ReferenceEquals(this, other))
            return true;
");
      }

      _sb.Append(@"
         if (this._valueIndex != other._valueIndex)
            return false;

         return this._valueIndex switch
         {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
            0 => throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values.""),");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
            ").Append(i + 1).Append(" => ");

         if (memberType.IsReferenceType)
            _sb.Append("this._").Append(memberType.BackingFieldName).Append(" is null ? other._").Append(memberType.BackingFieldName).Append(" is null : ");

         _sb.Append("this._").Append(memberType.BackingFieldName).Append(".Equals(other._").Append(memberType.BackingFieldName);

         if (memberType.SpecialType == SpecialType.System_String)
            _sb.Append(", global::System.StringComparison.").Append(Enum.GetName(typeof(StringComparison), _state.Settings.DefaultStringComparison));

         _sb.Append("),");
      }

      _sb.Append(@"
            _ => throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."")
         };
      }");
   }

   private void GenerateSwitchForAction(bool withState, bool isPartially)
   {
      _sb.Append(@"

      /// <summary>
      /// Executes an action depending on the current value.
      /// </summary>");

      if (withState)
      {
         _sb.Append(@"
      /// <param name=""state"">State to be passed to the callbacks.</param>");
      }

      if (isPartially)
      {
         _sb.Append(@"
      /// <param name=""default"">The action to execute if no value-specific action is provided.</param>");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
      /// <param name=""").Append(memberType.ArgumentName).Append(@""">The action to execute if the current value is of type ").AppendTypeForXmlComment(memberType).Append(".</param>");
      }

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"
      /// <exception cref=""System.InvalidOperationException"">If the union (struct) is not initialized or initialized with default value.</exception>");
      }

      var methodName = isPartially ? "SwitchPartially" : "Switch";

      if (withState)
      {
         _sb.Append(@"
      public void ").Append(methodName).Append(@"<TState>(
         TState state,");
      }
      else
      {
         _sb.Append(@"
      public void ").Append(methodName).Append("(");
      }

      if (isPartially)
      {
         _sb.Append(@"
         global::System.Action<");

         if (withState)
            _sb.Append("TState, ");

         _sb.Append("object?>? @default = null,");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         global::System.Action<");

         if (withState)
            _sb.Append("TState, ");

         _sb.AppendTypeFullyQualified(memberType).Append(">");

         if (isPartially)
            _sb.Append('?');

         _sb.Append(' ').AppendEscaped(memberType.ArgumentName);

         if (isPartially)
            _sb.Append(" = null");
      }

      _sb.Append(")");

      if (withState)
      {
         _sb.Append(@"
#if NET9_0_OR_GREATER
		where TState : allows ref struct
#endif");
      }

      _sb.Append(@"
      {");

      GenerateIndexBasedActionSwitchBody(withState, isPartially);

      _sb.Append(@"
      }");
   }

   private void GenerateIndexBasedActionSwitchBody(bool withState, bool isPartially)
   {
      _sb.Append(@"
         switch (this._valueIndex)
         {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
            case 0:
               throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values."");");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
            case ").Append(i + 1).Append(":");

         if (isPartially)
         {
            _sb.Append(@"
               if (").AppendEscaped(memberType.ArgumentName).Append(@" is null)
                  break;
");
         }

         _sb.Append(@"
               ").AppendEscaped(memberType.ArgumentName).Append("(");

         if (withState)
            _sb.Append("state, ");

         _sb.Append("this._").Append(memberType.BackingFieldName).Append(memberType.IsReferenceType && memberType.NullableAnnotation != NullableAnnotation.Annotated ? "!" : null).Append(@");
               return;");
      }

      _sb.Append(@"
            default:
               throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         @default?.Invoke(");

         if (withState)
            _sb.Append("state, ");

         _sb.Append("this.Value);");
      }
   }

   private void GenerateSwitchForFunc(bool withState, bool isPartially)
   {
      _sb.Append(@"

      /// <summary>
      /// Executes a function depending on the current value.
      /// </summary>");

      if (withState)
      {
         _sb.Append(@"
      /// <param name=""state"">State to be passed to the callbacks.</param>");
      }

      if (isPartially)
      {
         _sb.Append(@"
      /// <param name=""default"">The function to execute if no value-specific action is provided.</param>");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
      /// <param name=""").Append(memberType.ArgumentName).Append(@""">The function to execute if the current value is of type ").AppendTypeForXmlComment(memberType).Append(".</param>");
      }

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"
      /// <exception cref=""System.InvalidOperationException"">If the union (struct) is not initialized or initialized with default value.</exception>");
      }

      var methodName = isPartially ? "SwitchPartially" : "Switch";

      if (withState)
      {
         _sb.Append(@"
      public TResult ").Append(methodName).Append(@"<TState, TResult>(
         TState state,");
      }
      else
      {
         _sb.Append(@"
      public TResult ").Append(methodName).Append("<TResult>(");
      }

      if (isPartially)
      {
         _sb.Append(@"
         global::System.Func<");

         if (withState)
            _sb.Append("TState, ");

         _sb.Append("object?, TResult> @default,");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         ");

         _sb.Append("global::System.Func<");

         if (withState)
            _sb.Append("TState, ");

         _sb.AppendTypeFullyQualified(memberType).Append(", TResult>");

         if (isPartially)
            _sb.Append('?');

         _sb.Append(' ').AppendEscaped(memberType.ArgumentName);

         if (isPartially)
            _sb.Append(" = null");
      }

      _sb.Append(@")
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct");

      if (withState)
      {
         _sb.Append(@"
		   where TState : allows ref struct");
      }

      _sb.Append(@"
#endif
      {");

      GenerateIndexBasedFuncSwitchBody(withState, isPartially);

      _sb.Append(@"
      }");
   }

   private void GenerateIndexBasedFuncSwitchBody(bool withState, bool isPartially)
   {
      _sb.Append(@"
         switch (this._valueIndex)
         {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
            case 0:
               throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values."");");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
            case ").Append(i + 1).Append(":");

         if (isPartially)
         {
            _sb.Append(@"
               if (").AppendEscaped(memberType.ArgumentName).Append(@" is null)
                  break;
");
         }

         _sb.Append(@"
               return ").AppendEscaped(memberType.ArgumentName).Append("(");

         if (withState)
            _sb.Append("state, ");

         _sb.Append("this._").Append(memberType.BackingFieldName).Append(memberType is { IsReferenceType: true, Setting.IsNullableReferenceType: false } ? "!" : null).Append(");");
      }

      _sb.Append(@"
            default:
               throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         return @default(");

         if (withState)
            _sb.Append("state, ");

         _sb.Append("this.Value);");
      }
   }

   private void GenerateMap(bool isPartially)
   {
      _sb.Append(@"

      /// <summary>
      /// Maps current value to an instance of type <typeparamref name=""TResult""/>.
      /// </summary>");

      if (isPartially)
      {
         _sb.Append(@"
      /// <param name=""default"">The instance to return if no value is provided for the current value.</param>");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
      /// <param name=""").Append(memberType.ArgumentName).Append(@""">The instance to return if the current value is of type ").AppendTypeForXmlComment(memberType).Append(".</param>");
      }

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"
      /// <exception cref=""System.InvalidOperationException"">If the union (struct) is not initialized or initialized with default value.</exception>");
      }

      var methodName = isPartially ? "MapPartially" : "Map";

      _sb.Append(@"
      public TResult ").Append(methodName).Append("<TResult>(");

      if (isPartially)
      {
         _sb.Append(@"
         TResult @default,");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         ");

         if (isPartially)
            _sb.Append("global::Thinktecture.Argument<");

         _sb.Append("TResult");

         if (isPartially)
            _sb.Append(">");

         _sb.Append(" ").AppendEscaped(_state.MemberTypes[i].ArgumentName);

         if (isPartially)
            _sb.Append(" = default");
      }

      _sb.Append(@")
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
#endif
      {");

      GenerateIndexBasedMapSwitchBody(isPartially);

      _sb.Append(@"
      }");
   }

   private void GenerateIndexBasedMapSwitchBody(bool isPartially)
   {
      _sb.Append(@"
         switch (this._valueIndex)
         {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
            case 0:
               throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values."");");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
            case ").Append(i + 1).Append(":");

         if (isPartially)
         {
            _sb.Append(@"
               if (!").AppendEscaped(memberType.ArgumentName).Append(@".IsSet)
                  break;
");
         }

         _sb.Append(@"
               return ").AppendEscaped(memberType.ArgumentName);

         if (isPartially)
            _sb.Append(".Value");

         _sb.Append(";");
      }

      _sb.Append(@"
            default:
               throw new global::System.ArgumentOutOfRangeException($""Unexpected value index '{this._valueIndex}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         return @default;");
      }
   }

   private void GenerateConstructors()
   {
      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         if (memberType.TypeDuplicateIndex > 1)
            continue;

         var hasDuplicates = memberType.TypeDuplicateIndex is not null;
         var argName = hasDuplicates ? "value" : memberType.ArgumentName;

         _sb.Append(@"
");

         if (!hasDuplicates)
         {
            _sb.Append(@"
      /// <summary>
      /// Initializes new instance with <paramref name=""").Append(argName).Append(@"""/>.
      /// </summary>
      /// <param name=""").Append(argName).Append(@""">Value to create a new instance for.</param>");
         }

         _sb.Append(@"
      ").AppendAccessModifier(hasDuplicates ? UnionConstructorAccessModifier.Private : _state.Settings.ConstructorAccessModifier).Append(" ").Append(_state.Name).Append("(")
            ;

         if (hasDuplicates)
         {
            _sb.AppendTypeFullyQualifiedNullAnnotated(memberType).Append(" ").Append("@value, int @valueIndex");
         }
         else
         {
            _sb.AppendTypeFullyQualified(memberType).Append(" ").AppendEscaped(memberType.ArgumentName);
         }

         _sb.Append(@")
      {
         this._").Append(memberType.BackingFieldName).Append(" = ").AppendEscaped(argName).Append(@";
         this._valueIndex = ");

         if (hasDuplicates)
         {
            _sb.Append("@valueIndex");
         }
         else
         {
            _sb.Append(i + 1);
         }

         _sb.Append(@";
      }");
      }
   }

   private void GenerateFactoriesForTypeDuplicates()
   {
      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         if (memberType.TypeDuplicateIndex is null)
            continue;

         _sb.Append(@"

      /// <summary>
      /// Creates new instance with <paramref name=""").Append(memberType.ArgumentName).Append(@"""/>.
      /// </summary>
      /// <param name=""").Append(memberType.ArgumentName).Append(@""">Value to create a new instance for.</param>");

         _sb.Append(@"
      ").AppendAccessModifier(_state.Settings.ConstructorAccessModifier).Append(" static ").Append(_state.Name).Append(" Create").Append(memberType.Name).Append("(")
            .AppendTypeFullyQualified(memberType).Append(" ").AppendEscaped(memberType.ArgumentName).Append(@")
      {
         return new ").Append(_state.Name).Append("(").AppendEscaped(memberType.ArgumentName).Append(", ").Append(i + 1).Append(@");
      }");
      }
   }

   private void GenerateMemberTypeFieldsAndProps()
   {
      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         if (memberType.TypeDuplicateIndex > 1)
            continue;

         _sb.Append(@"
      private readonly ").AppendTypeFullyQualifiedNullAnnotated(memberType).Append(" _").Append(memberType.BackingFieldName).Append(";");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];
         _sb.Append(@"

      /// <summary>
      /// Indication whether the current value is of type ").AppendTypeForXmlComment(memberType).Append(@".
      /// </summary>
      public bool Is").Append(memberType.Name).Append(" => this._valueIndex == ").Append(i + 1).Append(";");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];
         _sb.Append(@"

      /// <summary>
      /// Gets the current value as ").AppendTypeForXmlComment(memberType).Append(@".
      /// </summary>
      /// <exception cref=""global::System.InvalidOperationException"">If the current value is not of type ").AppendTypeForXmlComment(memberType).Append(@".</exception>
      public ").AppendTypeFullyQualified(memberType).Append(" As").Append(memberType.Name).Append(" => Is").Append(memberType.Name)
            .Append(" ? this._").Append(memberType.BackingFieldName).Append(memberType.IsReferenceType && memberType.NullableAnnotation != NullableAnnotation.Annotated ? "!" : null)
            .Append(" : throw new global::System.InvalidOperationException($\"'{nameof(").AppendTypeFullyQualified(_state).Append(")}' is not of type '").AppendTypeMinimallyQualified(memberType).Append("'.\");");
      }
   }

   private void GenerateRawValueGetter()
   {
      var hasNullableTypes = _state.MemberTypes.Any(t => t.IsNullableStruct || t.NullableAnnotation == NullableAnnotation.Annotated);

      _sb.Append(@"

      /// <summary>
      /// Gets the current value as <see cref=""object""/>.
      /// </summary>");

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"
      /// <exception cref=""System.InvalidOperationException"">If the union (struct) is not initialized or initialized with default value.</exception>");
      }

      _sb.Append(@"
      public object").Append(hasNullableTypes ? "?" : null).Append(@" Value => this._valueIndex switch
      {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
         0 => throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values.""),");
      }

      for (var i = 0; i < _state.MemberTypes.Count; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
         ").Append(i + 1).Append(" => this._").Append(memberType.BackingFieldName).Append(memberType.IsReferenceType && !hasNullableTypes ? "!" : null).Append(",");
      }

      _sb.Append(@"
         _ => throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."")
      };");
   }
}
