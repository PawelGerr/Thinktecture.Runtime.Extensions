using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   /// <summary>
   /// Source generator for enum-like class.
   /// </summary>
   [Generator]
   public class EnumSourceGenerator : EnumSourceGeneratorBase
   {
      private static readonly DiagnosticDescriptor _classMustBePartial = new("TTRE002", "Class must be partial", "The class '{0}' must be partial", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);
      private static readonly DiagnosticDescriptor _invalidImplementationOfCreateInvalidItem = new("TTRE003", "Incorrect implementation of the method 'CreateInvalidItem'", "The signature of the method 'CreateInvalidItem' must be 'private static {0} CreateInvalidItem({1} key)'", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);

      /// <inheritdoc />
      protected override string GenerateCode(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         var isEnumARefType = state.TypeDeclarationSyntax is ClassDeclarationSyntax;
         var derivedTypes = state.FindDerivedTypes();

         var sb = new StringBuilder($@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Linq.Expressions;
using Thinktecture;

{(String.IsNullOrWhiteSpace(state.Namespace) ? null : $"namespace {state.Namespace}")}
{{
   public class {state.EnumType}_EnumTypeConverter : Thinktecture.EnumTypeConverter<{state.EnumType}, {state.KeyType}>
   {{
      /// <inheritdoc />
      [return: NotNullIfNotNull(""key"")]
      protected override {state.EnumType}{(state.IsKeyARefType ? state.NullableQuestionMarkEnum : null)} ConvertFrom({state.KeyType}{state.NullableQuestionMarkKey} key)
      {{
         return {state.EnumType}.Get(key);
      }}
   }}

   [System.ComponentModel.TypeConverter(typeof({state.TypeDeclarationSyntax.Identifier}_EnumTypeConverter))]
   partial {(isEnumARefType ? "class" : "struct")} {state.EnumType} : IEquatable<{state.EnumType}{state.NullableQuestionMarkEnum}>
   {{
      [System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {{
         var convert = new Func<{state.KeyType}, {state.EnumType}>({state.EnumType}.Get);
         Expression<Func<{state.KeyType}, {state.EnumType}>> convertExpression = key => {state.EnumType}.Get(key);

         var metadata = new EnumMetadata(typeof({state.EnumType}), typeof({state.KeyType}), convert, convertExpression);

         EnumMetadataLookup.AddEnumMetadata(typeof({state.EnumType}), metadata);");

         foreach (var typeDeclaration in derivedTypes)
         {
            var typeInfo = state.Model.GetDeclaredSymbol(typeDeclaration);

            sb.Append($@"
         EnumMetadataLookup.AddEnumMetadata(typeof({typeInfo}), metadata);");
         }

         sb.Append($@"
      }}

      private static readonly int _typeHashCode = typeof({state.EnumType}).GetHashCode() * 397;
");

         if (state.NeedsDefaultComparer)
         {
            var defaultComparer = state.KeyType.IsString() ? "StringComparer.OrdinalIgnoreCase" : $"EqualityComparer<{state.KeyType}>.Default";

            sb.Append($@"
      private static readonly IEqualityComparer<{state.KeyType}> _defaultKeyComparerMember = {defaultComparer};
");
         }

         sb.Append($@"
      private static IReadOnlyDictionary<{state.KeyType}, {state.EnumType}>? _itemsLookup;
      private static IReadOnlyDictionary<{state.KeyType}, {state.EnumType}> ItemsLookup => _itemsLookup ??= GetLookup();

      private static IReadOnlyList<{state.EnumType}>? _items;
      private static IReadOnlyList<{state.EnumType}> Items => _items ??= ItemsLookup.Values.ToList().AsReadOnly();

      /// <summary>
      /// The key of the enumeration item.
      /// </summary>
      [NotNull]
      public {state.KeyType} {state.KeyPropertyName} {{ get; }}

      /// <inheritdoc />
      public bool IsValid {{ get; init; }}

#nullable disable
      private {state.EnumType}({state.KeyType} key)
      {{");

         if (state.IsKeyARefType)
         {
            sb.Append(@"
        if (key is null)
            throw new ArgumentNullException(nameof(key));
");
         }

         sb.Append($@"
         {state.KeyPropertyName} = key;
         IsValid = true;
      }}
#nullable enable

      static partial void InitializeInvalidItem({state.EnumType} invalidItem);

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref=""InvalidOperationException"">The enumeration item is not valid.</exception>
      public void EnsureValid()
      {{
         if (!IsValid)
            throw new InvalidOperationException($""The current enumeration item of type '{state.EnumType}' with key '{{{state.KeyPropertyName}}}' is not valid."");
      }}

      /// <summary>
      /// Gets the key of the item.
      /// </summary>
      {state.KeyType} IEnum<{state.KeyType}>.GetKey()
      {{
         return {state.KeyPropertyName};
      }}

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      /// <returns>A collection with all valid items.</returns>
      public static IReadOnlyList<{state.EnumType}> GetAll()
      {{
         return Items;
      }}

      /// <summary>
      /// Gets an enumeration item for provided <paramref name=""key""/>.
      /// </summary>
      /// <param name=""key"">The key to return an enumeration item for.</param>
      /// <returns>An instance of <see cref=""{state.EnumType}"" /> if <paramref name=""key""/> is not <c>null</c>; otherwise <c>null</c>.</returns>
      [return: NotNullIfNotNull(""key"")]
      public static {state.EnumType}{(state.IsKeyARefType ? state.NullableQuestionMarkEnum : null)} Get({state.KeyType}{state.NullableQuestionMarkKey} key)
      {{");

         if (state.IsKeyARefType)
         {
            sb.Append(@"
        if (key is null)
            return default;
");
         }

         sb.Append(@"
         if (!ItemsLookup.TryGetValue(key, out var item))
         {
            item = CreateInvalidItem(key);
");

         if (isEnumARefType)
         {
            sb.Append(@"
            if(item is null)
               throw new Exception(""The implementation of method 'CreateInvalidItem' must not return 'null'."");
");
         }

         sb.Append(@"
            if(item.IsValid)
               throw new Exception(""The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'."");

            InitializeInvalidItem(item);
         }

         return item;
      }
");

         if (NeedCreateInvalidImplementation(state))
         {
            sb.Append($@"
      private static {state.EnumType} CreateInvalidItem({state.KeyType} key)
      {{
         return new {state.EnumType}(key) {{ IsValid = false }};
      }}
");
         }

         sb.Append($@"

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name=""key""/> if a valid item exists.
      /// </summary>
      /// <param name=""key"">The key to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""{state.EnumType}""/>; otherwise <c>null</c>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name=""key""/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([AllowNull] {state.KeyType} key, [MaybeNullWhen(false)] out {state.EnumType} item)
      {{");

         if (state.IsKeyARefType)
         {
            sb.Append(@"
         if (key is null)
         {{
            item = default;
            return false;
         }}
");
         }

         sb.Append($@"
         return ItemsLookup.TryGetValue(key, out item);
      }}

      /// <summary>
      /// Implicit conversion to the type of <see cref=""{state.KeyType}""/>.
      /// </summary>
      /// <param name=""item"">Item to covert.</param>
      /// <returns>The <see cref=""{state.KeyPropertyName}""/> of provided <paramref name=""item""/> or <c>null</c> if a<paramref name=""item""/> is <c>null</c>.</returns>
      [return: NotNullIfNotNull(""item"")]
      public static implicit operator {state.KeyType}{state.NullableQuestionMarkKey}({state.EnumType}{state.NullableQuestionMarkEnum} item)
      {{");

         if (isEnumARefType)
         {
            sb.Append($@"
         return item is null ? default : item.{state.KeyPropertyName};
");
         }
         else
         {
            sb.Append($@"
         return item.{state.KeyPropertyName};
");
         }

         sb.Append($@"
      }}

      /// <summary>
      /// Compares to instances of <see cref=""{state.EnumType}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>true</c> if items are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==({state.EnumType}{state.NullableQuestionMarkEnum} item1, {state.EnumType}{state.NullableQuestionMarkEnum} item2)
      {{");

         if (isEnumARefType)
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
      /// Compares to instances of <see cref=""{state.EnumType}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>false</c> if items are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=({state.EnumType}{state.NullableQuestionMarkEnum} item1, {state.EnumType}{state.NullableQuestionMarkEnum} item2)
      {{
         return !(item1 == item2);
      }}

      /// <inheritdoc />
      public bool Equals({state.EnumType}{state.NullableQuestionMarkEnum} other)
      {{");

         if (isEnumARefType)
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

         sb.Append($@"
         if (IsValid != other.IsValid)
            return false;

         return {state.KeyComparerMember}.Equals({state.KeyPropertyName}, other.{state.KeyPropertyName});
      }}

      /// <inheritdoc />
      public override bool Equals(object? otherEnum)
      {{
         return otherEnum is {state.EnumType} item && Equals(item);
      }}

      /// <inheritdoc />
      public override int GetHashCode()
      {{
         return _typeHashCode ^ {state.KeyComparerMember}.GetHashCode({state.KeyPropertyName});
      }}

      /// <inheritdoc />
      public override string? ToString()
      {{
         return {state.KeyPropertyName}.ToString();
      }}

      private static IReadOnlyDictionary<{state.KeyType}, {state.EnumType}> GetLookup()
      {{
         var items = new {state.EnumType}[] {{ {String.Join(", ", state.Items.Select(i => i.Declaration.Variables[0].Identifier))} }};
         var lookup = new Dictionary<{state.KeyType}, {state.EnumType}>({state.KeyComparerMember});

         foreach (var item in items)
         {{
            if(!item.IsValid)
               throw new ArgumentException(""All 'public static readonly' fields of type \""{state.EnumType}\"" must be valid but the item with the key \""{{item.Key}}\"" is not."");

            if (lookup.ContainsKey(item.{state.KeyPropertyName}))
               throw new ArgumentException($""The type \""{state.EnumType}\"" has multiple items with the key \""{{item.{state.KeyPropertyName}}}\""."");

            lookup.Add(item.{state.KeyPropertyName}, item);
         }}

         return lookup;
      }}
   }}
}}
");

         return sb.ToString();
      }

      /// <inheritdoc />
      protected override bool IsValid(EnumDeclaration enumDeclaration, GeneratorExecutionContext context, SemanticModel model)
      {
         if (enumDeclaration is null)
            throw new ArgumentNullException(nameof(enumDeclaration));

         var isValid = base.IsValid(enumDeclaration, context, model);

         if (isValid)
         {
            if (!enumDeclaration.TypeDeclarationSyntax.IsPartial())
            {
               context.ReportDiagnostic(Diagnostic.Create(_classMustBePartial,
                                                          enumDeclaration.TypeDeclarationSyntax.GetLocation(),
                                                          enumDeclaration.TypeDeclarationSyntax.Identifier));
               return false;
            }
         }

         return true;
      }

      private static bool NeedCreateInvalidImplementation(EnumSourceGeneratorState state)
      {
         foreach (var member in state.TypeDeclarationSyntax.Members)
         {
            if (member is MethodDeclarationSyntax method)
            {
               if (method.Identifier.Text == "CreateInvalidItem")
               {
                  if (method.ParameterList.Parameters.Count == 1)
                  {
                     var paramTypeSyntax = method.ParameterList.Parameters[0].Type;
                     var parameterType = paramTypeSyntax is not null ? state.Model.GetTypeInfo(paramTypeSyntax).Type : null;
                     var returnType = state.Model.GetTypeInfo(method.ReturnType).Type;

                     if (member.IsStatic() &&
                         SymbolEqualityComparer.Default.Equals(parameterType, state.KeyType) &&
                         SymbolEqualityComparer.Default.Equals(returnType, state.ClassTypeInfo))
                     {
                        return false;
                     }
                  }

                  state.Context.ReportDiagnostic(Diagnostic.Create(_invalidImplementationOfCreateInvalidItem,
                                                                   method.GetLocation(),
                                                                   state.TypeDeclarationSyntax.Identifier,
                                                                   state.KeyType));
               }
            }
         }

         return true;
      }
   }
}
