using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.CodeFixes
{
   [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ThinktectureRuntimeExtensionsCodeFixProvider)), Shared]
   public class ThinktectureRuntimeExtensionsCodeFixProvider : CodeFixProvider
   {
      private const string _MAKE_PARTIAL = "Make the type partial";
      private const string _MAKE_STRUCT_READONLY = "Make the type read-only";
      private const string _MAKE_MEMBER_PUBLIC = "Make the member public";
      private const string _MAKE_FIELD_READONLY = "Make the field read-only";
      private const string _REMOVE_PROPERTY_SETTER = "Remove property setter";
      private const string _IMPLEMENT_CREATE_INVALID = "Implement 'CreateInvalidItem'";
      private const string _MAKE_TYPE_PRIVATE = "Make type private";
      private const string _MAKE_TYPE_PUBLIC = "Make type public";

      /// <inheritdoc />
      public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial.Id,
                                                                                                   DiagnosticsDescriptors.StructMustBeReadOnly.Id,
                                                                                                   DiagnosticsDescriptors.FieldMustBePublic.Id,
                                                                                                   DiagnosticsDescriptors.FieldMustBeReadOnly.Id,
                                                                                                   DiagnosticsDescriptors.PropertyMustBeReadOnly.Id,
                                                                                                   DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation.Id,
                                                                                                   DiagnosticsDescriptors.FirstLevelInnerTypeMustBePrivate.Id,
                                                                                                   DiagnosticsDescriptors.NonFirstLevelInnerTypeMustBePublic.Id);

      /// <inheritdoc />
      public override FixAllProvider GetFixAllProvider()
      {
         return WellKnownFixAllProviders.BatchFixer;
      }

      /// <inheritdoc />
      public override async Task RegisterCodeFixesAsync(CodeFixContext context)
      {
         var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

         if (root is not null)
         {
            var ctx = new CodeFixesContext(context, root);

            var makePartialDiagnostic = FindDiagnostics(context, DiagnosticsDescriptors.TypeMustBePartial);

            if (makePartialDiagnostic is not null)
               context.RegisterCodeFix(CodeAction.Create(_MAKE_PARTIAL, _ => AddTypeModifierAsync(context.Document, root, ctx.TypeDeclaration, SyntaxKind.PartialKeyword), _MAKE_PARTIAL), makePartialDiagnostic);

            var makeStructReadOnlyDiagnostic = FindDiagnostics(context, DiagnosticsDescriptors.StructMustBeReadOnly);

            if (makeStructReadOnlyDiagnostic is not null)
               context.RegisterCodeFix(CodeAction.Create(_MAKE_STRUCT_READONLY, _ => AddTypeModifierAsync(context.Document, root, ctx.TypeDeclaration, SyntaxKind.ReadOnlyKeyword), _MAKE_STRUCT_READONLY), makeStructReadOnlyDiagnostic);

            var makeFieldReadOnlyDiagnostic = FindDiagnostics(context, DiagnosticsDescriptors.FieldMustBeReadOnly);

            if (makeFieldReadOnlyDiagnostic is not null)
               context.RegisterCodeFix(CodeAction.Create(_MAKE_FIELD_READONLY, _ => AddTypeModifierAsync(context.Document, root, ctx.FieldDeclaration, SyntaxKind.ReadOnlyKeyword), _MAKE_FIELD_READONLY), makeFieldReadOnlyDiagnostic);

            var makeFieldPublicDiagnostic = FindDiagnostics(context, DiagnosticsDescriptors.FieldMustBePublic);

            if (makeFieldPublicDiagnostic is not null)
               context.RegisterCodeFix(CodeAction.Create(_MAKE_MEMBER_PUBLIC, _ => ChangeAccessibilityAsync(context.Document, root, ctx.FieldDeclaration, SyntaxKind.PublicKeyword), _MAKE_MEMBER_PUBLIC), makeFieldPublicDiagnostic);

            var makePropertyReadOnlyDiagnostic = FindDiagnostics(context, DiagnosticsDescriptors.PropertyMustBeReadOnly);

            if (makePropertyReadOnlyDiagnostic is not null)
               context.RegisterCodeFix(CodeAction.Create(_REMOVE_PROPERTY_SETTER, _ => RemovePropertySetterAsync(context.Document, root, ctx.PropertyDeclaration), _REMOVE_PROPERTY_SETTER), makePropertyReadOnlyDiagnostic);

            var needsCreateInvalidDiagnostic = FindDiagnostics(context, DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation);

            if (needsCreateInvalidDiagnostic is not null)
               context.RegisterCodeFix(CodeAction.Create(_IMPLEMENT_CREATE_INVALID, t => AddCreateInvalidItemAsync(context.Document, root, ctx.TypeDeclaration, t), _IMPLEMENT_CREATE_INVALID), needsCreateInvalidDiagnostic);

            var makeDerivedTypePrivate = FindDiagnostics(context, DiagnosticsDescriptors.FirstLevelInnerTypeMustBePrivate);

            if (makeDerivedTypePrivate is not null)
               context.RegisterCodeFix(CodeAction.Create(_MAKE_TYPE_PRIVATE, _ => ChangeAccessibilityAsync(context.Document, root, ctx.TypeDeclaration, SyntaxKind.PrivateKeyword), _MAKE_TYPE_PRIVATE), makeDerivedTypePrivate);

            var makeDerivedTypePublic = FindDiagnostics(context, DiagnosticsDescriptors.NonFirstLevelInnerTypeMustBePublic);

            if (makeDerivedTypePublic is not null)
               context.RegisterCodeFix(CodeAction.Create(_MAKE_TYPE_PUBLIC, _ => ChangeAccessibilityAsync(context.Document, root, ctx.TypeDeclaration, SyntaxKind.PublicKeyword), _MAKE_TYPE_PUBLIC), makeDerivedTypePublic);
         }
      }

      private static Diagnostic? FindDiagnostics(CodeFixContext context, DiagnosticDescriptor diagnostic)
      {
         return context.Diagnostics.FirstOrDefault(d => d.Id == diagnostic.Id);
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
         MemberDeclarationSyntax? declaration,
         SyntaxKind accessibility)
      {
         if (declaration is null)
            return Task.FromResult(document);

         var firstModifier = declaration.Modifiers.FirstOrDefault();
         var newModifiers = declaration.Modifiers;
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

         var publicSyntax = SyntaxFactory.Token(accessibility);

         if (firstModifier.HasLeadingTrivia)
            publicSyntax = publicSyntax.WithLeadingTrivia(declaration.Modifiers.FirstOrDefault().LeadingTrivia);

         if (firstModifier.HasTrailingTrivia)
            publicSyntax = publicSyntax.WithTrailingTrivia(declaration.Modifiers.FirstOrDefault().TrailingTrivia);

         newModifiers = newModifiers.Insert(0, publicSyntax);

         var newDeclaration = declaration.WithModifiers(newModifiers);
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

         if (enumType is null || !enumType.IsEnum(out var enumInterfaces))
            return document;

         var enumInterface = enumInterfaces.GetValidEnumInterface(enumType, declaration.Identifier.GetLocation());

         if (enumInterface is null)
            return document;

         var keyType = enumInterface.TypeArguments[0];

         var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("key"))
                                      .WithType(SyntaxFactory.ParseTypeName(keyType.ToMinimalDisplayString(model, declaration.GetLocation().SourceSpan.Start)));

         var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(enumType.ToMinimalDisplayString(model, declaration.GetLocation().SourceSpan.Start)), "CreateInvalidItem")
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
         var notImplementedExceptionType = SyntaxFactory.ParseTypeName(typeof(NotImplementedException).FullName);
         var newNotImplementedException = SyntaxFactory.ObjectCreationExpression(notImplementedExceptionType, SyntaxFactory.ArgumentList(), null);
         var throwStatement = SyntaxFactory.ThrowStatement(newNotImplementedException);
         return throwStatement;
      }

      private class CodeFixesContext
      {
         private readonly CodeFixContext _context;
         private readonly SyntaxNode _root;

         private TypeDeclarationSyntax? _typeDeclaration;
         public TypeDeclarationSyntax? TypeDeclaration => _typeDeclaration ??= GetDeclaration<TypeDeclarationSyntax>();

         private FieldDeclarationSyntax? _fieldDeclaration;
         public FieldDeclarationSyntax? FieldDeclaration => _fieldDeclaration ??= GetDeclaration<FieldDeclarationSyntax>();

         private PropertyDeclarationSyntax? _propertyDeclaration;
         public PropertyDeclarationSyntax? PropertyDeclaration => _propertyDeclaration ??= GetDeclaration<PropertyDeclarationSyntax>();

         public CodeFixesContext(CodeFixContext context, SyntaxNode root)
         {
            _context = context;
            _root = root;
         }

         private T? GetDeclaration<T>()
            where T : MemberDeclarationSyntax
         {
            var diagnosticSpan = _context.Diagnostics.First().Location.SourceSpan;
            return _root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<T>().First();
         }
      }
   }
}
