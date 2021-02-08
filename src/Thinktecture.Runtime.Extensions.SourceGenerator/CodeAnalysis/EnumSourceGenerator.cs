using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinktecture.CodeAnalysis
{
   public class EnumSourceGenerator
   {
      private readonly EnumSourceGeneratorState _state;
      private readonly StringBuilder _sb;

      public EnumSourceGenerator(EnumSourceGeneratorState state)
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
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq.Expressions;
using Thinktecture;

{(String.IsNullOrWhiteSpace(_state.Namespace) ? null : $"namespace {_state.Namespace}")}
{{");
         GenerateTypeConverter();
         GenerateEnum();

         _sb.Append($@"
}}
");

         return _sb.ToString();
      }

      private void GenerateEnum()
      {
         var derivedTypes = _state.EnumType.FindDerivedInnerTypes();
         var needCreateInvalidImplementation = _state.IsValidatable && !_state.EnumType.HasCreateInvalidImplementation(_state.KeyType);
         var newKeyword = _state.HasBaseEnum ? "new " : null;

         _sb.GenerateStructLayoutAttributeIfRequired(_state.EnumType);

         if (_state.IsExtensible)
         {
            _sb.Append($@"
   [Thinktecture.EnumConstructor(nameof({_state.KeyPropertyName})");

            foreach (var member in _state.AssignableInstanceFieldsAndProperties)
            {
               var memberName = member.Symbol.FindEnumGenerationMemberAttribute()?.FindMapsToMember() ?? member.Identifier.ToString();

               _sb.Append($@", nameof({memberName})");
            }

            _sb.Append(")]");
         }

         _sb.Append($@"
   [System.ComponentModel.TypeConverter(typeof({_state.EnumIdentifier}_EnumTypeConverter))]
   partial {(_state.EnumType.IsValueType ? "struct" : "class")} {_state.EnumIdentifier} : IEquatable<{_state.EnumIdentifier}{_state.NullableQuestionMarkEnum}>
   {{
      [System.Runtime.CompilerServices.ModuleInitializer]
      internal {(_state.HasBaseEnum && _state.BaseEnum.IsSameAssembly ? newKeyword : null)}static void ModuleInit()
      {{
         var convertFromKey = new Func<{_state.KeyType}{_state.NullableQuestionMarkKey}, {_state.EnumIdentifier}{_state.NullableQuestionMarkEnum}>({_state.EnumIdentifier}.Get);
         Expression<Func<{_state.KeyType}{_state.NullableQuestionMarkKey}, {_state.EnumIdentifier}{_state.NullableQuestionMarkEnum}>> convertFromKeyExpression = {_state.KeyArgumentName} => {_state.EnumIdentifier}.Get({_state.KeyArgumentName});

         var convertToKey = new Func<{_state.EnumIdentifier}, {_state.KeyType}{_state.NullableQuestionMarkKey}>(item => item.{_state.KeyPropertyName});
         Expression<Func<{_state.EnumIdentifier}, {_state.KeyType}{_state.NullableQuestionMarkKey}>> convertToKeyExpression = item => item.{_state.KeyPropertyName};

         var validate = new Thinktecture.Internal.Validate<{_state.EnumIdentifier}, {_state.KeyType}>({_state.EnumIdentifier}.Validate);

         var enumType = typeof({_state.EnumIdentifier});
         var metadata = new ValueTypeMetadata(enumType, typeof({_state.KeyType}), {(_state.IsValidatable ? "true" : "false")}, convertFromKey, convertFromKeyExpression, convertToKey, convertToKeyExpression, validate);

         ValueTypeMetadataLookup.AddMetadata(enumType, metadata);");

         foreach (var derivedType in derivedTypes)
         {
            _sb.Append($@"
         ValueTypeMetadataLookup.AddMetadata(typeof({derivedType.Type}), metadata);");
         }

         _sb.Append($@"
      }}

      private static readonly int _typeHashCode = typeof({_state.EnumIdentifier}).GetHashCode() * 397;");

         if (_state.NeedsDefaultComparer)
         {
            var defaultComparer = _state.KeyType.IsString() ? "StringComparer.OrdinalIgnoreCase" : $"EqualityComparer<{_state.KeyType}>.Default";

            _sb.Append($@"
      {(_state.IsExtensible ? "protected" : "private")} static readonly IEqualityComparer<{_state.KeyType}{_state.NullableQuestionMarkKey}> _defaultKeyComparerMember = {defaultComparer};");
         }

         _sb.Append($@"

      private static IReadOnlyDictionary<{_state.KeyType}, {_state.EnumIdentifier}>? _itemsLookup;
      private static IReadOnlyDictionary<{_state.KeyType}, {_state.EnumIdentifier}> ItemsLookup => _itemsLookup ??= GetLookup();

      private static IReadOnlyList<{_state.EnumIdentifier}>? _items;

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public {newKeyword}static IReadOnlyList<{_state.EnumIdentifier}> Items => _items ??= ItemsLookup.Values.ToList().AsReadOnly();");

         if (!_state.HasBaseEnum)
         {
            _sb.Append($@"

      /// <summary>
      /// The identifier of the item.
      /// </summary>
      [NotNull]
      public {_state.KeyType} {_state.KeyPropertyName} {{ get; }}");

            if (_state.IsValidatable)
            {
               _sb.Append($@"

      /// <inheritdoc />
      public bool IsValid {{ get; }}");

               GenerateEnsureValid();
            }
         }

         if (_state.HasBaseEnum)
            GenerateBaseItems(_state.BaseEnum);

         GenerateConstructors();

         if (!_state.HasBaseEnum)
            GenerateGetKey();

         GenerateGet(needCreateInvalidImplementation);

         if (needCreateInvalidImplementation && !_state.EnumType.IsAbstract)
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
         return other is {_state.EnumIdentifier} item && Equals(item);
      }}

      /// <inheritdoc />
      public override int GetHashCode()
      {{
         return _typeHashCode ^ {_state.KeyComparerMember}.GetHashCode(this.{_state.KeyPropertyName});
      }}");

         if (!_state.HasBaseEnum)
         {
            _sb.Append($@"

      /// <inheritdoc />
      public override string? ToString()
      {{
         return this.{_state.KeyPropertyName}{(_state.EnumType.IsValueType && _state.KeyType.IsReferenceType ? "?" : null)}.ToString();
      }}");
         }

         GenerateGetLookup();
      }

      private void GenerateBaseItems(IBaseEnumState baseEnum)
      {
         if (baseEnum.Items.Count == 0)
            return;

         _sb.Append(@"
");

         foreach (var item in baseEnum.Items)
         {
            _sb.Append($@"
      public new static readonly {_state.EnumType} {item.Identifier} = new {_state.EnumType}(");

            for (var i = 0; i < baseEnum.ConstructorArguments.Count; i++)
            {
               if (i > 0)
                  _sb.Append($@", ");

               var arg = baseEnum.ConstructorArguments[i];
               _sb.Append($@"{baseEnum.Type}.{item.Identifier}.{arg.Identifier}");
            }

            _sb.Append($@");");
         }
      }

      private void GenerateTryGet()
      {
         _sb.Append($@"

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name=""{_state.KeyArgumentName}""/> if a valid item exists.
      /// </summary>
      /// <param name=""{_state.KeyArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""{_state.EnumIdentifier}""/>; otherwise <c>null</c>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name=""{_state.KeyArgumentName}""/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([AllowNull] {_state.KeyType} {_state.KeyArgumentName}, [MaybeNullWhen(false)] out {_state.EnumIdentifier} item)
      {{");

         if (_state.KeyType.IsReferenceType)
         {
            _sb.Append($@"
         if ({_state.KeyArgumentName} is null)
         {{
            item = default;
            return false;
         }}
");
         }

         _sb.Append($@"
         return ItemsLookup.TryGetValue({_state.KeyArgumentName}, out item);
      }}");
      }

      private void GenerateValidate()
      {
         _sb.Append($@"

      /// <summary>
      /// Validates the provided <paramref name=""{_state.KeyArgumentName}""/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name=""{_state.KeyArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""{_state.EnumIdentifier}""/>; otherwise <c>null</c>.</param>
      /// <returns> <see cref=""ValidationResult.Success""/> if a valid item with provided <paramref name=""{_state.KeyArgumentName}""/> exists; <see cref=""ValidationResult""/> with an error message otherwise.</returns>
      public static ValidationResult? Validate({_state.KeyType} {_state.KeyArgumentName}, [MaybeNull] out {_state.EnumIdentifier} item)
      {{
         return {_state.EnumIdentifier}.TryGet({_state.KeyArgumentName}, out item)
               ? ValidationResult.Success
               : new ValidationResult($""The enumeration item of type '{_state.EnumIdentifier}' with identifier '{{{_state.KeyArgumentName}}}' is not valid."");
      }}");
      }

      private void GenerateEqualityOperators()
      {
         _sb.Append($@"

      /// <summary>
      /// Compares to instances of <see cref=""{_state.EnumIdentifier}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>true</c> if items are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==({_state.EnumIdentifier}{_state.NullableQuestionMarkEnum} item1, {_state.EnumIdentifier}{_state.NullableQuestionMarkEnum} item2)
      {{");

         if (_state.EnumType.IsReferenceType)
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
      /// Compares to instances of <see cref=""{_state.EnumIdentifier}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>false</c> if items are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=({_state.EnumIdentifier}{_state.NullableQuestionMarkEnum} item1, {_state.EnumIdentifier}{_state.NullableQuestionMarkEnum} item2)
      {{
         return !(item1 == item2);
      }}");
      }

      private void GenerateImplicitConversion()
      {
         _sb.Append($@"

      /// <summary>
      /// Implicit conversion to the type <see cref=""{_state.KeyType}""/>.
      /// </summary>
      /// <param name=""item"">Item to covert.</param>
      /// <returns>The <see cref=""{(_state.HasBaseEnum ? _state.BaseEnum.Type : _state.EnumIdentifier)}.{_state.KeyPropertyName}""/> of provided <paramref name=""item""/> or <c>default</c> if <paramref name=""item""/> is <c>null</c>.</returns>
      [return: NotNullIfNotNull(""item"")]
      public static implicit operator {_state.KeyType}{_state.NullableQuestionMarkKey}({_state.EnumIdentifier}{_state.NullableQuestionMarkEnum} item)
      {{");

         if (_state.EnumType.IsReferenceType)
         {
            _sb.Append($@"
         return item is null ? default : item.{_state.KeyPropertyName};");
         }
         else
         {
            _sb.Append($@"
         return item.{_state.KeyPropertyName};");
         }

         _sb.Append($@"
      }}");
      }

      private void GenerateExplicitConversion()
      {
         _sb.Append($@"

      /// <summary>
      /// Explicit conversion from the type <see cref=""{_state.KeyType}""/>.
      /// </summary>
      /// <param name=""{_state.KeyArgumentName}"">Value to covert.</param>
      /// <returns>An instance of <see cref=""{_state.EnumIdentifier}""/> if the <paramref name=""{_state.KeyArgumentName}""/> is a known item or implements <see cref=""IValidatableEnum{{TKey}}""/>.</returns>
      [return: NotNullIfNotNull(""{_state.KeyArgumentName}"")]
      public static explicit operator {_state.EnumIdentifier}{_state.NullableQuestionMarkEnum}({_state.KeyType}{_state.NullableQuestionMarkKey} {_state.KeyArgumentName})
      {{
         return {_state.EnumIdentifier}.Get({_state.KeyArgumentName});
      }}");
      }

      private void GenerateTypedEquals()
      {
         _sb.Append($@"

      /// <inheritdoc />
      public bool Equals({_state.EnumIdentifier}{_state.NullableQuestionMarkEnum} other)
      {{");

         if (_state.EnumType.IsReferenceType)
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

         if (_state.IsValidatable)
         {
            _sb.Append($@"
         if (this.IsValid != other.IsValid)
            return false;
");
         }

         _sb.Append($@"
         return {_state.KeyComparerMember}.Equals(this.{_state.KeyPropertyName}, other.{_state.KeyPropertyName});
      }}");
      }

      private void GenerateGetLookup()
      {
         _sb.Append($@"

      private static IReadOnlyDictionary<{_state.KeyType}, {_state.EnumIdentifier}> GetLookup()
      {{
         var lookup = new Dictionary<{_state.KeyType}, {_state.EnumIdentifier}>({_state.KeyComparerMember});");

         if (_state.Items.Count > 0)
         {
            _sb.Append($@"

         void AddItem({_state.EnumIdentifier} item, string itemName)
         {{");

            if (_state.EnumType.IsReferenceType)
            {
               _sb.Append($@"
            if(item is null)
               throw new ArgumentNullException($""The item \""{{itemName}}\"" of type \""{_state.EnumIdentifier}\"" must not be null."");
");
            }

            if (_state.KeyType.IsReferenceType)
            {
               _sb.Append($@"
            if(item.{_state.KeyPropertyName} is null)
               throw new ArgumentException($""The \""{_state.KeyPropertyName}\"" of the item \""{{itemName}}\"" of type \""{_state.EnumIdentifier}\"" must not be null."");
");
            }

            if (_state.IsValidatable)
            {
               _sb.Append($@"
            if(!item.IsValid)
               throw new ArgumentException($""All 'public static readonly' fields of type \""{_state.EnumIdentifier}\"" must be valid but the item \""{{itemName}}\"" with the identifier \""{{item.{_state.KeyPropertyName}}}\"" is not."");
");
            }

            _sb.Append($@"
            if (lookup.ContainsKey(item.{_state.KeyPropertyName}))
               throw new ArgumentException($""The type \""{_state.EnumIdentifier}\"" has multiple items with the identifier \""{{item.{_state.KeyPropertyName}}}\""."");

            lookup.Add(item.{_state.KeyPropertyName}, item);
         }}
");

            if (_state.HasBaseEnum)
            {
               foreach (var item in _state.BaseEnum.Items)
               {
                  _sb.Append($@"
         AddItem({item.Identifier}, nameof({item.Identifier}));");
               }
            }

            foreach (var item in _state.Items)
            {
               _sb.Append($@"
         AddItem({item.Name}, nameof({item.Name}));");
            }
         }

         _sb.Append($@"

         return lookup;
      }}
   }}");
      }

      private void GenerateEnsureValid()
      {
         _sb.Append($@"

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref=""InvalidOperationException"">The enumeration item is not valid.</exception>
      public void EnsureValid()
      {{
         if (!IsValid)
            throw new InvalidOperationException($""The current enumeration item of type '{_state.RuntimeTypeName}' with identifier '{{this.{_state.KeyPropertyName}}}' is not valid."");
      }}");
      }

      private void GenerateGetKey()
      {
         _sb.Append($@"

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      {_state.KeyType} IEnum<{_state.KeyType}>.GetKey()
      {{
         return this.{_state.KeyPropertyName};
      }}");
      }

      private void GenerateGet(bool needCreateInvalidImplementation)
      {
         _sb.Append($@"

      /// <summary>
      /// Gets an enumeration item for provided <paramref name=""{_state.KeyArgumentName}""/>.
      /// </summary>
      /// <param name=""{_state.KeyArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref=""{_state.EnumIdentifier}"" /> if <paramref name=""{_state.KeyArgumentName}""/> is not <c>null</c>; otherwise <c>null</c>.</returns>");

         if (!_state.IsValidatable)
         {
            _sb.Append($@"
      /// <exception cref=""KeyNotFoundException"">If there is no item with the provided <paramref name=""{_state.KeyArgumentName}""/>.</exception>");
         }

         _sb.Append($@"
      [return: NotNullIfNotNull(""{_state.KeyArgumentName}"")]
      public {(_state.HasBaseEnum ? "new " : null)}static {_state.EnumIdentifier}{(_state.KeyType.IsReferenceType ? _state.NullableQuestionMarkEnum : null)} Get({_state.KeyType}{_state.NullableQuestionMarkKey} {_state.KeyArgumentName})
      {{");

         if (_state.KeyType.IsReferenceType)
         {
            _sb.Append($@"
        if ({_state.KeyArgumentName} is null)
            return default;
");
         }

         _sb.Append($@"
         if (!ItemsLookup.TryGetValue({_state.KeyArgumentName}, out var item))
         {{");

         if (_state.IsValidatable)
         {
            _sb.Append($@"
            item = {(needCreateInvalidImplementation && _state.EnumType.IsAbstract ? "null" : $"CreateInvalidItem({_state.KeyArgumentName})")};
");

            if (_state.EnumType.IsReferenceType)
            {
               _sb.Append(@"
            if (item is null)
               throw new Exception(""The implementation of method 'CreateInvalidItem' must not return 'null'."");
");
            }

            _sb.Append(@"
            if (item.IsValid)
               throw new Exception(""The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'."");");
         }
         else
         {
            _sb.Append($@"
            throw new KeyNotFoundException($""There is no item of type '{_state.EnumIdentifier}' with the identifier '{{{_state.KeyArgumentName}}}'."");");
         }

         _sb.Append($@"
         }}

         return item;
      }}");
      }

      private void GenerateCreateInvalidItem()
      {
         _sb.Append($@"

      private static {_state.EnumIdentifier} CreateInvalidItem({_state.KeyType} {_state.KeyArgumentName})
      {{
         return new {_state.EnumIdentifier}({_state.KeyArgumentName}, false");

         foreach (var member in _state.AssignableInstanceFieldsAndProperties)
         {
            _sb.Append(", default");

            if (member.Type.IsReferenceType)
               _sb.Append('!');
         }

         if (_state.HasBaseEnum)
         {
            foreach (var arg in _state.BaseEnum.ConstructorArguments.Skip(1))
            {
               _sb.Append(", default");

               if (arg.Type.IsReferenceType)
                  _sb.Append('!');
            }
         }

         _sb.Append($@");
      }}");
      }

      private void GenerateConstructors()
      {
         var baseCtorArgs = _state.BaseEnum?.ConstructorArguments.Skip(1) ?? Array.Empty<ISymbolState>();
         var ctorArgs = _state.AssignableInstanceFieldsAndProperties
                              .Concat(baseCtorArgs);
         var accessibilityModifier = _state.IsExtensible ? "protected" : "private";

         if (_state.IsValidatable)
         {
            _sb.Append($@"

      {accessibilityModifier} {_state.EnumIdentifier}({_state.KeyType} {_state.KeyArgumentName}");

            foreach (var member in ctorArgs)
            {
               _sb.Append($@", {member.Type} {member.ArgumentName}");
            }

            _sb.Append($@")
         : this({_state.KeyArgumentName}, true");

            foreach (var members in ctorArgs)
            {
               _sb.Append($@", {members.ArgumentName}");
            }

            _sb.Append($@")
      {{
      }}");
         }

         _sb.Append($@"

      {accessibilityModifier} {_state.EnumIdentifier}({_state.KeyType} {_state.KeyArgumentName}");

         if (_state.IsValidatable)
            _sb.Append(", bool isValid");

         foreach (var member in ctorArgs)
         {
            _sb.Append($@", {member.Type} {member.ArgumentName}");
         }

         _sb.Append($@")");

         if (_state.HasBaseEnum)
         {
            _sb.Append($@"
         : base({_state.KeyArgumentName}");

            if (_state.IsValidatable)
               _sb.Append($@", isValid");

            foreach (var baseArg in baseCtorArgs)
            {
               _sb.Append($@", {baseArg.ArgumentName}");
            }

            _sb.Append($@")");
         }

         _sb.Append($@"
      {{");

         _sb.Append($@"
         ValidateConstructorArguments(ref {_state.KeyArgumentName}");

         if (_state.IsValidatable)
            _sb.Append(", isValid");

         foreach (var members in ctorArgs)
         {
            _sb.Append($@", ref {members.ArgumentName}");
         }

         _sb.Append($@");
");

         if (!_state.HasBaseEnum && _state.KeyType.IsReferenceType)
         {
            _sb.Append($@"
         if ({_state.KeyArgumentName} is null)
            throw new ArgumentNullException(nameof({_state.KeyArgumentName}));
");
         }

         if (!_state.HasBaseEnum)
         {
            _sb.Append($@"
         this.{_state.KeyPropertyName} = {_state.KeyArgumentName};");

            if (_state.IsValidatable)
            {
               _sb.Append(@"
         this.IsValid = isValid;");
            }
         }

         foreach (var memberInfo in _state.AssignableInstanceFieldsAndProperties)
         {
            _sb.Append($@"
         this.{memberInfo.Identifier} = {memberInfo.ArgumentName};");
         }

         _sb.Append($@"
      }}

      static partial void ValidateConstructorArguments(ref {_state.KeyType} {_state.KeyArgumentName}");

         if (_state.IsValidatable)
            _sb.Append(", bool isValid");

         foreach (var members in ctorArgs)
         {
            _sb.Append($@", ref {members.Type} {members.ArgumentName}");
         }

         _sb.Append($@");");
      }

      private void GenerateTypeConverter()
      {
         _sb.Append($@"
   public class {_state.EnumIdentifier}_EnumTypeConverter : Thinktecture.ValueTypeConverter<{_state.EnumIdentifier}, {_state.KeyType}>
   {{
      /// <inheritdoc />
      [return: NotNullIfNotNull(""{_state.KeyArgumentName}"")]
      protected override {_state.EnumIdentifier}{(_state.KeyType.IsReferenceType ? _state.NullableQuestionMarkEnum : null)} ConvertFrom({_state.KeyType}{_state.NullableQuestionMarkKey} {_state.KeyArgumentName})
      {{");

         if (_state.IsValidatable)
         {
            _sb.Append($@"
         return {_state.EnumIdentifier}.Get({_state.KeyArgumentName});");
         }
         else
         {
            if (_state.KeyType.IsReferenceType)
            {
               _sb.Append($@"
         if({_state.KeyArgumentName} is null)
            return default;
");
            }

            _sb.Append($@"
         if({_state.EnumIdentifier}.TryGet({_state.KeyArgumentName}, out var item))
            return item;

         throw new FormatException($""There is no item of type '{_state.RuntimeTypeName}' with the identifier '{{{_state.KeyArgumentName}}}'."");");
         }

         _sb.Append($@"
      }}

      /// <inheritdoc />
      protected override {_state.KeyType} GetKeyValue({_state.EnumIdentifier} item)
      {{
         return item.{_state.KeyPropertyName};
      }}
   }}
");
      }
   }
}
