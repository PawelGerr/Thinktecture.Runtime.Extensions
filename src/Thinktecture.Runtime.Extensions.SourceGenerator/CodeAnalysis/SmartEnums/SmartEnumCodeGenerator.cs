using System.Runtime.CompilerServices;
using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SmartEnumCodeGenerator : SmartEnumAndValueObjectCodeGeneratorBase
{
   private readonly SmartEnumSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "SmartEnum-CodeGenerator";
   public override string FileNameSuffix => ".SmartEnum";

   public SmartEnumCodeGenerator(SmartEnumSourceGeneratorState state, StringBuilder stringBuilder)
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

      GenerateEnum(cancellationToken);

      _sb.RenderContainingTypesEnd(_state.ContainingTypes);

      if (hasNamespace)
      {
         _sb.Append(@"
}");
      }

      _sb.Append(@"
");
   }

   private void GenerateEnum(CancellationToken cancellationToken)
   {
      if (_state.KeyMember is not null)
      {
         _sb.Append(@"
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ThinktectureTypeConverter<").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(_state.KeyMember).Append(", ").AppendTypeFullyQualified(_state.ValidationError).Append(">))]");
      }

      _sb.Append(@"
   [global::System.Diagnostics.CodeAnalysis.SuppressMessage(""ThinktectureRuntimeExtensionsAnalyzer"", ""TTRESG1000:Internal Thinktecture.Runtime.Extensions API usage"")]
   ");

      if (!_state.HasDerivedTypes)
         _sb.Append("sealed ");

      _sb.Append("partial ").AppendTypeKind(_state).Append(" ").Append(_state.Name).Append(@" :
      global::Thinktecture.Internal.IMetadataOwner,");

      if (_state.KeyMember is not null)
      {
         _sb.Append(@"
      global::Thinktecture.ISmartEnum<").AppendTypeFullyQualified(_state.KeyMember).Append(", ").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(_state.ValidationError).Append(">,");

         if (_state.KeyMember.IsString())
         {
            _sb.Append(@"
#if NET9_0_OR_GREATER
      global::Thinktecture.IObjectFactory<").AppendTypeFullyQualified(_state).Append(", global::System.ReadOnlySpan<char>, ").AppendTypeFullyQualified(_state.ValidationError).Append(@">,
#endif");
         }
      }

      _sb.Append(@"
      global::System.IEquatable<").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@">
   {");

      GenerateCustomDelegateTypes();

      if (_state.KeyMember is not null)
      {
         _sb.Append(@"
      static global::Thinktecture.Internal.Metadata global::Thinktecture.Internal.IMetadataOwner.Metadata { get; }
         = new global::Thinktecture.Internal.Metadata.Keyed.SmartEnum(typeof(").AppendTypeFullyQualified(_state).Append(@"))
         {
            KeyType = typeof(").AppendTypeFullyQualified(_state.KeyMember).Append(@"),
            ValidationErrorType = typeof(").AppendTypeFullyQualified(_state.ValidationError).Append(@"),
            Items = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Internal.SmartEnumItemMetadata>>(
                     () => new global::System.Collections.Generic.List<global::Thinktecture.Internal.SmartEnumItemMetadata>(
                        global::System.Linq.Enumerable.Select(").AppendTypeFullyQualified(_state).Append(@".Items, (item, index) =>
                        {
                           string identifier = index switch
                           {");

         for (var i = 0; i < _state.Items.Count; i++)
         {
            var item = _state.Items[i];

            _sb.Append(@"
                              ").Append(i).Append(" => \"").Append(item.Name).Append("\", ");
         }

         _sb.Append(@"
                              _ => throw new global::System.ArgumentOutOfRangeException($""Unknown item at index {index}."")
                           };

                           return new global::Thinktecture.Internal.SmartEnumItemMetadata
                           {
                              Key = item.").Append(_state.KeyMember.Name).Append(@",
                              Item = item,
                              Identifier = identifier
                           };
                        })).AsReadOnly()),
            ConvertToKey = static ").AppendTypeFullyQualified(_state.KeyMember).Append(" (").AppendTypeFullyQualified(_state).Append(" item) => item.").Append(_state.KeyMember.Name).Append(@",
            ConvertToKeyExpression = static ").AppendTypeFullyQualified(_state.KeyMember).Append(" (").AppendTypeFullyQualified(_state).Append(" item) => item.").Append(_state.KeyMember.Name).Append(@",
            GetKey = static object (object item) => ((").AppendTypeFullyQualified(_state).Append(")item).").Append(_state.KeyMember.Name).Append(@",
            ConvertFromKey = static ").AppendTypeFullyQualified(_state).Append(" (").AppendTypeFullyQualified(_state.KeyMember).Append(" key) => ").AppendTypeFullyQualified(_state).Append(@".Get(key),
            ConvertFromKeyExpression = static ").AppendTypeFullyQualified(_state).Append(" (").AppendTypeFullyQualified(_state.KeyMember).Append(" key) => ").AppendTypeFullyQualified(_state).Append(@".Get(key),
            TryGetFromKey =
               (object? key,
                out object? obj,
                [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(true)] out object error) =>
               {
                  error = ").AppendTypeFullyQualified(_state).Append(".Validate(key is ").AppendTypeFullyQualified(_state.KeyMember).Append(@" typedKey ? typedKey : default, null, out var item)!;
                  obj = item;

                  return error is null;
               }
         };

      private static readonly global::System.Lazy<Lookups> _lookups = new global::System.Lazy<Lookups>(GetLookups, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public static global::System.Collections.Generic.IReadOnlyList<").AppendTypeFullyQualified(_state).Append("> Items => _lookups.Value.List;");

         GenerateKeyMember(_sb, _state.KeyMember, true);
      }
      else
      {
         _sb.Append(@"
      static global::Thinktecture.Internal.Metadata global::Thinktecture.Internal.IMetadataOwner.Metadata { get; }
         = new global::Thinktecture.Internal.Metadata.KeylessSmartEnum(typeof(").AppendTypeFullyQualified(_state).Append(@"))
         {
            Items = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Internal.KeylessSmartEnumItemMetadata>>(
                     () => new global::System.Collections.Generic.List<global::Thinktecture.Internal.KeylessSmartEnumItemMetadata>(
                        global::System.Linq.Enumerable.Select(").AppendTypeFullyQualified(_state).Append(@".Items, (item, index) =>
                        {
                           string identifier = index switch
                           {");

         for (var i = 0; i < _state.Items.Count; i++)
         {
            var item = _state.Items[i];

            _sb.Append(@"
                              ").Append(i).Append(" => \"").Append(item.Name).Append("\", ");
         }

         _sb.Append(@"
                              _ => throw new global::System.ArgumentOutOfRangeException($""Unknown item at index {index}."")
                           };

                           return new global::Thinktecture.Internal.KeylessSmartEnumItemMetadata
                           {
                              Item = item,
                              Identifier = identifier
                           };
                        })).AsReadOnly()),
         };

      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<").AppendTypeFullyQualified(_state).Append(@">> _items
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<").AppendTypeFullyQualified(_state).Append(@">>(GetItems, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public static global::System.Collections.Generic.IReadOnlyList<").AppendTypeFullyQualified(_state).Append("> Items => _items.Value;");
      }

      _sb.Append(@"

      private readonly int _hashCode;
      private readonly global::System.Lazy<int> _itemIndex;");

      cancellationToken.ThrowIfCancellationRequested();

      foreach (var method in _state.DelegateMethods)
      {
         _sb.Append(@"
      private readonly ").AppendDelegateType(method).Append(" _").Append(method.ArgumentName).Append(";");
      }

      GenerateConstructors();

      if (_state.KeyMember is not null)
      {
         GenerateToValue(_state.KeyMember);
         GenerateGet(_state.KeyMember);

         if (_state.KeyMember.IsString())
            GenerateGetForReadOnlySpanOfChar(_state.KeyMember);

         cancellationToken.ThrowIfCancellationRequested();

         GenerateTryGet(_state.KeyMember);

         if (_state.KeyMember.IsString())
            GenerateTryGetForReadOnlySpanOfChar(_state.KeyMember);

         GenerateValidate(_state.KeyMember);

         if (_state.KeyMember.IsString())
            GenerateValidateForReadOnlySpanOfChar(_state.KeyMember);

         GenerateConversionToKeyType(_state.KeyMember);
         GenerateConversionFromKeyType(_state.KeyMember);
      }

      GenerateEquals();

      cancellationToken.ThrowIfCancellationRequested();

      _sb.Append(@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {
         return other is ").AppendTypeFullyQualified(_state).Append(@" item && Equals(item);
      }

      /// <inheritdoc />
      public override int GetHashCode()
      {
         return _hashCode;
      }");

      if (_state.KeyMember is not null && !_state.Settings.SkipToString)
         GenerateToString(_state.KeyMember);

      GenerateDelegatedMethods();

      var hasSaneNumberOfItems = _state.Items.Count < 1000;

      if (_state.Settings.SwitchMethods != SwitchMapMethodsGeneration.None && hasSaneNumberOfItems)
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

      if (_state.Settings.MapMethods != SwitchMapMethodsGeneration.None && hasSaneNumberOfItems)
      {
         GenerateMap(false);

         if (_state.Settings.MapMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateMap(true);
      }

      if (_state.KeyMember is not null)
      {
         GenerateGetLookups(_state.KeyMember);
      }
      else
      {
         GenerateGetItems();
      }

      _sb.Append(@"
   }");
   }

   private void GenerateCustomDelegateTypes()
   {
      for (var i = 0; i < _state.DelegateMethods.Count; i++)
      {
         var method = _state.DelegateMethods[i];

         if (!method.NeedsCustomDelegate())
            continue;

         _sb.Append(@"
      private delegate ").Append(method.ReturnType ?? "void").Append(" ").AppendDelegateType(method).Append("(");

         for (var j = 0; j < method.Parameters.Count; j++)
         {
            if (j > 0)
               _sb.Append(", ");

            var param = method.Parameters[j];

            _sb.AppendRefKindParameterPrefix(param.RefKind).Append(param.Type).Append(" ").Append(param.Name);
         }

         _sb.Append(");");
      }

      if (_state.DelegateMethods.Count > 0)
         _sb.AppendLine();
   }

   private void GenerateDelegatedMethods()
   {
      foreach (var method in _state.DelegateMethods)
      {
         _sb.Append(@"

      ").AppendAccessibility(method.Accessibility).Append(" partial ").Append(method.ReturnType ?? "void").Append(" ").Append(method.MethodName).Append("(");

         for (var i = 0; i < method.Parameters.Count; i++)
         {
            if (i > 0)
               _sb.Append(", ");

            var param = method.Parameters[i];
            _sb.AppendRefKindParameterPrefix(param.RefKind).Append(param.Type).Append(" ").Append(param.Name);
         }

         _sb.Append(@")
      {
         ");

         if (method.ReturnType != null)
            _sb.Append("return ");

         _sb.Append("_").Append(method.ArgumentName).Append("(");

         for (var i = 0; i < method.Parameters.Count; i++)
         {
            if (i > 0)
               _sb.Append(", ");

            var param = method.Parameters[i];
            _sb.AppendRefKindArgumentPrefix(param.RefKind).Append(param.Name);
         }

         _sb.Append(@");
      }");
      }
   }

   private void GenerateToString(KeyMemberState keyProperty)
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override string").Append(keyProperty.IsToStringReturnTypeNullable ? "?" : null).Append(@" ToString()
      {
         return this.").Append(keyProperty.Name).Append(@".ToString();
      }");
   }

   private void GenerateSwitchForAction(bool withState, bool isPartially)
   {
      if (_state.Items.Count == 0)
         return;

      _sb.Append(@"

#pragma warning disable CS0436 // InstantHandleAttribute may come from a different assembly
      /// <summary>
      /// Executes an action depending on the current item.
      /// </summary>");

      if (withState)
      {
         _sb.Append(@"
      /// <param name=""").Append(_state.Settings.SwitchMapStateParameterName).Append(@""">State to be passed to the callbacks.</param>");
      }

      if (isPartially)
      {
         _sb.Append(@"
      /// <param name=""default"">The action to execute if no item-specific action is provided.</param>");
      }

      for (var i = 0; i < _state.Items.Count; i++)
      {
         _sb.Append(@"
      /// <param name=""");

         var item = _state.Items[i];
         _sb.Append(item.ArgumentName);
         _sb.Append(@""">The action to execute if the current item is equal to <see cref=""").Append(item.Name).Append(@"""/>.</param>");
      }

      _sb.Append(@"
      [global::System.Diagnostics.DebuggerStepThroughAttribute]
      public void ").Append(isPartially ? "SwitchPartially" : "Switch");

      if (withState)
      {
         _sb.Append(@"<TState>(
         TState ").AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(",");
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

         _sb.AppendTypeFullyQualified(_state).Append(">? @default = null,");
      }

      for (var i = 0; i < _state.Items.Count; i++)
      {
         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         [global::JetBrains.Annotations.InstantHandleAttribute] global::System.Action");

         if (withState)
            _sb.Append("<TState>");

         if (isPartially)
            _sb.Append('?');

         _sb.Append(' ').AppendEscaped(_state.Items[i].ArgumentName);

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
         switch (_itemIndex.Value)
         {");

      for (var i = 0; i < _state.Items.Count; i++)
      {
         _sb.Append(@"
            case ").Append(i).Append(":");

         if (isPartially)
         {
            _sb.Append(@"
               if (").AppendEscaped(_state.Items[i].ArgumentName).Append(@" is null)
                  break;
");
         }

         _sb.Append(@"
               ").AppendEscaped(_state.Items[i].ArgumentName).Append("(");

         if (withState)
            _sb.AppendEscaped(_state.Settings.SwitchMapStateParameterName);

         _sb.Append(@");
               return;");
      }

      _sb.Append(@"
            default:
               throw new global::System.ArgumentOutOfRangeException($""Unknown item '{this}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         @default?.Invoke(");

         if (withState)
            _sb.AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(", ");

         _sb.Append("this);");
      }
   }

   private void GenerateSwitchForFunc(bool withState, bool isPartially)
   {
      if (_state.Items.Count == 0)
         return;

      _sb.Append(@"

#pragma warning disable CS0436 // InstantHandleAttribute may come from a different assembly
      /// <summary>
      /// Executes a function depending on the current item.
      /// </summary>");

      if (withState)
      {
         _sb.Append(@"
      /// <param name=""").Append(_state.Settings.SwitchMapStateParameterName).Append(@""">State to be passed to the callbacks.</param>");
      }

      if (isPartially)
      {
         _sb.Append(@"
      /// <param name=""default"">The function to execute if no item-specific action is provided.</param>");
      }

      for (var i = 0; i < _state.Items.Count; i++)
      {
         _sb.Append(@"
      /// <param name=""");

         var item = _state.Items[i];
         _sb.Append(item.ArgumentName);
         _sb.Append(@""">The function to execute if the current item is equal to <see cref=""").Append(item.Name).Append(@"""/>.</param>");
      }

      _sb.Append(@"
      [global::System.Diagnostics.DebuggerStepThroughAttribute]
      public TResult ").Append(isPartially ? "SwitchPartially" : "Switch");

      if (withState)
      {
         _sb.Append(@"<TState, TResult>(
         TState ").AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(",");
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

         _sb.AppendTypeFullyQualified(_state).Append(", TResult> @default,");
      }

      for (var i = 0; i < _state.Items.Count; i++)
      {
         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         ");

         _sb.Append("[global::JetBrains.Annotations.InstantHandleAttribute] global::System.Func<");

         if (withState)
            _sb.Append("TState, ");

         _sb.Append("TResult>");

         if (isPartially)
            _sb.Append('?');

         _sb.Append(' ').AppendEscaped(_state.Items[i].ArgumentName);

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
         switch (_itemIndex.Value)
         {");

      for (var i = 0; i < _state.Items.Count; i++)
      {
         _sb.Append(@"
            case ").Append(i).Append(":");

         if (isPartially)
         {
            _sb.Append(@"
               if (").AppendEscaped(_state.Items[i].ArgumentName).Append(@" is null)
                  break;
");
         }

         _sb.Append(@"
               return ").AppendEscaped(_state.Items[i].ArgumentName).Append("(");

         if (withState)
            _sb.AppendEscaped(_state.Settings.SwitchMapStateParameterName);

         _sb.Append(");");
      }

      _sb.Append(@"
            default:
               throw new global::System.ArgumentOutOfRangeException($""Unknown item '{this}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         return @default(");

         if (withState)
            _sb.AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(", ");

         _sb.Append("this);");
      }
   }

   private void GenerateMap(bool isPartially)
   {
      if (_state.Items.Count == 0)
         return;

      _sb.Append(@"

      /// <summary>
      /// Maps an item to an instance of type <typeparamref name=""TResult""/>.
      /// </summary>");

      if (isPartially)
      {
         _sb.Append(@"
      /// <param name=""default"">The instance to return if no value is provided for current item.</param>");
      }

      for (var i = 0; i < _state.Items.Count; i++)
      {
         var item = _state.Items[i];

         _sb.Append(@"
      /// <param name=""").Append(item.ArgumentName).Append(@""">The instance to return if the current item is equal to <see cref=""").Append(item.Name).Append(@"""/>.</param>");
      }

      _sb.Append(@"
      [global::System.Diagnostics.DebuggerStepThroughAttribute]
      public TResult ").Append(isPartially ? "MapPartially" : "Map").Append("<TResult>(");

      if (isPartially)
      {
         _sb.Append(@"
         TResult @default,");
      }

      for (var i = 0; i < _state.Items.Count; i++)
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

         _sb.Append(" ").AppendEscaped(_state.Items[i].ArgumentName);

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
         switch (_itemIndex.Value)
         {");

      for (var i = 0; i < _state.Items.Count; i++)
      {
         _sb.Append(@"
            case ").Append(i).Append(":");

         if (isPartially)
         {
            _sb.Append(@"
               if (!").AppendEscaped(_state.Items[i].ArgumentName).Append(@".IsSet)
                  break;
");
         }

         _sb.Append(@"
               return ").AppendEscaped(_state.Items[i].ArgumentName);

         if (isPartially)
            _sb.Append(".Value");

         _sb.Append(";");
      }

      _sb.Append(@"
            default:
               throw new global::System.ArgumentOutOfRangeException($""Unknown item '{this}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         return @default;");
      }
   }

   private void GenerateTryGet(IMemberState keyProperty)
   {
      _sb.Append(@"

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> if a valid item exists.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName).Append(@""">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">An instance of ").AppendTypeForXmlComment(_state).Append(@".</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] ").AppendTypeFullyQualified(keyProperty).Append(" ").AppendEscaped(keyProperty.ArgumentName).Append(", [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ").AppendTypeFullyQualified(_state).Append(@" item)
      {");

      if (keyProperty.IsReferenceType)
      {
         _sb.Append(@"
         if (").AppendEscaped(keyProperty.ArgumentName).Append(@" is null)
         {
            item = default;
            return false;
         }
");
      }

      _sb.Append(@"
         return _lookups.Value.Lookup.TryGetValue(").AppendEscaped(keyProperty.ArgumentName).Append(", out item);");

      _sb.Append(@"
      }");
   }

   private void GenerateTryGetForReadOnlySpanOfChar(IMemberState keyProperty)
   {
      _sb.Append(@"

#if NET9_0_OR_GREATER
      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> if a valid item exists.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName).Append(@""">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">An instance of ").AppendTypeForXmlComment(_state).Append(@".</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet(global::System.ReadOnlySpan<char> ").AppendEscaped(keyProperty.ArgumentName).Append(", [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ").AppendTypeFullyQualified(_state).Append(@" item)
      {
         return _lookups.Value.AlternateLookup.TryGetValue(").AppendEscaped(keyProperty.ArgumentName).Append(@", out item);
      }
#endif");
   }

   private void GenerateValidate(IMemberState keyProperty)
   {
      var providerArgumentName = keyProperty.ArgumentName == "provider" ? "formatProvider" : "provider";

      _sb.Append(@"

      /// <summary>
      /// Validates the provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName).Append(@""">The identifier to return an enumeration item for.</param>
      /// <param name=""").Append(providerArgumentName).Append(@""">An object that provides culture-specific formatting information.</param>
      /// <param name=""item"">An instance of ").AppendTypeForXmlComment(_state).Append(@".</param>
      /// <returns><c>null</c> if a valid item with provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> exists; ").AppendTypeForXmlComment(_state.ValidationError).Append(@" with an error message otherwise.</returns>
      public static ").AppendTypeFullyQualified(_state.ValidationError).Append("? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] ").AppendTypeFullyQualified(keyProperty).Append(" ").AppendEscaped(keyProperty.ArgumentName).Append(", global::System.IFormatProvider? ").AppendEscaped(providerArgumentName).Append(", [global::System.Diagnostics.CodeAnalysis.MaybeNull] out ").AppendTypeFullyQualified(_state).Append(@" item)
      {
         if(").AppendTypeFullyQualified(_state).Append(".TryGet(").AppendEscaped(keyProperty.ArgumentName).Append(@", out item))
         {
            return null;
         }
         else
         {
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<").AppendTypeFullyQualified(_state.ValidationError).Append(@">($""There is no item of type '").AppendTypeMinimallyQualified(_state).Append("' with the identifier '{").AppendEscaped(keyProperty.ArgumentName).Append(@"}'."");
         }
      }");
   }

   private void GenerateValidateForReadOnlySpanOfChar(IMemberState keyProperty)
   {
      var providerArgumentName = keyProperty.ArgumentName == "provider" ? "formatProvider" : "provider";

      _sb.Append(@"

#if NET9_0_OR_GREATER
      /// <summary>
      /// Validates the provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName).Append(@""">The identifier to return an enumeration item for.</param>
      /// <param name=""").Append(providerArgumentName).Append(@""">An object that provides culture-specific formatting information.</param>
      /// <param name=""item"">An instance of ").AppendTypeForXmlComment(_state).Append(@".</param>
      /// <returns><c>null</c> if a valid item with provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> exists; ").AppendTypeForXmlComment(_state.ValidationError).Append(@" with an error message otherwise.</returns>
      public static ").AppendTypeFullyQualified(_state.ValidationError).Append("? Validate(global::System.ReadOnlySpan<char> ").AppendEscaped(keyProperty.ArgumentName).Append(", global::System.IFormatProvider? ").AppendEscaped(providerArgumentName).Append(", [global::System.Diagnostics.CodeAnalysis.MaybeNull] out ").AppendTypeFullyQualified(_state).Append(@" item)
      {
         if(").AppendTypeFullyQualified(_state).Append(".TryGet(").AppendEscaped(keyProperty.ArgumentName).Append(@", out item))
         {
            return null;
         }
         else
         {
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<").AppendTypeFullyQualified(_state.ValidationError).Append(@">($""There is no item of type '").AppendTypeMinimallyQualified(_state).Append("' with the identifier '{").AppendEscaped(keyProperty.ArgumentName).Append(@"}'."");
         }
      }
#endif");
   }

   private void GenerateConversionToKeyType(KeyMemberState keyProperty)
   {
      if (keyProperty.IsInterface
          || keyProperty.SpecialType == SpecialType.System_Object
          || _state.Settings.ConversionToKeyMemberType == ConversionOperatorsGeneration.None)
         return;

      _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.ConversionToKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion to the type ").AppendTypeForXmlComment(keyProperty).Append(@".
      /// </summary>
      /// <param name=""item"">Item to covert.</param>
      /// <returns>The ").AppendTypeForXmlComment(_state, (keyProperty.Name, ".")).Append(@" of provided <paramref name=""item""/> or <c>default</c> if <paramref name=""item""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""item"")]
      public static ").AppendConversionOperator(_state.Settings.ConversionToKeyMemberType).Append(" operator ").AppendTypeFullyQualifiedNullAnnotated(keyProperty).Append("(").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" item)
      {
         return item is null ? default : item.").Append(keyProperty.Name).Append(@";
      }");
   }

   private void GenerateConversionFromKeyType(KeyMemberState keyProperty)
   {
      if (keyProperty.IsInterface
          || keyProperty.SpecialType == SpecialType.System_Object
          || _state.Settings.ConversionFromKeyMemberType == ConversionOperatorsGeneration.None)
         return;

      _sb.Append(@"

      /// <summary>
      /// ").Append(_state.Settings.ConversionFromKeyMemberType == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion from the type ").AppendTypeForXmlComment(keyProperty).Append(@".
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName).Append(@""">Value to covert.</param>
      /// <returns>An instance of ").AppendTypeForXmlComment(_state).Append(@" if the <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> is a known item.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").Append(keyProperty.ArgumentName).Append(@""")]
      public static ").AppendConversionOperator(_state.Settings.ConversionFromKeyMemberType).Append(" operator ").AppendTypeFullyQualifiedNullAnnotated(_state).Append("(").AppendTypeFullyQualifiedNullAnnotated(keyProperty).Append(" ").AppendEscaped(keyProperty.ArgumentName).Append(@")
      {
         return ").AppendTypeFullyQualified(_state).Append(".").Append(Constants.Methods.GET).Append("(").AppendEscaped(keyProperty.ArgumentName).Append(@");
      }");
   }

   private void GenerateEquals()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public bool Equals(").AppendTypeFullyQualifiedNullAnnotated(_state).Append(@" other)
      {
         return global::System.Object.ReferenceEquals(this, other);
      }");
   }

   private void GenerateGetLookups(KeyMemberState keyMember)
   {
      var totalNumberOfItems = _state.Items.Count;

      _sb.Append(@"

      private static Lookups GetLookups()
      {
         var lookup = new global::System.Collections.Generic.Dictionary<").AppendTypeFullyQualified(keyMember).Append(", ").AppendTypeFullyQualified(_state).Append(">(").Append(totalNumberOfItems);

      if (_state.Settings.KeyMemberEqualityComparerAccessor is not null)
      {
         _sb.Append(", ").Append(_state.Settings.KeyMemberEqualityComparerAccessor).Append(".EqualityComparer");
      }
      else if (keyMember.IsString())
      {
         _sb.Append(", global::System.StringComparer.OrdinalIgnoreCase");
      }

      _sb.Append(@");
         var list = new global::System.Collections.Generic.List<").AppendTypeFullyQualified(_state).Append(">(").Append(totalNumberOfItems).Append(");");

      if (_state.Items.Count > 0)
      {
         _sb.Append(@"

         void AddItem(").AppendTypeFullyQualified(_state).Append(@" item, string itemName)
         {
            if (item is null)
               throw new global::System.ArgumentNullException($""The item \""{itemName}\"" of type \""").AppendTypeMinimallyQualified(_state).Append(@"\"" must not be null."");
");

         if (keyMember.IsReferenceType)
         {
            _sb.Append(@"
            if (item.").Append(keyMember.Name).Append(@" is null)
               throw new global::System.ArgumentException($""The \""").Append(keyMember.Name).Append(@"\"" of the item \""{itemName}\"" of type \""").AppendTypeMinimallyQualified(_state).Append(@"\"" must not be null."");
");
         }

         _sb.Append(@"
            if (lookup.ContainsKey(item.").Append(keyMember.Name).Append(@"))
               throw new global::System.ArgumentException($""The type \""").AppendTypeMinimallyQualified(_state).Append(@"\"" has multiple items with the identifier \""{item.").Append(keyMember.Name).Append(@"}\""."");

            lookup.Add(item.").Append(keyMember.Name).Append(@", item);
            list.Add(item);
         }
");

         for (var i = 0; i < _state.Items.Count; i++)
         {
            var itemName = _state.Items[i].Name;

            _sb.Append(@"
         AddItem(@").Append(itemName).Append(", nameof(@").Append(itemName).Append("));");
         }
      }

      _sb.Append(@"

#if NET8_0_OR_GREATER
         var frozenDictionary = global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup");

      if (_state.Settings.KeyMemberEqualityComparerAccessor is not null)
      {
         _sb.Append(", ").Append(_state.Settings.KeyMemberEqualityComparerAccessor).Append(".EqualityComparer");
      }
      else if (keyMember.IsString())
      {
         _sb.Append(", global::System.StringComparer.OrdinalIgnoreCase");
      }

      _sb.Append(");");

      if (keyMember.IsString())
      {
         _sb.Append(@"
#if NET9_0_OR_GREATER
         return new Lookups(frozenDictionary, frozenDictionary.GetAlternateLookup<global::System.ReadOnlySpan<char>>(), list.AsReadOnly());
#else
         return new Lookups(frozenDictionary, list.AsReadOnly());
#endif");
      }
      else
      {
         _sb.Append(@"
         return new Lookups(frozenDictionary, list.AsReadOnly());");
      }

      _sb.Append(@"
#else
         return new Lookups(lookup, list.AsReadOnly());
#endif
      }

      private readonly record struct Lookups(
         global::System.Collections.Generic.IReadOnlyDictionary<").AppendTypeFullyQualified(keyMember).Append(", ").AppendTypeFullyQualified(_state).Append("> Lookup,");

      if (keyMember.IsString())
      {
         _sb.Append(@"
#if NET9_0_OR_GREATER
         global::System.Collections.Frozen.FrozenDictionary<string, ").AppendTypeFullyQualified(_state).Append(">.AlternateLookup<global::System.ReadOnlySpan<char>>").Append(@" AlternateLookup,
#endif");
      }

      _sb.Append(@"
         global::System.Collections.Generic.IReadOnlyList<").AppendTypeFullyQualified(_state).Append("> List);");
   }

   private void GenerateGetItems()
   {
      var totalNumberOfItems = _state.Items.Count;

      _sb.Append(@"

      private static global::System.Collections.Generic.IReadOnlyList<").AppendTypeFullyQualified(_state).Append(@"> GetItems()
      {
         var list = new global::System.Collections.Generic.List<").AppendTypeFullyQualified(_state).Append(">(").Append(totalNumberOfItems).Append(");");

      if (_state.Items.Count > 0)
      {
         _sb.Append(@"

         void AddItem(").AppendTypeFullyQualified(_state).Append(@" item, string itemName)
         {
            if (item is null)
               throw new global::System.ArgumentNullException($""The item \""{itemName}\"" of type \""").AppendTypeMinimallyQualified(_state).Append(@"\"" must not be null."");

            list.Add(item);
         }
");

         for (var i = 0; i < _state.Items.Count; i++)
         {
            var itemName = _state.Items[i].Name;

            _sb.Append(@"
         AddItem(@").Append(itemName).Append(", nameof(@").Append(itemName).Append("));");
         }
      }

      _sb.Append(@"

         return list.AsReadOnly();
      }");
   }

   private void GenerateEnsureValid(IMemberState keyProperty)
   {
      _sb.Append(@"

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref=""System.InvalidOperationException"">The enumeration item is not valid.</exception>
      public void EnsureValid()
      {
         if (!IsValid)
            throw new global::System.InvalidOperationException($""The current enumeration item of type \""").Append(_state.Name).Append(@"\"" with identifier \""{this.").Append(keyProperty.Name).Append(@"}\"" is not valid."");
      }");
   }

   private void GenerateToValue(IMemberState keyProperty)
   {
      _sb.Append(@"

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      ").AppendTypeFullyQualified(keyProperty).Append(" global::Thinktecture.IConvertible<").AppendTypeFullyQualified(keyProperty).Append(@">.ToValue()
      {
         return this.").Append(keyProperty.Name).Append(@";
      }");
   }

   private void GenerateGet(IMemberState keyProperty)
   {
      _sb.Append(@"

      /// <summary>
      /// Gets an enumeration item for provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/>.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName).Append(@""">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of ").AppendTypeForXmlComment(_state).Append(@" if <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> is not <c>null</c>; otherwise <c>null</c>.</returns>
      /// <exception cref=""Thinktecture.UnknownSmartEnumIdentifierException"">If there is no item with the provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/>.</exception>");

      if (keyProperty.IsReferenceType)
      {
         _sb.Append(@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").Append(keyProperty.ArgumentName).Append(@""")]");
      }

      _sb.Append(@"
      public static ").AppendTypeFullyQualified(_state, nullable: keyProperty.IsReferenceType).Append(" ").Append(Constants.Methods.GET).Append("(").AppendTypeFullyQualifiedNullAnnotated(keyProperty).Append(" ").AppendEscaped(keyProperty.ArgumentName).Append(@")
      {");

      if (keyProperty.IsReferenceType)
      {
         _sb.Append(@"
         if (").AppendEscaped(keyProperty.ArgumentName).Append(@" is null)
            return default;
");
      }

      _sb.Append(@"
         if (!_lookups.Value.Lookup.TryGetValue(").AppendEscaped(keyProperty.ArgumentName).Append(@", out var item))
         {
            throw new global::Thinktecture.UnknownSmartEnumIdentifierException(typeof(").AppendTypeFullyQualified(_state).Append("), ").AppendEscaped(keyProperty.ArgumentName).Append(@");
         }

         return item;
      }");
   }

   private void GenerateGetForReadOnlySpanOfChar(IMemberState keyProperty)
   {
      _sb.Append(@"

#if NET9_0_OR_GREATER
      /// <summary>
      /// Gets an enumeration item for provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/>.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName).Append(@""">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of ").AppendTypeForXmlComment(_state).Append(@" if <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/> is not <c>null</c>; otherwise <c>null</c>.</returns>
      /// <exception cref=""Thinktecture.UnknownSmartEnumIdentifierException"">If there is no item with the provided <paramref name=""").Append(keyProperty.ArgumentName).Append(@"""/>.</exception>
      public static ").AppendTypeFullyQualified(_state).Append(" ").Append(Constants.Methods.GET).Append("(global::System.ReadOnlySpan<char> ").AppendEscaped(keyProperty.ArgumentName).Append(@")
      {
         if (!_lookups.Value.AlternateLookup.TryGetValue(").AppendEscaped(keyProperty.ArgumentName).Append(@", out var item))
         {
            throw new global::Thinktecture.UnknownSmartEnumIdentifierException(typeof(").AppendTypeFullyQualified(_state).Append("), ").AppendEscaped(keyProperty.ArgumentName).Append(@".ToString());
         }

         return item;
      }
#endif");
   }

   private void GenerateConstructors()
   {
      var ownCtorArgs = _state.AssignableInstanceFieldsAndProperties
                              .Where(p => !p.IsAbstract)
                              .Select(a => new ConstructorArgument(a.TypeFullyQualified, a.ArgumentName))
                              .ToList();

      if (_state.BaseType is null)
      {
         GenerateConstructor(ownCtorArgs, Array.Empty<ConstructorArgument>());
         return;
      }

      var baseCtorArgs = _state.BaseType.Constructors
                               .Select(ctor =>
                               {
                                  if (ctor.Arguments.Length == 0)
                                     return (IReadOnlyList<ConstructorArgument>) [];

                                  return ctor.Arguments
                                             .Select(a =>
                                             {
                                                var argName = a.ArgumentName;
                                                var counter = 0;

                                                while (_state.KeyMember?.ArgumentName == argName || ContainsArgument(ownCtorArgs, argName))
                                                {
                                                   counter++;
                                                   argName = $"{a.ArgumentName}{counter.ToString()}"; // rename the argument name if it collides with another argument
                                                }

                                                return new ConstructorArgument(a.TypeFullyQualified, argName);
                                             }).ToList();
                               })
                               .Distinct(ConstructorArgumentsComparer.Instance)
                               .ToList();

      foreach (var baseArgs in baseCtorArgs)
      {
         GenerateConstructor(ownCtorArgs.Concat(baseArgs).ToList(), baseArgs);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static bool ContainsArgument(List<ConstructorArgument> ownCtorArgs, string argName)
   {
      for (var i = 0; i < ownCtorArgs.Count; i++)
      {
         if (ownCtorArgs[i].ArgumentName == argName)
            return true;
      }

      return false;
   }

   private void GenerateConstructor(
      IReadOnlyList<ConstructorArgument> ctorArgs,
      IReadOnlyList<ConstructorArgument> baseCtorArgs)
   {
      bool hasArguments;

      _sb.Append(@"

      private ").Append(_state.Name).Append("(");

      if (_state.KeyMember is not null)
      {
         hasArguments = true;
         _sb.Append(@"
         ").AppendTypeFullyQualified(_state.KeyMember).Append(" ").AppendEscaped(_state.KeyMember.ArgumentName);
      }
      else
      {
         hasArguments = false;
      }

      for (var i = 0; i < ctorArgs.Count; i++)
      {
         if (hasArguments || i != 0)
            _sb.Append(",");

         var member = ctorArgs[i];
         hasArguments = true;

         _sb.Append(@"
         ").AppendTypeFullyQualified(member).Append(" ").AppendEscaped(member.ArgumentName);
      }

      if (_state.DelegateMethods.Count > 0)
      {
         for (var i = 0; i < _state.DelegateMethods.Count; i++)
         {
            var method = _state.DelegateMethods[i];

            if (hasArguments || i != 0)
               _sb.Append(",");

            _sb.Append(@"
         ").AppendDelegateType(method).Append(" ").AppendEscaped(method.ArgumentName);
         }
      }

      _sb.Append(")");

      if (baseCtorArgs.Count > 0)
      {
         _sb.Append(@"
         : base(");

         for (var i = 0; i < baseCtorArgs.Count; i++)
         {
            if (i != 0)
               _sb.Append(", ");

            _sb.AppendEscaped(baseCtorArgs[i].ArgumentName);
         }

         _sb.Append(")");
      }

      _sb.Append(@"
      {");

      GeneratedValidateConstructorArgumentsCall(ctorArgs);

      _sb.Append(@"
");

      if (_state.KeyMember is not null)
      {
         if (_state.KeyMember.IsReferenceType)
         {
            _sb.Append(@"
         if (").AppendEscaped(_state.KeyMember.ArgumentName).Append(@" is null)
            throw new global::System.ArgumentNullException(nameof(").AppendEscaped(_state.KeyMember.ArgumentName).Append(@"));
");
         }

         _sb.Append(@"
         this.").Append(_state.KeyMember.Name).Append(" = ").AppendEscaped(_state.KeyMember.ArgumentName).Append(";");
      }

      foreach (var memberInfo in _state.AssignableInstanceFieldsAndProperties.Where(p => !p.IsAbstract))
      {
         _sb.Append(@"
         this.").Append(memberInfo.Name).Append(" = ").AppendEscaped(memberInfo.ArgumentName).Append(";");
      }

      if (_state.DelegateMethods.Count > 0)
      {
         foreach (var method in _state.DelegateMethods)
         {
            _sb.Append(@"
         this._").Append(method.ArgumentName).Append(" = ").AppendEscaped(method.ArgumentName).Append(";");
         }
      }

      if (_state.KeyMember is not null)
      {
         _sb.Append(@"
         this._hashCode = global::System.HashCode.Combine(typeof(").AppendTypeFullyQualified(_state).Append("), ");

         if (_state.Settings.KeyMemberEqualityComparerAccessor is not null)
         {
            _sb.Append(_state.Settings.KeyMemberEqualityComparerAccessor).Append(".EqualityComparer.GetHashCode(").AppendEscaped(_state.KeyMember.ArgumentName).Append(")");
         }
         else if (_state.KeyMember.IsString())
         {
            _sb.Append("global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(").AppendEscaped(_state.KeyMember.ArgumentName).Append(")");
         }
         else
         {
            _sb.AppendEscaped(_state.KeyMember.ArgumentName).Append(".GetHashCode()");
         }

         _sb.Append(");");
      }
      else
      {
         _sb.Append(@"
         this._hashCode = base.GetHashCode();");
      }

      _sb.Append(@"
         this._itemIndex = new global::System.Lazy<int>(() =>
                                                        {
                                                           for (var i = 0; i < Items.Count; i++)
                                                           {
                                                              if (this == Items[i])
                                                                 return i;
                                                           }

                                                           throw new global::System.Exception($""Current item '{");

      if (_state.KeyMember is null)
      {
         _sb.Append("this");
      }
      else
      {
         _sb.AppendEscaped(_state.KeyMember.ArgumentName);
      }

      _sb.Append(@"}' not found in 'Items'."");
                                                        }, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);");

      _sb.Append(@"
      }

      static partial void ValidateConstructorArguments(");

      if (_state.KeyMember is not null)
      {
         hasArguments = true;
         _sb.Append(@"
         ref ").AppendTypeFullyQualified(_state.KeyMember).Append(" ").AppendEscaped(_state.KeyMember.ArgumentName);
      }
      else
      {
         hasArguments = false;
      }

      for (var i = 0; i < ctorArgs.Count; i++)
      {
         if (hasArguments || i != 0)
            _sb.Append(",");

         var members = ctorArgs[i];
         hasArguments = true;

         _sb.Append(@"
         ref ").AppendTypeFullyQualified(members).Append(" ").AppendEscaped(members.ArgumentName);
      }

      _sb.Append(");");
   }

   private void GeneratedValidateConstructorArgumentsCall(IReadOnlyList<ConstructorArgument> ctorArgs)
   {
      bool hasArguments;

      _sb.Append(@"
         ValidateConstructorArguments(");

      if (_state.KeyMember is not null)
      {
         hasArguments = true;
         _sb.Append(@"
            ref ").AppendEscaped(_state.KeyMember.ArgumentName);
      }
      else
      {
         hasArguments = false;
      }

      for (var i = 0; i < ctorArgs.Count; i++)
      {
         if (hasArguments || i != 0)
            _sb.Append(",");

         var members = ctorArgs[i];
         hasArguments = true;

         _sb.Append(@"
            ref ").AppendEscaped(members.ArgumentName);
      }

      _sb.Append(");");
   }

   private readonly record struct ConstructorArgument(string TypeFullyQualified, string ArgumentName) : ITypeFullyQualified;

   private sealed class ConstructorArgumentsComparer : IEqualityComparer<IReadOnlyList<ConstructorArgument>>
   {
      public static readonly ConstructorArgumentsComparer Instance = new();

      public bool Equals(IReadOnlyList<ConstructorArgument>? x, IReadOnlyList<ConstructorArgument>? y)
      {
         if (x is null)
            return y is null;

         if (y is null)
            return false;

         if (x.Count != y.Count)
            return false;

         for (var i = 0; i < x.Count; i++)
         {
            var arg = x[i];
            var otherArg = y[i];

            if (arg.TypeFullyQualified != otherArg.TypeFullyQualified)
               return false;
         }

         return true;
      }

      public int GetHashCode(IReadOnlyList<ConstructorArgument> args)
      {
         return args.Count;
      }
   }
}
