using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   private bool NeedsDefaultComparer => !_state.HasKeyComparerImplementation;

   public override string? FileNameSuffix => null;

   public SmartEnumCodeGenerator(EnumSourceGeneratorState state, StringBuilder stringBuilder)
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

      GenerateEnum();

      if (hasNamespace)
      {
         _sb.Append(@"
}");
      }

      _sb.Append(@"
");

      return _sb.ToString();
   }

   private void GenerateEnum()
   {
      var needCreateInvalidImplementation = _state is { IsValidatable: true, HasCreateInvalidImplementation: false };

      var interfaceCodeGenerators = _state.GetInterfaceCodeGenerators();

      _sb.GenerateStructLayoutAttributeIfRequired(_state);

      _sb.Append($@"
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<{_state.TypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}>))]
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name} : global::Thinktecture.IEnum<{_state.KeyProperty.TypeFullyQualified}, {_state.TypeFullyQualified}>, global::System.IEquatable<{_state.TypeFullyQualifiedNullAnnotated}>, global::System.Numerics.IEqualityOperators<{_state.TypeFullyQualified}, {_state.TypeFullyQualified}, bool>");

      for (var i = 0; i < interfaceCodeGenerators.Length; i++)
      {
         _sb.Append(", ");

         interfaceCodeGenerators[i].GenerateBaseTypes(_sb, _state);
      }

      _sb.Append(@"
   {");

      GenerateModuleInitializer(_state.KeyProperty);

      if (NeedsDefaultComparer)
      {
         var defaultComparer = _state.KeyProperty.IsString() ? "global::System.StringComparer.OrdinalIgnoreCase" : $"global::System.Collections.Generic.EqualityComparer<{_state.KeyProperty.TypeFullyQualifiedNullAnnotated}>.Default";

         _sb.Append($@"

      public static global::System.Collections.Generic.IEqualityComparer<{_state.KeyProperty.TypeFullyQualifiedNullAnnotated}> {EnumSourceGeneratorState.KEY_EQUALITY_COMPARER_NAME} => {defaultComparer};");
      }

      _sb.Append($@"

      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<{_state.KeyProperty.TypeFullyQualified}, {_state.TypeFullyQualified}>> _itemsLookup
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<{_state.KeyProperty.TypeFullyQualified}, {_state.TypeFullyQualified}>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<{_state.TypeFullyQualified}>> _items
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<{_state.TypeFullyQualified}>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public static global::System.Collections.Generic.IReadOnlyList<{_state.TypeFullyQualified}> Items => _items.Value;

      /// <summary>
      /// The identifier of the item.
      /// </summary>
      public {_state.KeyProperty.TypeFullyQualified} {_state.KeyProperty.Name} {{ get; }}");

      if (_state.IsValidatable)
      {
         _sb.Append(@"

      /// <inheritdoc />
      public bool IsValid { get; }");

         GenerateEnsureValid();
      }

      _sb.Append(@"

      private readonly int _hashCode;");

      GenerateConstructors();
      GenerateGetKey();
      GenerateGet();

      if (_state.IsValidatable)
         GenerateCreateAndCheckInvalidItem(needCreateInvalidImplementation);

      if (needCreateInvalidImplementation && !_state.IsAbstract)
         GenerateCreateInvalidItem();

      GenerateTryGet();
      GenerateValidate();
      GenerateImplicitConversion();
      GenerateExplicitConversion();
      GenerateEqualityOperators();
      GenerateEquals();

      _sb.Append($@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {{
         return other is {_state.TypeFullyQualified} item && Equals(item);
      }}

      /// <inheritdoc />
      public override int GetHashCode()
      {{
         return _hashCode;
      }}");

      if (!_state.Settings.SkipToString)
         GenerateToString();

      GenerateSwitchForAction(false);
      GenerateSwitchForAction(true);
      GenerateSwitchForFunc(false);
      GenerateSwitchForFunc(true);
      GenerateGetLookup();

      for (var i = 0; i < interfaceCodeGenerators.Length; i++)
      {
         interfaceCodeGenerators[i].GenerateImplementation(_sb, _state, _state.KeyProperty);
      }

      _sb.Append(@"
   }");
   }

   private void GenerateToString()
   {
      _sb.Append($@"

      /// <inheritdoc />
      public override string ToString()
      {{
         return this.{_state.KeyProperty.Name}.ToString();
      }}");
   }

   private void GenerateSwitchForAction(bool withContext)
   {
      if (_state.ItemNames.Count == 0)
         return;

      _sb.Append(@"

      /// <summary>
      /// Executes an action depending on the current item.
      /// </summary>");

      var itemNamePrefix = _state.Name.MakeArgumentName();

      if (withContext)
      {
         _sb.Append(@"
      /// <param name=""context"">Context to be passed to the callbacks.</param>");
      }

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         _sb.Append($@"
      /// <param name=""{itemNamePrefix}{i + 1}"">The item to compare to.</param>
      /// <param name=""{itemNamePrefix}Action{i + 1}"">The action to execute if the current item is equal to <paramref name=""{itemNamePrefix}{i + 1}""/>.</param>");
      }

      if (withContext)
      {
         _sb.Append(@"
      public void Switch<TContext>(
         TContext context,");
      }
      else
      {
         _sb.Append(@"
      public void Switch(");
      }

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         ").Append(_state.Name).Append(" ").Append(itemNamePrefix).Append(i + 1)
            .Append(", global::System.Action");

         if (withContext)
            _sb.Append("<TContext>");

         _sb.Append(' ').Append(itemNamePrefix).Append("Action").Append(i + 1);
      }

      _sb.Append(@")
      {
         ");

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         if (i != 0)
            _sb.Append(@"
         else ");

         _sb.Append("if (this == ").Append(itemNamePrefix).Append(i + 1).Append(@")
         {
            ").Append(itemNamePrefix).Append("Action").Append(i + 1).Append("(");

         if (withContext)
            _sb.Append("context");

         _sb.Append(@");
         }");
      }

      _sb.Append(@"
         else
         {
            throw new global::System.ArgumentOutOfRangeException($""No action provided for the item '{this}'."");
         }
      }");
   }

   private void GenerateSwitchForFunc(bool withContext)
   {
      if (_state.ItemNames.Count == 0)
         return;

      _sb.Append(@"

      /// <summary>
      /// Executes a function depending on the current item.
      /// </summary>");

      if (withContext)
      {
         _sb.Append(@"
      /// <param name=""context"">Context to be passed to the callbacks.</param>");
      }

      var itemNamePrefix = _state.Name.MakeArgumentName();

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         _sb.Append($@"
      /// <param name=""{itemNamePrefix}{i + 1}"">The item to compare to.</param>
      /// <param name=""{itemNamePrefix}Func{i + 1}"">The function to execute if the current item is equal to <paramref name=""{itemNamePrefix}{i + 1}""/>.</param>");
      }

      if (withContext)
      {
         _sb.Append(@"
      public T Switch<TContext, T>(
         TContext context,");
      }
      else
      {
         _sb.Append(@"
      public T Switch<T>(");
      }

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         ").Append(_state.Name).Append(" ").Append(itemNamePrefix).Append(i + 1)
            .Append(", global::System.Func<");

         if (withContext)
            _sb.Append("TContext, ");

         _sb.Append("T> ").Append(itemNamePrefix).Append("Func").Append(i + 1);
      }

      _sb.Append(@")
      {
         ");

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         if (i != 0)
            _sb.Append(@"
         else ");

         _sb.Append("if (this == ").Append(itemNamePrefix).Append(i + 1).Append(@")
         {
            return ").Append(itemNamePrefix).Append("Func").Append(i + 1).Append(@"(");

         if (withContext)
            _sb.Append("context");

         _sb.Append(@");
         }");
      }

      _sb.Append(@"
         else
         {
            throw new global::System.ArgumentOutOfRangeException($""No function provided for the item '{this}'."");
         }
      }");
   }

   private void GenerateModuleInitializer(IMemberState keyMember)
   {
      var enumType = _state.TypeFullyQualified;
      var enumTypeNullAnnotated = _state.TypeFullyQualifiedNullAnnotated;

      _sb.Append($@"
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {{
         var convertFromKey = new global::System.Func<{keyMember.TypeFullyQualifiedNullAnnotated}, {enumTypeNullAnnotated}>({enumType}.Get);
         global::System.Linq.Expressions.Expression<global::System.Func<{keyMember.TypeFullyQualifiedNullAnnotated}, {enumTypeNullAnnotated}>> convertFromKeyExpression = static {keyMember.ArgumentName} => {enumType}.Get({keyMember.ArgumentName});

         var convertToKey = new global::System.Func<{enumType}, {keyMember.TypeFullyQualified}>(static item => item.{keyMember.Name});
         global::System.Linq.Expressions.Expression<global::System.Func<{enumType}, {keyMember.TypeFullyQualified}>> convertToKeyExpression = static item => item.{keyMember.Name};

         var enumType = typeof({enumType});
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof({keyMember.TypeFullyQualified}), true, {(_state.IsValidatable ? "true" : "false")}, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);");

      foreach (var derivedType in _state.FullyQualifiedDerivedTypes)
      {
         _sb.Append($@"
         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(typeof({derivedType}), metadata);");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateTryGet()
   {
      _sb.Append($@"

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name=""{_state.KeyProperty.ArgumentName}""/> if a valid item exists.
      /// </summary>
      /// <param name=""{_state.KeyProperty.ArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""{_state.TypeMinimallyQualified}""/>; otherwise <c>null</c>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name=""{_state.KeyProperty.ArgumentName}""/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] {_state.KeyProperty.TypeFullyQualified} {_state.KeyProperty.ArgumentName}, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out {_state.TypeFullyQualified} item)
      {{");

      if (_state.KeyProperty.IsReferenceType)
      {
         _sb.Append($@"
         if ({_state.KeyProperty.ArgumentName} is null)
         {{
            item = default;
            return false;
         }}
");
      }

      if (_state.IsValidatable)
      {
         _sb.Append($@"
         if(_itemsLookup.Value.TryGetValue({_state.KeyProperty.ArgumentName}, out item))
            return true;

         item = CreateAndCheckInvalidItem({_state.KeyProperty.ArgumentName});
         return false;");
      }
      else
      {
         _sb.Append($@"
         return _itemsLookup.Value.TryGetValue({_state.KeyProperty.ArgumentName}, out item);");
      }

      _sb.Append($@"
      }}");
   }

   private void GenerateValidate()
   {
      _sb.Append($@"

      /// <summary>
      /// Validates the provided <paramref name=""{_state.KeyProperty.ArgumentName}""/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name=""{_state.KeyProperty.ArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""{_state.TypeMinimallyQualified}""/>; otherwise <c>null</c>.</param>
      /// <returns> <see cref=""System.ComponentModel.DataAnnotations.ValidationResult.Success""/> if a valid item with provided <paramref name=""{_state.KeyProperty.ArgumentName}""/> exists; <see cref=""System.ComponentModel.DataAnnotations.ValidationResult""/> with an error message otherwise.</returns>
      public static global::System.ComponentModel.DataAnnotations.ValidationResult? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] {_state.KeyProperty.TypeFullyQualified} {_state.KeyProperty.ArgumentName}, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out {_state.TypeFullyQualified} item)
      {{
         if({_state.TypeFullyQualified}.TryGet({_state.KeyProperty.ArgumentName}, out item))
         {{
            return global::System.ComponentModel.DataAnnotations.ValidationResult.Success;
         }}
         else
         {{");

      if (_state.IsValidatable)
      {
         if (_state.KeyProperty.IsReferenceType)
         {
            _sb.Append($@"
            if({_state.KeyProperty.ArgumentName} is not null)
               item = CreateAndCheckInvalidItem({_state.KeyProperty.ArgumentName});");
         }
         else
         {
            _sb.Append($@"
            item = CreateAndCheckInvalidItem({_state.KeyProperty.ArgumentName});");
         }
      }

      _sb.Append($@"
            return new global::System.ComponentModel.DataAnnotations.ValidationResult($""There is no item of type '{_state.TypeMinimallyQualified}' with the identifier '{{{_state.KeyProperty.ArgumentName}}}'."", global::Thinktecture.SingleItem.Collection(nameof({_state.TypeFullyQualified}.{_state.KeyProperty.Name})));
         }}
      }}");
   }

   private void GenerateEqualityOperators()
   {
      _sb.Append($@"

      /// <summary>
      /// Compares to instances of <see cref=""{_state.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>true</c> if items are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==({_state.TypeFullyQualifiedNullAnnotated} item1, {_state.TypeFullyQualifiedNullAnnotated} item2)
      {{");

      if (_state.IsValidatable)
      {
         if (_state.IsReferenceType)
         {
            _sb.Append(@"
         if (item1 is null)
            return item2 is null;
");
         }

         _sb.Append(@"
         return item1.Equals(item2);");
      }
      else
      {
         _sb.Append(@"
         return global::System.Object.ReferenceEquals(item1, item2);");
      }

      _sb.Append($@"
      }}

      /// <summary>
      /// Compares to instances of <see cref=""{_state.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>false</c> if items are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=({_state.TypeFullyQualifiedNullAnnotated} item1, {_state.TypeFullyQualifiedNullAnnotated} item2)
      {{
         return !(item1 == item2);
      }}");
   }

   private void GenerateImplicitConversion()
   {
      _sb.Append($@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""{_state.KeyProperty.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""item"">Item to covert.</param>
      /// <returns>The <see cref=""{_state.TypeMinimallyQualified}.{_state.KeyProperty.Name}""/> of provided <paramref name=""item""/> or <c>default</c> if <paramref name=""item""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""item"")]
      public static implicit operator {_state.KeyProperty.TypeFullyQualifiedNullAnnotated}({_state.TypeFullyQualifiedNullAnnotated} item)
      {{");

      if (_state.IsReferenceType)
      {
         _sb.Append($@"
         return item is null ? default : item.{_state.KeyProperty.Name};");
      }
      else
      {
         _sb.Append($@"
         return item.{_state.KeyProperty.Name};");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateExplicitConversion()
   {
      _sb.Append($@"

      /// <summary>
      /// Explicit conversion from the type <see cref=""{_state.KeyProperty.TypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""{_state.KeyProperty.ArgumentName}"">Value to covert.</param>
      /// <returns>An instance of <see cref=""{_state.TypeMinimallyQualified}""/> if the <paramref name=""{_state.KeyProperty.ArgumentName}""/> is a known item or implements <see cref=""Thinktecture.IValidatableEnum{{TKey}}""/>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{_state.KeyProperty.ArgumentName}"")]
      public static explicit operator {_state.TypeFullyQualifiedNullAnnotated}({_state.KeyProperty.TypeFullyQualifiedNullAnnotated} {_state.KeyProperty.ArgumentName})
      {{
         return {_state.TypeFullyQualified}.Get({_state.KeyProperty.ArgumentName});
      }}");
   }

   private void GenerateEquals()
   {
      _sb.Append($@"

      /// <inheritdoc />
      public bool Equals({_state.TypeFullyQualifiedNullAnnotated} other)
      {{");

      if (_state.IsValidatable)
      {
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

         _sb.Append(@"
         if (this.IsValid != other.IsValid)
            return false;
");

         _sb.Append($@"
         return {EnumSourceGeneratorState.KEY_EQUALITY_COMPARER_NAME}.Equals(this.{_state.KeyProperty.Name}, other.{_state.KeyProperty.Name});");
      }
      else
      {
         _sb.Append(@"
         return global::System.Object.ReferenceEquals(this, other);");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateGetLookup()
   {
      var totalNumberOfItems = _state.ItemNames.Count;

      _sb.Append($@"

      private static global::System.Collections.Generic.IReadOnlyDictionary<{_state.KeyProperty.TypeFullyQualified}, {_state.TypeFullyQualified}> GetLookup()
      {{
         var lookup = new global::System.Collections.Generic.Dictionary<{_state.KeyProperty.TypeFullyQualified}, {_state.TypeFullyQualified}>({totalNumberOfItems}, {EnumSourceGeneratorState.KEY_EQUALITY_COMPARER_NAME});");

      if (_state.ItemNames.Count > 0)
      {
         _sb.Append($@"

         void AddItem({_state.TypeFullyQualified} item, string itemName)
         {{");

         if (_state.IsReferenceType)
         {
            _sb.Append($@"
            if (item is null)
               throw new global::System.ArgumentNullException($""The item \""{{itemName}}\"" of type \""{_state.TypeMinimallyQualified}\"" must not be null."");
");
         }

         if (_state.KeyProperty.IsReferenceType)
         {
            _sb.Append($@"
            if (item.{_state.KeyProperty.Name} is null)
               throw new global::System.ArgumentException($""The \""{_state.KeyProperty.Name}\"" of the item \""{{itemName}}\"" of type \""{_state.TypeMinimallyQualified}\"" must not be null."");
");
         }

         if (_state.IsValidatable)
         {
            _sb.Append($@"
            if (!item.IsValid)
               throw new global::System.ArgumentException($""All \""public static readonly\"" fields of type \""{_state.TypeMinimallyQualified}\"" must be valid but the item \""{{itemName}}\"" with the identifier \""{{item.{_state.KeyProperty.Name}}}\"" is not."");
");
         }

         _sb.Append($@"
            if (lookup.ContainsKey(item.{_state.KeyProperty.Name}))
               throw new global::System.ArgumentException($""The type \""{_state.TypeMinimallyQualified}\"" has multiple items with the identifier \""{{item.{_state.KeyProperty.Name}}}\""."");

            lookup.Add(item.{_state.KeyProperty.Name}, item);
         }}
");

         foreach (var itemName in _state.ItemNames)
         {
            _sb.Append($@"
         AddItem({itemName}, nameof({itemName}));");
         }
      }

      _sb.Append(@"

         return lookup;
      }");
   }

   private void GenerateEnsureValid()
   {
      _sb.Append($@"

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref=""System.InvalidOperationException"">The enumeration item is not valid.</exception>
      public void EnsureValid()
      {{
         if (!IsValid)
            throw new global::System.InvalidOperationException($""The current enumeration item of type \""{_state.Name}\"" with identifier \""{{this.{_state.KeyProperty.Name}}}\"" is not valid."");
      }}");
   }

   private void GenerateGetKey()
   {
      _sb.Append($@"

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      {_state.KeyProperty.TypeFullyQualified} global::Thinktecture.IKeyedValueObject<{_state.KeyProperty.TypeFullyQualified}>.GetKey()
      {{
         return this.{_state.KeyProperty.Name};
      }}");
   }

   private void GenerateGet()
   {
      _sb.Append($@"

      /// <summary>
      /// Gets an enumeration item for provided <paramref name=""{_state.KeyProperty.ArgumentName}""/>.
      /// </summary>
      /// <param name=""{_state.KeyProperty.ArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref=""{_state.TypeMinimallyQualified}"" /> if <paramref name=""{_state.KeyProperty.ArgumentName}""/> is not <c>null</c>; otherwise <c>null</c>.</returns>");

      if (!_state.IsValidatable)
      {
         _sb.Append($@"
      /// <exception cref=""Thinktecture.UnknownEnumIdentifierException"">If there is no item with the provided <paramref name=""{_state.KeyProperty.ArgumentName}""/>.</exception>");
      }

      _sb.Append($@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{_state.KeyProperty.ArgumentName}"")]
      public static {(_state.KeyProperty.IsReferenceType ? _state.TypeFullyQualifiedNullAnnotated : _state.TypeFullyQualified)} Get({_state.KeyProperty.TypeFullyQualifiedNullAnnotated} {_state.KeyProperty.ArgumentName})
      {{");

      if (_state.KeyProperty.IsReferenceType)
      {
         _sb.Append($@"
         if ({_state.KeyProperty.ArgumentName} is null)
            return default;
");
      }

      _sb.Append($@"
         if (!_itemsLookup.Value.TryGetValue({_state.KeyProperty.ArgumentName}, out var item))
         {{");

      if (_state.IsValidatable)
      {
         _sb.Append($@"
            item = CreateAndCheckInvalidItem({_state.KeyProperty.ArgumentName});");
      }
      else
      {
         _sb.Append($@"
            throw new global::Thinktecture.UnknownEnumIdentifierException(typeof({_state.TypeFullyQualified}), {_state.KeyProperty.ArgumentName});");
      }

      _sb.Append(@"
         }

         return item;
      }");
   }

   private void GenerateCreateAndCheckInvalidItem(bool needCreateInvalidImplementation)
   {
      _sb.Append($@"

      private static {_state.TypeFullyQualified} CreateAndCheckInvalidItem({_state.KeyProperty.TypeFullyQualified} {_state.KeyProperty.ArgumentName})
      {{
            var item = {(needCreateInvalidImplementation && _state.IsAbstract ? "null" : $"CreateInvalidItem({_state.KeyProperty.ArgumentName})")};
");

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
            if (item is null)
               throw new global::System.Exception(""The implementation of method 'CreateInvalidItem' must not return 'null'."");
");
      }

      _sb.Append(@"
            if (item.IsValid)
               throw new global::System.Exception(""The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'."");");

      if (!needCreateInvalidImplementation)
      {
         _sb.Append($@"

            if (_itemsLookup.Value.ContainsKey(item.{_state.KeyProperty.Name}))
               throw new global::System.Exception(""The implementation of method 'CreateInvalidItem' must not return an instance with property '{_state.KeyProperty.Name}' equals to one of a valid item."");");
      }

      _sb.Append(@"
         return item;
      }");
   }

   private void GenerateCreateInvalidItem()
   {
      _sb.Append($@"

      private static {_state.TypeFullyQualified} CreateInvalidItem({_state.KeyProperty.TypeFullyQualified} {_state.KeyProperty.ArgumentName})
      {{
         return new {_state.TypeFullyQualified}({_state.KeyProperty.ArgumentName}, false");

      foreach (var member in _state.AssignableInstanceFieldsAndProperties)
      {
         _sb.Append(", default");

         if (member.IsReferenceType)
            _sb.Append('!');
      }

      _sb.Append(@");
      }");
   }

   private void GenerateConstructors()
   {
      var ownCtorArgs = _state.AssignableInstanceFieldsAndProperties
                              .Select(a => new ConstructorArgument(a.TypeFullyQualifiedWithNullability, a.ArgumentName))
                              .ToList();

      if (_state.BaseType is null)
      {
         GenerateConstructor(ownCtorArgs, Array.Empty<ConstructorArgument>());
         return;
      }

      var baseCtorArgs = _state.BaseType.Constructors
                               .Select(ctor =>
                                       {
                                          return ctor.Arguments
                                                     .Select(a =>
                                                             {
                                                                var argName = a.ArgumentName;
                                                                var counter = 0;

                                                                while (_state.KeyProperty.ArgumentName == argName || ownCtorArgs.Any(ownArg => ownArg.ArgumentName == argName))
                                                                {
                                                                   counter++;
                                                                   argName = $"{a.ArgumentName}{counter}"; // rename the argument name if it collides with another argument
                                                                }

                                                                return new ConstructorArgument(a.TypeFullyQualifiedWithNullability, argName);
                                                             }).ToList();
                                       })
                               .Distinct(ConstructorArgumentsComparer.Instance)
                               .ToList();

      foreach (var baseArgs in baseCtorArgs)
      {
         GenerateConstructor(ownCtorArgs.Concat(baseArgs).ToList(), baseArgs);
      }
   }

   private void GenerateConstructor(
      IReadOnlyList<ConstructorArgument> ctorArgs,
      IReadOnlyList<ConstructorArgument> baseCtorArgs)
   {
      if (_state.IsValidatable)
      {
         _sb.Append($@"

      private {_state.Name}({_state.KeyProperty.TypeFullyQualified} {_state.KeyProperty.ArgumentName}");

         foreach (var member in ctorArgs)
         {
            _sb.Append($@", {member.TypeFullyQualifiedWithNullability} {member.ArgumentName}");
         }

         _sb.Append($@")
         : this({_state.KeyProperty.ArgumentName}, true");

         foreach (var members in ctorArgs)
         {
            _sb.Append($@", {members.ArgumentName}");
         }

         _sb.Append(@")
      {
      }");
      }

      _sb.Append($@"

      private {_state.Name}({_state.KeyProperty.TypeFullyQualified} {_state.KeyProperty.ArgumentName}");

      if (_state.IsValidatable)
         _sb.Append(", bool isValid");

      foreach (var member in ctorArgs)
      {
         _sb.Append($@", {member.TypeFullyQualifiedWithNullability} {member.ArgumentName}");
      }

      _sb.Append(@")");

      if (baseCtorArgs.Count > 0)
      {
         _sb.Append(@"
         : base(");

         for (var i = 0; i < baseCtorArgs.Count; i++)
         {
            if (i != 0)
               _sb.Append(", ");

            _sb.Append(baseCtorArgs[i].ArgumentName);
         }

         _sb.Append(@")");
      }

      _sb.Append(@"
      {");

      _sb.Append($@"
         ValidateConstructorArguments(ref {_state.KeyProperty.ArgumentName}");

      if (_state.IsValidatable)
         _sb.Append(", isValid");

      foreach (var members in ctorArgs)
      {
         _sb.Append($@", ref {members.ArgumentName}");
      }

      _sb.Append(@");
");

      if (_state.KeyProperty.IsReferenceType)
      {
         _sb.Append($@"
         if ({_state.KeyProperty.ArgumentName} is null)
            throw new global::System.ArgumentNullException(nameof({_state.KeyProperty.ArgumentName}));
");
      }

      _sb.Append($@"
         this.{_state.KeyProperty.Name} = {_state.KeyProperty.ArgumentName};");

      if (_state.IsValidatable)
      {
         _sb.Append(@"
         this.IsValid = isValid;");
      }

      foreach (var memberInfo in _state.AssignableInstanceFieldsAndProperties)
      {
         _sb.Append($@"
         this.{memberInfo.Name} = {memberInfo.ArgumentName};");
      }

      _sb.Append($@"
         this._hashCode = global::System.HashCode.Combine(typeof({_state.TypeFullyQualified}), {EnumSourceGeneratorState.KEY_EQUALITY_COMPARER_NAME}.GetHashCode({_state.KeyProperty.ArgumentName}));
      }}

      static partial void ValidateConstructorArguments(ref {_state.KeyProperty.TypeFullyQualified} {_state.KeyProperty.ArgumentName}");

      if (_state.IsValidatable)
         _sb.Append(", bool isValid");

      foreach (var members in ctorArgs)
      {
         _sb.Append($@", ref {members.TypeFullyQualifiedWithNullability} {members.ArgumentName}");
      }

      _sb.Append(@");");
   }

   private record struct ConstructorArgument(string TypeFullyQualifiedWithNullability, string ArgumentName);

   private sealed class ConstructorArgumentsComparer : IEqualityComparer<IReadOnlyList<ConstructorArgument>>
   {
      public static readonly ConstructorArgumentsComparer Instance = new();

      public bool Equals(IReadOnlyList<ConstructorArgument> x, IReadOnlyList<ConstructorArgument> y)
      {
         if (x.Count != y.Count)
            return false;

         for (var i = 0; i < x.Count; i++)
         {
            var arg = x[i];
            var otherArg = y[i];

            if (arg.TypeFullyQualifiedWithNullability != otherArg.TypeFullyQualifiedWithNullability)
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
