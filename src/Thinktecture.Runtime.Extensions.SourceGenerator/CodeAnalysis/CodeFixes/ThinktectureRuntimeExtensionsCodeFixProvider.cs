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
   private const string _ALIGN_COMPARISON_EQUALITY_OPERATORS = "Align comparison/equality operators";
   private const string _GENERATE_VALIDATE_METHOD = "Generate Validate method";
   private const string _GENERATE_TO_VALUE_METHOD = "Generate ToValue method";
   private const string _MAKE_LAMBDA_STATIC = "Make lambdas static";

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
      DiagnosticsDescriptors.ComparisonAndEqualityOperatorsMismatch.Id,
      DiagnosticsDescriptors.ObjectFactoryMustImplementStaticValidateMethod.Id,
      DiagnosticsDescriptors.ObjectFactoryMustImplementToValueMethod.Id,
      DiagnosticsDescriptors.UseSwitchMapWithStaticLambda.Id,
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

         if (diagnostic.Id == DiagnosticsDescriptors.ComparisonAndEqualityOperatorsMismatch.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_ALIGN_COMPARISON_EQUALITY_OPERATORS, t => AlignComparisonEqualityOperatorsAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, t), _ALIGN_COMPARISON_EQUALITY_OPERATORS), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.TypeMustBePartial.Id)
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
            context.RegisterCodeFix(CodeAction.Create(_MAKE_INIT_PRIVATE, _ => ChangeAccessibilityAsync(context.Document, root, GetCodeFixesContext().PropertyDeclaration?.AccessorList?.Accessors.FirstNodeOrDefault(a => a.IsKind(SyntaxKind.InitAccessorDeclaration)), SyntaxKind.PrivateKeyword), _MAKE_INIT_PRIVATE), diagnostic);
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
         else if (diagnostic.Id == DiagnosticsDescriptors.ObjectFactoryMustImplementStaticValidateMethod.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_GENERATE_VALIDATE_METHOD, t => GenerateValidateMethodAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, t), _GENERATE_VALIDATE_METHOD), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.ObjectFactoryMustImplementToValueMethod.Id)
         {
            context.RegisterCodeFix(CodeAction.Create(_GENERATE_TO_VALUE_METHOD, t => GenerateToValueMethodAsync(context.Document, root, GetCodeFixesContext().TypeDeclaration, t), _GENERATE_TO_VALUE_METHOD), diagnostic);
         }
         else if (diagnostic.Id == DiagnosticsDescriptors.UseSwitchMapWithStaticLambda.Id)
         {
            var invocation = GetCodeFixesContext().InvocationExpressionSyntax;

            if (invocation is null)
               continue;

            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            if (semanticModel is not null
                // Don't offer "state" overload if it already has a state (first parameter is a type parameter TState in the original definition)
                && (semanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol { Parameters.Length: > 0 } methodSymbol
                    || methodSymbol.OriginalDefinition.Parameters[0].Type.TypeKind != TypeKind.TypeParameter)
                && TryGetCapturedVariables(semanticModel, invocation, out var captures)
                && captures.Count > 0)
            {
               context.RegisterCodeFix(CodeAction.Create(_MAKE_LAMBDA_STATIC, t => UseStateOverloadAsync(context.Document, invocation, captures, t), _MAKE_LAMBDA_STATIC), diagnostic);
            }
            else
            {
               context.RegisterCodeFix(CodeAction.Create(_MAKE_LAMBDA_STATIC, _ => MakeAllLambdasStaticAsync(context.Document, root, invocation), _MAKE_LAMBDA_STATIC), diagnostic);
            }
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
         var firstModifier = modifiers.FirstTokenOrDefault();
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

      var setter = declaration.AccessorList?.Accessors.FirstNodeOrDefault(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));

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
          && !valueObjectType.IsSmartEnum(out keyedAttribute))
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

   private static async Task<Document> GenerateValidateMethodAsync(
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

      var objectType = model.GetDeclaredSymbol(declaration, cancellationToken);

      if (objectType is null)
         return document;

      var validationErrorType = objectType.FindAttribute(static attr => attr.IsValidationErrorAttribute())?.AttributeClass?.TypeArguments[0];
      var newDeclaration = declaration;

      var attributes = objectType.GetAttributes();

      for (var i = 0; i < attributes.Length; i++)
      {
         var attribute = attributes[i];

         if (attribute.AttributeClass?.IsObjectFactoryAttribute() != true)
            continue;

         var valueType = attribute.AttributeClass.TypeArguments[0];

         if (objectType.HasValidateMethod(valueType, validationErrorType))
            continue;

         var objectTypeName = SyntaxFactory.ParseTypeName(objectType.ToMinimalDisplayString(model, declaration.GetLocation().SourceSpan.Start));
         var valueTypeName = SyntaxFactory.ParseTypeName(valueType.WithNullableAnnotation(NullableAnnotation.Annotated).ToMinimalDisplayString(model, declaration.GetLocation().SourceSpan.Start));
         var validationErrorTypeName = validationErrorType is null
                                          ? SyntaxFactory.ParseTypeName("ValidationError")
                                          : SyntaxFactory.ParseTypeName(validationErrorType.ToMinimalDisplayString(model, declaration.GetLocation().SourceSpan.Start));

         var valueParameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                                           .WithType(valueTypeName);

         var providerParameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("provider"))
                                              .WithType(SyntaxFactory.NullableType(SyntaxFactory.ParseTypeName("IFormatProvider")));

         var itemOutType = objectType.IsReferenceType
                              ? SyntaxFactory.NullableType(objectTypeName)
                              : objectTypeName;

         var itemParameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("item"))
                                          .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword)))
                                          .WithType(itemOutType);

         var returnType = SyntaxFactory.NullableType(validationErrorTypeName);

         var methodBody = SyntaxFactory.Block(BuildThrowNotImplementedException(false));

         var validateMethod = SyntaxFactory.MethodDeclaration(returnType, "Validate")
                                           .WithModifiers(SyntaxFactory.TokenList(
                                                             SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                                             SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                                           .WithParameterList(SyntaxFactory.ParameterList(
                                                                 SyntaxFactory.SeparatedList([valueParameter, providerParameter, itemParameter])))
                                           .WithBody(methodBody)
                                           .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);

         newDeclaration = newDeclaration.AddMembers(validateMethod);
      }

      var newRoot = root.ReplaceNode(declaration, newDeclaration);
      var newDoc = document.WithSyntaxRoot(newRoot);

      return newDoc;
   }

   private static async Task<Document> GenerateToValueMethodAsync(
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

      var objectType = model.GetDeclaredSymbol(declaration, cancellationToken);

      if (objectType is null)
         return document;

      var attributes = objectType.GetAttributes();
      var newDeclaration = declaration;

      for (var i = 0; i < attributes.Length; i++)
      {
         var attribute = attributes[i];

         if (attribute.AttributeClass?.IsObjectFactoryAttribute() != true)
            continue;

         var valueType = attribute.AttributeClass.TypeArguments[0];

         if (!attribute.NeedsToValueMethod() || objectType.HasToValueMethod(valueType))
            continue;

         var valueTypeName = SyntaxFactory.ParseTypeName(valueType.ToMinimalDisplayString(model, declaration.GetLocation().SourceSpan.Start));

         var methodBody = SyntaxFactory.Block(BuildThrowNotImplementedException(false));

         var toValueMethod = SyntaxFactory.MethodDeclaration(valueTypeName, "ToValue")
                                          .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                          .WithParameterList(SyntaxFactory.ParameterList())
                                          .WithBody(methodBody)
                                          .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);

         newDeclaration = newDeclaration.AddMembers(toValueMethod);
      }

      var newRoot = root.ReplaceNode(declaration, newDeclaration);
      var newDoc = document.WithSyntaxRoot(newRoot);

      return newDoc;
   }

   private static ThrowStatementSyntax BuildThrowNotImplementedException(bool fullyQualified = true)
   {
      var notImplementedExceptionType = SyntaxFactory.ParseTypeName(fullyQualified ? $"global::System.{nameof(NotImplementedException)}" : nameof(NotImplementedException));
      var newNotImplementedException = SyntaxFactory.ObjectCreationExpression(notImplementedExceptionType, SyntaxFactory.ArgumentList(), null);
      var throwStatement = SyntaxFactory.ThrowStatement(newNotImplementedException);
      return throwStatement;
   }

   private static async Task<Document> AlignComparisonEqualityOperatorsAsync(
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

      var type = model.GetDeclaredSymbol(declaration, cancellationToken);

      // Find SmartEnum or ValueObject attribute
      AttributeData? targetAttributeData = null;

      if (type.IsSmartEnum(out var smartEnumAttribute))
      {
         targetAttributeData = smartEnumAttribute;
      }
      else if (type.IsKeyedValueObjectType(out var valueObjectAttribute))
      {
         targetAttributeData = valueObjectAttribute;
      }

      if (targetAttributeData?.ApplicationSyntaxReference is null)
         return document;

      var attributeSyntax = await targetAttributeData.ApplicationSyntaxReference.GetSyntaxAsync(cancellationToken) as AttributeSyntax;

      if (attributeSyntax is null)
         return document;

      var argumentList = attributeSyntax.ArgumentList;
      AttributeArgumentSyntax? comparisonArg = null;
      AttributeArgumentSyntax? equalityArg = null;

      if (argumentList is not null)
      {
         for (var i = 0; i < argumentList.Arguments.Count; i++)
         {
            var arg = argumentList.Arguments[i];
            var name = arg.NameEquals?.Name.Identifier.Text;

            switch (name)
            {
               case Constants.Attributes.Properties.COMPARISON_OPERATORS:
                  comparisonArg = arg;
                  break;
               case Constants.Attributes.Properties.EQUALITY_COMPARISON_OPERATORS:
                  equalityArg = arg;
                  break;
            }
         }
      }

      var comparisonValue = GetOperatorsGenerationValue(model, comparisonArg?.Expression, cancellationToken);
      var equalityValue = GetOperatorsGenerationValue(model, equalityArg?.Expression, cancellationToken);

      if (comparisonValue == equalityValue)
         return document;

      var newOperatorsGeneration = (comparisonValue, equalityValue) switch
      {
         (null, not null) => equalityValue,
         (not null, null) => comparisonValue,
         (not null, not null) => comparisonValue is OperatorsGeneration.None || equalityValue is OperatorsGeneration.None
                                    ? OperatorsGeneration.None
                                    : comparisonValue.Value > equalityValue.Value ? comparisonValue : equalityValue,
         (_, _) => OperatorsGeneration.None
      };

      var newArgumentList = argumentList ?? SyntaxFactory.AttributeArgumentList();

      if (comparisonValue != newOperatorsGeneration)
      {
         var newArg = SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"OperatorsGeneration.{newOperatorsGeneration.ToString()}"))
                                   .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(Constants.Attributes.Properties.COMPARISON_OPERATORS)));

         newArgumentList = comparisonArg is null
                              ? newArgumentList.WithArguments(newArgumentList.Arguments.Add(newArg))
                              : newArgumentList.ReplaceNode(comparisonArg, newArg);
      }

      if (equalityValue != newOperatorsGeneration)
      {
         var newArg = SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"OperatorsGeneration.{newOperatorsGeneration.ToString()}"))
                                   .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(Constants.Attributes.Properties.EQUALITY_COMPARISON_OPERATORS)));

         newArgumentList = equalityArg is null
                              ? newArgumentList.WithArguments(newArgumentList.Arguments.Add(newArg))
                              : newArgumentList.ReplaceNode(equalityArg, newArg);
      }

      var newAttribute = attributeSyntax.WithArgumentList(newArgumentList);
      var newDeclaration = declaration.ReplaceNode(attributeSyntax, newAttribute);
      var newRoot = root.ReplaceNode(declaration, newDeclaration);
      return document.WithSyntaxRoot(newRoot);
   }

   // Helper that tries to resolve the enum value of an expression referencing OperatorsGeneration.
   private static OperatorsGeneration? GetOperatorsGenerationValue(SemanticModel model, ExpressionSyntax? expr, CancellationToken cancellationToken)
   {
      if (expr is null)
         return null;

      // Try constant value first.
      var constant = model.GetConstantValue(expr, cancellationToken);

      if (constant.HasValue)
      {
         switch (constant.Value)
         {
            case int i when Enum.IsDefined(typeof(OperatorsGeneration), i):
               return (OperatorsGeneration)i;
            case OperatorsGeneration og:
               return og;
         }
      }

      // Try symbol info (enum field access or qualified name)
      var symbol = model.GetSymbolInfo(expr, cancellationToken).Symbol;

      if (symbol is IFieldSymbol { ContainingType: { TypeKind: TypeKind.Enum, Name: nameof(OperatorsGeneration) } } field)
      {
         if (Enum.TryParse(field.Name, out OperatorsGeneration parsed))
            return parsed;
      }

      return null;
   }

   private static Task<Document> MakeAllLambdasStaticAsync(
      Document document,
      SyntaxNode root,
      InvocationExpressionSyntax invocation)
   {
      var lambdas = invocation.ArgumentList.Arguments
                              .Select(a => a.Expression)
                              .OfType<LambdaExpressionSyntax>()
                              .Where(l => !l.Modifiers.Any(SyntaxKind.StaticKeyword))
                              .ToList();

      var newRoot = root.ReplaceNodes(lambdas, (original, _) =>
      {
         var staticToken = SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SyntaxFactory.Space);
         return original.WithModifiers(original.Modifiers.Insert(0, staticToken));
      });

      return Task.FromResult(document.WithSyntaxRoot(newRoot));
   }

   private static bool TryGetCapturedVariables(
      SemanticModel semanticModel,
      InvocationExpressionSyntax invocation,
      out List<ISymbol> captures)
   {
      captures = [];
      var seenSymbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

      foreach (var argument in invocation.ArgumentList.Arguments)
      {
         if (argument.Expression is not LambdaExpressionSyntax lambda)
            continue;

         if (lambda.Modifiers.Any(SyntaxKind.StaticKeyword))
            continue;

         var lambdaBody = (SyntaxNode?)lambda.ExpressionBody ?? lambda.Block;

         if (lambdaBody is null)
            continue;

         var dataFlow = semanticModel.AnalyzeDataFlow(lambdaBody);

         if (!dataFlow.Succeeded)
            return false;

         var lambdaParams = GetLambdaParameterSymbols(semanticModel, lambda);

         foreach (var symbol in dataFlow.DataFlowsIn)
         {
            if (lambdaParams.Contains(symbol, SymbolEqualityComparer.Default))
               continue;

            switch (symbol)
            {
               // "this" capture: the lambda references instance members implicitly (e.g., Foo() instead of this.Foo()).
               // The identifier-replacement logic below can't reliably rewrite implicit "this" references to "state",
               // so we bail out of the state-overload refactoring for now.
               case IParameterSymbol { IsThis: true }:
               // Ref locals/parameters can't be packaged into a state tuple — copying would lose reference semantics.
               case IParameterSymbol { RefKind: not RefKind.None }:
               case ILocalSymbol { RefKind: not RefKind.None }:
                  return false;
            }

            if (seenSymbols.Add(symbol))
               captures.Add(symbol);
         }
      }

      return true;
   }

   private static ImmutableArray<ISymbol> GetLambdaParameterSymbols(
      SemanticModel semanticModel,
      LambdaExpressionSyntax lambda)
   {
      if (lambda is SimpleLambdaExpressionSyntax simple)
      {
         return semanticModel.GetDeclaredSymbol(simple.Parameter) is { } symbol
                   ? [symbol]
                   : [];
      }

      if (lambda is ParenthesizedLambdaExpressionSyntax paren)
      {
         var builder = ImmutableArray.CreateBuilder<ISymbol>();

         foreach (var param in paren.ParameterList.Parameters)
         {
            if (semanticModel.GetDeclaredSymbol(param) is { } symbol)
               builder.Add(symbol);
         }

         return builder.ToImmutable();
      }

      return [];
   }

   private static async Task<Document> UseStateOverloadAsync(
      Document document,
      InvocationExpressionSyntax invocation,
      List<ISymbol> captures,
      CancellationToken cancellationToken)
   {
      var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

      if (root is null)
         return document;

      var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

      if (semanticModel is null)
         return document;

      // Build state expression
      ExpressionSyntax stateExpression;

      if (captures.Count == 1)
      {
         stateExpression = SyntaxFactory.IdentifierName(captures[0].Name);
      }
      else
      {
         var tupleElements = captures.Select(c =>
                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(c.Name)));

         stateExpression = SyntaxFactory.TupleExpression(
            SyntaxFactory.SeparatedList(tupleElements));
      }

      // Build new arguments
      var newArguments = new List<ArgumentSyntax>();

      // State argument (first)
      var stateArg = SyntaxFactory.Argument(
         SyntaxFactory.NameColon("state"),
         default,
         stateExpression);
      newArguments.Add(stateArg);

      var captureSet = new HashSet<ISymbol>(captures, SymbolEqualityComparer.Default);

      // Transform each existing argument
      foreach (var argument in invocation.ArgumentList.Arguments)
      {
         if (argument.Expression is LambdaExpressionSyntax lambda
             && !lambda.Modifiers.Any(SyntaxKind.StaticKeyword))
         {
            var newLambda = TransformLambdaForStateOverload(semanticModel, lambda, captures, captureSet);
            newArguments.Add(argument.WithExpression(newLambda));
         }
         else
         {
            newArguments.Add(argument);
         }
      }

      var newArgumentList = invocation.ArgumentList.WithArguments(
         SyntaxFactory.SeparatedList(newArguments));

      var newInvocation = invocation.WithArgumentList(newArgumentList);
      var newRoot = root.ReplaceNode(invocation, newInvocation);

      return document.WithSyntaxRoot(newRoot);
   }

   private static LambdaExpressionSyntax TransformLambdaForStateOverload(
      SemanticModel semanticModel,
      LambdaExpressionSyntax lambda,
      List<ISymbol> captures,
      HashSet<ISymbol> captureSet)
   {
      // Step 1: Replace capture references in the body
      var body = (SyntaxNode?)lambda.ExpressionBody ?? lambda.Block;

      if (body is null)
         return lambda;

      var identifiersToReplace = new Dictionary<IdentifierNameSyntax, ExpressionSyntax>();

      foreach (var identifier in body.DescendantNodes().OfType<IdentifierNameSyntax>())
      {
         var symbolInfo = semanticModel.GetSymbolInfo(identifier);

         if (symbolInfo.Symbol is null || !captureSet.Contains(symbolInfo.Symbol))
            continue;

         ExpressionSyntax replacement;

         if (captures.Count == 1)
         {
            replacement = SyntaxFactory.IdentifierName("state").WithTriviaFrom(identifier);
         }
         else
         {
            replacement = SyntaxFactory.MemberAccessExpression(
               SyntaxKind.SimpleMemberAccessExpression,
               SyntaxFactory.IdentifierName("state"),
               SyntaxFactory.IdentifierName(identifier.Identifier.Text)).WithTriviaFrom(identifier);
         }

         identifiersToReplace[identifier] = replacement;
      }

      // Apply body replacements
      var transformedLambda = identifiersToReplace.Count > 0
                                 ? lambda.ReplaceNodes(identifiersToReplace.Keys, (original, _) => identifiersToReplace[original])
                                 : lambda;

      // Step 2: Add static modifier
      var newModifiers = transformedLambda.Modifiers;

      if (!newModifiers.Any(SyntaxKind.StaticKeyword))
      {
         var staticToken = SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SyntaxFactory.Space);
         newModifiers = newModifiers.Insert(0, staticToken);
      }

      // Step 3: Add state parameter
      var stateParam = SyntaxFactory.Parameter(SyntaxFactory.Identifier("state"));

      if (transformedLambda is SimpleLambdaExpressionSyntax simple)
      {
         var paramList = SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList([stateParam, simple.Parameter]));

         return SyntaxFactory.ParenthesizedLambdaExpression()
                             .WithModifiers(newModifiers)
                             .WithParameterList(paramList)
                             .WithArrowToken(simple.ArrowToken)
                             .WithExpressionBody(simple.ExpressionBody)
                             .WithBlock(simple.Block)
                             .WithTriviaFrom(transformedLambda);
      }

      if (transformedLambda is ParenthesizedLambdaExpressionSyntax paren)
      {
         var allParams = paren.ParameterList.Parameters.Insert(0, stateParam);

         if (allParams.Count == 1)
         {
            // Single parameter — use SimpleLambdaExpressionSyntax: state => ...
            return SyntaxFactory.SimpleLambdaExpression(stateParam)
                                .WithModifiers(newModifiers)
                                .WithArrowToken(paren.ArrowToken)
                                .WithExpressionBody(paren.ExpressionBody)
                                .WithBlock(paren.Block)
                                .WithTriviaFrom(transformedLambda);
         }

         var newParamList = paren.ParameterList.WithParameters(allParams);
         return paren.WithModifiers(newModifiers).WithParameterList(newParamList);
      }

      return transformedLambda;
   }

   private sealed class CodeFixesContext(Diagnostic diagnostic, SyntaxNode root)
   {
      public TypeDeclarationSyntax? TypeDeclaration => field ??= GetDeclaration<TypeDeclarationSyntax>();
      public FieldDeclarationSyntax? FieldDeclaration => field ??= GetDeclaration<FieldDeclarationSyntax>();
      public MemberDeclarationSyntax? MemberDeclaration => field ??= GetDeclaration<MemberDeclarationSyntax>();
      public PropertyDeclarationSyntax? PropertyDeclaration => field ??= GetDeclaration<PropertyDeclarationSyntax>();
      public MethodDeclarationSyntax? MethodDeclaration => field ??= GetDeclaration<MethodDeclarationSyntax>();
      public InvocationExpressionSyntax? InvocationExpressionSyntax => field ??= GetDeclaration<InvocationExpressionSyntax>();

      private T? GetDeclaration<T>()
         where T : CSharpSyntaxNode
      {
         var diagnosticSpan = diagnostic.Location.SourceSpan;
         return root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<T>().FirstOrDefault();
      }
   }
}
