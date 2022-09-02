using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public class SmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   private string? NullableQuestionMarkEnum => _state.IsReferenceType ? "?" : null;
   private string? NullableQuestionMarkKey => _state.KeyProperty.IsReferenceType ? "?" : null;
   private bool NeedsDefaultComparer => _state.Settings.KeyComparer is null;
   private string KeyComparerMember => _state.Settings.KeyComparer ?? "_defaultKeyComparerMember";

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

      GenerateTypeConverter();
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
      var needCreateInvalidImplementation = _state.IsValidatable && !_state.HasCreateInvalidImplementation;

      _sb.GenerateStructLayoutAttributeIfRequired(_state);

      _sb.Append($@"
   [global::Thinktecture.Internal.ValueObjectConstructor(nameof({_state.KeyProperty.Name})");

      foreach (var member in _state.AssignableInstanceFieldsAndProperties)
      {
         var memberName = member.EnumMemberSettings.MappedMemberName ?? member.Name;

         _sb.Append($@", nameof({memberName})");
      }

      _sb.Append($@")]
   [global::Thinktecture.Internal.KeyedValueObject]
   [global::System.ComponentModel.TypeConverter(typeof({_state.EnumTypeFullyQualified}_EnumTypeConverter))]
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name} : global::System.IEquatable<{_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum}>
   {{
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {{
         var convertFromKey = new global::System.Func<{_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey}, {_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum}>({_state.EnumTypeFullyQualified}.Get);
         global::System.Linq.Expressions.Expression<global::System.Func<{_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey}, {_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum}>> convertFromKeyExpression = {_state.KeyProperty.ArgumentName} => {_state.EnumTypeFullyQualified}.Get({_state.KeyProperty.ArgumentName});

         var convertToKey = new global::System.Func<{_state.EnumTypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey}>(item => item.{_state.KeyProperty.Name});
         global::System.Linq.Expressions.Expression<global::System.Func<{_state.EnumTypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey}>> convertToKeyExpression = item => item.{_state.KeyProperty.Name};

         var validate = new global::Thinktecture.Internal.Validate<{_state.EnumTypeFullyQualified}, {_state.KeyProperty.TypeFullyQualifiedWithNullability}>({_state.EnumTypeFullyQualified}.Validate);

         var enumType = typeof({_state.EnumTypeFullyQualified});
         var metadata = new global::Thinktecture.Internal.ValueObjectMetadata(enumType, typeof({_state.KeyProperty.TypeFullyQualifiedWithNullability}), true, {(_state.IsValidatable ? "true" : "false")}, convertFromKey, convertFromKeyExpression, convertToKey, convertToKeyExpression, validate);

         global::Thinktecture.Internal.ValueObjectMetadataLookup.AddMetadata(enumType, metadata);");

      foreach (var derivedType in _state.FullyQualifiedDerivedTypes)
      {
         _sb.Append($@"
         global::Thinktecture.Internal.ValueObjectMetadataLookup.AddMetadata(typeof({derivedType}), metadata);");
      }

      _sb.Append(@"
      }");

      if (NeedsDefaultComparer)
      {
         var defaultComparer = _state.KeyProperty.IsString() ? "global::System.StringComparer.OrdinalIgnoreCase" : $"global::System.Collections.Generic.EqualityComparer<{_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey}>.Default";

         _sb.Append($@"

      private static readonly global::System.Collections.Generic.IEqualityComparer<{_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey}> {KeyComparerMember} = {defaultComparer};");
      }

      _sb.Append($@"

      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<{_state.KeyProperty.TypeFullyQualifiedWithNullability}, {_state.EnumTypeFullyQualified}>> _itemsLookup
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<{_state.KeyProperty.TypeFullyQualifiedWithNullability}, {_state.EnumTypeFullyQualified}>>(GetLookup);

      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<{_state.EnumTypeFullyQualified}>> _items
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<{_state.EnumTypeFullyQualified}>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly());

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public static global::System.Collections.Generic.IReadOnlyList<{_state.EnumTypeFullyQualified}> Items => _items.Value;

      /// <summary>
      /// The identifier of the item.
      /// </summary>
      [global::System.Diagnostics.CodeAnalysis.NotNull]
      public {_state.KeyProperty.TypeFullyQualifiedWithNullability} {_state.KeyProperty.Name} {{ get; }}");

      if (_state.IsValidatable)
      {
         _sb.Append(@"

      /// <inheritdoc />
      public bool IsValid { get; }");

         GenerateEnsureValid();
      }

      _sb.Append(@"

      private readonly global::System.Lazy<int> _hashCode;");

      GenerateConstructors();
      GenerateGetKey();
      GenerateGet(needCreateInvalidImplementation);

      if (needCreateInvalidImplementation && !_state.IsAbstract)
         GenerateCreateInvalidItem();

      GenerateTryGet();
      GenerateValidate();
      GenerateImplicitConversion();
      GenerateExplicitConversion();
      GenerateEqualityOperators();
      GenerateTypedEquals();

      _sb.Append($@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {{
         return other is {_state.EnumTypeFullyQualified} item && Equals(item);
      }}

      /// <inheritdoc />
      public override int GetHashCode()
      {{
         return _hashCode.Value;
      }}

      /// <inheritdoc />
      public override string? ToString()
      {{
         return this.{_state.KeyProperty.Name}{(!_state.IsReferenceType && _state.KeyProperty.IsReferenceType ? "?" : null)}.ToString();
      }}");

      GenerateGetLookup();
   }

   private void GenerateTryGet()
   {
      _sb.Append($@"

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name=""{_state.KeyProperty.ArgumentName}""/> if a valid item exists.
      /// </summary>
      /// <param name=""{_state.KeyProperty.ArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""{_state.EnumTypeMinimallyQualified}""/>; otherwise <c>null</c>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name=""{_state.KeyProperty.ArgumentName}""/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] {_state.KeyProperty.TypeFullyQualifiedWithNullability} {_state.KeyProperty.ArgumentName}, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out {_state.EnumTypeFullyQualified} item)
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

      _sb.Append($@"
         return _itemsLookup.Value.TryGetValue({_state.KeyProperty.ArgumentName}, out item);
      }}");
   }

   private void GenerateValidate()
   {
      _sb.Append($@"

      /// <summary>
      /// Validates the provided <paramref name=""{_state.KeyProperty.ArgumentName}""/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name=""{_state.KeyProperty.ArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""{_state.EnumTypeMinimallyQualified}""/>; otherwise <c>null</c>.</param>
      /// <returns> <see cref=""System.ComponentModel.DataAnnotations.ValidationResult.Success""/> if a valid item with provided <paramref name=""{_state.KeyProperty.ArgumentName}""/> exists; <see cref=""System.ComponentModel.DataAnnotations.ValidationResult""/> with an error message otherwise.</returns>
      public static global::System.ComponentModel.DataAnnotations.ValidationResult? Validate({_state.KeyProperty.TypeFullyQualifiedWithNullability} {_state.KeyProperty.ArgumentName}, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out {_state.EnumTypeFullyQualified} item)
      {{
         return {_state.EnumTypeFullyQualified}.TryGet({_state.KeyProperty.ArgumentName}, out item)
               ? global::System.ComponentModel.DataAnnotations.ValidationResult.Success
               : new global::System.ComponentModel.DataAnnotations.ValidationResult($""The enumeration item of type \""{_state.EnumTypeMinimallyQualified}\"" with identifier \""{{{_state.KeyProperty.ArgumentName}}}\"" is not valid."");
      }}");
   }

   private void GenerateEqualityOperators()
   {
      _sb.Append($@"

      /// <summary>
      /// Compares to instances of <see cref=""{_state.EnumTypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>true</c> if items are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==({_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum} item1, {_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum} item2)
      {{");

      if (_state.IsReferenceType)
      {
         _sb.Append(@"
         if (item1 is null)
            return item2 is null;
");
      }

      _sb.Append($@"
         return item1.Equals(item2);
      }}

      /// <summary>
      /// Compares to instances of <see cref=""{_state.EnumTypeMinimallyQualified}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>false</c> if items are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=({_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum} item1, {_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum} item2)
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
      /// <returns>The <see cref=""{_state.EnumTypeMinimallyQualified}.{_state.KeyProperty.Name}""/> of provided <paramref name=""item""/> or <c>default</c> if <paramref name=""item""/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""item"")]
      public static implicit operator {_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey}({_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum} item)
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
      /// <returns>An instance of <see cref=""{_state.EnumTypeMinimallyQualified}""/> if the <paramref name=""{_state.KeyProperty.ArgumentName}""/> is a known item or implements <see cref=""Thinktecture.IValidatableEnum{{TKey}}""/>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{_state.KeyProperty.ArgumentName}"")]
      public static explicit operator {_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum}({_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey} {_state.KeyProperty.ArgumentName})
      {{
         return {_state.EnumTypeFullyQualified}.Get({_state.KeyProperty.ArgumentName});
      }}");
   }

   private void GenerateTypedEquals()
   {
      _sb.Append($@"

      /// <inheritdoc />
      public bool Equals({_state.EnumTypeFullyQualified}{NullableQuestionMarkEnum} other)
      {{");

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

      if (_state.IsValidatable)
      {
         _sb.Append(@"
         if (this.IsValid != other.IsValid)
            return false;
");
      }

      _sb.Append($@"
         return {KeyComparerMember}.Equals(this.{_state.KeyProperty.Name}, other.{_state.KeyProperty.Name});
      }}");
   }

   private void GenerateGetLookup()
   {
      var totalNumberOfItems = _state.ItemNames.Count;

      _sb.Append($@"

      private static global::System.Collections.Generic.IReadOnlyDictionary<{_state.KeyProperty.TypeFullyQualifiedWithNullability}, {_state.EnumTypeFullyQualified}> GetLookup()
      {{
         var lookup = new global::System.Collections.Generic.Dictionary<{_state.KeyProperty.TypeFullyQualifiedWithNullability}, {_state.EnumTypeFullyQualified}>({totalNumberOfItems}, {KeyComparerMember});");

      if (_state.ItemNames.Count > 0)
      {
         _sb.Append($@"

         void AddItem({_state.EnumTypeFullyQualified} item, string itemName)
         {{");

         if (_state.IsReferenceType)
         {
            _sb.Append($@"
            if(item is null)
               throw new global::System.ArgumentNullException($""The item \""{{itemName}}\"" of type \""{_state.EnumTypeMinimallyQualified}\"" must not be null."");
");
         }

         if (_state.KeyProperty.IsReferenceType)
         {
            _sb.Append($@"
            if(item.{_state.KeyProperty.Name} is null)
               throw new global::System.ArgumentException($""The \""{_state.KeyProperty.Name}\"" of the item \""{{itemName}}\"" of type \""{_state.EnumTypeMinimallyQualified}\"" must not be null."");
");
         }

         if (_state.IsValidatable)
         {
            _sb.Append($@"
            if(!item.IsValid)
               throw new global::System.ArgumentException($""All \""public static readonly\"" fields of type \""{_state.EnumTypeMinimallyQualified}\"" must be valid but the item \""{{itemName}}\"" with the identifier \""{{item.{_state.KeyProperty.Name}}}\"" is not."");
");
         }

         _sb.Append($@"
            if (lookup.ContainsKey(item.{_state.KeyProperty.Name}))
               throw new global::System.ArgumentException($""The type \""{_state.EnumTypeMinimallyQualified}\"" has multiple items with the identifier \""{{item.{_state.KeyProperty.Name}}}\""."");

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
      }
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
      {_state.KeyProperty.TypeFullyQualifiedWithNullability} global::Thinktecture.IEnum<{_state.KeyProperty.TypeFullyQualifiedWithNullability}>.GetKey()
      {{
         return this.{_state.KeyProperty.Name};
      }}");
   }

   private void GenerateGet(bool needCreateInvalidImplementation)
   {
      _sb.Append($@"

      /// <summary>
      /// Gets an enumeration item for provided <paramref name=""{_state.KeyProperty.ArgumentName}""/>.
      /// </summary>
      /// <param name=""{_state.KeyProperty.ArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref=""{_state.EnumTypeMinimallyQualified}"" /> if <paramref name=""{_state.KeyProperty.ArgumentName}""/> is not <c>null</c>; otherwise <c>null</c>.</returns>");

      if (!_state.IsValidatable)
      {
         _sb.Append($@"
      /// <exception cref=""Thinktecture.UnknownEnumIdentifierException"">If there is no item with the provided <paramref name=""{_state.KeyProperty.ArgumentName}""/>.</exception>");
      }

      _sb.Append($@"
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{_state.KeyProperty.ArgumentName}"")]
      public static {_state.EnumTypeFullyQualified}{(_state.KeyProperty.IsReferenceType ? NullableQuestionMarkEnum : null)} Get({_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey} {_state.KeyProperty.ArgumentName})
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
            item = {(needCreateInvalidImplementation && _state.IsAbstract ? "null" : $"CreateInvalidItem({_state.KeyProperty.ArgumentName})")};
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

            if ({_state.EnumTypeFullyQualified}.TryGet(item.{_state.KeyProperty.Name}, out _))
               throw new global::System.Exception(""The implementation of method 'CreateInvalidItem' must not return an instance with property '{_state.KeyProperty.Name}' equals to one of a valid item."");");
         }
      }
      else
      {
         _sb.Append($@"
            throw new global::Thinktecture.UnknownEnumIdentifierException(typeof({_state.EnumTypeFullyQualified}), {_state.KeyProperty.ArgumentName});");
      }

      _sb.Append(@"
         }

         return item;
      }");
   }

   private void GenerateCreateInvalidItem()
   {
      _sb.Append($@"

      private static {_state.EnumTypeFullyQualified} CreateInvalidItem({_state.KeyProperty.TypeFullyQualifiedWithNullability} {_state.KeyProperty.ArgumentName})
      {{
         return new {_state.EnumTypeFullyQualified}({_state.KeyProperty.ArgumentName}, false");

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

      private {_state.Name}({_state.KeyProperty.TypeFullyQualifiedWithNullability} {_state.KeyProperty.ArgumentName}");

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

      private {_state.Name}({_state.KeyProperty.TypeFullyQualifiedWithNullability} {_state.KeyProperty.ArgumentName}");

      if (_state.IsValidatable)
         _sb.Append(", bool isValid");

      foreach (var member in ctorArgs)
      {
         _sb.Append($@", {member.TypeFullyQualifiedWithNullability} {member.ArgumentName}");
      }

      _sb.Append(@")");

      if (baseCtorArgs.Count > 0)
      {
         _sb.Append($@"
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
         this._hashCode = new global::System.Lazy<int>(() => typeof({_state.EnumTypeFullyQualified}).GetHashCode() * 397 ^ {KeyComparerMember}.GetHashCode({_state.KeyProperty.ArgumentName}));
      }}

      static partial void ValidateConstructorArguments(ref {_state.KeyProperty.TypeFullyQualifiedWithNullability} {_state.KeyProperty.ArgumentName}");

      if (_state.IsValidatable)
         _sb.Append(", bool isValid");

      foreach (var members in ctorArgs)
      {
         _sb.Append($@", ref {members.TypeFullyQualifiedWithNullability} {members.ArgumentName}");
      }

      _sb.Append(@");");
   }

   private void GenerateTypeConverter()
   {
      _sb.Append($@"
   public class {_state.Name}_EnumTypeConverter : global::Thinktecture.ValueObjectTypeConverter<{_state.EnumTypeFullyQualified}, {_state.KeyProperty.TypeFullyQualifiedWithNullability}>
   {{
      /// <inheritdoc />
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""{_state.KeyProperty.ArgumentName}"")]
      protected override {_state.EnumTypeFullyQualified}{(_state.KeyProperty.IsReferenceType ? NullableQuestionMarkEnum : null)} ConvertFrom({_state.KeyProperty.TypeFullyQualified}{NullableQuestionMarkKey} {_state.KeyProperty.ArgumentName})
      {{");

      if (_state.IsValidatable)
      {
         _sb.Append($@"
         return {_state.EnumTypeFullyQualified}.Get({_state.KeyProperty.ArgumentName});");
      }
      else
      {
         if (_state.KeyProperty.IsReferenceType)
         {
            _sb.Append($@"
         if({_state.KeyProperty.ArgumentName} is null)
            return default;
");
         }

         _sb.Append($@"
         if({_state.EnumTypeFullyQualified}.TryGet({_state.KeyProperty.ArgumentName}, out var item))
            return item;

         throw new global::System.FormatException($""There is no item of type '{_state.Name}' with the identifier '{{{_state.KeyProperty.ArgumentName}}}'."");");
      }

      _sb.Append($@"
      }}

      /// <inheritdoc />
      protected override {_state.KeyProperty.TypeFullyQualifiedWithNullability} GetKeyValue({_state.EnumTypeFullyQualified} item)
      {{
         return item.{_state.KeyProperty.Name};
      }}
   }}
");
   }

   private record struct ConstructorArgument(string TypeFullyQualifiedWithNullability, string ArgumentName);

   private class ConstructorArgumentsComparer : IEqualityComparer<IReadOnlyList<ConstructorArgument>>
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
