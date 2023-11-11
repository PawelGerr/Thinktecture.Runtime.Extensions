using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ThinktectureRuntimeExtensionsCodeFixProvider))]
public sealed class ThinktectureRuntimeExtensionsCodeFixProvider : CodeFixProvider
{
   private const string _MAKE_PARTIAL = "Make the type partial";
   private const string _MAKE_STRUCT_READONLY = "Make the type read-only";
   private const string _MAKE_MEMBER_PUBLIC = "Make the member public";
   private const string _MAKE_FIELD_READONLY = "Make the field read-only";
   private const string _REMOVE_PROPERTY_SETTER = "Remove property setter";
   private const string _MAKE_INIT_PRIVATE = "Make init private";
   private const string _IMPLEMENT_CREATE_INVALID = $"Implement '{Constants.Methods.CREATE_INVALID_ITEM}'";
   private const string _MAKE_TYPE_PRIVATE = "Make type private";
   private const string _MAKE_TYPE_PUBLIC = "Make type public";
   private const string _SEAL_CLASS = "Seal class";

   /// <inheritdoc />
   public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial.Id,
                                                                                                DiagnosticsDescriptors.StructMustBeReadOnly.Id,
                                                                                                DiagnosticsDescriptors.EnumItemMustBePublic.Id,
                                                                                                DiagnosticsDescriptors.FieldMustBeReadOnly.Id,
                                                                                                DiagnosticsDescriptors.PropertyMustBeReadOnly.Id,
                                                                                                DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation.Id,
                                                                                                DiagnosticsDescriptors.InnerEnumOnFirstLevelMustBePrivate.Id,
                                                                                                DiagnosticsDescriptors.InnerEnumOnNonFirstLevelMustBePublic.Id,
                                                                                                DiagnosticsDescriptors.EnumWithoutDerivedTypesMustBeSealed.Id,
                                                                                                DiagnosticsDescriptors.ValueObjectMustBeSealed.Id,
                                                                                                DiagnosticsDescriptors.InitAccessorMustBePrivate.Id);

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
         else if (diagnostic.Id == DiagnosticsDescriptors.StructMustBeReadOnly.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_STRUCT_READONLY, _ => AddTypeModifierAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, SyntaxKind.ReadOnlyKeyword), _MAKE_STRUCT_READONLY), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.FieldMustBeReadOnly.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_FIELD_READONLY, _ => AddTypeModifierAsync(context.Document, root, GetCodeFixesContext().FieldDeclaration, SyntaxKind.ReadOnlyKeyword), _MAKE_FIELD_READONLY), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.EnumItemMustBePublic.Id)
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
         else if (diagnostic.Id == DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_IMPLEMENT_CREATE_INVALID, t => AddCreateInvalidItemAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, t), _IMPLEMENT_CREATE_INVALID), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.InnerEnumOnFirstLevelMustBePrivate.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_TYPE_PRIVATE, _ => ChangeAccessibilityAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, SyntaxKind.PrivateKeyword), _MAKE_TYPE_PRIVATE), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.InnerEnumOnNonFirstLevelMustBePublic.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_MAKE_TYPE_PUBLIC, _ => ChangeAccessibilityAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, SyntaxKind.PublicKeyword), _MAKE_TYPE_PUBLIC), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.EnumWithoutDerivedTypesMustBeSealed.Id || diagnostic.Id == DiagnosticsDescriptors.ValueObjectMustBeSealed.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_SEAL_CLASS, _ => AddTypeModifierAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, SyntaxKind.SealedKeyword), _SEAL_CLASS), diagnostic);
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

   private static async Task<Document> AddCreateInvalidItemAsync(
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

      var enumType = model.GetDeclaredSymbol(declaration);

      if (!enumType.IsEnum(out var smartEnumAttribute))
         return document;

      var keyType = smartEnumAttribute.AttributeClass?.TypeArguments[0];

      if (keyType is null)
         return document;

      var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("key"))
                                   .WithType(SyntaxFactory.ParseTypeName(keyType.ToMinimalDisplayString(model, declaration.GetLocation().SourceSpan.Start)));

      var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(enumType.ToMinimalDisplayString(model, declaration.GetLocation().SourceSpan.Start)), Constants.Methods.CREATE_INVALID_ITEM)
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                                .AddParameterListParameters(parameter)
                                .WithBody(SyntaxFactory.Block(BuildThrowNotImplementedException()));

      var newDeclaration = declaration.AddMembers(method);
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

      private PropertyDeclarationSyntax? _propertyDeclaration;
      public PropertyDeclarationSyntax? PropertyDeclaration => _propertyDeclaration ??= GetDeclaration<PropertyDeclarationSyntax>();

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
