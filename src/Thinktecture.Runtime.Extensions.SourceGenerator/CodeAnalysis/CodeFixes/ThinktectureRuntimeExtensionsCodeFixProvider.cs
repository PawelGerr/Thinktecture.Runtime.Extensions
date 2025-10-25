using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ThinktectureRuntimeExtensionsCodeFixProvider))]
public sealed class ThinktectureRuntimeExtensionsCodeFixProvider : CodeFixProvider
{
   private const string _MAKE_PARTIAL = "Make the type partial";
   private const string _MAKE_METHOD_PARTIAL = "Make the method partial";
   private const string _MAKE_MEMBER_PUBLIC = "Make the member public";
   private const string _MAKE_MEMBER_REQUIRED = "Make the member required";
   private const string _MAKE_FIELD_READONLY = "Make the field read-only";
   private const string _REMOVE_PROPERTY_SETTER = "Remove property setter";
   private const string _MAKE_INIT_PRIVATE = "Make init private";
   private const string _MAKE_TYPE_PRIVATE = "Make type private";
   private const string _MAKE_TYPE_PUBLIC = "Make type public";
   private const string _SEAL_CLASS = "Seal class";
   private const string _DEFINE_VALUE_OBJECT_EQUALITY_COMPARER = "Define Value Object equality comparer";
   private const string _DEFINE_VALUE_OBJECT_COMPARER = "Define Value Object comparer";

   /// <inheritdoc />
   public override ImmutableArray<string> FixableDiagnosticIds { get; } =
   [
      DiagnosticsDescriptors.TypeMustBePartial.Id,
      DiagnosticsDescriptors.SmartEnumItemMustBePublic.Id,
      DiagnosticsDescriptors.FieldMustBeReadOnly.Id,
      DiagnosticsDescriptors.PropertyMustBeReadOnly.Id,
      DiagnosticsDescriptors.InnerSmartEnumOnFirstLevelMustBePrivate.Id,
      DiagnosticsDescriptors.InnerSmartEnumOnNonFirstLevelMustBePublic.Id,
      DiagnosticsDescriptors.SmartEnumWithoutDerivedTypesMustBeSealed.Id,
      DiagnosticsDescriptors.InitAccessorMustBePrivate.Id,
      DiagnosticsDescriptors.StringBasedValueObjectNeedsEqualityComparer.Id,
      DiagnosticsDescriptors.ComplexValueObjectWithStringMembersNeedsDefaultEqualityComparer.Id,
      DiagnosticsDescriptors.ExplicitComparerWithoutEqualityComparer.Id,
      DiagnosticsDescriptors.ExplicitEqualityComparerWithoutComparer.Id,
      DiagnosticsDescriptors.MethodWithUseDelegateFromConstructorMustBePartial.Id,
      DiagnosticsDescriptors.UnionRecordMustBeSealed.Id,
      DiagnosticsDescriptors.MembersDisallowingDefaultValuesMustBeRequired.Id,
   ];

   /// <inheritdoc />
   public override FixAllProvider GetFixAllProvider()
   {
      return WellKnownFixAllProviders.BatchFixer;
   }

   /// <inheritdoc />
   public override async Task RegisterCodeFixesAsync(CodeFixContext context)
   {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      if (root is null)
         return;

      foreach (var diagnostic in context.Diagnostics)
      {
         CodeFixesContext? ctx = null;
         CodeFixesContext GetCodeFixesContext() => ctx ??= new CodeFixesContext(diagnostic, root);

         if (diagnostic.Id == DiagnosticsDescriptors.TypeMustBePartial.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_PARTIAL, _ => AddTypeModifierAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, SyntaxKind.PartialKeyword), _MAKE_PARTIAL), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.FieldMustBeReadOnly.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_FIELD_READONLY, _ => AddTypeModifierAsync(context.Document, root, GetCodeFixesContext().FieldDeclaration, SyntaxKind.ReadOnlyKeyword), _MAKE_FIELD_READONLY), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.SmartEnumItemMustBePublic.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_MEMBER_PUBLIC, _ => ChangeAccessibilityAsync(context.Document, root, GetCodeFixesContext().FieldDeclaration, SyntaxKind.PublicKeyword), _MAKE_MEMBER_PUBLIC), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.PropertyMustBeReadOnly.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_REMOVE_PROPERTY_SETTER, _ => RemovePropertySetterAsync(context.Document, root, GetCodeFixesContext().PropertyDeclaration), _REMOVE_PROPERTY_SETTER), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.InitAccessorMustBePrivate.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_INIT_PRIVATE, _ => ChangeAccessibilityAsync(context.Document, root, GetCodeFixesContext().PropertyDeclaration?.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.InitAccessorDeclaration)), SyntaxKind.PrivateKeyword), _MAKE_INIT_PRIVATE), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.InnerSmartEnumOnFirstLevelMustBePrivate.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_TYPE_PRIVATE, _ => ChangeAccessibilityAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, SyntaxKind.PrivateKeyword), _MAKE_TYPE_PRIVATE), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.InnerSmartEnumOnNonFirstLevelMustBePublic.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_TYPE_PUBLIC, _ => ChangeAccessibilityAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, SyntaxKind.PublicKeyword), _MAKE_TYPE_PUBLIC), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.SmartEnumWithoutDerivedTypesMustBeSealed.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_SEAL_CLASS, _ => ReplaceOrAddTypeModifierAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, SyntaxKind.AbstractKeyword, SyntaxKind.SealedKeyword), _SEAL_CLASS), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.StringBasedValueObjectNeedsEqualityComparer.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_DEFINE_VALUE_OBJECT_EQUALITY_COMPARER, t => AddKeyMemberEqualityComparerAttributeAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, t), _DEFINE_VALUE_OBJECT_EQUALITY_COMPARER), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.ComplexValueObjectWithStringMembersNeedsDefaultEqualityComparer.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_DEFINE_VALUE_OBJECT_EQUALITY_COMPARER, t => AddDefaultStringComparisonAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, t), _DEFINE_VALUE_OBJECT_EQUALITY_COMPARER), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.ExplicitComparerWithoutEqualityComparer.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_DEFINE_VALUE_OBJECT_EQUALITY_COMPARER, t => AddKeyMemberEqualityComparerAttributeAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, t), _DEFINE_VALUE_OBJECT_EQUALITY_COMPARER), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.ExplicitEqualityComparerWithoutComparer.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_DEFINE_VALUE_OBJECT_COMPARER, t => AddKeyMemberComparerAttributeAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, t), _DEFINE_VALUE_OBJECT_COMPARER), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.MethodWithUseDelegateFromConstructorMustBePartial.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_METHOD_PARTIAL, _ => AddTypeModifierAsync(context.Document, root, GetCodeFixesContext().MethodDeclaration, SyntaxKind.PartialKeyword), _MAKE_METHOD_PARTIAL), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.UnionRecordMustBeSealed.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_SEAL_CLASS, _ => ReplaceOrAddTypeModifierAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, SyntaxKind.AbstractKeyword, SyntaxKind.SealedKeyword), _SEAL_CLASS), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.MembersDisallowingDefaultValuesMustBeRequired.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_MEMBER_REQUIRED, _ => AddTypeModifierAsync(context.Document, root, GetCodeFixesContext().MemberDeclaration, SyntaxKind.RequiredKeyword), _MAKE_MEMBER_REQUIRED), diagnostic);
         }
      }
   }

   private static Task<Document> AddTypeModifierAsync(
      Document document,
      SyntaxNode root,
      MemberDeclarationSyntax? declaration,
      SyntaxKind modifier)
   {
      if (declaration is null)
         return Task.FromResult(document);

      var newModifier = SyntaxFactory.Token(modifier);
      var indexOfPartialKeyword = declaration.Modifiers.IndexOf(SyntaxKind.PartialKeyword);
      var newDeclaration = indexOfPartialKeyword < 0 ? declaration.AddModifiers(newModifier) : declaration.WithModifiers(declaration.Modifiers.Insert(indexOfPartialKeyword, newModifier));
      var newRoot = root.ReplaceNode(declaration, newDeclaration);
      var newDoc = document.WithSyntaxRoot(newRoot);

      return Task.FromResult(newDoc);
   }

   private static Task<Document> ReplaceOrAddTypeModifierAsync(
      Document document,
      SyntaxNode root,
      MemberDeclarationSyntax? declaration,
      SyntaxKind oldModifier,
      SyntaxKind newModifier)
   {
      if (declaration is null)
         return Task.FromResult(document);

      var newModifierToken = SyntaxFactory.Token(newModifier);
      var indexOfOldModifier = declaration.Modifiers.IndexOf(oldModifier);

      MemberDeclarationSyntax newDeclaration;

      if (indexOfOldModifier >= 0)
      {
         var oldToken = declaration.Modifiers[indexOfOldModifier];
         newDeclaration = declaration.ReplaceToken(oldToken, newModifierToken);
      }
      else
      {
         var indexOfPartialKeyword = declaration.Modifiers.IndexOf(SyntaxKind.PartialKeyword);
         newDeclaration = indexOfPartialKeyword < 0 ? declaration.AddModifiers(newModifierToken) : declaration.WithModifiers(declaration.Modifiers.Insert(indexOfPartialKeyword, newModifierToken));
      }

      var newRoot = root.ReplaceNode(declaration, newDeclaration);
      var newDoc = document.WithSyntaxRoot(newRoot);

      return Task.FromResult(newDoc);
   }

   private static Task<Document> ChangeAccessibilityAsync(
      Document document,
      SyntaxNode root,
      AccessorDeclarationSyntax? declaration,
      SyntaxKind accessibility)
   {
      return ChangeAccessibilityAsync(document, root, declaration, declaration?.Modifiers ?? default, static (node, newModifiers) => node.WithModifiers(newModifiers), accessibility);
   }

   private static Task<Document> ChangeAccessibilityAsync(
      Document document,
      SyntaxNode root,
      MemberDeclarationSyntax? declaration,
      SyntaxKind accessibility)
   {
      return ChangeAccessibilityAsync(document, root, declaration, declaration?.Modifiers ?? default, static (node, newModifiers) => node.WithModifiers(newModifiers), accessibility);
   }

   private static Task<Document> ChangeAccessibilityAsync<T>(
      Document document,
      SyntaxNode root,
      T? declaration,
      SyntaxTokenList modifiers,
      Func<T, SyntaxTokenList, T> useNewModifiers,
      SyntaxKind accessibility)
      where T : CSharpSyntaxNode
   {
      if (declaration is null)
         return Task.FromResult(document);

      var newAccessibilitySyntax = SyntaxFactory.Token(accessibility);
      var newModifiers = modifiers;
      var newDeclaration = declaration;

      if (modifiers.Count > 0)
      {
         var firstModifier = modifiers.FirstOrDefault();
         var isFirstModiferRemoved = false;

         foreach (var currentModifier in newModifiers)
         {
            if (currentModifier.Kind() is SyntaxKind.PrivateKeyword or SyntaxKind.ProtectedKeyword or SyntaxKind.InternalKeyword or SyntaxKind.PublicKeyword && !currentModifier.IsKind(accessibility))
            {
               newModifiers = newModifiers.Remove(currentModifier);

               if (currentModifier == firstModifier)
                  isFirstModiferRemoved = true;
            }
         }

         if (!isFirstModiferRemoved && firstModifier.HasLeadingTrivia)
            newModifiers = newModifiers.RemoveAt(0).Insert(0, firstModifier.WithLeadingTrivia(SyntaxTriviaList.Empty));

         if (firstModifier.HasLeadingTrivia)
            newAccessibilitySyntax = newAccessibilitySyntax.WithLeadingTrivia(firstModifier.LeadingTrivia);

         if (firstModifier.HasTrailingTrivia)
            newAccessibilitySyntax = newAccessibilitySyntax.WithTrailingTrivia(firstModifier.TrailingTrivia);
      }
      else if (declaration.HasLeadingTrivia)
      {
         newAccessibilitySyntax = newAccessibilitySyntax.WithLeadingTrivia(declaration.GetLeadingTrivia())
                                                        .WithTrailingTrivia(SyntaxFactory.Whitespace(" "));
         newDeclaration = declaration.WithLeadingTrivia(SyntaxTriviaList.Empty);
      }

      newModifiers = newModifiers.Insert(0, newAccessibilitySyntax);

      newDeclaration = useNewModifiers(newDeclaration, newModifiers);
      var newRoot = root.ReplaceNode(declaration, newDeclaration);
      var newDoc = document.WithSyntaxRoot(newRoot);

      return Task.FromResult(newDoc);
   }

   private static Task<Document> RemovePropertySetterAsync(
      Document document,
      SyntaxNode root,
      PropertyDeclarationSyntax? declaration)
   {
      if (declaration is null)
         return Task.FromResult(document);

      var setter = declaration.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));

      if (setter is not null)
      {
         var newAccessors = declaration.AccessorList!.Accessors.Remove(setter);
         var newAccessorList = declaration.AccessorList.WithAccessors(newAccessors);
         var newDeclaration = declaration.WithAccessorList(newAccessorList);
         var newRoot = root.ReplaceNode(declaration, newDeclaration);
         var newDoc = document.WithSyntaxRoot(newRoot);

         document = newDoc;
      }

      return Task.FromResult(document);
   }

   private static async Task<Document> AddKeyMemberEqualityComparerAttributeAsync(
      Document document,
      SyntaxNode root,
      TypeDeclarationSyntax? declaration,
      CancellationToken cancellationToken)
   {
      return await AddKeyMemberComparerAsync(
                document,
                root,
                declaration,
                "KeyMemberEqualityComparer",
                cancellationToken);
   }

   private static async Task<Document> AddKeyMemberComparerAttributeAsync(
      Document document,
      SyntaxNode root,
      TypeDeclarationSyntax? declaration,
      CancellationToken cancellationToken)
   {
      return await AddKeyMemberComparerAsync(
                document,
                root,
                declaration,
                "KeyMemberComparer",
                cancellationToken);
   }

   private static async Task<Document> AddKeyMemberComparerAsync(
      Document document,
      SyntaxNode root,
      TypeDeclarationSyntax? declaration,
      string comparerAttributeName,
      CancellationToken cancellationToken)
   {
      if (declaration is null)
         return document;

      var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

      if (model is null)
         return document;

      var valueObjectType = model.GetDeclaredSymbol(declaration, cancellationToken);

      if ((!valueObjectType.IsValueObjectType(out var keyedAttribute)
           || keyedAttribute.AttributeClass?.IsKeyedValueObjectAttribute() != true)
          && !valueObjectType.IsEnum(out keyedAttribute))
      {
         return document;
      }

      var keyType = keyedAttribute.AttributeClass?.TypeArguments[0];

      if (keyType is null)
         return document;

      var keyTypeName = SyntaxFactory.ParseTypeName(keyType.ToMinimalDisplayString(model, declaration.GetLocation().SourceSpan.Start));

      var accessorType = keyType.SpecialType == SpecialType.System_String
                            ? SyntaxFactory.ParseTypeName("ComparerAccessors.StringOrdinalIgnoreCase")
                            : SyntaxFactory.ParseTypeName($"ComparerAccessors.Default<{keyTypeName}>");

      var genericParameters = new SyntaxNodeOrToken[]
                              {
                                 accessorType,
                                 SyntaxFactory.Token(SyntaxKind.CommaToken),
                                 keyTypeName
                              };

      var comparerAttributeType = SyntaxFactory.GenericName(comparerAttributeName)
                                               .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(genericParameters)));
      var comparerAttribute = SyntaxFactory.Attribute(comparerAttributeType);

      var newDeclaration = declaration.AddAttributeLists(SyntaxFactory.AttributeList([comparerAttribute]));
      var newRoot = root.ReplaceNode(declaration, newDeclaration);
      var newDoc = document.WithSyntaxRoot(newRoot);

      return newDoc;
   }

   private static async Task<Document> AddDefaultStringComparisonAsync(
      Document document,
      SyntaxNode root,
      TypeDeclarationSyntax? declaration,
      CancellationToken cancellationToken)
   {
      if (declaration is null)
         return document;

      var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

      if (model is null)
         return document;

      var valueObjectType = model.GetDeclaredSymbol(declaration, cancellationToken);

      if (!valueObjectType.IsValueObjectType(out var valueObjectAttribute)
          || valueObjectAttribute.ApplicationSyntaxReference is null
          || valueObjectAttribute.AttributeClass?.IsComplexValueObjectAttribute() != true)
      {
         return document;
      }

      var attributeSyntax = await valueObjectAttribute.ApplicationSyntaxReference.GetSyntaxAsync(cancellationToken) as AttributeSyntax;

      if (attributeSyntax is null)
      {
         return document;
      }

      var newAttribute = attributeSyntax.AddArgumentListArguments(
         SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression("StringComparison.OrdinalIgnoreCase"))
                      .WithNameEquals(SyntaxFactory.NameEquals("DefaultStringComparison")));

      var newDeclaration = declaration.ReplaceNode(attributeSyntax, newAttribute);
      var newRoot = root.ReplaceNode(declaration, newDeclaration);
      var newDoc = document.WithSyntaxRoot(newRoot);

      return newDoc;
   }

   private static ThrowStatementSyntax BuildThrowNotImplementedException()
   {
      var notImplementedExceptionType = SyntaxFactory.ParseTypeName($"global::System.{nameof(NotImplementedException)}");
      var newNotImplementedException = SyntaxFactory.ObjectCreationExpression(notImplementedExceptionType, SyntaxFactory.ArgumentList(), null);
      var throwStatement = SyntaxFactory.ThrowStatement(newNotImplementedException);
      return throwStatement;
   }

   private sealed class CodeFixesContext
   {
      private readonly Diagnostic _diagnostic;
      private readonly SyntaxNode _root;

      private TypeDeclarationSyntax? _typeDeclaration;
      public TypeDeclarationSyntax? TypeDeclaration => _typeDeclaration ??= GetDeclaration<TypeDeclarationSyntax>();

      private FieldDeclarationSyntax? _fieldDeclaration;
      public FieldDeclarationSyntax? FieldDeclaration => _fieldDeclaration ??= GetDeclaration<FieldDeclarationSyntax>();

      private MemberDeclarationSyntax? _memberDeclaration;
      public MemberDeclarationSyntax? MemberDeclaration => _memberDeclaration ??= GetDeclaration<MemberDeclarationSyntax>();

      private PropertyDeclarationSyntax? _propertyDeclaration;
      public PropertyDeclarationSyntax? PropertyDeclaration => _propertyDeclaration ??= GetDeclaration<PropertyDeclarationSyntax>();

      private MethodDeclarationSyntax? _methodDeclaration;
      public MethodDeclarationSyntax? MethodDeclaration => _methodDeclaration ??= GetDeclaration<MethodDeclarationSyntax>();

      public CodeFixesContext(Diagnostic diagnostic, SyntaxNode root)
      {
         _diagnostic = diagnostic;
         _root = root;
      }

      private T? GetDeclaration<T>()
         where T : MemberDeclarationSyntax
      {
         var diagnosticSpan = _diagnostic.Location.SourceSpan;
         return _root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<T>().First();
      }
   }
}
