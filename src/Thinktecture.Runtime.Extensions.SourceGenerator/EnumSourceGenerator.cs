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
      private static readonly DiagnosticDescriptor _classMustBePartial = new("TTRESG020", "Class must be partial", "The class '{0}' must be partial", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);
      private static readonly DiagnosticDescriptor _invalidImplementationOfCreateInvalidItem = new("TTRESG021", "Incorrect implementation of the method 'CreateInvalidItem'", "The signature of the method 'CreateInvalidItem' must be 'private static {0} CreateInvalidItem({1} key)'", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);
      private static readonly DiagnosticDescriptor _abstractEnumNeedsCreateInvalidItemImplementation = new("TTRESG022", "An abstract class needs an implementation of the method 'CreateInvalidItem'", "An abstract class needs an implementation of the method 'CreateInvalidItem', the signature should be 'private static {0} CreateInvalidItem({1} key)'", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);
      private static readonly DiagnosticDescriptor _constructorsMustBePrivate = new("TTRESG023", "An enumeration must have private constructors only", "All constructors of the enumeration '{0}' must be private", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);
      private static readonly DiagnosticDescriptor _nonValidatableEnumsMustBeClass = new("TTRESG024", "An non-validatable enumeration must be a class", "A non-validatable enum must be a class", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);

      private static readonly DiagnosticDescriptor _noItemsWarning = new("TTRESG100", "The enumeration has no items", "The enumeration '{0}' has no items", nameof(EnumSourceGenerator), DiagnosticSeverity.Warning, true);

      /// <inheritdoc />
      protected override string GenerateCode(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         if (state.Items.Count == 0)
         {
            state.Context.ReportDiagnostic(Diagnostic.Create(_noItemsWarning,
                                                             state.TypeDeclarationSyntax.GetLocation(),
                                                             state.EnumIdentifier));
         }

         if (!state.IsValidatable && !state.IsEnumARefType)
         {
            state.Context.ReportDiagnostic(Diagnostic.Create(_nonValidatableEnumsMustBeClass,
                                                             state.EnumIdentifier.GetLocation(),
                                                             state.EnumIdentifier));
         }

         CheckConstructors(state);

         var derivedTypes = state.FindDerivedTypes();
         var needCreateInvalidImplementation = NeedCreateInvalidImplementation(state);

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

         sb.Append($@"
   [System.ComponentModel.TypeConverter(typeof({state.TypeDeclarationSyntax.Identifier}_EnumTypeConverter))]
   partial {(state.IsEnumARefType ? "class" : "struct")} {state.EnumIdentifier} : IEquatable<{state.EnumIdentifier}{state.NullableQuestionMarkEnum}>
   {{
      [System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {{
         var convert = new Func<{state.KeyType}, {state.EnumIdentifier}>({state.EnumIdentifier}.Get);
         Expression<Func<{state.KeyType}, {state.EnumIdentifier}>> convertExpression = key => {state.EnumIdentifier}.Get(key);

         var metadata = new EnumMetadata(typeof({state.EnumIdentifier}), typeof({state.KeyType}), convert, convertExpression);

         EnumMetadataLookup.AddEnumMetadata(typeof({state.EnumIdentifier}), metadata);");

         foreach (var typeDeclaration in derivedTypes)
         {
            var typeInfo = state.Model.GetDeclaredSymbol(typeDeclaration);

            sb.Append($@"
         EnumMetadataLookup.AddEnumMetadata(typeof({typeInfo}), metadata);");
         }

         sb.Append($@"
      }}

      private static readonly int _typeHashCode = typeof({state.EnumIdentifier}).GetHashCode() * 397;
");

         if (state.NeedsDefaultComparer)
         {
            var defaultComparer = state.KeyType.IsString() ? "StringComparer.OrdinalIgnoreCase" : $"EqualityComparer<{state.KeyType}>.Default";

            sb.Append($@"
      private static readonly IEqualityComparer<{state.KeyType}> _defaultKeyComparerMember = {defaultComparer};
");
         }

         sb.Append($@"
      private static IReadOnlyDictionary<{state.KeyType}, {state.EnumIdentifier}>? _itemsLookup;
      private static IReadOnlyDictionary<{state.KeyType}, {state.EnumIdentifier}> ItemsLookup => _itemsLookup ??= GetLookup();

      private static IReadOnlyList<{state.EnumIdentifier}>? _items;
      private static IReadOnlyList<{state.EnumIdentifier}> Items => _items ??= ItemsLookup.Values.ToList().AsReadOnly();

      /// <summary>
      /// The key of the enumeration item.
      /// </summary>
      [NotNull]
      public {state.KeyType} {state.KeyPropertyName} {{ get; }}");

         if (state.IsValidatable)
         {
            sb.Append($@"

      /// <inheritdoc />
      public bool IsValid {{ get; init; }}");
         }

         sb.Append($@"

#nullable disable
      private {state.EnumIdentifier}({state.KeyType} key)
      {{");

         if (state.IsKeyARefType)
         {
            sb.Append(@"
        if (key is null)
            throw new ArgumentNullException(nameof(key));
");
         }

         sb.Append($@"
         {state.KeyPropertyName} = key;");

         if (state.IsValidatable)
         {
            sb.Append($@"
         IsValid = true;");
         }

         sb.Append($@"
      }}
#nullable enable
");

         if (state.IsValidatable)
         {
            sb.Append($@"
      static partial void InitializeInvalidItem({state.EnumIdentifier} invalidItem);

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref=""InvalidOperationException"">The enumeration item is not valid.</exception>
      public void EnsureValid()
      {{
         if (!IsValid)
            throw new InvalidOperationException($""The current enumeration item of type '{state.EnumIdentifier}' with key '{{{state.KeyPropertyName}}}' is not valid."");
      }}");
         }

         sb.Append($@"

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
      public static IReadOnlyList<{state.EnumIdentifier}> GetAll()
      {{
         return Items;
      }}

      /// <summary>
      /// Gets an enumeration item for provided <paramref name=""key""/>.
      /// </summary>
      /// <param name=""key"">The key to return an enumeration item for.</param>
      /// <returns>An instance of <see cref=""{state.EnumIdentifier}"" /> if <paramref name=""key""/> is not <c>null</c>; otherwise <c>null</c>.</returns>");

         if (!state.IsValidatable)
         {
            sb.Append($@"
      /// <exception cref=""KeyNotFoundException"">If there is no item with the provided <paramref name=""key""/>.</exception>");
         }

         sb.Append($@"
      [return: NotNullIfNotNull(""key"")]
      public static {state.EnumIdentifier}{(state.IsKeyARefType ? state.NullableQuestionMarkEnum : null)} Get({state.KeyType}{state.NullableQuestionMarkKey} key)
      {{");

         if (state.IsKeyARefType)
         {
            sb.Append(@"
        if (key is null)
            return default;
");
         }

         sb.Append($@"
         if (!ItemsLookup.TryGetValue(key, out var item))
         {{
");

         if (state.IsValidatable)
         {
            sb.Append($@"
            item = {(needCreateInvalidImplementation && state.EnumTypeInfo.IsAbstract ? "null" : "CreateInvalidItem(key)")};
");

            if (state.IsEnumARefType)
            {
               sb.Append(@"
            if (item is null)
               throw new Exception(""The implementation of method 'CreateInvalidItem' must not return 'null'."");
");
            }

            sb.Append(@"
            if (item.IsValid)
               throw new Exception(""The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'."");

            InitializeInvalidItem(item);
");
         }
         else
         {
            sb.Append($@"
            throw new KeyNotFoundException($""There is no item of type '{state.EnumIdentifier}' with the key '{{key}}'."");
");
         }

         sb.Append($@"
         }}

         return item;
      }}
");

         if (needCreateInvalidImplementation)
         {
            if (state.EnumTypeInfo.IsAbstract)
            {
               state.Context.ReportDiagnostic(Diagnostic.Create(_abstractEnumNeedsCreateInvalidItemImplementation,
                                                                state.TypeDeclarationSyntax.GetLocation(),
                                                                state.TypeDeclarationSyntax.Identifier,
                                                                state.KeyType));
            }
            else
            {
               sb.Append($@"
      private static {state.EnumIdentifier} CreateInvalidItem({state.KeyType} key)
      {{
         return new {state.EnumIdentifier}(key) {{ IsValid = false }};
      }}
");
            }
         }

         sb.Append($@"

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name=""key""/> if a valid item exists.
      /// </summary>
      /// <param name=""key"">The key to return an enumeration item for.</param>
      /// <param name=""item"">A valid instance of <see cref=""{state.EnumIdentifier}""/>; otherwise <c>null</c>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name=""key""/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([AllowNull] {state.KeyType} key, [MaybeNullWhen(false)] out {state.EnumIdentifier} item)
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
      public static implicit operator {state.KeyType}{state.NullableQuestionMarkKey}({state.EnumIdentifier}{state.NullableQuestionMarkEnum} item)
      {{");

         if (state.IsEnumARefType)
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
      /// Compares to instances of <see cref=""{state.EnumIdentifier}""/>.
      /// </summary>
      /// <param name=""item1"">Instance to compare.</param>
      /// <param name=""item2"">Another instance to compare.</param>
      /// <returns><c>true</c> if items are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==({state.EnumIdentifier}{state.NullableQuestionMarkEnum} item1, {state.EnumIdentifier}{state.NullableQuestionMarkEnum} item2)
      {{");

         if (state.IsEnumARefType)
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
      }}

      /// <inheritdoc />
      public bool Equals({state.EnumIdentifier}{state.NullableQuestionMarkEnum} other)
      {{");

         if (state.IsEnumARefType)
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
         if (IsValid != other.IsValid)
            return false;");
         }

         sb.Append($@"

         return {state.KeyComparerMember}.Equals({state.KeyPropertyName}, other.{state.KeyPropertyName});
      }}

      /// <inheritdoc />
      public override bool Equals(object? otherEnum)
      {{
         return otherEnum is {state.EnumIdentifier} item && Equals(item);
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

      private static IReadOnlyDictionary<{state.KeyType}, {state.EnumIdentifier}> GetLookup()
      {{
         var items = new {state.EnumIdentifier}[] {{ {String.Join(", ", state.Items.Select(i => i.Declaration.Variables[0].Identifier))} }};
         var lookup = new Dictionary<{state.KeyType}, {state.EnumIdentifier}>({state.KeyComparerMember});

         foreach (var item in items)
         {{");

         if (state.IsValidatable)
         {
            sb.Append($@"
            if(!item.IsValid)
               throw new ArgumentException($""All 'public static readonly' fields of type \""{state.EnumIdentifier}\"" must be valid but the item with the key \""{{item.{state.KeyPropertyName}}}\"" is not."");");
         }

         sb.Append($@"

            if (lookup.ContainsKey(item.{state.KeyPropertyName}))
               throw new ArgumentException($""The type \""{state.EnumIdentifier}\"" has multiple items with the key \""{{item.{state.KeyPropertyName}}}\""."");

            lookup.Add(item.{state.KeyPropertyName}, item);
         }}

         return lookup;
      }}
   }}
}}
");

         return sb.ToString();
      }

      private static void GenerateTypeConverter(StringBuilder sb, EnumSourceGeneratorState state)
      {
         sb.Append($@"
   public class {state.EnumIdentifier}_EnumTypeConverter : Thinktecture.EnumTypeConverter<{state.EnumIdentifier}, {state.KeyType}>
   {{
      /// <inheritdoc />
      [return: NotNullIfNotNull(""key"")]
      protected override {state.EnumIdentifier}{(state.IsKeyARefType ? state.NullableQuestionMarkEnum : null)} ConvertFrom({state.KeyType}{state.NullableQuestionMarkKey} key)
      {{");

         if (state.IsValidatable)
         {
            sb.Append($@"
         return {state.EnumIdentifier}.Get(key);");
         }
         else
         {
            sb.Append($@"
         if({state.EnumIdentifier}.TryGet(key, out var item))
            return item;

         throw new NotSupportedException($""There is no item of type '{state.EnumIdentifier}' with the key '{{key}}'."");");
         }

         sb.Append($@"
      }}
   }}
");
      }

      private static void CheckConstructors(EnumSourceGeneratorState state)
      {
         foreach (var declarationSyntax in state.TypeDeclarationSyntax.Members)
         {
            if (declarationSyntax is ConstructorDeclarationSyntax constructor)
            {
               if (!IsPrivate(constructor))
               {
                  state.Context.ReportDiagnostic(Diagnostic.Create(_constructorsMustBePrivate,
                                                                   constructor.GetLocation(),
                                                                   state.TypeDeclarationSyntax.Identifier));
               }
            }
         }
      }

      private static bool IsPrivate(MemberDeclarationSyntax constructor)
      {
         var isPrivate = false;

         foreach (var modifier in constructor.Modifiers)
         {
            switch ((SyntaxKind)modifier.RawKind)
            {
               case SyntaxKind.PrivateKeyword:
                  isPrivate = true;
                  break;

               case SyntaxKind.ProtectedKeyword:
               case SyntaxKind.InternalKeyword:
               case SyntaxKind.PublicKeyword:
                  return false;
            }
         }

         return isPrivate;
      }

      /// <inheritdoc />
      protected override EnumInterfaceInfo? GetValidEnumInterface(EnumDeclaration enumDeclaration, GeneratorExecutionContext context, SemanticModel model)
      {
         if (enumDeclaration is null)
            throw new ArgumentNullException(nameof(enumDeclaration));

         var enumInterface = base.GetValidEnumInterface(enumDeclaration, context, model);

         if (enumInterface != null && !enumDeclaration.TypeDeclarationSyntax.IsPartial())
         {
            context.ReportDiagnostic(Diagnostic.Create(_classMustBePartial,
                                                       enumDeclaration.TypeDeclarationSyntax.GetLocation(),
                                                       enumDeclaration.TypeDeclarationSyntax.Identifier));
            return null;
         }

         return enumInterface;
      }

      private static bool NeedCreateInvalidImplementation(EnumSourceGeneratorState state)
      {
         if (!state.IsValidatable)
            return false;

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
                         SymbolEqualityComparer.Default.Equals(returnType, state.EnumTypeInfo))
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
