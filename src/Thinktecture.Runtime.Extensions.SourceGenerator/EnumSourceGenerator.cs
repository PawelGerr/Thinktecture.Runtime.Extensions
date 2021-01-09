using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture
{
   /// <summary>
   /// Source generator for enum-like class.
   /// </summary>
   [Generator]
   public class EnumSourceGenerator : EnumSourceGeneratorBase
   {
      /// <inheritdoc />
      protected override string GenerateCode(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         var sb = new StringBuilder($@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Linq.Expressions;
using Thinktecture;

{(String.IsNullOrWhiteSpace(state.Namespace) ? null : $"namespace {state.Namespace}")}
{{");
         GenerateTypeConverter(sb, state);
         GenerateEnum(sb, state);

         sb.Append($@"
}}
");

         return sb.ToString();
      }

      private static void GenerateEnum(StringBuilder sb, EnumSourceGeneratorState state)
      {
         var derivedTypes = state.EnumType.FindDerivedInnerTypes();
         var needCreateInvalidImplementation = state.IsValidatable && !state.EnumType.HasCreateInvalidImplementation(state.KeyType);

         if (state.EnumType.IsValueType && !state.HasAttribute("System.Runtime.InteropServices.StructLayoutAttribute"))
         {
            sb.Append($@"
   [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]");
         }

         sb.Append($@"
   [System.ComponentModel.TypeConverter(typeof({state.EnumIdentifier}_EnumTypeConverter))]
   partial {(state.EnumType.IsValueType ? "struct" : "class")} {state.EnumIdentifier} : IEquatable<{state.EnumIdentifier}{state.NullableQuestionMarkEnum}>
   {{
      [System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {{
         var convert = new Func<{state.KeyType}{state.NullableQuestionMarkKey}, {state.EnumIdentifier}{state.NullableQuestionMarkEnum}>({state.EnumIdentifier}.Get);
         Expression<Func<{state.KeyType}{state.NullableQuestionMarkKey}, {state.EnumIdentifier}{state.NullableQuestionMarkEnum}>> convertExpression = {state.KeyArgumentName} => {state.EnumIdentifier}.Get({state.KeyArgumentName});

         var enumType = typeof({state.EnumIdentifier});
         var metadata = new EnumMetadata(enumType, typeof({state.KeyType}), convert, convertExpression);

         EnumMetadataLookup.AddEnumMetadata(enumType, metadata);");

         foreach (var derivedType in derivedTypes)
         {
            sb.Append($@"
         EnumMetadataLookup.AddEnumMetadata(typeof({derivedType.Type}), metadata);");
         }

         sb.Append($@"
      }}

      private static readonly int _typeHashCode = typeof({state.EnumIdentifier}).GetHashCode() * 397;");

         if (state.NeedsDefaultComparer)
         {
            var defaultComparer = state.KeyType.IsString() ? "StringComparer.OrdinalIgnoreCase" : $"EqualityComparer<{state.KeyType}>.Default";

            sb.Append($@"
      private static readonly IEqualityComparer<{state.KeyType}{state.NullableQuestionMarkKey}> _defaultKeyComparerMember = {defaultComparer};");
         }

         sb.Append($@"

      private static IReadOnlyDictionary<{state.KeyType}, {state.EnumIdentifier}>? _itemsLookup;
      private static IReadOnlyDictionary<{state.KeyType}, {state.EnumIdentifier}> ItemsLookup => _itemsLookup ??= GetLookup();

      private static IReadOnlyList<{state.EnumIdentifier}>? _items;

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public static IReadOnlyList<{state.EnumIdentifier}> Items => _items ??= ItemsLookup.Values.ToList().AsReadOnly();

      /// <summary>
      /// The identifier of the item.
      /// </summary>
      [NotNull]
      public {state.KeyType} {state.KeyPropertyName} {{ get; }}");

         if (state.IsValidatable)
         {
            sb.Append($@"

      /// <inheritdoc />
      public bool IsValid {{ get; }}");

            GenerateEnsureValid(sb, state);
         }

         GenerateConstructors(sb, state);
         GenerateGetKey(sb, state);
         GeneratedGet(sb, state, needCreateInvalidImplementation);

         if (needCreateInvalidImplementation && !state.EnumType.IsAbstract)
            GenerateCreateInvalidItem(sb, state);

         GeneratedTryGet(sb, state);
         GenerateImplicitConversion(sb, state);
         GenerateEqualityOperators(sb, state);
         GenerateTypedEquals(sb, state);

         sb.Append($@"

      /// <inheritdoc />
      public override bool Equals(object? other)
      {{
         return other is {state.EnumIdentifier} item && Equals(item);
      }}

      /// <inheritdoc />
      public override int GetHashCode()
      {{
         return _typeHashCode ^ {state.KeyComparerMember}.GetHashCode(this.{state.KeyPropertyName});
      }}

      /// <inheritdoc />
      public override string? ToString()
      {{
         return this.{state.KeyPropertyName}{(state.EnumType.IsValueType && state.KeyType.IsReferenceType ? "?" : null)}.ToString();
      }}");

         GenerateGetLookup(sb, state);
      }

      private static void GeneratedTryGet(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name=""{state.KeyArgumentName}""/> if a valid item exists.
      /// </summary>
      /// <param name=""{state.KeyArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""{state.EnumIdentifier}""/>; otherwise <c>null</c>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name=""{state.KeyArgumentName}""/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([AllowNull] {state.KeyType} {state.KeyArgumentName}, [MaybeNullWhen(false)] out {state.EnumIdentifier} item)
      {{");

         if (state.KeyType.IsReferenceType)
         {
            sb.Append($@"
         if ({state.KeyArgumentName} is null)
         {{
            item = default;
            return false;
         }}
");
         }

         sb.Append($@"
         return ItemsLookup.TryGetValue({state.KeyArgumentName}, out item);
      }}");
      }

      private static void GenerateEqualityOperators(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"

      /// <summary>
      /// Compares to instances of <see cref=""{state.EnumIdentifier}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>true</c> if items are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==({state.EnumIdentifier}{state.NullableQuestionMarkEnum} item1, {state.EnumIdentifier}{state.NullableQuestionMarkEnum} item2)
      {{");

         if (state.EnumType.IsReferenceType)
         {
            sb.Append(@"
         if (item1 is null)
            return item2 is null;
");
         }

         sb.Append($@"
         return item1.Equals(item2);
      }}

      /// <summary>
      /// Compares to instances of <see cref=""{state.EnumIdentifier}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>false</c> if items are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=({state.EnumIdentifier}{state.NullableQuestionMarkEnum} item1, {state.EnumIdentifier}{state.NullableQuestionMarkEnum} item2)
      {{
         return !(item1 == item2);
      }}");
      }

      private static void GenerateImplicitConversion(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"

      /// <summary>
      /// Implicit conversion to the type of <see cref=""{state.KeyType}""/>.
      /// </summary>
      /// <param name=""item"">Item to covert.</param>
      /// <returns>The <see cref=""{state.KeyPropertyName}""/> of provided <paramref name=""item""/> or <c>null</c> if a<paramref name=""item""/> is <c>null</c>.</returns>
      [return: NotNullIfNotNull(""item"")]
      public static implicit operator {state.KeyType}{state.NullableQuestionMarkKey}({state.EnumIdentifier}{state.NullableQuestionMarkEnum} item)
      {{");

         if (state.EnumType.IsReferenceType)
         {
            sb.Append($@"
         return item is null ? default : item.{state.KeyPropertyName};");
         }
         else
         {
            sb.Append($@"
         return item.{state.KeyPropertyName};");
         }

         sb.Append($@"
      }}");
      }

      private static void GenerateTypedEquals(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"

      /// <inheritdoc />
      public bool Equals({state.EnumIdentifier}{state.NullableQuestionMarkEnum} other)
      {{");

         if (state.EnumType.IsReferenceType)
         {
            sb.Append(@"
         if (other is null)
            return false;

         if (!ReferenceEquals(GetType(), other.GetType()))
            return false;

         if (ReferenceEquals(this, other))
            return true;
");
         }

         if (state.IsValidatable)
         {
            sb.Append($@"
         if (this.IsValid != other.IsValid)
            return false;
");
         }

         sb.Append($@"
         return {state.KeyComparerMember}.Equals(this.{state.KeyPropertyName}, other.{state.KeyPropertyName});
      }}");
      }

      private static void GenerateGetLookup(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"

      private static IReadOnlyDictionary<{state.KeyType}, {state.EnumIdentifier}> GetLookup()
      {{
         var lookup = new Dictionary<{state.KeyType}, {state.EnumIdentifier}>({state.KeyComparerMember});");

         if (state.Items.Count > 0)
         {
            sb.Append($@"

         void AddItem({state.EnumIdentifier} item, string itemName)
         {{");

            if (state.EnumType.IsReferenceType)
            {
               sb.Append($@"
            if(item is null)
               throw new ArgumentNullException($""The item \""{{itemName}}\"" of type \""{state.EnumIdentifier}\"" must not be null."");
");
            }

            if (state.KeyType.IsReferenceType)
            {
               sb.Append($@"
            if(item.{state.KeyPropertyName} is null)
               throw new ArgumentException($""The \""{state.KeyPropertyName}\"" of the item \""{{itemName}}\"" of type \""{state.EnumIdentifier}\"" must not be null."");
");
            }

            if (state.IsValidatable)
            {
               sb.Append($@"
            if(!item.IsValid)
               throw new ArgumentException($""All 'public static readonly' fields of type \""{state.EnumIdentifier}\"" must be valid but the item \""{{itemName}}\"" with the identifier \""{{item.{state.KeyPropertyName}}}\"" is not."");
");
            }

            sb.Append($@"
            if (lookup.ContainsKey(item.{state.KeyPropertyName}))
               throw new ArgumentException($""The type \""{state.EnumIdentifier}\"" has multiple items with the identifier \""{{item.{state.KeyPropertyName}}}\""."");

            lookup.Add(item.{state.KeyPropertyName}, item);
         }}
");

            foreach (var item in state.Items)
            {
               sb.Append($@"
         AddItem({item.Name}, ""{item.Name}"");");
            }
         }

         sb.Append($@"

         return lookup;
      }}
   }}");
      }

      private static void GenerateEnsureValid(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref=""InvalidOperationException"">The enumeration item is not valid.</exception>
      public void EnsureValid()
      {{
         if (!IsValid)
            throw new InvalidOperationException($""The current enumeration item of type '{state.EnumIdentifier}' with identifier '{{this.{state.KeyPropertyName}}}' is not valid."");
      }}");
      }

      private static void GenerateGetKey(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      {state.KeyType} IEnum<{state.KeyType}>.GetKey()
      {{
         return this.{state.KeyPropertyName};
      }}");
      }

      private static void GeneratedGet(StringBuilder sb, EnumSourceGeneratorState state, bool needCreateInvalidImplementation)
      {
         sb.Append($@"

      /// <summary>
      /// Gets an enumeration item for provided <paramref name=""{state.KeyArgumentName}""/>.
      /// </summary>
      /// <param name=""{state.KeyArgumentName}"">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref=""{state.EnumIdentifier}"" /> if <paramref name=""{state.KeyArgumentName}""/> is not <c>null</c>; otherwise <c>null</c>.</returns>");

         if (!state.IsValidatable)
         {
            sb.Append($@"
      /// <exception cref=""KeyNotFoundException"">If there is no item with the provided <paramref name=""{state.KeyArgumentName}""/>.</exception>");
         }

         sb.Append($@"
      [return: NotNullIfNotNull(""{state.KeyArgumentName}"")]
      public static {state.EnumIdentifier}{(state.KeyType.IsReferenceType ? state.NullableQuestionMarkEnum : null)} Get({state.KeyType}{state.NullableQuestionMarkKey} {state.KeyArgumentName})
      {{");

         if (state.KeyType.IsReferenceType)
         {
            sb.Append($@"
        if ({state.KeyArgumentName} is null)
            return default;
");
         }

         sb.Append($@"
         if (!ItemsLookup.TryGetValue({state.KeyArgumentName}, out var item))
         {{");

         if (state.IsValidatable)
         {
            sb.Append($@"
            item = {(needCreateInvalidImplementation && state.EnumType.IsAbstract ? "null" : $"CreateInvalidItem({state.KeyArgumentName})")};
");

            if (state.EnumType.IsReferenceType)
            {
               sb.Append(@"
            if (item is null)
               throw new Exception(""The implementation of method 'CreateInvalidItem' must not return 'null'."");
");
            }

            sb.Append(@"
            if (item.IsValid)
               throw new Exception(""The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'."");");
         }
         else
         {
            sb.Append($@"
            throw new KeyNotFoundException($""There is no item of type '{state.EnumIdentifier}' with the identifier '{{{state.KeyArgumentName}}}'."");");
         }

         sb.Append($@"
         }}

         return item;
      }}");
      }

      private static void GenerateCreateInvalidItem(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"

      private static {state.EnumIdentifier} CreateInvalidItem({state.KeyType} {state.KeyArgumentName})
      {{
         return new {state.EnumIdentifier}({state.KeyArgumentName}, false");

         foreach (var member in state.AssignableInstanceFieldsAndProperties)
         {
            sb.Append(", default");

            if (member.Type.IsReferenceType)
               sb.Append('!');
         }

         sb.Append($@");
      }}");
      }

      private static void GenerateConstructors(StringBuilder sb, EnumSourceGeneratorState state)
      {
         var fieldsAndProperties = state.AssignableInstanceFieldsAndProperties;

         if (state.IsValidatable)
         {
            sb.Append($@"

      private {state.EnumIdentifier}({state.KeyType} {state.KeyArgumentName}");

            foreach (var members in fieldsAndProperties)
            {
               sb.Append($@", {members.Type} {members.ArgumentName}");
            }

            sb.Append($@")
         : this({state.KeyArgumentName}, true");

            foreach (var members in fieldsAndProperties)
            {
               sb.Append($@", {members.ArgumentName}");
            }

            sb.Append($@")
      {{
      }}");
         }

         sb.Append($@"

      private {state.EnumIdentifier}({state.KeyType} {state.KeyArgumentName}");

         if (state.IsValidatable)
            sb.Append(", bool isValid");

         foreach (var members in fieldsAndProperties)
         {
            sb.Append($@", {members.Type} {members.ArgumentName}");
         }

         sb.Append($@")
      {{");

         if (state.KeyType.IsReferenceType)
         {
            sb.Append($@"
        if ({state.KeyArgumentName} is null)
            throw new ArgumentNullException(nameof({state.KeyArgumentName}));
");
         }

         sb.Append($@"
         ValidateConstructorArguments({state.KeyArgumentName}");

         if (state.IsValidatable)
            sb.Append(", isValid");

         foreach (var members in fieldsAndProperties)
         {
            sb.Append($@", {members.ArgumentName}");
         }

         sb.Append($@");

         this.{state.KeyPropertyName} = {state.KeyArgumentName};");

         if (state.IsValidatable)
         {
            sb.Append(@"
         this.IsValid = isValid;");
         }

         foreach (var memberInfo in fieldsAndProperties)
         {
            sb.Append($@"
         this.{memberInfo.Symbol.Name} = {memberInfo.ArgumentName};");
         }

         sb.Append($@"
      }}");

         sb.Append($@"

      static partial void ValidateConstructorArguments({state.KeyType} {state.KeyArgumentName}");

         if (state.IsValidatable)
            sb.Append(", bool isValid");

         foreach (var members in fieldsAndProperties)
         {
            sb.Append($@", {members.Type} {members.ArgumentName}");
         }

         sb.Append($@");");
      }

      private static void GenerateTypeConverter(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"
   public class {state.EnumIdentifier}_EnumTypeConverter : Thinktecture.EnumTypeConverter<{state.EnumIdentifier}, {state.KeyType}>
   {{
      /// <inheritdoc />
      [return: NotNullIfNotNull(""{state.KeyArgumentName}"")]
      protected override {state.EnumIdentifier}{(state.KeyType.IsReferenceType ? state.NullableQuestionMarkEnum : null)} ConvertFrom({state.KeyType}{state.NullableQuestionMarkKey} {state.KeyArgumentName})
      {{");

         if (state.IsValidatable)
         {
            sb.Append($@"
         return {state.EnumIdentifier}.Get({state.KeyArgumentName});");
         }
         else
         {
            sb.Append($@"
         if({state.EnumIdentifier}.TryGet({state.KeyArgumentName}, out var item))
            return item;

         throw new NotSupportedException($""There is no item of type '{state.EnumIdentifier}' with the identifier '{{{state.KeyArgumentName}}}'."");");
         }

         sb.Append($@"
      }}
   }}
");
      }
   }
}
