using System.Runtime.CompilerServices;
using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   private bool NeedsDefaultComparer => !_state.HasKeyComparerImplementation;

   public override string CodeGeneratorName => "SmartEnum-CodeGenerator";
   public override string? FileNameSuffix => null;

   public SmartEnumCodeGenerator(EnumSourceGeneratorState state, StringBuilder stringBuilder)
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

      GenerateEnum(cancellationToken);

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
      var needCreateInvalidItemImplementation = _state is { Settings.IsValidatable: true, HasCreateInvalidItemImplementation: false };

      _sb.GenerateStructLayoutAttributeIfRequired(_state);

      if (_state.KeyProperty is not null)
      {
         _sb.Append(@"
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<").Append(_state.TypeFullyQualified).Append(", ").Append(_state.KeyProperty.TypeFullyQualified).Append(", ").Append(_state.ValidationError.TypeFullyQualified).Append(">))]");
      }

      _sb.Append(@"
   partial ").Append(_state.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Name).Append(" :");

      if (_state.KeyProperty is not null)
         _sb.Append(" global::Thinktecture.IEnum<").Append(_state.KeyProperty.TypeFullyQualified).Append(", ").Append(_state.TypeFullyQualified).Append(", ").Append(_state.ValidationError.TypeFullyQualified).Append(">,");

      foreach (var desiredFactory in _state.Settings.DesiredFactories)
      {
         if (desiredFactory.Equals(_state.KeyProperty))
            continue;

         _sb.Append(@"
      global::Thinktecture.IValueObjectFactory<").Append(_state.TypeFullyQualified).Append(", ").Append(desiredFactory.TypeFullyQualified).Append(", ").Append(_state.ValidationError.TypeFullyQualified).Append(">,");

         if (desiredFactory.UseForSerialization != SerializationFrameworks.None)
         {
            _sb.Append(@"
      global::Thinktecture.IValueObjectConvertable<").Append(desiredFactory.TypeFullyQualified).Append(">,");
         }
      }

      if (_state.Settings.IsValidatable)
      {
         _sb.Append(@"
      global::Thinktecture.IValidatableEnum,");
      }

      _sb.Append(@"
      global::System.IEquatable<").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@">
   {");

      if (_state.KeyProperty is not null)
      {
         GenerateModuleInitializer(_state.KeyProperty);

         if (NeedsDefaultComparer)
         {
            _sb.Append(@"

      public static global::System.Collections.Generic.IEqualityComparer<").Append(_state.KeyProperty.TypeFullyQualifiedNullAnnotated).Append("> ").Append(Constants.KEY_EQUALITY_COMPARER_NAME).Append(" => ");

            if (_state.KeyProperty.IsString())
            {
               _sb.Append("global::System.StringComparer.OrdinalIgnoreCase");
            }
            else
            {
               _sb.Append("global::System.Collections.Generic.EqualityComparer<").Append(_state.KeyProperty.TypeFullyQualifiedNullAnnotated).Append(">.Default");
            }

            _sb.Append(";");
         }

         _sb.Append(@"

      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<").Append(_state.KeyProperty.TypeFullyQualified).Append(", ").Append(_state.TypeFullyQualified).Append(@">> _itemsLookup
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<").Append(_state.KeyProperty.TypeFullyQualified).Append(", ").Append(_state.TypeFullyQualified).Append(@">>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<").Append(_state.TypeFullyQualified).Append(@">> _items
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<").Append(_state.TypeFullyQualified).Append(@">>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public static global::System.Collections.Generic.IReadOnlyList<").Append(_state.TypeFullyQualified).Append(@"> Items => _items.Value;

      /// <summary>
      /// The identifier of the item.
      /// </summary>
      public ").Append(_state.KeyProperty.TypeFullyQualified).Append(" ").Append(_state.KeyProperty.Name).Append(" { get; }");
      }
      else
      {
         _sb.Append(@"
      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<").Append(_state.TypeFullyQualified).Append(@">> _items
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<").Append(_state.TypeFullyQualified).Append(@">>(GetItems, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public static global::System.Collections.Generic.IReadOnlyList<").Append(_state.TypeFullyQualified).Append("> Items => _items.Value;");
      }

      if (_state.KeyProperty is not null && _state.Settings.IsValidatable)
      {
         _sb.Append(@"

      /// <inheritdoc />
      public bool IsValid { get; }");

         GenerateEnsureValid(_state.KeyProperty);
      }

      _sb.Append(@"

      private readonly int _hashCode;");

      cancellationToken.ThrowIfCancellationRequested();

      GenerateConstructors();

      if (_state.KeyProperty is not null)
      {
         GenerateToValue(_state.KeyProperty);
         GenerateGet(_state.KeyProperty);

         if (_state.Settings.IsValidatable)
            GenerateCreateAndCheckInvalidItem(_state.KeyProperty, needCreateInvalidItemImplementation);

         if (needCreateInvalidItemImplementation && !_state.IsAbstract)
            GenerateCreateInvalidItem(_state.KeyProperty);

         cancellationToken.ThrowIfCancellationRequested();

         GenerateTryGet(_state.KeyProperty);
         GenerateValidate(_state.KeyProperty);
         GenerateImplicitConversion(_state.KeyProperty);
         GenerateExplicitConversion(_state.KeyProperty);
      }

      GenerateEquals();

      cancellationToken.ThrowIfCancellationRequested();

      _sb.Append(@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {
         return other is ").Append(_state.TypeFullyQualified).Append(@" item && Equals(item);
      }

      /// <inheritdoc />
      public override int GetHashCode()
      {
         return _hashCode;
      }");

      if (_state.KeyProperty is not null && !_state.Settings.SkipToString)
         GenerateToString(_state.KeyProperty);

      var hasSaneNumberOfItems = _state.ItemNames.Count < 1000;

      if (!_state.Settings.SkipSwitchMethods.GetValueOrDefault() && hasSaneNumberOfItems)
      {
         GenerateSwitchForAction(false);
         GenerateSwitchForAction(true);
         GenerateSwitchForFunc(false);
         GenerateSwitchForFunc(true);
      }

      if (!_state.Settings.SkipMapMethods.GetValueOrDefault() && hasSaneNumberOfItems)
         GenerateMap();

      if (_state.KeyProperty is not null)
      {
         GenerateGetLookup(_state.KeyProperty);
         GenerateValidationErrorFactory();
      }
      else
      {
         GenerateGetItems();
      }

      _sb.Append(@"
   }");
   }

   private void GenerateToString(IMemberState keyProperty)
   {
      _sb.Append(@"

      /// <inheritdoc />
      public override string ToString()
      {
         return this.").Append(keyProperty.Name).Append(@".ToString();
      }");
   }

   private void GenerateSwitchForAction(bool withContext)
   {
      if (_state.ItemNames.Count == 0)
         return;

      _sb.Append(@"

      /// <summary>
      /// Executes an action depending on the current item.
      /// </summary>");

      var itemNamePrefix = _state.ArgumentName.Raw;

      if (withContext)
      {
         _sb.Append(@"
      /// <param name=""context"">Context to be passed to the callbacks.</param>");
      }

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         _sb.Append(@"
      /// <param name=""").Append(itemNamePrefix).Append(i + 1).Append(@""">The item to compare to.</param>
      /// <param name=""").Append(itemNamePrefix).Append("Action").Append(i + 1).Append(@""">The action to execute if the current item is equal to <paramref name=""").Append(itemNamePrefix).Append(i + 1).Append(@"""/>.</param>");
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

      var itemNamePrefix = _state.ArgumentName.Raw;

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         _sb.Append(@"
      /// <param name=""").Append(itemNamePrefix).Append(i + 1).Append(@""">The item to compare to.</param>
      /// <param name=""").Append(itemNamePrefix).Append("Func").Append(i + 1).Append(@""">The function to execute if the current item is equal to <paramref name=""").Append(itemNamePrefix).Append(i + 1).Append(@"""/>.</param>");
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
            return ").Append(itemNamePrefix).Append("Func").Append(i + 1).Append("(");

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

   private void GenerateMap()
   {
      if (_state.ItemNames.Count == 0)
         return;

      _sb.Append(@"

      /// <summary>
      /// Maps an item to an instance of type <typeparamref name=""T""/>.
      /// </summary>");

      var itemNamePrefix = _state.ArgumentName.Raw;

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         _sb.Append(@"
      /// <param name=""").Append(itemNamePrefix).Append(i + 1).Append(@""">The item to compare to.</param>
      /// <param name=""other").Append(i + 1).Append(@""">The instance to return if the current item is equal to <paramref name=""").Append(itemNamePrefix).Append(i + 1).Append(@"""/>.</param>");
      }

      _sb.Append(@"
      public T Map<T>(");

      for (var i = 0; i < _state.ItemNames.Count; i++)
      {
         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         ").Append(_state.Name).Append(" ").Append(itemNamePrefix).Append(i + 1)
            .Append(", T ").Append("other").Append(i + 1);
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
            return other").Append(i + 1).Append(@";
         }");
      }

      _sb.Append(@"
         else
         {
            throw new global::System.ArgumentOutOfRangeException($""No instance provided for the item '{this}'."");
         }
      }");
   }

   private void GenerateModuleInitializer(IMemberState keyMember)
   {
      var enumType = _state.TypeFullyQualified;
      var enumTypeNullAnnotated = _state.TypeFullyQualifiedNullAnnotated;

      _sb.Append(@"
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         var convertFromKey = new global::System.Func<").Append(keyMember.TypeFullyQualifiedNullAnnotated).Append(", ").Append(enumTypeNullAnnotated).Append(">(").Append(enumType).Append(@".Get);
         global::System.Linq.Expressions.Expression<global::System.Func<").Append(keyMember.TypeFullyQualifiedNullAnnotated).Append(", ").Append(enumTypeNullAnnotated).Append(">> convertFromKeyExpression = static ").Append(keyMember.ArgumentName.Escaped).Append(" => ").Append(enumType).Append(".Get(").Append(keyMember.ArgumentName.Escaped).Append(@");

         var convertToKey = new global::System.Func<").Append(enumType).Append(", ").Append(keyMember.TypeFullyQualified).Append(">(static item => item.").Append(keyMember.Name).Append(@");
         global::System.Linq.Expressions.Expression<global::System.Func<").Append(enumType).Append(", ").Append(keyMember.TypeFullyQualified).Append(">> convertToKeyExpression = static item => item.").Append(keyMember.Name).Append(@";

         var enumType = typeof(").Append(enumType).Append(@");
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(").Append(keyMember.TypeFullyQualified).Append("), true, ").Append(_state.Settings.IsValidatable ? "true" : "false").Append(@", convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
      }");
   }

   private void GenerateTryGet(IMemberState keyProperty)
   {
      _sb.Append(@"

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name=""").Append(keyProperty.ArgumentName.Raw).Append(@"""/> if a valid item exists.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName.Raw).Append(@""">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""").Append(_state.TypeMinimallyQualified).Append(@"""/>; otherwise <c>null</c>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name=""").Append(keyProperty.ArgumentName.Raw).Append(@"""/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] ").Append(keyProperty.TypeFullyQualified).Append(" ").Append(keyProperty.ArgumentName.Escaped).Append(", [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ").Append(_state.TypeFullyQualified).Append(@" item)
      {");

      if (keyProperty.IsReferenceType)
      {
         _sb.Append(@"
         if (").Append(keyProperty.ArgumentName.Escaped).Append(@" is null)
         {
            item = default;
            return false;
         }
");
      }

      if (_state.Settings.IsValidatable)
      {
         _sb.Append(@"
         if(_itemsLookup.Value.TryGetValue(").Append(keyProperty.ArgumentName.Escaped).Append(@", out item))
            return true;

         item = CreateAndCheckInvalidItem(").Append(keyProperty.ArgumentName.Escaped).Append(@");
         return false;");
      }
      else
      {
         _sb.Append(@"
         return _itemsLookup.Value.TryGetValue(").Append(keyProperty.ArgumentName.Escaped).Append(", out item);");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateValidate(IMemberState keyProperty)
   {
      var providerArgumentName = keyProperty.ArgumentName.Escaped == "provider" ? "formatProvider" : "provider";

      _sb.Append(@"

      /// <summary>
      /// Validates the provided <paramref name=""").Append(keyProperty.ArgumentName.Raw).Append(@"""/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName.Raw).Append(@""">The identifier to return an enumeration item for.</param>
      /// <param name=""").Append(providerArgumentName).Append(@""">An object that provides culture-specific formatting information.</param>
      /// <param name=""item"">A valid instance of <see cref=""").Append(_state.TypeMinimallyQualified).Append(@"""/>; otherwise <c>null</c>.</param>
      /// <returns><c>null</c> if a valid item with provided <paramref name=""").Append(keyProperty.ArgumentName.Raw).Append($@"""/> exists; <see cref=""").Append(_state.ValidationError.TypeFullyQualified).Append(@"""/> with an error message otherwise.</returns>
      public static ").Append(_state.ValidationError.TypeFullyQualified).Append("? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] ").Append(keyProperty.TypeFullyQualified).Append(" ").Append(keyProperty.ArgumentName.Escaped).Append(", global::System.IFormatProvider? ").Append(providerArgumentName).Append(", [global::System.Diagnostics.CodeAnalysis.MaybeNull] out ").Append(_state.TypeFullyQualified).Append(@" item)
      {
         if(").Append(_state.TypeFullyQualified).Append(".TryGet(").Append(keyProperty.ArgumentName.Escaped).Append(@", out item))
         {
            return null;
         }
         else
         {");

      if (_state.Settings.IsValidatable)
      {
         if (keyProperty.IsReferenceType)
         {
            _sb.Append(@"
            if(").Append(keyProperty.ArgumentName.Escaped).Append(@" is not null)
               item = CreateAndCheckInvalidItem(").Append(keyProperty.ArgumentName.Escaped).Append(");");
         }
         else
         {
            _sb.Append(@"
            item = CreateAndCheckInvalidItem(").Append(keyProperty.ArgumentName.Escaped).Append(");");
         }
      }

      _sb.Append(@"
            return CreateValidationError<").Append(_state.ValidationError.TypeFullyQualified).Append(@">($""There is no item of type '").Append(_state.TypeMinimallyQualified).Append("' with the identifier '{").Append(keyProperty.ArgumentName.Escaped).Append(@"}'."");
         }
      }");
   }

   private void GenerateImplicitConversion(IMemberState keyProperty)
   {
      _sb.Append(@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""").Append(keyProperty.TypeFullyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""item"">Item to covert.</param>
      /// <returns>The <see cref=""").Append(_state.TypeMinimallyQualified).Append(".").Append(keyProperty.Name).Append(@"""/> of provided <paramref name=""item""/> or <c>default</c> if <paramref name=""item""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""item"")]
      public static implicit operator ").Append(keyProperty.TypeFullyQualifiedNullAnnotated).Append("(").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" item)
      {");

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
         return item is null ? default : item.").Append(keyProperty.Name).Append(";");
      }
      else
      {
         _sb.Append(@"
         return item.").Append(keyProperty.Name).Append(";");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateExplicitConversion(IMemberState keyProperty)
   {
      _sb.Append(@"

      /// <summary>
      /// Explicit conversion from the type <see cref=""").Append(keyProperty.TypeFullyQualified).Append(@"""/>.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName.Raw).Append(@""">Value to covert.</param>
      /// <returns>An instance of <see cref=""").Append(_state.TypeMinimallyQualified).Append(@"""/> if the <paramref name=""").Append(keyProperty.ArgumentName.Raw).Append(@"""/> is a known item or implements <see cref=""Thinktecture.IValidatableEnum""/>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").Append(keyProperty.ArgumentName.Escaped).Append(@""")]
      public static explicit operator ").Append(_state.TypeFullyQualifiedNullAnnotated).Append("(").Append(keyProperty.TypeFullyQualifiedNullAnnotated).Append(" ").Append(keyProperty.ArgumentName.Escaped).Append(@")
      {
         return ").Append(_state.TypeFullyQualified).Append(".Get(").Append(keyProperty.ArgumentName.Escaped).Append(@");
      }");
   }

   private void GenerateEquals()
   {
      _sb.Append(@"

      /// <inheritdoc />
      public bool Equals(").Append(_state.TypeFullyQualifiedNullAnnotated).Append(@" other)
      {");

      if (_state.Settings.IsValidatable)
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

         if (_state.KeyProperty is not null)
         {
            _sb.Append(@"
         return ").Append(Constants.KEY_EQUALITY_COMPARER_NAME).Append(".Equals(this.").Append(_state.KeyProperty.Name).Append(", other.").Append(_state.KeyProperty.Name).Append(");");
         }
         else
         {
            if (_state.IsReferenceType)
            {
               _sb.Append(@"
         return global::System.Object.ReferenceEquals(this, other);");
            }
            else
            {
               // structs without key-member are always evaluate to false, because there is nothing to compare on.
               _sb.Append(@"
         return false;");
            }
         }
      }
      else
      {
         // ReferenceEquals is ok because non-validatable smart enums are classes only
         _sb.Append(@"
         return global::System.Object.ReferenceEquals(this, other);");
      }

      _sb.Append(@"
      }");
   }

   private void GenerateGetLookup(IMemberState keyProperty)
   {
      var totalNumberOfItems = _state.ItemNames.Count;

      _sb.Append(@"

      private static global::System.Collections.Generic.IReadOnlyDictionary<").Append(keyProperty.TypeFullyQualified).Append(", ").Append(_state.TypeFullyQualified).Append(@"> GetLookup()
      {
         var lookup = new global::System.Collections.Generic.Dictionary<").Append(keyProperty.TypeFullyQualified).Append(", ").Append(_state.TypeFullyQualified).Append(">(").Append(totalNumberOfItems).Append(", ").Append(Constants.KEY_EQUALITY_COMPARER_NAME).Append(");");

      if (_state.ItemNames.Count > 0)
      {
         _sb.Append(@"

         void AddItem(").Append(_state.TypeFullyQualified).Append(@" item, string itemName)
         {");

         if (_state.IsReferenceType)
         {
            _sb.Append(@"
            if (item is null)
               throw new global::System.ArgumentNullException($""The item \""{itemName}\"" of type \""").Append(_state.TypeMinimallyQualified).Append(@"\"" must not be null."");
");
         }

         if (keyProperty.IsReferenceType)
         {
            _sb.Append(@"
            if (item.").Append(keyProperty.Name).Append(@" is null)
               throw new global::System.ArgumentException($""The \""").Append(keyProperty.Name).Append(@"\"" of the item \""{itemName}\"" of type \""").Append(_state.TypeMinimallyQualified).Append(@"\"" must not be null."");
");
         }

         if (_state.Settings.IsValidatable)
         {
            _sb.Append(@"
            if (!item.IsValid)
               throw new global::System.ArgumentException($""All \""public static readonly\"" fields of type \""").Append(_state.TypeMinimallyQualified).Append(@"\"" must be valid but the item \""{itemName}\"" with the identifier \""{item.").Append(keyProperty.Name).Append(@"}\"" is not."");
");
         }

         _sb.Append(@"
            if (lookup.ContainsKey(item.").Append(keyProperty.Name).Append(@"))
               throw new global::System.ArgumentException($""The type \""").Append(_state.TypeMinimallyQualified).Append(@"\"" has multiple items with the identifier \""{item.").Append(keyProperty.Name).Append(@"}\""."");

            lookup.Add(item.").Append(keyProperty.Name).Append(@", item);
         }
");

         for (var i = 0; i < _state.ItemNames.Count; i++)
         {
            var itemName = _state.ItemNames[i];

            _sb.Append(@"
         AddItem(").Append(itemName).Append(", nameof(").Append(itemName).Append("));");
         }
      }

      _sb.Append(@"

#if NET8_0_OR_GREATER
         return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, ").Append(Constants.KEY_EQUALITY_COMPARER_NAME).Append(@");
#else
         return lookup;
#endif
      }");
   }

   private void GenerateGetItems()
   {
      var totalNumberOfItems = _state.ItemNames.Count;

      _sb.Append(@"

      private static global::System.Collections.Generic.IReadOnlyList<").Append(_state.TypeFullyQualified).Append(@"> GetItems()
      {
         var list = new global::System.Collections.Generic.List<").Append(_state.TypeFullyQualified).Append(">(").Append(totalNumberOfItems).Append(");");

      if (_state.ItemNames.Count > 0)
      {
         _sb.Append(@"

         void AddItem(").Append(_state.TypeFullyQualified).Append(@" item, string itemName)
         {
            if (item is null)
               throw new global::System.ArgumentNullException($""The item \""{itemName}\"" of type \""").Append(_state.TypeMinimallyQualified).Append(@"\"" must not be null."");

            list.Add(item);
         }
");

         for (var i = 0; i < _state.ItemNames.Count; i++)
         {
            var itemName = _state.ItemNames[i];

            _sb.Append(@"
         AddItem(").Append(itemName).Append(", nameof(").Append(itemName).Append("));");
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
      ").Append(keyProperty.TypeFullyQualified).Append(" global::Thinktecture.IValueObjectConvertable<").Append(keyProperty.TypeFullyQualified).Append(@">.ToValue()
      {
         return this.").Append(keyProperty.Name).Append(@";
      }");
   }

   private void GenerateGet(IMemberState keyProperty)
   {
      _sb.Append(@"

      /// <summary>
      /// Gets an enumeration item for provided <paramref name=""").Append(keyProperty.ArgumentName.Raw).Append(@"""/>.
      /// </summary>
      /// <param name=""").Append(keyProperty.ArgumentName.Raw).Append(@""">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref=""").Append(_state.TypeMinimallyQualified).Append(@""" /> if <paramref name=""").Append(keyProperty.ArgumentName.Raw).Append(@"""/> is not <c>null</c>; otherwise <c>null</c>.</returns>");

      if (!_state.Settings.IsValidatable)
      {
         _sb.Append(@"
      /// <exception cref=""Thinktecture.UnknownEnumIdentifierException"">If there is no item with the provided <paramref name=""").Append(keyProperty.ArgumentName.Raw).Append(@"""/>.</exception>");
      }

      _sb.Append(@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""").Append(keyProperty.ArgumentName.Escaped).Append(@""")]
      public static ").Append(keyProperty.IsReferenceType ? _state.TypeFullyQualifiedNullAnnotated : _state.TypeFullyQualified).Append(" Get(").Append(keyProperty.TypeFullyQualifiedNullAnnotated).Append(" ").Append(keyProperty.ArgumentName.Escaped).Append(@")
      {");

      if (keyProperty.IsReferenceType)
      {
         _sb.Append(@"
         if (").Append(keyProperty.ArgumentName.Escaped).Append(@" is null)
            return default;
");
      }

      _sb.Append(@"
         if (!_itemsLookup.Value.TryGetValue(").Append(keyProperty.ArgumentName.Escaped).Append(@", out var item))
         {");

      if (_state.Settings.IsValidatable)
      {
         _sb.Append(@"
            item = CreateAndCheckInvalidItem(").Append(keyProperty.ArgumentName.Escaped).Append(");");
      }
      else
      {
         _sb.Append(@"
            throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(").Append(_state.TypeFullyQualified).Append("), ").Append(keyProperty.ArgumentName.Escaped).Append(");");
      }

      _sb.Append(@"
         }

         return item;
      }");
   }

   private void GenerateCreateAndCheckInvalidItem(IMemberState keyProperty, bool needsCreateInvalidItemImplementation)
   {
      _sb.Append(@"

      private static ").Append(_state.TypeFullyQualified).Append(" CreateAndCheckInvalidItem(").Append(keyProperty.TypeFullyQualified).Append(" ").Append(keyProperty.ArgumentName.Escaped).Append(@")
      {
         var item = ");

      if (needsCreateInvalidItemImplementation && _state.IsAbstract)
      {
         _sb.Append("null");
      }
      else
      {
         _sb.Append(Constants.Methods.CREATE_INVALID_ITEM).Append("(").Append(keyProperty.ArgumentName.Escaped).Append(")");
      }

      _sb.Append(@";
");

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
         if (item is null)
            throw new global::System.Exception(""The implementation of method '").Append(Constants.Methods.CREATE_INVALID_ITEM).Append(@"' must not return 'null'."");
");
      }

      _sb.Append(@"
         if (item.IsValid)
            throw new global::System.Exception(""The implementation of method '").Append(Constants.Methods.CREATE_INVALID_ITEM).Append(@"' must return an instance with property 'IsValid' equals to 'false'."");");

      if (!needsCreateInvalidItemImplementation)
      {
         _sb.Append(@"

         if (_itemsLookup.Value.ContainsKey(item.").Append(keyProperty.Name).Append(@"))
            throw new global::System.Exception(""The implementation of method '").Append(Constants.Methods.CREATE_INVALID_ITEM).Append("' must not return an instance with property '").Append(keyProperty.Name).Append(@"' equals to one of a valid item."");");
      }

      _sb.Append(@"

         return item;
      }");
   }

   private void GenerateCreateInvalidItem(IMemberState keyProperty)
   {
      _sb.Append(@"

      private static ").Append(_state.TypeFullyQualified).Append(" ").Append(Constants.Methods.CREATE_INVALID_ITEM).Append("(").Append(keyProperty.TypeFullyQualified).Append(" ").Append(keyProperty.ArgumentName.Escaped).Append(@")
      {
         return new ").Append(_state.TypeFullyQualified).Append("(").Append(keyProperty.ArgumentName.Escaped).Append(", false");

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
                                          if (ctor.Arguments.Length == 0)
                                             return (IReadOnlyList<ConstructorArgument>)Array.Empty<ConstructorArgument>();

                                          return ctor.Arguments
                                                     .Select(a =>
                                                             {
                                                                var argName = a.ArgumentName.Escaped;
                                                                var counter = 0;

                                                                while (_state.KeyProperty?.ArgumentName.Escaped == argName || _state.KeyProperty?.ArgumentName.Raw == argName || ContainsArgument(ownCtorArgs, argName))
                                                                {
                                                                   counter++;
                                                                   argName = $"{a.ArgumentName.Raw}{counter.ToString()}"; // rename the argument name if it collides with another argument
                                                                }

                                                                return new ConstructorArgument(a.TypeFullyQualifiedWithNullability, new ArgumentName(argName, argName));
                                                             }).ToList();
                                       })
                               .Distinct(ConstructorArgumentsComparer.Instance)
                               .ToList();

      foreach (var baseArgs in baseCtorArgs)
      {
         GenerateConstructor(ownCtorArgs.Concat(baseArgs).ToList(), baseArgs);
      }
   }

   private void GenerateValidationErrorFactory()
   {
      _sb.Append(@"

      private static TError CreateValidationError<TError>(string message)
         where TError : class, global::Thinktecture.IValidationError<TError>
      {
         return TError.Create(message);
      }");
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static bool ContainsArgument(List<ConstructorArgument> ownCtorArgs, string argName)
   {
      for (var i = 0; i < ownCtorArgs.Count; i++)
      {
         if (ownCtorArgs[i].ArgumentName.Escaped == argName)
            return true;
      }

      return false;
   }

   private void GenerateConstructor(
      IReadOnlyList<ConstructorArgument> ctorArgs,
      IReadOnlyList<ConstructorArgument> baseCtorArgs)
   {
      bool hasKeyMember;

      if (_state.Settings.IsValidatable)
      {
         _sb.Append(@"

      private ").Append(_state.Name).Append("(");

         if (_state.KeyProperty is not null)
         {
            hasKeyMember = true;
            _sb.Append(_state.KeyProperty.TypeFullyQualified).Append(" ").Append(_state.KeyProperty.ArgumentName.Escaped);
         }
         else
         {
            hasKeyMember = false;
         }

         for (var i = 0; i < ctorArgs.Count; i++)
         {
            if (hasKeyMember || i != 0)
               _sb.Append(", ");

            var member = ctorArgs[i];
            _sb.Append(member.TypeFullyQualifiedWithNullability).Append(" ").Append(member.ArgumentName.Escaped);
         }

         _sb.Append(@")
         : this(");

         if (_state.KeyProperty is not null)
         {
            _sb.Append(_state.KeyProperty.ArgumentName.Escaped).Append(", true");
         }
         else
         {
            _sb.Append("true");
         }

         for (var i = 0; i < ctorArgs.Count; i++)
         {
            _sb.Append(", ").Append(ctorArgs[i].ArgumentName.Escaped);
         }

         _sb.Append(@")
      {
      }");
      }

      _sb.Append(@"

      private ").Append(_state.Name).Append("(");

      if (_state.KeyProperty is not null)
      {
         hasKeyMember = true;
         _sb.Append(_state.KeyProperty.TypeFullyQualified).Append(" ").Append(_state.KeyProperty.ArgumentName.Escaped);
      }
      else
      {
         hasKeyMember = false;
      }

      if (_state.Settings.IsValidatable)
      {
         if (hasKeyMember)
            _sb.Append(", ");

         _sb.Append("bool isValid");
      }

      for (var i = 0; i < ctorArgs.Count; i++)
      {
         if (i != 0 || hasKeyMember || _state.Settings.IsValidatable)
            _sb.Append(", ");

         var member = ctorArgs[i];
         _sb.Append(member.TypeFullyQualifiedWithNullability).Append(" ").Append(member.ArgumentName.Escaped);
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

            _sb.Append(baseCtorArgs[i].ArgumentName.Escaped);
         }

         _sb.Append(")");
      }

      _sb.Append(@"
      {");

      _sb.Append(@"
         ValidateConstructorArguments(");

      if (_state.KeyProperty is not null)
      {
         _sb.Append("ref ").Append(_state.KeyProperty.ArgumentName.Escaped);
      }

      if (_state.Settings.IsValidatable)
      {
         if (hasKeyMember)
            _sb.Append(", ");

         _sb.Append("isValid");
      }

      for (var i = 0; i < ctorArgs.Count; i++)
      {
         if (i != 0 || hasKeyMember || _state.Settings.IsValidatable)
            _sb.Append(", ");

         var members = ctorArgs[i];
         _sb.Append("ref ").Append(members.ArgumentName.Escaped);
      }

      _sb.Append(@");
");

      if (_state.KeyProperty is not null)
      {
         if (_state.KeyProperty.IsReferenceType)
         {
            _sb.Append(@"
         if (").Append(_state.KeyProperty.ArgumentName.Escaped).Append(@" is null)
            throw new global::System.ArgumentNullException(nameof(").Append(_state.KeyProperty.ArgumentName.Escaped).Append(@"));
");
         }

         _sb.Append(@"
         this.").Append(_state.KeyProperty.Name).Append(" = ").Append(_state.KeyProperty.ArgumentName.Escaped).Append(";");
      }

      if (_state.Settings.IsValidatable)
      {
         _sb.Append(@"
         this.IsValid = isValid;");
      }

      foreach (var memberInfo in _state.AssignableInstanceFieldsAndProperties)
      {
         _sb.Append(@"
         this.").Append(memberInfo.Name).Append(" = ").Append(memberInfo.ArgumentName.Escaped).Append(";");
      }

      if (_state.KeyProperty is not null)
      {
         _sb.Append(@"
         this._hashCode = global::System.HashCode.Combine(typeof(").Append(_state.TypeFullyQualified).Append("), ").Append(Constants.KEY_EQUALITY_COMPARER_NAME).Append(".GetHashCode(").Append(_state.KeyProperty.ArgumentName.Escaped).Append(@"));
      }");
      }
      else
      {
         _sb.Append(@"
         this._hashCode = base.GetHashCode();
      }");
      }

      _sb.Append(@"

      static partial void ValidateConstructorArguments(");

      if (_state.KeyProperty is not null)
      {
         _sb.Append("ref ").Append(_state.KeyProperty.TypeFullyQualified).Append(" ").Append(_state.KeyProperty.ArgumentName.Escaped);
      }

      if (_state.Settings.IsValidatable)
      {
         if (hasKeyMember)
            _sb.Append(", ");

         _sb.Append("bool isValid");
      }

      for (var i = 0; i < ctorArgs.Count; i++)
      {
         if (i != 0 || hasKeyMember || _state.Settings.IsValidatable)
            _sb.Append(", ");

         var members = ctorArgs[i];
         _sb.Append("ref ").Append(members.TypeFullyQualifiedWithNullability).Append(" ").Append(members.ArgumentName.Escaped);
      }

      _sb.Append(");");
   }

   private readonly record struct ConstructorArgument(string TypeFullyQualifiedWithNullability, ArgumentName ArgumentName);

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
