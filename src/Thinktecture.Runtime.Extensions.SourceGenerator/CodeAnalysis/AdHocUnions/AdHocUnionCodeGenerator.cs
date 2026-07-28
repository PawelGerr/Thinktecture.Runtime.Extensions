using System.Text;

namespace Thinktecture.CodeAnalysis.AdHocUnions;

public sealed class AdHocUnionCodeGenerator : CodeGeneratorBase
{
   public override string CodeGeneratorName => "AdHocUnion-CodeGenerator";
   public override string FileNameSuffix => ".AdHocUnion";

   private readonly AdHocUnionSourceGenState _state;
   private readonly StringBuilder _sb;
   private readonly bool _useSharedObjectForRefTypes;
   private readonly bool _needsFactoryMethods;

   // The fixed Switch/Map parameters ('default' for partial overloads and the state parameter for with-state
   // overloads) must not collide with a per-member parameter whose name renders to the same identifier (e.g. a
   // member named "Default" or "State"), which would otherwise produce CS0100 in the generated signature.
   private readonly string _switchMapStateArgumentName;
   private readonly string _switchMapDefaultArgumentName;

   public AdHocUnionCodeGenerator(
      AdHocUnionSourceGenState state,
      StringBuilder sb)
   {
      _state = state;
      _sb = sb;
      _useSharedObjectForRefTypes = state.Settings.UseSingleBackingField
                                    || _state.MemberTypes.Where(t => t.IsReferenceType && !t.IsTypeParameter && t is { TypeDuplicateCounter: <= 1, Setting.IsStateless: false }).Select(t => t.TypeFullyQualified).Count() >= 2;
      _needsFactoryMethods = state.Settings.FactoryMethodGeneration switch
      {
         FactoryMethodGeneration.Always => true,
         FactoryMethodGeneration.None => false,
         _ => state.MemberTypes.Any(m => m.IsTypeParameter
                                         || m.TypeDuplicateCounter != 0
                                         || m.IsInterface
                                         || m.SpecialType == SpecialType.System_Object)
      };

      var memberArgumentNames = new HashSet<string>(StringComparer.Ordinal);

      foreach (var member in _state.MemberTypes)
      {
         memberArgumentNames.Add(StringBuilderExtensions.RenderArgumentName(member.ArgumentName));
      }

      _switchMapStateArgumentName = StringBuilderExtensions.MakeNonCollidingParameterName(_state.Settings.SwitchMapStateParameterName, memberArgumentNames);

      var namesTakenByDefault = new HashSet<string>(memberArgumentNames, StringComparer.Ordinal) { _switchMapStateArgumentName };
      _switchMapDefaultArgumentName = StringBuilderExtensions.MakeNonCollidingParameterName("default", namesTakenByDefault);
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
      _sb.GenerateStructLayoutAttributeIfRequired(_state.IsReferenceType, _state.AttributeInfo.HasStructLayoutAttribute);

      _sb.Append(@"
   [global::System.Diagnostics.CodeAnalysis.SuppressMessage(""ThinktectureRuntimeExtensionsAnalyzer"", ""TTRESG1000:Internal Thinktecture.Runtime.Extensions API usage"")]
   ").Append(_state.IsReferenceType ? "sealed " : "readonly ").Append("partial ").AppendTypeKind(_state).Append(" ").Append(_state.Name).AppendGenericTypeParameters(_state).Append(" :");

      if (!_state.IsRefStruct && !_state.Settings.SkipEqualityComparison)
      {
         _sb.Append(@"
      global::System.IEquatable<").AppendTypeFullyQualified(_state).Append(@">,
      global::System.Numerics.IEqualityOperators<").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(_state).Append(", bool>,");
      }

      if (_state.DisallowsDefaultValue)
      {
         _sb.Append(@"
      global::Thinktecture.IDisallowDefaultValue,");
      }

      var singleBackingFieldType = _state.Settings.SingleBackingFieldType?.FullyQualified;
      var valueType = singleBackingFieldType is not null
                         ? singleBackingFieldType + (_state.SingleBackingFieldNeedsNullableAnnotation() ? "?" : null)
                         : "object" + (_state.HasNullableMemberTypes() ? "?" : null);

      _sb.Append(@"
      global::Thinktecture.Internal.IMetadataOwner
   {
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
      static global::Thinktecture.Internal.Metadata global::Thinktecture.Internal.IMetadataOwner.Metadata { get; }
         = new global::Thinktecture.Internal.Metadata.AdHocUnion(typeof(").AppendTypeFullyQualified(_state).Append(@"))
      {
         MemberTypes = new global::System.Collections.Generic.List<global::System.Type>
                       {").AppendMemberTypes(_state.MemberTypes).Append(@"
                       }
                       .AsReadOnly(),");

      if (_state.IsRefStruct)
      {
         // Ref structs cannot be boxed and cannot appear in expression trees, so all three
         // value-accessor members are emitted as throw-based stubs using object-typed delegates.
         _sb.Append(@"
         ConvertToValue = static object? (object _) => throw new global::System.NotSupportedException(""Value accessor is not supported for ref struct unions.""),
         ConvertToValueExpression = global::System.Linq.Expressions.Expression.Lambda(
                                       global::System.Linq.Expressions.Expression.Throw(
                                          global::System.Linq.Expressions.Expression.New(typeof(global::System.NotSupportedException)),
                                          typeof(object)),
                                       global::System.Linq.Expressions.Expression.Parameter(typeof(object), ""_"")),
         GetValue = static object? (object _) => throw new global::System.NotSupportedException(""Value accessor is not supported for ref struct unions."")");
      }
      else
      {
         _sb.Append(@"
         ConvertToValue = static ").Append(valueType).Append(" (").AppendTypeFullyQualified(_state).Append(@" item) => item.Value,
         ConvertToValueExpression = static ").Append(valueType).Append(" (").AppendTypeFullyQualified(_state).Append(@" item) => item.Value,
         GetValue = static object? (object item) => ((").AppendTypeFullyQualified(_state).Append(@")item).Value");
      }

      _sb.Append(@"
      };

      private readonly int _valueIndex;
"); // index is 1-based

      GenerateMemberTypeFieldsAndProps();
      GenerateGetMemberTypeName();
      GenerateRawValueGetter();
      GenerateConstructors();
      GenerateFactoryMethods();

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

      GenerateConversionsFromValue();
      GenerateConversionsToValue();

      if (!_state.Settings.SkipEqualityComparison)
      {
         GenerateEqualityOperators();
         GenerateEquals();
         GenerateGetHashCode();
      }

      if (!_state.Settings.SkipToString)
         GenerateToString();

      GenerateNormalizePartialDeclarations();

      _sb.Append(@"
   }");
   }

   private void GenerateNormalizePartialDeclarations()
   {
      foreach (var memberType in _state.MemberTypes)
      {
         if (memberType.Setting.IsStateless)
            continue;

         // No call site: with FactoryMethodGeneration = None and TypeDuplicateCounter != 0,
         // no factory is generated and the indexed ctor is private. Skip the declaration.
         if (!_needsFactoryMethods && memberType.TypeDuplicateCounter != 0)
            continue;

         _sb.Append(@"

      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      static partial void ").Append(Constants.Methods.NORMALIZE).Append(memberType.Name).Append("(ref ").AppendTypeFullyQualified(memberType).Append(" ").AppendEscaped(memberType.ArgumentName).Append(");");
      }
   }

   private void GenerateFactoryMethods()
   {
      if (!_needsFactoryMethods)
         return;

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         GenerateFactoryMethod(_state.MemberTypes[i], i);
      }
   }

   private void GenerateConversionsFromValue()
   {
      if (_state.Settings.ConversionFromValue == ConversionOperatorsGeneration.None)
         return;

      foreach (var memberType in _state.MemberTypes)
      {
         if (memberType.IsInterface
             || memberType.SpecialType == SpecialType.System_Object
             || memberType.TypeDuplicateCounter != 0
             || memberType.IsTypeParameter)
            continue;

         _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.ConversionFromValue == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion from type ").AppendMemberTypeForXmlComment(memberType).Append(@".
      /// </summary>
      /// <param name=""").AppendArgumentName(memberType.ArgumentName).Append(@""">Value to convert from.</param>
      /// <returns>A new instance of ").AppendTypeForXmlComment(_state).Append(@" converted from <paramref name=""").AppendArgumentName(memberType.ArgumentName).Append(@"""/>.</returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static ").AppendConversionOperator(_state.Settings.ConversionFromValue).Append(" operator ").AppendTypeFullyQualified(_state).Append("(").AppendTypeFullyQualified(memberType).Append(" ").AppendEscaped(memberType.ArgumentName).Append(@")
      {
         return new ").AppendTypeFullyQualified(_state).Append("(").AppendEscaped(memberType.ArgumentName).Append(@");
      }");
      }
   }

   private void GenerateFactoryMethod(AdHocUnionMemberTypeState memberType, int memberIndex)
   {
      _sb.Append(@"

      /// <summary>
      /// Creates a new instance of ").AppendTypeForXmlComment(_state).Append(" from a value of type ").AppendMemberTypeForXmlComment(memberType).Append(@".
      /// </summary>");

      if (!memberType.Setting.IsStateless)
      {
         _sb.Append(@"
      /// <param name=""").AppendArgumentName(memberType.ArgumentName).Append(@""">Value to create a new instance for.</param>");
      }

      _sb.Append(@"
      /// <returns>A new instance of ").AppendTypeForXmlComment(_state).Append(@".</returns>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      ").AppendAccessModifier(_state.Settings.ConstructorAccessModifier).Append(" static ").AppendTypeFullyQualified(_state).Append(" Create").Append(memberType.Name).Append("(");

      if (!memberType.Setting.IsStateless)
      {
         _sb.AppendTypeFullyQualified(memberType).Append(" ").AppendEscaped(memberType.ArgumentName);
      }

      _sb.Append(@")
      {");

      if (memberType.TypeDuplicateCounter != 0 && !memberType.Setting.IsStateless)
      {
         _sb.Append(@"
         ").Append(Constants.Methods.NORMALIZE).Append(memberType.Name).Append("(ref ").AppendEscaped(memberType.ArgumentName).Append(");");
      }

      _sb.Append(@"
         return new ").AppendTypeFullyQualified(_state).Append("(");

      if (memberType.Setting.IsStateless)
      {
         _sb.Append("default(").AppendTypeFullyQualified(memberType).Append(")");
      }
      else
      {
         _sb.AppendEscaped(memberType.ArgumentName);
      }

      if (memberType.TypeDuplicateCounter != 0)
      {
         _sb.Append(", ").Append(memberIndex + 1);
      }

      _sb.Append(@");
      }");
   }

   private void GenerateConversionsToValue()
   {
      if (_state.Settings.ConversionToValue == ConversionOperatorsGeneration.None)
         return;

      foreach (var memberType in _state.MemberTypes)
      {
         if (memberType.IsInterface
             || memberType.SpecialType == SpecialType.System_Object
             || memberType.TypeDuplicateCounter != 0
             || memberType.IsTypeParameter)
            continue;

         _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.ConversionToValue == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion to type ").AppendMemberTypeForXmlComment(memberType).Append(@".
      /// </summary>
      /// <param name=""obj"">Object to convert.</param>
      /// <returns>Inner value of type ").AppendMemberTypeForXmlComment(memberType).Append(@".</returns>
      /// <exception cref=""System.InvalidOperationException"">If the inner value is not a ").AppendMemberTypeForXmlComment(memberType).Append(@".</exception>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static ").AppendConversionOperator(_state.Settings.ConversionToValue).Append(" operator ").AppendTypeFullyQualified(memberType).Append("(").AppendTypeFullyQualified(_state).Append(@" obj)
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
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
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
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static bool operator !=(").AppendTypeFullyQualifiedNullAnnotated(_state).Append(" obj, ").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" other)
      {
         return !(obj == other);
      }");
   }

   private void GenerateToString()
   {
      _sb.Append(@"

      /// <inheritdoc />
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override string? ToString()
      {
         return this._valueIndex switch
         {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
            0 => throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values.""),");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
            ").Append(i + 1).Append(" => ");

         if (memberType.Setting.IsStateless)
         {
            _sb.Append("default(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberType).Append(")");
         }
         else
         {
            _sb.AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType);
         }

         if (memberType.SpecialType != SpecialType.System_String)
         {
            if (memberType.IsReferenceType || memberType is { IsTypeParameter: true, IsValueType: false })
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
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override int GetHashCode()
      {
         return this._valueIndex switch
         {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
            0 => throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values.""),");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
            ").Append(i + 1).Append(" => ");

         if (memberType.Setting.IsStateless)
         {
            _sb.Append("typeof(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberType).Append(")");
         }
         else if (memberType.IsTypeParameter)
         {
            _sb.Append("global::System.Collections.Generic.EqualityComparer<").AppendTypeFullyQualified(memberType).Append(">.Default.GetHashCode(")
               .AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType, nullAnnotated: false, suppressed: true).Append("!)");

            _sb.Append(",");
            continue;
         }
         else
         {
            _sb.AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType);
         }

         if (memberType.IsReferenceType)
            _sb.Append("?");

         _sb.Append(".GetHashCode(");

         if (memberType.SpecialType == SpecialType.System_String)
            _sb.Append("global::System.StringComparison.").Append(_state.Settings.DefaultStringComparison);

         _sb.Append(")");

         if (memberType.IsReferenceType)
            _sb.Append(" ?? 0");

         _sb.Append(",");
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
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
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
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
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

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];
         var useSharedObjectBackingField = _state.UseSharedObjectBackingField(_useSharedObjectForRefTypes, memberType);

         _sb.Append(@"
            ").Append(i + 1).Append(" => ");

         if (memberType.Setting.IsStateless)
         {
            _sb.Append("true");
         }
         else if (memberType.IsTypeParameter)
         {
            _sb.Append("global::System.Collections.Generic.EqualityComparer<").AppendTypeFullyQualified(memberType).Append(">.Default.Equals(")
               .AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType, nullAnnotated: false, suppressed: true).Append(", ")
               .AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType, nullAnnotated: false, suppressed: true, qualifier: "other").Append(")");
         }
         else
         {
            // The "is null" short-circuit only makes sense when the backing field can actually be
            // null. When the typed single backing field is non-nullable (SingleBackingFieldType
            // set to a non-nullable value type, or a reference type with no nullable members),
            // the check is unreachable and would emit CS8073 in user code -- skip it.
            var sharedFieldIsNullable = _state.SharedBackingFieldIsNullable();
            var emitNullShortCircuit = memberType.IsReferenceType
                                       && (!useSharedObjectBackingField || sharedFieldIsNullable);

            if (emitNullShortCircuit)
            {
               _sb.Append("this.").AppendBackingFieldName(useSharedObjectBackingField, memberType).Append(" is null ? other.").AppendBackingFieldName(_state, _useSharedObjectForRefTypes, memberType).Append(" is null : ");
            }

            _sb.AppendBackingFieldAccess(useSharedObjectBackingField, memberType, nullAnnotated: false, suppressed: true, sharedFieldIsNullable: sharedFieldIsNullable).Append(".Equals(").AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType, qualifier: "other");

            if (memberType.SpecialType == SpecialType.System_String)
               _sb.Append(", global::System.StringComparison.").Append(_state.Settings.DefaultStringComparison);

            _sb.Append(")");
         }

         _sb.Append(",");
      }

      _sb.Append(@"
            _ => throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."")
         };
      }");
   }

   private void GenerateSwitchForAction(bool withState, bool isPartially)
   {
      _sb.Append(@"

#pragma warning disable CS0436 // InstantHandleAttribute may come from a different assembly
      /// <summary>
      /// Executes an action depending on the current value.
      /// </summary>");

      if (withState)
      {
         _sb.Append(@"
      /// <param name=""").Append(_switchMapStateArgumentName).Append(@""">State to be passed to the callbacks.</param>");
      }

      if (isPartially)
      {
         _sb.Append(@"
      /// <param name=""").Append(_switchMapDefaultArgumentName).Append(@""">The action to execute if no value-specific action is provided.</param>");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
      /// <param name=""").AppendArgumentName(memberType.ArgumentName).Append(@""">The action to execute if the current value is of type ").AppendMemberTypeForXmlComment(memberType).Append(".</param>");
      }

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"
      /// <exception cref=""System.InvalidOperationException"">If the union (struct) is not initialized or initialized with default value.</exception>");
      }

      _sb.Append(@"
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerStepThroughAttribute]
      public void ").Append(isPartially ? Constants.Methods.SWITCH_PARTIALLY : Constants.Methods.SWITCH);

      if (withState)
      {
         _sb.Append(@"<TState>(
         TState ").AppendEscaped(_switchMapStateArgumentName).Append(",");
      }
      else
      {
         _sb.Append("(");
      }

      if (isPartially)
      {
         _sb.Append(@"
         [global::JetBrains.Annotations.InstantHandleAttribute] global::System.Action<");

         if (withState)
            _sb.Append("TState, ");

         _sb.Append("object?>? @").Append(_switchMapDefaultArgumentName).Append(" = null,");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         [global::JetBrains.Annotations.InstantHandleAttribute] global::System.Action<");

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
      }
#pragma warning restore CS0436 // InstantHandleAttribute may come from a different assembly");
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

      for (var i = 0; i < _state.MemberTypes.Length; i++)
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
            _sb.AppendEscaped(_switchMapStateArgumentName).Append(", ");

         if (memberType.Setting.IsStateless)
         {
            _sb.Append("default(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberType).Append(")");
         }
         else
         {
            _sb.AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType).Append((memberType.IsReferenceType || memberType is { IsTypeParameter: true, IsValueType: false }) && memberType.NullableAnnotation != NullableAnnotation.Annotated ? "!" : null);
         }

         _sb.Append(@");
               return;");
      }

      _sb.Append(@"
            default:
               throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         @").Append(_switchMapDefaultArgumentName).Append("?.Invoke(");

         if (withState)
            _sb.AppendEscaped(_switchMapStateArgumentName).Append(", ");

         _sb.Append("this.Value);");
      }
   }

   private void GenerateSwitchForFunc(bool withState, bool isPartially)
   {
      _sb.Append(@"

#pragma warning disable CS0436 // InstantHandleAttribute may come from a different assembly
      /// <summary>
      /// Executes a function depending on the current value.
      /// </summary>");

      if (withState)
      {
         _sb.Append(@"
      /// <param name=""").Append(_switchMapStateArgumentName).Append(@""">State to be passed to the callbacks.</param>");
      }

      if (isPartially)
      {
         _sb.Append(@"
      /// <param name=""").Append(_switchMapDefaultArgumentName).Append(@""">The function to execute if no value-specific action is provided.</param>");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
      /// <param name=""").AppendArgumentName(memberType.ArgumentName).Append(@""">The function to execute if the current value is of type ").AppendMemberTypeForXmlComment(memberType).Append(".</param>");
      }

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"
      /// <exception cref=""System.InvalidOperationException"">If the union (struct) is not initialized or initialized with default value.</exception>");
      }

      _sb.Append(@"
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerStepThroughAttribute]
      public TResult ").Append(isPartially ? Constants.Methods.SWITCH_PARTIALLY : Constants.Methods.SWITCH);

      if (withState)
      {
         _sb.Append(@"<TState, TResult>(
         TState ").AppendEscaped(_switchMapStateArgumentName).Append(",");
      }
      else
      {
         _sb.Append("<TResult>(");
      }

      if (isPartially)
      {
         _sb.Append(@"
         [global::JetBrains.Annotations.InstantHandleAttribute] global::System.Func<");

         if (withState)
            _sb.Append("TState, ");

         _sb.Append("object?, TResult> @").Append(_switchMapDefaultArgumentName).Append(",");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         ");

         _sb.Append("[global::JetBrains.Annotations.InstantHandleAttribute] global::System.Func<");

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
      }
#pragma warning restore CS0436 // InstantHandleAttribute may come from a different assembly");
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

      for (var i = 0; i < _state.MemberTypes.Length; i++)
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
            _sb.AppendEscaped(_switchMapStateArgumentName).Append(", ");

         if (memberType.Setting.IsStateless)
         {
            _sb.Append("default(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberType).Append(")");
         }
         else
         {
            _sb.AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType).Append(memberType is { IsReferenceType: true, Setting.IsNullableReferenceType: false } or { IsTypeParameter: true, IsValueType: false } ? "!" : null);
         }

         _sb.Append(");");
      }

      _sb.Append(@"
            default:
               throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         return @").Append(_switchMapDefaultArgumentName).Append("(");

         if (withState)
            _sb.AppendEscaped(_switchMapStateArgumentName).Append(", ");

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
      /// <param name=""").Append(_switchMapDefaultArgumentName).Append(@""">The instance to return if no value is provided for the current value.</param>");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
      /// <param name=""").AppendArgumentName(memberType.ArgumentName).Append(@""">The instance to return if the current value is of type ").AppendMemberTypeForXmlComment(memberType).Append(".</param>");
      }

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"
      /// <exception cref=""System.InvalidOperationException"">If the union (struct) is not initialized or initialized with default value.</exception>");
      }

      _sb.Append(@"
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerStepThroughAttribute]
      public TResult ").Append(isPartially ? Constants.Methods.MAP_PARTIALLY : Constants.Methods.MAP).Append("<TResult>(");

      if (isPartially)
      {
         _sb.Append(@"
         TResult @").Append(_switchMapDefaultArgumentName).Append(",");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
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

      for (var i = 0; i < _state.MemberTypes.Length; i++)
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
               throw new global::System.InvalidOperationException($""Unexpected value index '{this._valueIndex}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         return @").Append(_switchMapDefaultArgumentName).Append(";");
      }
   }

   private void GenerateConstructors()
   {
      var valueArgName = ArgumentName.Create("value", renderAsIs: true);

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         if (memberType.TypeDuplicateCounter > 1)
            continue;

         var needsIndexedConstructor = memberType.TypeDuplicateCounter != 0;
         var argName = needsIndexedConstructor ? valueArgName : memberType.ArgumentName;

         _sb.Append(@"
");

         if (!needsIndexedConstructor)
         {
            _sb.Append(@"
      /// <summary>
      /// Initializes new instance with <paramref name=""").AppendArgumentName(argName).Append(@"""/>.
      /// </summary>
      /// <param name=""").AppendArgumentName(argName).Append(@""">Value to create a new instance for.</param>");
         }

         _sb.Append(@"
      ").Append(GENERATED_CODE_ATTRIBUTE);

         if (needsIndexedConstructor)
         {
            _sb.Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]");
         }

         _sb.Append(@"
      ").AppendAccessModifier(needsIndexedConstructor ? UnionConstructorAccessModifier.Private : _state.Settings.ConstructorAccessModifier).Append(" ").Append(_state.Name).Append("(")
            ;

         if (needsIndexedConstructor)
         {
            _sb.AppendTypeFullyQualifiedNullAnnotated(memberType).Append(" ").Append("@value, int @valueIndex");
         }
         else
         {
            _sb.AppendTypeFullyQualified(memberType).Append(" ").AppendEscaped(memberType.ArgumentName);
         }

         _sb.Append(@")
      {");

         if (!needsIndexedConstructor && !memberType.Setting.IsStateless)
         {
            _sb.Append(@"
         ").Append(Constants.Methods.NORMALIZE).Append(memberType.Name).Append("(ref ").AppendEscaped(memberType.ArgumentName).Append(");");
         }

         if (!memberType.Setting.IsStateless)
         {
            _sb.Append(@"
         ").AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType, false).Append(" = ").AppendEscaped(argName).Append(";");
         }
         else if (_state.Settings.SingleBackingFieldType is not null
                  && memberType.IsValueType
                  && !memberType.IsTypeParameter)
         {
            // Stateless struct member with a typed single backing field: assign the cached boxed default.
            _sb.Append(@"
         this._obj = _cachedBoxed").Append(memberType.Name).Append(";");
         }

         _sb.Append(@"
         this._valueIndex = ");

         if (needsIndexedConstructor)
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

   private void GenerateMemberTypeFieldsAndProps()
   {
      var singleBackingFieldType = _state.Settings.SingleBackingFieldType?.FullyQualified;
      var singleBackingFieldIsNullable = _state.SingleBackingFieldNeedsNullableAnnotation();

      // Pass 1: emit `private static readonly` cached boxed defaults for stateless struct members.
      // Static fields are emitted before any instance fields per project convention.
      if (singleBackingFieldType is not null)
      {
         for (var i = 0; i < _state.MemberTypes.Length; i++)
         {
            var memberType = _state.MemberTypes[i];

            if (!memberType.Setting.IsStateless || !memberType.IsValueType || memberType.IsTypeParameter)
               continue;

            if (memberType.TypeDuplicateCounter > 1)
               continue;

            _sb.Append(@"
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      private static readonly ").Append(singleBackingFieldType).Append(" _cachedBoxed").Append(memberType.Name).Append(" = default(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberType).Append(");");
         }
      }

      // Pass 2: emit the shared non-static `_obj` field. The condition lives in
      // HasSharedObjectBackingField so that the field emission and the Value getter, which reads
      // the field, cannot drift apart.
      if (_state.HasSharedObjectBackingField(_useSharedObjectForRefTypes))
      {
         if (singleBackingFieldType is not null)
         {
            _sb.Append(@"
      private readonly ").Append(singleBackingFieldType).Append(singleBackingFieldIsNullable ? "?" : null).Append(" _obj;");
         }
         else
         {
            _sb.Append(@"
      private readonly object? _obj;");
         }
      }

      // Pass 3: emit per-member non-shared instance fields.
      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         if (memberType.Setting.IsStateless)
            continue;

         if (memberType.TypeDuplicateCounter > 1)
            continue;

         if (_state.UseSharedObjectBackingField(_useSharedObjectForRefTypes, memberType))
            continue;

         _sb.Append(@"
      private readonly ").AppendTypeFullyQualifiedNullAnnotated(memberType).Append(" ").AppendBackingFieldName(memberType.BackingFieldName).Append(";");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];
         _sb.Append(@"

      /// <summary>
      /// Indication whether the current value is of type ").AppendMemberTypeForXmlComment(memberType).Append(@".
      /// </summary>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public bool Is").Append(memberType.Name).Append(" => this._valueIndex == ").Append(i + 1).Append(";");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];
         _sb.Append(@"

      /// <summary>
      /// Gets the current value as ").AppendMemberTypeForXmlComment(memberType).Append(@".
      /// </summary>
      /// <exception cref=""global::System.InvalidOperationException"">If the current value is not of type ").AppendMemberTypeForXmlComment(memberType).Append(@".</exception>
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public ").AppendTypeFullyQualified(memberType).Append(" As").Append(memberType.Name).Append(" => Is").Append(memberType.Name)
            .Append(" ? ");

         if (memberType.Setting.IsStateless)
         {
            _sb.Append("default(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberType).Append(")");
         }
         else
         {
            _sb.AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType).Append((memberType.IsReferenceType || memberType is { IsTypeParameter: true, IsValueType: false }) && memberType.NullableAnnotation != NullableAnnotation.Annotated ? "!" : null);
         }

         _sb.Append(" : throw new global::System.InvalidOperationException($\"'{nameof(").AppendTypeFullyQualified(_state).Append(")}' is not of type '").AppendTypeMinimallyQualified(memberType).Append("' but of type '{GetMemberTypeName()}'.\");");
      }
   }

   private void GenerateGetMemberTypeName()
   {
      _sb.Append(@"

      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      private string GetMemberTypeName()
      {
         return this._valueIndex switch
         {
            0 => ""<uninitialized>"",");

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
            ").Append(i + 1).Append(@" => """).AppendTypeMinimallyQualified(memberType).Append(@""",");
      }

      _sb.Append(@"
            _ => throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."")
         };
      }");
   }

   private void GenerateRawValueGetter()
   {
      var hasNullableTypes = _state.HasNullableMemberTypes();
      var singleBackingFieldType = _state.Settings.SingleBackingFieldType?.FullyQualified;
      var singleBackingFieldNeedsNullable = _state.SingleBackingFieldNeedsNullableAnnotation();

      _sb.Append(@"

      /// <summary>
      /// Gets the current value as ");

      if (singleBackingFieldType is not null)
      {
         // Match the codebase XML-doc convention (see AppendTypeForXmlComment in
         // StringBuilderExtensions): generic / array / nullable / tuple type names contain
         // characters that are invalid inside a "cref" attribute -- use <c>...&lt;T&gt;</c>.
         var hasProblematicCharsForXmlDocs = singleBackingFieldType.IndexOfAny(['<', '[', '?', '(']) > -1;

         if (hasProblematicCharsForXmlDocs)
         {
            _sb.Append("<c>").Append(singleBackingFieldType.Replace("<", "&lt;").Replace(">", "&gt;")).Append("</c>.");
         }
         else
         {
            _sb.Append("<see cref=\"").Append(singleBackingFieldType).Append("\"/>.");
         }
      }
      else
      {
         _sb.Append(@"<see cref=""object""/>.");
      }

      _sb.Append(@"
      /// </summary>");

      if (!_state.IsReferenceType)
      {
         _sb.Append(@"
      /// <exception cref=""System.InvalidOperationException"">If the union (struct) is not initialized or initialized with default value.</exception>");
      }

      _sb.Append(@"
      ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
      public ");

      if (singleBackingFieldType is not null)
      {
         // Value-type backing fields are honored as written: typeof(int) -> int, typeof(int?) -> int?.
         // Only reference-type backing fields gain an extra "?" when any member contributes nullability.
         _sb.Append(singleBackingFieldType).Append(singleBackingFieldNeedsNullable ? "?" : null);
      }
      else
      {
         _sb.Append("object").Append(hasNullableTypes ? "?" : null);
      }

      _sb.Append(" Value => ");

      // The short-circuit is only valid when the shared field exists and every member really reads
      // it. A union whose members are all stateless has no shared field at all, and a stateless
      // struct member without a typed single backing field never gets a value assigned.
      // The auto-sharing path reaches this point as well: with 2 or more distinct non-stateless
      // reference type members the generator merges them into `_obj` without any opt-in setting,
      // and then every arm of the discriminator switch is the same expression.
      var collapseToSharedField = _state.HasSharedObjectBackingField(_useSharedObjectForRefTypes)
                                  && _state.AllMembersReadSharedObjectField(_useSharedObjectForRefTypes);

      // The legacy UseSingleBackingField path emits "!" unconditionally to keep its snapshots
      // byte-for-byte stable. The auto-sharing path follows the rule of the switch arms it
      // replaces: suppress only when Value is declared non-nullable.
      var emitSuppression = singleBackingFieldType is null
                            && (_state.Settings.UseSingleBackingField || !hasNullableTypes);

      // Class unions: a class instance cannot be uninitialized via `default(...)`, so no
      // discriminator throw is needed.
      if (collapseToSharedField && _state.IsReferenceType)
      {
         _sb.Append("this._obj").Append(emitSuppression ? "!" : null).Append(";");
         return;
      }

      // Struct unions: every arm of a full discriminator switch would collapse to the same
      // `this._obj`. Emit a single index check + return instead. A `default(StructUnion).Value`
      // must throw to match the contract documented above and emitted by IsTx/AsTx/Switch/Map.
      if (collapseToSharedField)
      {
         _sb.Append($@"this._valueIndex == 0
         ? throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values."")
         : this._obj").Append(emitSuppression ? "!" : null).Append(";");
         return;
      }

      _sb.Append(@"this._valueIndex switch
      {");

      if (!_state.IsReferenceType)
      {
         _sb.Append($@"
         0 => throw new global::System.InvalidOperationException($""This struct of type '{_state.Name}' is not initialized. Make sure all fields, properties and variables are initialized with non-default values.""),");
      }

      for (var i = 0; i < _state.MemberTypes.Length; i++)
      {
         var memberType = _state.MemberTypes[i];

         _sb.Append(@"
         ").Append(i + 1).Append(" => ");

         if (memberType.Setting.IsStateless)
         {
            // A type parameter carries no constraint that makes `default(T)` convertible to a typed
            // single backing field, so the value is routed through `object` to keep the generated
            // code compiling for every type argument. The suppression sits on `default(T)` so that
            // the cast never produces a nullability warning.
            if (singleBackingFieldType is not null && memberType.IsTypeParameter)
            {
               _sb.Append("(").Append(singleBackingFieldType).Append(singleBackingFieldNeedsNullable ? "?" : null)
                  .Append(")(object?)default(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberType).Append(")!");
            }
            else
            {
               // `default(T)` of a type parameter that is not known to be a value type may be null,
               // so it needs the same suppression the non-stateless arms below use when Value is
               // declared non-nullable.
               var suppressNull = memberType is { IsTypeParameter: true, IsValueType: false } && !hasNullableTypes;

               _sb.Append("default(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberType).Append(")").Append(suppressNull ? "!" : null);
            }
         }
         else
         {
            _sb.AppendBackingFieldAccess(_state, _useSharedObjectForRefTypes, memberType, withCast: false, nullAnnotated: false, suppressed: false).Append((memberType.IsReferenceType || memberType is { IsTypeParameter: true, IsValueType: false }) && !hasNullableTypes ? "!" : null);
         }

         _sb.Append(",");
      }

      _sb.Append(@"
         _ => throw new global::System.IndexOutOfRangeException($""Unexpected value index '{this._valueIndex}'."")
      };");
   }
}

file static class Extensions
{
   public static StringBuilder AppendMemberTypes(this StringBuilder sb, ImmutableArray<AdHocUnionMemberTypeState> memberTypes)
   {
      for (var i = 0; i < memberTypes.Length; i++)
      {
         var member = memberTypes[i];

         if (i > 0)
            sb.Append(",");

         sb.Append(@"
                           typeof(").AppendTypeFullyQualifiedWithoutNullAnnotation(member).Append(")");
      }

      return sb;
   }

   public static bool HasNullableMemberTypes(this AdHocUnionSourceGenState state)
   {
      return state.MemberTypes.Any(t => t.IsNullableStruct || t.NullableAnnotation == NullableAnnotation.Annotated);
   }

   /// <summary>
   /// Whether the union declares the shared <c>_obj</c> field. The field is declared when at least one
   /// non-duplicate, non-stateless member uses it, or when a typed single backing field is configured,
   /// which forces the declaration even for a union whose members are all stateless.
   /// <c>GenerateMemberTypeFieldsAndProps</c> emits the field from this predicate, and the collapsed
   /// raw value getter reads the field only when this predicate holds. Other generated members decide
   /// per member through <see cref="UseSharedObjectBackingField"/>, which is a separate condition.
   /// </summary>
   public static bool HasSharedObjectBackingField(
      this AdHocUnionSourceGenState state,
      bool useSharedObjectForRefTypes)
   {
      if (state.Settings.SingleBackingFieldType is not null && state.Settings.UseSingleBackingField)
         return true;

      return state.MemberTypes.Any(m => !m.Setting.IsStateless
                                        && m.TypeDuplicateCounter <= 1
                                        && state.UseSharedObjectBackingField(useSharedObjectForRefTypes, m));
   }

   /// <summary>
   /// Whether every member's raw value reads the shared <c>_obj</c> field, so that the discriminator
   /// switch of the raw value getter would collapse into arms that are all identical.
   /// </summary>
   public static bool AllMembersReadSharedObjectField(
      this AdHocUnionSourceGenState state,
      bool useSharedObjectForRefTypes)
   {
      return state.MemberTypes.All(m => m.Setting.IsStateless
                                           ? StatelessMemberReadsSharedObjectField(state, m)
                                           : state.UseSharedObjectBackingField(useSharedObjectForRefTypes, m));
   }

   /// <summary>
   /// A stateless member has no assignment of its own, so it only reads the same value from the
   /// shared field that the <c>default(T)</c> switch arm would return when:
   ///  - it is a reference type, because the field stays null and <c>default(T)</c> is null as well, or
   ///  - it is a struct and a typed single backing field is set, because then the constructor assigns
   ///    the cached boxed default.
   /// A stateless type parameter gets no assignment at all, not even the cached boxed default, so it
   /// qualifies only when the type parameter is known to be a reference type. For a struct or an
   /// unconstrained type parameter the field stays null while <c>default(T)</c> is a boxed zero value.
   /// </summary>
   private static bool StatelessMemberReadsSharedObjectField(
      AdHocUnionSourceGenState state,
      AdHocUnionMemberTypeState memberType)
   {
      if (memberType.IsTypeParameter)
         return memberType.IsReferenceType;

      if (memberType.IsReferenceType)
         return true;

      return state.Settings.SingleBackingFieldType is not null;
   }

   /// <summary>
   /// Whether the typed single backing field declaration needs an additional <c>?</c> annotation.
   /// Only reference-type backing fields gain a <c>?</c>; value-type backing fields are written
   /// exactly as the user specified them (so <c>typeof(int)</c> stays <c>int</c> and
   /// <c>typeof(int?)</c> stays <c>int?</c>) -- upgrading them based on member nullability would
   /// silently change the user's chosen type.
   /// </summary>
   public static bool SingleBackingFieldNeedsNullableAnnotation(this AdHocUnionSourceGenState state)
   {
      return state.Settings.SingleBackingFieldType is { IsReferenceType: true }
             && state.HasNullableMemberTypes();
   }

   /// <summary>
   /// Whether the shared backing field can hold a null value at runtime. Drives the "is null"
   /// short-circuit in Equals and the "!" null-suppression operators.
   /// True when:
   ///  - the legacy <c>object?</c> path is used (SingleBackingFieldType not set), OR
   ///  - SingleBackingFieldType is itself a <c>Nullable&lt;T&gt;</c> struct (HasValue can be false), OR
   ///  - SingleBackingFieldType is a reference type and any member contributes nullability.
   /// </summary>
   public static bool SharedBackingFieldIsNullable(this AdHocUnionSourceGenState state)
   {
      return state.Settings.SingleBackingFieldType is null
             || state.Settings.SingleBackingFieldType is { IsNullableStruct: true }
             || (state.Settings.SingleBackingFieldType is { IsReferenceType: true } && state.HasNullableMemberTypes());
   }

   public static bool UseSharedObjectBackingField(
      this AdHocUnionSourceGenState state,
      bool useSharedObjectForRefTypes,
      AdHocUnionMemberTypeState memberType)
   {
      if (memberType.IsTypeParameter)
         return state.Settings.UseSingleBackingField;

      return state.Settings.UseSingleBackingField || (useSharedObjectForRefTypes && memberType.IsReferenceType);
   }

   public static StringBuilder AppendBackingFieldAccess(
      this StringBuilder sb,
      AdHocUnionSourceGenState state,
      bool useSharedObjectForRefTypes,
      AdHocUnionMemberTypeState memberType,
      bool withCast = true,
      bool nullAnnotated = true,
      bool suppressed = false,
      string qualifier = "this")
   {
      var useSharedObjectBackingField = state.UseSharedObjectBackingField(useSharedObjectForRefTypes, memberType);
      // When the single backing field is typed (SingleBackingFieldType set) and not annotated nullable,
      // the "!" null-suppression is redundant and we should not emit it.
      var sharedFieldIsNullable = state.SharedBackingFieldIsNullable();

      return AppendBackingFieldAccess(sb, useSharedObjectBackingField, memberType, withCast, nullAnnotated, suppressed, qualifier, sharedFieldIsNullable);
   }

   public static StringBuilder AppendBackingFieldAccess(
      this StringBuilder sb,
      bool useSharedObjectBackingField,
      AdHocUnionMemberTypeState memberType,
      bool withCast = true,
      bool nullAnnotated = true,
      bool suppressed = false,
      string qualifier = "this",
      bool sharedFieldIsNullable = true)
   {
      if (useSharedObjectBackingField)
      {
         if (withCast)
         {
            sb.Append("((");

            if (nullAnnotated)
            {
               sb.AppendTypeFullyQualifiedNullAnnotated(memberType);
            }
            else
            {
               sb.AppendTypeFullyQualified(memberType);
            }

            sb.Append(")");
         }

         sb.Append(qualifier).Append("._obj");

         if (withCast)
         {
            if (sharedFieldIsNullable
                && (suppressed || memberType is { IsReferenceType: false, IsNullableStruct: false }))
               sb.Append("!");

            sb.Append(")");
         }
      }
      else
      {
         sb.Append(qualifier).Append(".").AppendBackingFieldName(memberType.BackingFieldName);
      }

      return sb;
   }

   public static StringBuilder AppendBackingFieldName(
      this StringBuilder sb,
      AdHocUnionSourceGenState state,
      bool useSharedObjectForRefTypes,
      AdHocUnionMemberTypeState memberType)
   {
      var useSharedObjectBackingField = state.UseSharedObjectBackingField(useSharedObjectForRefTypes, memberType);

      return AppendBackingFieldName(sb, useSharedObjectBackingField, memberType);
   }

   public static StringBuilder AppendBackingFieldName(
      this StringBuilder sb,
      bool useSharedObjectBackingField,
      AdHocUnionMemberTypeState memberType)
   {
      return useSharedObjectBackingField
                ? sb.Append("_obj")
                : sb.AppendBackingFieldName(memberType.BackingFieldName);
   }
}
