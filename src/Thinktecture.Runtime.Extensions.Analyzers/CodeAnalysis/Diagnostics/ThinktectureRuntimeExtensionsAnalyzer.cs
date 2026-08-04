using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ThinktectureRuntimeExtensionsAnalyzer : DiagnosticAnalyzer
{
   /// <inheritdoc />
   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
   [
      DiagnosticsDescriptors.TypeMustBePartial,
      DiagnosticsDescriptors.TypeMustBeClassOrStruct,
      DiagnosticsDescriptors.ConstructorsMustBePrivate,
      DiagnosticsDescriptors.SmartEnumHasNoItems,
      DiagnosticsDescriptors.SmartEnumItemMustBePublic,
      DiagnosticsDescriptors.FieldMustBeReadOnly,
      DiagnosticsDescriptors.PropertyMustBeReadOnly,
      DiagnosticsDescriptors.SmartEnumKeyMemberNameNotAllowed,
      DiagnosticsDescriptors.InnerSmartEnumOnFirstLevelMustBePrivate,
      DiagnosticsDescriptors.InnerSmartEnumOnNonFirstLevelMustBePublic,
      DiagnosticsDescriptors.KeyMemberShouldNotBeNullable,
      DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems,
      DiagnosticsDescriptors.BaseClassFieldMustBeReadOnly,
      DiagnosticsDescriptors.BaseClassPropertyMustBeReadOnly,
      DiagnosticsDescriptors.SmartEnumKeyShouldNotBeNullable,
      DiagnosticsDescriptors.SmartEnumWithoutDerivedTypesMustBeSealed,
      DiagnosticsDescriptors.ComparerTypeMustMatchMemberType,
      DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
      DiagnosticsDescriptors.InitAccessorMustBePrivate,
      DiagnosticsDescriptors.PrimaryConstructorNotAllowed,
      DiagnosticsDescriptors.CustomKeyMemberImplementationNotFound,
      DiagnosticsDescriptors.CustomKeyMemberImplementationTypeMismatch,
      DiagnosticsDescriptors.IndexBasedSwitchAndMapMustUseNamedParameters,
      DiagnosticsDescriptors.VariableMustBeInitializedWithNonDefaultValue,
      DiagnosticsDescriptors.StringBasedValueObjectNeedsEqualityComparer,
      DiagnosticsDescriptors.ComplexValueObjectWithStringMembersNeedsDefaultEqualityComparer,
      DiagnosticsDescriptors.ExplicitComparerWithoutEqualityComparer,
      DiagnosticsDescriptors.ExplicitEqualityComparerWithoutComparer,
      DiagnosticsDescriptors.MethodWithUseDelegateFromConstructorMustBePartial,
      DiagnosticsDescriptors.MethodWithUseDelegateFromConstructorMustNotHaveGenerics,
      DiagnosticsDescriptors.UnionDerivedTypesMustNotBeGeneric,
      DiagnosticsDescriptors.UnionMustBeSealedOrHavePrivateConstructorsOnly,
      DiagnosticsDescriptors.UnionRecordMustBeSealed,
      DiagnosticsDescriptors.NonAbstractDerivedUnionIsLessAccessibleThanBaseUnion,
      DiagnosticsDescriptors.InnerTypeDoesNotDeriveFromUnion,
      DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfValueObjectIsStructButKeyTypeIsClass,
      DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfSomeMembersDisallowDefaultValues,
      DiagnosticsDescriptors.MembersDisallowingDefaultValuesMustBeRequired,
      DiagnosticsDescriptors.ObjectFactoryMustHaveCorrespondingConstructor,
      DiagnosticsDescriptors.SmartEnumMustNotHaveObjectFactoryConstructor,
      DiagnosticsDescriptors.ObjectFactoryMustImplementStaticValidateMethod,
      DiagnosticsDescriptors.ObjectFactoryMustImplementToValueMethod,
      DiagnosticsDescriptors.TypeMustNotHaveMoreThanOneAttribute,
      DiagnosticsDescriptors.MultipleObjectFactoryAttributesWithUseWithEntityFramework,
      DiagnosticsDescriptors.MultipleObjectFactoryAttributesWithUseForModelBinding,
      DiagnosticsDescriptors.MultipleObjectFactoryAttributesWithOverlappingSerializationFrameworks,
      DiagnosticsDescriptors.RefStructObjectFactoryMustNotBeUsedWithEntityFrameworkOrModelBinding,
      DiagnosticsDescriptors.RefStructObjectFactoryIgnoredBySerializationFrameworks,
      DiagnosticsDescriptors.EmptyStringInFactoryMethodsYieldsNullHasNoEffectOnStructs,
      DiagnosticsDescriptors.AdHocUnionMemberTypeNotConvertibleToSingleBackingFieldType,
      DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneSmartEnumAttribute,
      DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneValueObjectAttribute,
      DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneDiscriminatedUnionAttribute,
      DiagnosticsDescriptors.AdHocUnionMustHaveAtLeastTwoMemberTypes,
      DiagnosticsDescriptors.ComparisonAndEqualityOperatorsMismatch,
      DiagnosticsDescriptors.UseSwitchMapWithStaticLambda,
      DiagnosticsDescriptors.TypeParamRefRequiresNotnullConstraint,
      DiagnosticsDescriptors.SingleBackingFieldTypeConflictsWithUseSingleBackingField,
      DiagnosticsDescriptors.ValidateFactoryArgumentsAdditionalParameterMustNotHaveDefaultOrBeByRef,
      DiagnosticsDescriptors.AdHocUnionMemberTypeIsLessAccessibleThanUnion,
      DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfTypeImplementsIDisallowDefaultValue,
      DiagnosticsDescriptors.IDisallowDefaultValueHasNoEffectOnReferenceTypes,
      DiagnosticsDescriptors.DefaultValueHandlingMapToFirstMemberRequiresStructUnion,
      DiagnosticsDescriptors.DefaultValueHandlingMapToFirstMemberRequiresStatelessFirstMember,
   ];

   /// <inheritdoc />
   public override void Initialize(AnalysisContext context)
   {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterSymbolAction(AnalyzeNamedTypes, SymbolKind.NamedType);

      context.RegisterOperationAction(AnalyzeMethodWithUseDelegateFromConstructor, OperationKind.Attribute);

      context.RegisterOperationAction(AnalyzeMethodCall, OperationKind.Invocation);
      context.RegisterOperationAction(AnalyzeDefaultValueAssignment, OperationKind.DefaultValue);
      context.RegisterOperationAction(AnalyzeObjectCreation, OperationKind.ObjectCreation);

      context.RegisterSyntaxNodeAction(AnalyzeFieldDisallowingDefaultValues, SyntaxKind.FieldDeclaration);
      context.RegisterSyntaxNodeAction(AnalyzePropertyDisallowingDefaultValues, SyntaxKind.PropertyDeclaration);
   }

   private void AnalyzeNamedTypes(SymbolAnalysisContext context)
   {
      var type = (INamedTypeSymbol)context.Symbol;

      try
      {
         if (!TryGetThinktectureAttibutes(
                type,
                out var smartEnumAttribute,
                out var keyedValueObjectAttribute,
                out var complexValueObjectAttribute,
                out var regularUnionAttribute,
                out var adHocUnionAttribute,
                out var objectFactoryAttributes))
         {
            // The type carries no Thinktecture attribute, but it may still implement 'IDisallowDefaultValue'
            // manually. A reference type gains no effect from the interface (TTRESG110).
            CheckManualIDisallowDefaultValue(context, type, keyedValueObjectAttribute: null, complexValueObjectAttribute: null);
            return;
         }

         CheckManualIDisallowDefaultValue(context, type, keyedValueObjectAttribute, complexValueObjectAttribute);

         var needsObjectFactoryHandling = true;

         if (smartEnumAttribute is not null)
         {
            ValidateSmartEnum(context, type, smartEnumAttribute);
            ValidateObjectFactories(context, type, objectFactoryAttributes, true);

            needsObjectFactoryHandling = false;
         }

         if (keyedValueObjectAttribute is not null)
         {
            var tdsLocation = type.GetTypeIdentifierLocation(context.CancellationToken);
            var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.Compilation);

            if (factory is null)
            {
               ReportDiagnostic(
                  context,
                  DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                  tdsLocation,
                  type.ToFullyQualifiedDisplayString(),
                  "Could not fetch type information for analysis of the value object.");
               return;
            }

            var assignableMembers = ValidateSharedValueObject(context, type, tdsLocation, factory);

            ValidateKeyedValueObject(context, assignableMembers, type, keyedValueObjectAttribute, tdsLocation, factory);
         }

         if (complexValueObjectAttribute is not null)
         {
            var tdsLocation = type.GetTypeIdentifierLocation(context.CancellationToken);
            var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.Compilation);

            if (factory is null)
            {
               ReportDiagnostic(
                  context,
                  DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                  tdsLocation,
                  type.ToFullyQualifiedDisplayString(),
                  "Could not fetch type information for analysis of the value object.");
               return;
            }

            var assignableMembers = ValidateSharedValueObject(context, type, tdsLocation, factory);

            if (assignableMembers is not null)
            {
               CheckAssignableMembers(context, assignableMembers, type, complexValueObjectAttribute, tdsLocation);
               ValidateFactoryArgumentsAdditionalParameters(context, type, 1 + assignableMembers.Count, tdsLocation);
            }
         }

         if (regularUnionAttribute is not null)
         {
            ValidateRegularUnion(context, type);
            ValidateObjectFactories(context, type, objectFactoryAttributes, false);

            needsObjectFactoryHandling = false;
         }

         if (adHocUnionAttribute is not null)
         {
            ValidateAdHocUnion(context, type, adHocUnionAttribute);
            ValidateObjectFactories(context, type, objectFactoryAttributes, false);

            needsObjectFactoryHandling = false;
         }

         if (needsObjectFactoryHandling)
            ValidateObjectFactories(context, type, objectFactoryAttributes, false);
      }
      catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    Location.None,
                                                    type.ToFullyQualifiedDisplayString(), ex.ToString()));
      }
   }

   /// <summary>
   /// Reports on a manual (user-declared) implementation of 'IDisallowDefaultValue'.
   /// <list type="bullet">
   /// <item>TTRESG110 (Warning): the type is a reference type, where the interface has no effect.</item>
   /// <item>TTRESG080 (Error): the type is a value object with 'AllowDefaultStructs = true', which conflicts
   /// with the interface because the interface disallows the default value.</item>
   /// </list>
   /// Only a base/interface list written by the user counts. The interface that the source generator emits on the
   /// generated partial declaration is ignored, so re-emitting it never triggers these diagnostics.
   /// </summary>
   private static void CheckManualIDisallowDefaultValue(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      AttributeData? keyedValueObjectAttribute,
      AttributeData? complexValueObjectAttribute)
   {
      var location = GetUserDeclaredIDisallowDefaultValueLocation(context, type);

      if (location is null)
         return;

      if (type.IsReferenceType)
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.IDisallowDefaultValueHasNoEffectOnReferenceTypes,
            location,
            BuildTypeName(type));

         return;
      }

      var allowsDefaultStructs = (keyedValueObjectAttribute?.FindAllowDefaultStructs() ?? false)
                                 || (complexValueObjectAttribute?.FindAllowDefaultStructs() ?? false);

      if (allowsDefaultStructs)
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfTypeImplementsIDisallowDefaultValue,
            location,
            BuildTypeName(type));
      }
   }

   /// <summary>
   /// Returns the location of the 'IDisallowDefaultValue' entry in a base/interface list that the user wrote,
   /// or <c>null</c> if the user did not implement the interface manually. The generated partial declaration is
   /// skipped, so the interface the source generator emits is not treated as a manual implementation.
   /// </summary>
   private static Location? GetUserDeclaredIDisallowDefaultValueLocation(
      SymbolAnalysisContext context,
      INamedTypeSymbol type)
   {
      var references = type.DeclaringSyntaxReferences;

      for (var i = 0; i < references.Length; i++)
      {
         if (references[i].GetSyntax(context.CancellationToken) is not TypeDeclarationSyntax tds
             || tds.BaseList is null
             || tds.SyntaxTree.IsGeneratedTree(context.CancellationToken))
         {
            continue;
         }

         // The base/interface list is written by the user, so resolving its entries needs a semantic model.
         // The symbol action does not provide one, and the resolution runs only for a non-generated declaration
         // that has a base list, which keeps the cost low.
#pragma warning disable RS1030
         var semanticModel = context.Compilation.GetSemanticModel(tds.SyntaxTree);
#pragma warning restore RS1030
         var baseTypes = tds.BaseList.Types;

         for (var j = 0; j < baseTypes.Count; j++)
         {
            var baseTypeSyntax = baseTypes[j];
            var typeSymbol = semanticModel.GetTypeInfo(baseTypeSyntax.Type, context.CancellationToken).Type;

            if (typeSymbol.IsIDisallowDefaultValue())
               return baseTypeSyntax.GetLocation();
         }
      }

      return null;
   }

   private static bool TryGetThinktectureAttibutes(
      INamedTypeSymbol type,
      out AttributeData? smartEnumAttribute,
      out AttributeData? keyedValueObjectAttribute,
      out AttributeData? complexValueObjectAttribute,
      out AttributeData? regularUnionAttribute,
      out AttributeData? adHocUnionAttribute,
      out ImmutableArray<AttributeData> objectFactoryAttributes)
   {
      smartEnumAttribute = null;
      keyedValueObjectAttribute = null;
      complexValueObjectAttribute = null;
      regularUnionAttribute = null;
      adHocUnionAttribute = null;
      objectFactoryAttributes = ImmutableArray<AttributeData>.Empty;

      var hasAnyThinktectureAttribute = false;
      var attributes = type.GetAttributes();

      for (var i = 0; i < attributes.Length; i++)
      {
         var attribute = attributes[i];

         if (attribute.AttributeClass.IsSmartEnumAttribute())
         {
            smartEnumAttribute = attribute;
            hasAnyThinktectureAttribute = true;
         }
         else if (attribute.AttributeClass.IsKeyedValueObjectAttribute())
         {
            keyedValueObjectAttribute = attribute;
            hasAnyThinktectureAttribute = true;
         }
         else if (attribute.AttributeClass.IsComplexValueObjectAttribute())
         {
            complexValueObjectAttribute = attribute;
            hasAnyThinktectureAttribute = true;
         }
         else if (attribute.AttributeClass.IsRegularUnionAttribute())
         {
            regularUnionAttribute = attribute;
            hasAnyThinktectureAttribute = true;
         }
         else if (attribute.AttributeClass.IsAdHocUnionAttribute())
         {
            adHocUnionAttribute = attribute;
            hasAnyThinktectureAttribute = true;
         }
         else if (attribute.AttributeClass.IsObjectFactoryAttribute())
         {
            objectFactoryAttributes = objectFactoryAttributes.Add(attribute);
            hasAnyThinktectureAttribute = true;
         }
      }

      return hasAnyThinktectureAttribute;
   }

   private static void AnalyzePropertyDisallowingDefaultValues(SyntaxNodeAnalysisContext context)
   {
      if (context.Node is not PropertyDeclarationSyntax propertyDeclarationSyntax
          || propertyDeclarationSyntax.ExpressionBody is not null // public MyStruct Member => ...;
          || propertyDeclarationSyntax.Initializer is not null    // public MyStruct Member { get; } = ...;
          || context.ContainingSymbol is not IPropertySymbol propertySymbol
          || propertySymbol.SetMethod is null // public MyStruct Member { get; }
          || propertySymbol.IsStatic
          || propertySymbol.ContainingType.TypeKind == TypeKind.Interface                                          // interfaces cannot have required members
          || propertySymbol.DeclaredAccessibility < propertySymbol.ContainingType.DeclaredAccessibility            // required members must not be less visible than the containing type
          || propertySymbol.SetMethod.DeclaredAccessibility < propertySymbol.ContainingType.DeclaredAccessibility) // setter of required members must not be less visible than the containing type
         return;

      MemberDisallowingDefaultValuesMustBeRequired(context, propertyDeclarationSyntax, propertyDeclarationSyntax.GetLocation(), propertySymbol.Type, "property", propertySymbol.Name);
   }

   private static void AnalyzeFieldDisallowingDefaultValues(SyntaxNodeAnalysisContext context)
   {
      if (context.Node is not FieldDeclarationSyntax fieldDeclarationSyntax
          || context.ContainingSymbol is not IFieldSymbol fieldSymbol
          || fieldSymbol.IsReadOnly
          || fieldSymbol.IsStatic
          || fieldSymbol.DeclaredAccessibility < fieldSymbol.ContainingType.DeclaredAccessibility) // required members must not be less visible than the containing type
         return;

      // A single field statement may declare several variables (e.g. `public MyStruct a = ..., b;`).
      // The type and modifiers are shared, but each variable has its own initializer, so evaluate the
      // initializer per variable and report only on variables that lack one. The location and name are
      // taken from the specific variable, so a multi-declarator field reports precisely.
      foreach (var variable in fieldDeclarationSyntax.Declaration.Variables)
      {
         if (variable.Initializer is not null) // public MyStruct Member = ...;
            continue;

         MemberDisallowingDefaultValuesMustBeRequired(
            context,
            fieldDeclarationSyntax,
            variable.Identifier.GetLocation(),
            fieldSymbol.Type,
            "field",
            variable.Identifier.Text);
      }
   }

   private static void MemberDisallowingDefaultValuesMustBeRequired(
      SyntaxNodeAnalysisContext context,
      MemberDeclarationSyntax memberDeclarationSyntax,
      Location location,
      ITypeSymbol memberType,
      string memberKind,
      string memberName)
   {
      if (memberDeclarationSyntax.Modifiers.Any(SyntaxKind.RequiredKeyword))
         return;

      if (memberType.SpecialType != SpecialType.None || !memberType.IsValueType)
         return;

      if (!memberType.ImplementsIDisallowDefaultValue())
         return;

      context.ReportDiagnostic(Diagnostic.Create(
                                  DiagnosticsDescriptors.MembersDisallowingDefaultValuesMustBeRequired,
                                  location,
                                  memberKind, memberName, BuildTypeName(memberType)));
   }

   private static void AnalyzeMethodWithUseDelegateFromConstructor(OperationAnalysisContext context)
   {
      if (context.ContainingSymbol.Kind != SymbolKind.Method
          || context.Operation is not IAttributeOperation { Operation: IObjectCreationOperation attrCreation }
          || !attrCreation.Type.IsUseDelegateFromConstructorAttribute()
          || context.ContainingSymbol is not IMethodSymbol method)
      {
         return;
      }

      try
      {
         if (method.DeclaringSyntaxReferences.IsDefaultOrEmpty)
            return;

         for (var i = 0; i < method.DeclaringSyntaxReferences.Length; i++)
         {
            var methodSyntax = method.DeclaringSyntaxReferences[i];

            if (methodSyntax.GetSyntax(context.CancellationToken) is not MethodDeclarationSyntax mds)
               continue;

            if (!mds.IsPartial())
            {
               ReportDiagnostic(
                  context,
                  DiagnosticsDescriptors.MethodWithUseDelegateFromConstructorMustBePartial,
                  mds.Identifier.GetLocation(),
                  method.Name);
            }

            if (!method.TypeParameters.IsDefaultOrEmpty)
            {
               ReportDiagnostic(
                  context,
                  DiagnosticsDescriptors.MethodWithUseDelegateFromConstructorMustNotHaveGenerics,
                  mds.Identifier.GetLocation(),
                  method.Name);
            }
         }
      }
      catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    Location.None,
                                                    method.ToDisplayString(), ex.ToString()));
      }
   }

   private static void AnalyzeObjectCreation(OperationAnalysisContext context)
   {
      var operation = (IObjectCreationOperation)context.Operation;

      if (operation.Type is null
          || operation.Type.IsReferenceType
          || !operation.Arguments.IsDefaultOrEmpty)
         return;

      if (operation.Type.ImplementsIDisallowDefaultValue())
         ReportDiagnostic(context, DiagnosticsDescriptors.VariableMustBeInitializedWithNonDefaultValue, operation.Syntax.GetLocation(), operation.Type);
   }

   private static void AnalyzeDefaultValueAssignment(OperationAnalysisContext context)
   {
      var operation = (IDefaultValueOperation)context.Operation;

      if (operation.Type is null
          || operation.Type.IsReferenceType
          || !IsAssignmentOrInitialization(operation.Parent))
      {
         return;
      }

      if (operation.Type.ImplementsIDisallowDefaultValue())
         ReportDiagnostic(context, DiagnosticsDescriptors.VariableMustBeInitializedWithNonDefaultValue, operation.Syntax.GetLocation(), operation.Type);
   }

   private static bool IsAssignmentOrInitialization(IOperation? operation)
   {
      while (operation is not null)
      {
         switch (operation.Kind)
         {
            case OperationKind.Binary:
               return false;

            case OperationKind.Conversion:
            case OperationKind.Conditional: // ternary: condition ? default : value
            case OperationKind.ArrayInitializer:
               operation = operation.Parent;
               break;

            case OperationKind.VariableInitializer:
            case OperationKind.ParameterInitializer: // void MyMethod(MyUnion u = default)
            case OperationKind.FieldInitializer:     // private MyUnion _f = default;
            case OperationKind.PropertyInitializer:  // public MyUnion P { get; } = default;
            case OperationKind.SimpleAssignment:
            case OperationKind.CoalesceAssignment:
            case OperationKind.CompoundAssignment:
            case OperationKind.DeconstructionAssignment:
            case OperationKind.Argument:
            case OperationKind.Tuple:         // (42, default(MyUnion))
            case OperationKind.Return:        // return default;
            case OperationKind.ArrayCreation: // new[] { default }
               return true;

            default:
               return false;
         }
      }

      return false;
   }

   private static void AnalyzeMethodCall(OperationAnalysisContext context)
   {
      var operation = (IInvocationOperation)context.Operation;

      if (operation.Instance is null
          || operation.Arguments.IsDefaultOrEmpty
          || operation.TargetMethod.IsStatic
          || (operation.TargetMethod.Name != Constants.Methods.SWITCH
              && operation.TargetMethod.Name != Constants.Methods.SWITCH_PARTIALLY
              && operation.TargetMethod.Name != Constants.Methods.MAP
              && operation.TargetMethod.Name != Constants.Methods.MAP_PARTIALLY))
      {
         return;
      }

      var declaredType = operation.TargetMethod.ContainingType;

      if (declaredType.IsSmartEnum(out var attribute))
      {
         var items = declaredType.GetEnumItems();

         AnalyzeEnumSwitchMap(context,
                              items,
                              operation.Arguments,
                              operation);
      }
      else if (declaredType.IsAnyUnionType(out attribute)
               && attribute.AttributeClass is not null)
      {
         AnalyzeAnyUnionSwitchMap(context,
                                  operation.Arguments,
                                  operation);
      }
      else
      {
         return;
      }

      if (operation.TargetMethod.Name is Constants.Methods.SWITCH or Constants.Methods.SWITCH_PARTIALLY)
      {
         AnalyzeSwitchMapLambdas(context, operation);
      }
   }

   private static void AnalyzeEnumSwitchMap(
      OperationAnalysisContext context,
      ImmutableArray<IFieldSymbol> items,
      ImmutableArray<IArgumentOperation> args,
      IInvocationOperation operation)
   {
      var numberOfCallbacks = (items.IsDefaultOrEmpty ? 0 : items.Length)
                              + (operation.TargetMethod.Name is Constants.Methods.SWITCH_PARTIALLY or Constants.Methods.MAP_PARTIALLY ? 1 : 0);

      AnalyzeSwitchMap(context, args, operation, numberOfCallbacks);
   }

   private static void AnalyzeAnyUnionSwitchMap(
      OperationAnalysisContext context,
      ImmutableArray<IArgumentOperation> args,
      IInvocationOperation operation)
   {
      var numberOfCallbacks = operation.TargetMethod.Parameters.IsDefaultOrEmpty ? 0 : operation.TargetMethod.Parameters.Length;

      var originalMethod = operation.TargetMethod.OriginalDefinition;

      if (numberOfCallbacks > 0
          && originalMethod.Parameters[0].Type is { TypeKind: TypeKind.TypeParameter } firstParamType
          && !SymbolEqualityComparer.Default.Equals(firstParamType, originalMethod.ReturnType))
      {
         numberOfCallbacks--;
      }

      AnalyzeSwitchMap(context, args, operation, numberOfCallbacks);
   }

   private static void AnalyzeSwitchMap(
      OperationAnalysisContext context,
      ImmutableArray<IArgumentOperation> args,
      IInvocationOperation operation,
      int numberOfCallbacks)
   {
      if (args.IsDefaultOrEmpty)
         return;

      var hasNonNamedParameters = false;
      var numberOfParameters = operation.TargetMethod.Parameters.IsDefaultOrEmpty ? 0 : operation.TargetMethod.Parameters.Length;
      var argsStartIndex = numberOfParameters == numberOfCallbacks ? 0 : 1;

      for (var argIndex = argsStartIndex; argIndex < args.Length; argIndex++)
      {
         var argument = args[argIndex];

         if (argument.ArgumentKind == ArgumentKind.DefaultValue || argument.Syntax is not ArgumentSyntax argSyntax)
            continue;

         if (argSyntax.NameColon is not null)
            continue;

         hasNonNamedParameters = true;
         break;
      }

      if (hasNonNamedParameters)
         ReportDiagnostic(context, DiagnosticsDescriptors.IndexBasedSwitchAndMapMustUseNamedParameters, operation.Syntax.GetLocation(), operation.TargetMethod.ContainingType);
   }

   private static void AnalyzeSwitchMapLambdas(
      OperationAnalysisContext context,
      IInvocationOperation operation)
   {
      var args = operation.Arguments;

      if (args.IsDefaultOrEmpty)
         return;

      var numberOfParameters = operation.TargetMethod.Parameters.IsDefaultOrEmpty ? 0 : operation.TargetMethod.Parameters.Length;
      var argsStartIndex = numberOfParameters > 0 && operation.TargetMethod.OriginalDefinition.Parameters[0].Type.TypeKind == TypeKind.TypeParameter ? 1 : 0;

      for (var argIndex = argsStartIndex; argIndex < args.Length; argIndex++)
      {
         var argument = args[argIndex];

         if (argument.ArgumentKind == ArgumentKind.DefaultValue)
            continue;

         if (argument.Syntax is not ArgumentSyntax { Expression: LambdaExpressionSyntax lambda })
            continue;

         if (lambda.Modifiers.Any(SyntaxKind.StaticKeyword))
            continue;

         // Report once on the method name, not on individual lambdas
         if (operation.Syntax is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax memberAccess })
            ReportDiagnostic(context, DiagnosticsDescriptors.UseSwitchMapWithStaticLambda, memberAccess.Name.GetLocation(), operation.TargetMethod.Name);

         return;
      }
   }

   private static void ValidateAdHocUnion(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      AttributeData adHocUnionAttribute)
   {
      if (type.IsRecord || type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.TypeMustBeClassOrStruct,
            type.GetTypeIdentifierLocation(context.CancellationToken),
            type);
         return;
      }

      CheckConstructors(context, type, mustBePrivate: false, canHavePrimaryConstructor: false);
      TypeMustBePartial(context, type);

      // TTRESG075: explicit 'UseSingleBackingField = false' conflicts with 'SingleBackingFieldType'.
      var singleBackingFieldType = adHocUnionAttribute.FindSingleBackingFieldType();
      var useSingleBackingField = adHocUnionAttribute.FindUseSingleBackingField();

      if (singleBackingFieldType is not null && useSingleBackingField == false)
      {
         var location = adHocUnionAttribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation()
                        ?? type.GetTypeIdentifierLocation(context.CancellationToken);

         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.SingleBackingFieldTypeConflictsWithUseSingleBackingField,
            location,
            BuildTypeName(type));
      }
      else if (singleBackingFieldType is not null)
      {
         CheckAdHocUnionMemberTypesAreConvertibleToBackingFieldType(context, type, adHocUnionAttribute, singleBackingFieldType);
      }

      CheckAdHocUnionMemberTypeAccessibility(context, type, adHocUnionAttribute);
      CheckAdHocUnionDefaultValueHandling(context, type, adHocUnionAttribute);
   }

   /// <summary>
   /// TTRESG081/TTRESG082: 'DefaultValueHandling = MapToFirstMember' maps 'default(TUnion)' to the first member.
   /// This requires a struct union (a reference type default is 'null') and a stateless first member (the default
   /// value carries no state).
   /// </summary>
   private static void CheckAdHocUnionDefaultValueHandling(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      AttributeData adHocUnionAttribute)
   {
      if (adHocUnionAttribute.FindDefaultValueHandling() != UnionDefaultValueHandling.MapToFirstMember)
         return;

      var location = GetNamedArgumentLocationOrFallback(
         adHocUnionAttribute,
         Constants.Attributes.Properties.DEFAULT_VALUE_HANDLING,
         type.GetTypeIdentifierLocation(context.CancellationToken),
         context.CancellationToken);

      if (type.IsReferenceType)
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.DefaultValueHandlingMapToFirstMemberRequiresStructUnion,
            location,
            BuildTypeName(type));

         return;
      }

      if (!adHocUnionAttribute.FindTxIsStateless(1))
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.DefaultValueHandlingMapToFirstMemberRequiresStatelessFirstMember,
            location,
            BuildTypeName(type));
      }
   }

   /// <summary>
   /// TTRESG079: the generator assigns every member value directly into the backing field typed as
   /// 'SingleBackingFieldType'. Without a built-in implicit conversion the generated code does not compile (CS0029).
   /// A user-defined implicit conversion would store a different instance and is rejected as well.
   /// A stateless reference-type member is exempt as long as the union has at least one non-stateless member,
   /// because then the generator never converts that member's value into the backing field.
   /// </summary>
   private static void CheckAdHocUnionMemberTypesAreConvertibleToBackingFieldType(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      AttributeData adHocUnionAttribute,
      ITypeSymbol singleBackingFieldType)
   {
      // Deliberate limitation: types containing TypeParamRef markers are not resolved here, so they are skipped.
      if (singleBackingFieldType.GetMaxTypeParamRefIndex() > 0 || context.Compilation is not CSharpCompilation csharpCompilation)
         return;

      var memberTypes = GetAdHocUnionMemberTypes(adHocUnionAttribute);
      var hasNonStatelessMember = HasNonStatelessAdHocUnionMember(adHocUnionAttribute, memberTypes.Length);

      for (var i = 0; i < memberTypes.Length; i++)
      {
         var memberType = memberTypes[i];

         if (memberType.TypeKind == TypeKind.Error || memberType.GetMaxTypeParamRefIndex() > 0)
            continue;

         // The value of a stateless reference-type member never reaches the backing field: the constructor
         // emits no assignment, 'AsTx' returns 'default(T)' and the raw value getter reads the shared field,
         // which stays null for that member. Only when every member is stateless does the getter convert
         // 'default(T)' to the backing field type, so the check stays active for that case.
         if (memberType.IsReferenceType && hasNonStatelessMember && adHocUnionAttribute.FindTxIsStateless(i + 1))
            continue;

         var conversion = csharpCompilation.ClassifyConversion(memberType, singleBackingFieldType);

         if (conversion.IsImplicit && !conversion.IsUserDefined)
            continue;

         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.AdHocUnionMemberTypeNotConvertibleToSingleBackingFieldType,
            GetNamedArgumentLocationOrFallback(adHocUnionAttribute, Constants.Attributes.Properties.SINGLE_BACKING_FIELD_TYPE, type.GetTypeIdentifierLocation(context.CancellationToken), context.CancellationToken),
            BuildTypeName(memberType),
            BuildTypeName(type),
            BuildTypeName(singleBackingFieldType));
      }
   }

   private static bool HasNonStatelessAdHocUnionMember(AttributeData adHocUnionAttribute, int memberCount)
   {
      for (var i = 0; i < memberCount; i++)
      {
         if (!adHocUnionAttribute.FindTxIsStateless(i + 1))
            return true;
      }

      return false;
   }

   private static void CheckAdHocUnionMemberTypeAccessibility(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      AttributeData adHocUnionAttribute)
   {
      var unionAccessibility = type.GetEffectiveAccessibility();
      var memberTypes = GetAdHocUnionMemberTypes(adHocUnionAttribute);

      for (var i = 0; i < memberTypes.Length; i++)
      {
         var memberType = memberTypes[i];

         if (memberType.TypeKind == TypeKind.Error)
            continue;

         if (memberType.GetEffectiveAccessibility() >= unionAccessibility)
            continue;

         var location = adHocUnionAttribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation()
                        ?? type.GetTypeIdentifierLocation(context.CancellationToken);

         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.AdHocUnionMemberTypeIsLessAccessibleThanUnion,
            location,
            memberType,
            type);
      }
   }

   private static ImmutableArray<ITypeSymbol> GetAdHocUnionMemberTypes(AttributeData adHocUnionAttribute)
   {
      var attributeClass = adHocUnionAttribute.AttributeClass;

      if (attributeClass is null)
         return ImmutableArray<ITypeSymbol>.Empty;

      if (!attributeClass.TypeArguments.IsDefaultOrEmpty)
         return attributeClass.TypeArguments;

      var constructorArguments = adHocUnionAttribute.ConstructorArguments;

      if (constructorArguments.IsDefaultOrEmpty)
         return ImmutableArray<ITypeSymbol>.Empty;

      var memberTypes = ImmutableArray.CreateBuilder<ITypeSymbol>(constructorArguments.Length);

      for (var i = 0; i < constructorArguments.Length; i++)
      {
         if (constructorArguments[i].Value is ITypeSymbol memberType)
            memberTypes.Add(memberType);
      }

      return memberTypes.DrainToImmutable();
   }

   private static void ValidateRegularUnion(
      SymbolAnalysisContext context,
      INamedTypeSymbol type)
   {
      CheckConstructors(context, type, mustBePrivate: true, canHavePrimaryConstructor: false);
      TypeMustBePartial(context, type);
      CheckForNonDerivedUnionTypes(context, type);
      ValidateUnionDerivedTypes(context, type);
   }

   private static void ValidateObjectFactories(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      ImmutableArray<AttributeData> objectFactoryAttributes,
      bool isSmartEnum)
   {
      if (objectFactoryAttributes.Length > 1)
      {
         CheckObjectFactoryUseWithEntityFrameworkConflicts(context, type, objectFactoryAttributes);
         CheckObjectFactoryUseForModelBindingConflicts(context, type, objectFactoryAttributes);
         CheckObjectFactorySerializationFrameworksConflicts(context, type, objectFactoryAttributes);
      }

      for (var i = 0; i < objectFactoryAttributes.Length; i++)
      {
         ValidateObjectFactory(context, type, objectFactoryAttributes[i], isSmartEnum);
      }
   }

   private static void ValidateKeyedValueObject(
      SymbolAnalysisContext context,
      IReadOnlyList<InstanceMemberInfo>? assignableMembers,
      INamedTypeSymbol type,
      AttributeData keyedValueObjectAttribute,
      Location tdsLocation,
      TypedMemberStateFactory factory)
   {
      var keyType = keyedValueObjectAttribute.AttributeClass?.TypeArguments.FirstOrDefault();

      if (keyType is null)
         return;

      if (keyType.TypeKind == TypeKind.Error)
         return;

      // The resolution of TypeParamRef markers to the actual type parameter is required before running the
      // follow-up checks, mirroring the source generator (ValueObjectSourceGenerator). Otherwise the unresolved
      // marker class (a reference type) causes false positives such as TTRESG057 and TTRESG045 for generic keyed types.
      if (ReportIfTypeParamRefMissingNotnullConstraint(context, keyType, type, tdsLocation, out var resolvedKeyType))
         return;

      keyType = resolvedKeyType;

      if (keyType.NullableAnnotation == NullableAnnotation.Annotated || keyType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.KeyMemberShouldNotBeNullable,
            keyedValueObjectAttribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? tdsLocation);
         return;
      }

      // TTRESG109: the setting is ignored for structs by the source generator (KeyedValueObjectCodeGenerator),
      // together with the implied 'NullInFactoryMethodsYieldsNull'.
      if (type.IsValueType && keyedValueObjectAttribute.FindEmptyStringInFactoryMethodsYieldsNull() == true)
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.EmptyStringInFactoryMethodsYieldsNullHasNoEffectOnStructs,
            GetNamedArgumentLocationOrFallback(keyedValueObjectAttribute, Constants.Attributes.Properties.EMPTY_STRING_IN_FACTORY_METHODS_YIELDS_NULL, tdsLocation, context.CancellationToken),
            BuildTypeName(type));
      }

      if (keyedValueObjectAttribute.FindSkipKeyMember() == true)
         ValidateValueObjectCustomKeyMemberImplementation(context, keyType, assignableMembers, keyedValueObjectAttribute, tdsLocation);

      ValidateKeyMemberComparers(context, type, keyType, keyedValueObjectAttribute, factory, tdsLocation, true);

      var allowDefaultStructs = keyedValueObjectAttribute.FindAllowDefaultStructs();

      if (allowDefaultStructs)
      {
         if (type.IsValueType && keyType.IsReferenceType)
         {
            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfValueObjectIsStructButKeyTypeIsClass,
               keyedValueObjectAttribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? tdsLocation,
               type,
               keyType);
         }
         else if (keyType.ImplementsIDisallowDefaultValue())
         {
            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfSomeMembersDisallowDefaultValues,
               keyedValueObjectAttribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? tdsLocation,
               type,
               keyType.Name);
         }
      }

      if (keyedValueObjectAttribute.FindSkipEqualityComparison() != true)
         CheckForComparisonMismatch(context, type, keyedValueObjectAttribute, tdsLocation);

      ValidateFactoryArgumentsAdditionalParameters(context, type, 2, tdsLocation);
   }

   private static void ValidateFactoryArgumentsAdditionalParameters(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      int prefixParameterCount,
      Location tdsLocation)
   {
      var members = type.GetMembers();

      for (var i = 0; i < members.Length; i++)
      {
         if (!members[i].IsValidateFactoryArgumentsImplementation(out var method))
            continue;

         // Inspect the user's implementing declaration: on the merged partial-method symbol the parameter
         // defaults come from the generated defining declaration, which would cause false positives.
         var implementation = method.PartialImplementationPart ?? method;
         var parameters = implementation.Parameters;

         // The prefix parameters (ref validationError, ref value(s)) are legitimately 'ref' - only check the additional ones.
         for (var j = prefixParameterCount; j < parameters.Length; j++)
         {
            var parameter = parameters[j];

            if (parameter is { RefKind: RefKind.None, HasExplicitDefaultValue: false })
               continue;

            var location = parameter.Locations.IsDefaultOrEmpty ? tdsLocation : parameter.Locations[0];

            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.ValidateFactoryArgumentsAdditionalParameterMustNotHaveDefaultOrBeByRef,
               location,
               parameter.Name,
               type.ToFullyQualifiedDisplayString());
         }
      }
   }

   private static void CheckForComparisonMismatch(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      AttributeData attribute,
      Location tdsLocation)
   {
      var comparison = attribute.FindComparisonOperators();
      var equality = attribute.FindEqualityComparisonOperators();

      if (comparison != equality)
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.ComparisonAndEqualityOperatorsMismatch,
            attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? tdsLocation,
            type.Name,
            comparison.ToString(),
            equality.ToString());
      }
   }

   private static void ValidateKeyMemberComparers(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      ITypeSymbol keyType,
      AttributeData keyedObjectAttribute,
      TypedMemberStateFactory factory,
      Location tdsLocation,
      bool stringBasedRequiresEqualityComparer)
   {
      AttributeData? keyMemberComparerAttr = null;
      AttributeData? keyMemberEqualityComparerAttr = null;

      foreach (var attribute in type.GetAttributes())
      {
         if (attribute.AttributeClass.IsKeyMemberComparerAttribute())
         {
            keyMemberComparerAttr = attribute;
         }
         else if (attribute.AttributeClass.IsKeyMemberEqualityComparerAttribute())
         {
            keyMemberEqualityComparerAttr = attribute;
         }
      }

      ValidateComparer(context, keyType, keyMemberComparerAttr, tdsLocation);
      ValidateComparer(context, keyType, keyMemberEqualityComparerAttr, tdsLocation);

      var skipEqualityComparison = keyedObjectAttribute.FindSkipEqualityComparison() ?? false;

      if (keyMemberComparerAttr is not null && keyMemberEqualityComparerAttr is null)
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ExplicitComparerWithoutEqualityComparer,
                          tdsLocation,
                          BuildTypeName(type));
      }
      else if (stringBasedRequiresEqualityComparer
               && keyType.SpecialType == SpecialType.System_String
               && keyMemberEqualityComparerAttr is null && !skipEqualityComparison)
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.StringBasedValueObjectNeedsEqualityComparer,
                          tdsLocation);
      }

      if (keyMemberEqualityComparerAttr is not null
          && keyMemberComparerAttr is null
          && keyedObjectAttribute.FindSkipIComparable() != true
          && factory.Create(keyType).IsComparable)
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ExplicitEqualityComparerWithoutComparer,
                          tdsLocation,
                          BuildTypeName(type));
      }
   }

   private static void ValidateComparer(
      SymbolAnalysisContext context,
      ITypeSymbol keyType,
      AttributeData? keyMemberComparerAttr,
      Location tdsLocation)
   {
      var comparerGenericTypes = keyMemberComparerAttr?.GetComparerTypes();

      if (comparerGenericTypes is null || SymbolEqualityComparer.Default.Equals(comparerGenericTypes.Value.ItemType, keyType))
         return;

      ReportDiagnostic(
         context,
         DiagnosticsDescriptors.ComparerTypeMustMatchMemberType,
         keyMemberComparerAttr?.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? tdsLocation,
         BuildTypeName(comparerGenericTypes.Value.ComparerType),
         BuildTypeName(keyType));
   }

   private static void ValidateValueObjectCustomKeyMemberImplementation(
      SymbolAnalysisContext context,
      ITypeSymbol keyType,
      IReadOnlyList<InstanceMemberInfo>? assignableMembers,
      AttributeData attribute,
      Location tdsLocation)
   {
      var keyMemberAccessModifier = attribute.FindKeyMemberAccessModifier() ?? Constants.ValueObject.DEFAULT_KEY_MEMBER_ACCESS_MODIFIER;
      var keyMemberKind = attribute.FindKeyMemberKind() ?? Constants.ValueObject.DEFAULT_KEY_MEMBER_KIND;
      var keyMemberName = attribute.FindKeyMemberName() ?? keyMemberAccessModifier.GetDefaultValueObjectKeyMemberName(keyMemberKind);

      ValidateCustomKeyMemberImplementation(context, keyType, assignableMembers, keyMemberName, tdsLocation);
   }

   private static void ValidateCustomKeyMemberImplementation(
      SymbolAnalysisContext context,
      ITypeSymbol keyType,
      IReadOnlyList<InstanceMemberInfo>? assignableMembers,
      string keyMemberName,
      Location tdsLocation)
   {
      var keyMember = assignableMembers?.FirstOrDefault(m => !m.IsStatic && m.Name == keyMemberName);

      if (keyMember is null)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.CustomKeyMemberImplementationNotFound, tdsLocation, keyMemberName);
         return;
      }

      if (!keyMember.IsOfType(keyType))
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.CustomKeyMemberImplementationTypeMismatch,
            keyMember.GetIdentifierLocation(context.CancellationToken) ?? tdsLocation,
            keyMemberName,
            keyMember.TypeMinimallyQualified,
            BuildTypeName(keyType));
      }
   }

   private static List<InstanceMemberInfo>? ValidateSharedValueObject(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      Location tdsLocation,
      TypedMemberStateFactory factory)
   {
      if (type.IsRecord || type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, tdsLocation, type);
         return null;
      }

      CheckConstructors(context, type, mustBePrivate: false, canHavePrimaryConstructor: false);
      TypeMustBePartial(context, type);

      var assignableMembers = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, false, true, context.CancellationToken, context)
                                  .Where(m => !m.IsStatic)
                                  .ToList();

      var baseClass = type.BaseType;

      while (!baseClass.IsNullOrDotnetBaseType())
      {
         baseClass.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, tdsLocation, context).Enumerate();

         baseClass = baseClass.BaseType;
      }

      return assignableMembers;
   }

   private static void CheckAssignableMembers(
      SymbolAnalysisContext context,
      List<InstanceMemberInfo> assignableMembers,
      INamedTypeSymbol type,
      AttributeData attribute,
      Location tdsLocation)
   {
      var allowDefaultStructs = attribute.FindAllowDefaultStructs();
      var skipEqualityComparison = attribute.FindSkipEqualityComparison() ?? false;

      List<string>? membersWithDisallowDefaultValue = null;
      var hasStringMembersWithoutComparer = false;

      for (var i = 0; i < assignableMembers.Count; i++)
      {
         var assignableMember = assignableMembers[i];

         if (allowDefaultStructs && assignableMember.DisallowsDefaultValue)
         {
            membersWithDisallowDefaultValue ??= [];
            membersWithDisallowDefaultValue.Add(assignableMember.Name);
         }

         var isString = assignableMember.SpecialType == SpecialType.System_String;

         if (!assignableMember.ValueObjectMemberSettings.IsExplicitlyDeclared)
         {
            hasStringMembersWithoutComparer |= isString;
            continue;
         }

         hasStringMembersWithoutComparer |= isString && assignableMember.ValueObjectMemberSettings.EqualityComparerAccessor is null;

         CheckComparerTypes(context, assignableMember, tdsLocation);
      }

      if (hasStringMembersWithoutComparer && !attribute.HasDefaultStringComparison() && !skipEqualityComparison)
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ComplexValueObjectWithStringMembersNeedsDefaultEqualityComparer,
                          tdsLocation);
      }

      if (membersWithDisallowDefaultValue is not null)
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfSomeMembersDisallowDefaultValues,
                          tdsLocation,
                          type,
                          String.Join(", ", membersWithDisallowDefaultValue)
         );
      }
   }

   private static void CheckComparerTypes(SymbolAnalysisContext context, InstanceMemberInfo member, Location tdsLocation)
   {
      if (member.ValueObjectMemberSettings is { HasInvalidEqualityComparerType: true, EqualityComparerAccessor: not null })
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ComparerTypeMustMatchMemberType,
                          member.ValueObjectMemberSettings.GetEqualityComparerAttributeLocationOrNull(context.CancellationToken) ?? member.GetIdentifierLocation(context.CancellationToken) ?? tdsLocation,
                          member.ValueObjectMemberSettings.EqualityComparerAccessor,
                          member.TypeMinimallyQualified);
      }
   }

   private static void ValidateObjectFactory(
      in SymbolAnalysisContext context,
      INamedTypeSymbol objectType,
      AttributeData attribute,
      bool isSmartEnum)
   {
      if (attribute.AttributeClass is null)
         return;

      var valueType = attribute.AttributeClass.TypeArguments[0];

      // A ref struct cannot be used as the generic value type argument required by the Entity Framework Core
      // value converters, the ASP.NET Core model binders, MessagePack and Newtonsoft.Json. Only
      // ReadOnlySpan<char> with System.Text.Json is supported (zero-allocation deserialization).
      if (valueType.IsRefLikeType)
      {
         var fallbackLocation = objectType.GetTypeIdentifierLocation(context.CancellationToken);

         // TTRESG078: both flags would cause a runtime failure, so forbid them on a ref-struct object factory.
         if (attribute.FindUseWithEntityFramework())
         {
            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.RefStructObjectFactoryMustNotBeUsedWithEntityFrameworkOrModelBinding,
               GetNamedArgumentLocationOrFallback(attribute, Constants.Attributes.Properties.USE_WITH_ENTITY_FRAMEWORK, fallbackLocation, context.CancellationToken),
               objectType,
               Constants.Attributes.Properties.USE_WITH_ENTITY_FRAMEWORK);
         }

         if (attribute.FindUseForModelBinding())
         {
            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.RefStructObjectFactoryMustNotBeUsedWithEntityFrameworkOrModelBinding,
               GetNamedArgumentLocationOrFallback(attribute, Constants.Attributes.Properties.USE_FOR_MODEL_BINDING, fallbackLocation, context.CancellationToken),
               objectType,
               Constants.Attributes.Properties.USE_FOR_MODEL_BINDING);
         }

         // TTRESG108: serialization frameworks that silently ignore the factory at runtime.
         var useForSerialization = attribute.FindUseForSerialization();
         var ignoredFrameworks = useForSerialization & (SerializationFrameworks.MessagePack | SerializationFrameworks.NewtonsoftJson);

         if (!valueType.IsReadOnlySpanOfChar())
            ignoredFrameworks |= useForSerialization & SerializationFrameworks.SystemTextJson;

         if (ignoredFrameworks != SerializationFrameworks.None)
         {
            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.RefStructObjectFactoryIgnoredBySerializationFrameworks,
               GetNamedArgumentLocationOrFallback(attribute, Constants.Attributes.Properties.USE_FOR_SERIALIZATION, fallbackLocation, context.CancellationToken),
               BuildTypeName(objectType),
               BuildTypeName(valueType),
               ignoredFrameworks.ToString());
         }
      }

      if (attribute.FindHasCorrespondingConstructor())
      {
         if (isSmartEnum)
         {
            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.SmartEnumMustNotHaveObjectFactoryConstructor,
               objectType.GetTypeIdentifierLocation(context.CancellationToken),
               objectType,
               valueType);
         }
         else
         {
            CheckForConstructorWithArgument(context, objectType, valueType);
         }
      }

      var validationErrorType = objectType.FindAttribute(static attr => attr.IsValidationErrorAttribute())?.AttributeClass?.TypeArguments[0];

      // TTRESG061: Check for static Validate method (always required)
      if (!objectType.HasValidateMethod(valueType, validationErrorType))
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.ObjectFactoryMustImplementStaticValidateMethod,
            objectType.GetTypeIdentifierLocation(context.CancellationToken),
            BuildTypeName(objectType),
            BuildTypeName(objectType.WithNullableAnnotation(NullableAnnotation.Annotated)),
            BuildTypeName(valueType),
            BuildTypeName(valueType.WithNullableAnnotation(NullableAnnotation.Annotated)),
            validationErrorType is null ? Constants.ValidationError.NAME : BuildTypeName(validationErrorType));
      }

      // TTRESG062: Check for ToValue method (conditionally required)
      if (attribute.NeedsToValueMethod() && !objectType.HasToValueMethod(valueType))
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.ObjectFactoryMustImplementToValueMethod,
            objectType.GetTypeIdentifierLocation(context.CancellationToken),
            objectType,
            valueType);
      }
   }

   /// <summary>
   /// Returns the location of the named argument <paramref name="propertyName"/> of the provided attribute,
   /// so the diagnostic points at the offending setting instead of the whole attribute.
   /// </summary>
   private static Location GetNamedArgumentLocationOrFallback(
      AttributeData attribute,
      string propertyName,
      Location fallback,
      CancellationToken cancellationToken)
   {
      var attributeSyntax = attribute.ApplicationSyntaxReference?.GetSyntax(cancellationToken) as AttributeSyntax;

      return attributeSyntax?.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.Identifier.Text == propertyName)?.GetLocation()
             ?? attributeSyntax?.GetLocation()
             ?? fallback;
   }

   private static void CheckObjectFactoryUseWithEntityFrameworkConflicts(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      ImmutableArray<AttributeData> objectFactoryAttributes)
   {
      var countWithEntityFramework = 0;

      for (var i = 0; i < objectFactoryAttributes.Length; i++)
      {
         // Ref-struct factories cannot carry this flag at all (TTRESG078); counting them here would
         // double-report every conflict they are involved in.
         if (objectFactoryAttributes[i].AttributeClass?.TypeArguments[0] is not { IsRefLikeType: false })
            continue;

         if (!objectFactoryAttributes[i].FindUseWithEntityFramework())
            continue;

         countWithEntityFramework++;

         if (countWithEntityFramework <= 1)
            continue;

         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.MultipleObjectFactoryAttributesWithUseWithEntityFramework,
            type.GetTypeIdentifierLocation(context.CancellationToken),
            type);

         return;
      }
   }

   private static void CheckObjectFactoryUseForModelBindingConflicts(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      ImmutableArray<AttributeData> objectFactoryAttributes)
   {
      var countWithModelBinding = 0;

      for (var i = 0; i < objectFactoryAttributes.Length; i++)
      {
         // Ref-struct factories cannot carry this flag at all (TTRESG078); counting them here would
         // double-report every conflict they are involved in.
         if (objectFactoryAttributes[i].AttributeClass?.TypeArguments[0] is not { IsRefLikeType: false })
            continue;

         if (!objectFactoryAttributes[i].FindUseForModelBinding())
            continue;

         countWithModelBinding++;

         if (countWithModelBinding <= 1)
            continue;

         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.MultipleObjectFactoryAttributesWithUseForModelBinding,
            type.GetTypeIdentifierLocation(context.CancellationToken),
            type);

         return;
      }
   }

   private static void CheckObjectFactorySerializationFrameworksConflicts(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      ImmutableArray<AttributeData> objectFactoryAttributes)
   {
      var combinedFrameworks = SerializationFrameworks.None;

      for (var i = 0; i < objectFactoryAttributes.Length; i++)
      {
         var frameworks = objectFactoryAttributes[i].FindUseForSerialization();
         var overlap = combinedFrameworks & frameworks;

         if (overlap != SerializationFrameworks.None)
         {
            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.MultipleObjectFactoryAttributesWithOverlappingSerializationFrameworks,
               type.GetTypeIdentifierLocation(context.CancellationToken),
               BuildTypeName(type),
               overlap.ToString());

            return;
         }

         combinedFrameworks |= frameworks;
      }
   }

   private static void ValidateSmartEnum(
      SymbolAnalysisContext context,
      INamedTypeSymbol enumType,
      AttributeData attribute)
   {
      if (enumType.IsRecord || enumType.TypeKind is not (TypeKind.Class or TypeKind.Struct))
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.TypeMustBeClassOrStruct,
            enumType.GetTypeIdentifierLocation(context.CancellationToken),
            enumType);
         return;
      }

      var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.Compilation);

      if (factory is null)
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
            enumType.GetTypeIdentifierLocation(context.CancellationToken),
            enumType.ToFullyQualifiedDisplayString(),
            "Could not fetch type information for analysis of the smart enum.");
         return;
      }

      var tdsLocation = enumType.GetTypeIdentifierLocation(context.CancellationToken);

      CheckConstructors(context, enumType, mustBePrivate: true, canHavePrimaryConstructor: false);
      TypeMustBePartial(context, enumType);

      var items = enumType.GetEnumItems();

      if (items.IsDefaultOrEmpty)
         ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumHasNoItems, tdsLocation, enumType);

      Check_ItemLike_StaticProperties(context, enumType);
      EnumItemsMustBePublic(context, enumType, items);

      enumType.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, reportDiagnostic: context).Enumerate();

      var baseClass = enumType.BaseType;

      while (!baseClass.IsNullOrDotnetBaseType())
      {
         baseClass.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, tdsLocation, context).Enumerate();
         baseClass = baseClass.BaseType;
      }

      ValidateEnumDerivedTypes(context, enumType);
      EnumKeyMemberNameMustNotBeItems(context, attribute, tdsLocation);
      ValidateKeyedSmartEnum(context, enumType, attribute, tdsLocation, factory);
      CheckForComparisonMismatch(context, enumType, attribute, tdsLocation);
   }

   private static void ValidateKeyedSmartEnum(
      SymbolAnalysisContext context,
      INamedTypeSymbol enumType,
      AttributeData smartEnumAttribute,
      Location tdsLocation,
      TypedMemberStateFactory factory)
   {
      var keyType = smartEnumAttribute.AttributeClass?.TypeArguments.FirstOrDefault();

      if (keyType is null)
         return;

      if (keyType.TypeKind == TypeKind.Error)
         return;

      // The resolution of TypeParamRef markers to the actual type parameter is required before running the
      // follow-up checks, mirroring the source generator (SmartEnumSourceGenerator). Otherwise the unresolved
      // marker class is used as the key type in the comparer checks below.
      if (ReportIfTypeParamRefMissingNotnullConstraint(context, keyType, enumType, tdsLocation, out var resolvedKeyType))
         return;

      keyType = resolvedKeyType;

      if (keyType.NullableAnnotation == NullableAnnotation.Annotated || keyType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumKeyShouldNotBeNullable, tdsLocation);
         return;
      }

      ValidateKeyMemberComparers(context, enumType, keyType, smartEnumAttribute, factory, tdsLocation, false);
   }

   private static void Check_ItemLike_StaticProperties(
      SymbolAnalysisContext context,
      INamedTypeSymbol enumType)
   {
      var members = enumType.GetMembers();

      if (members.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < members.Length; i++)
      {
         var member = members[i];

         if (member.IsStatic && member is IPropertySymbol property && SymbolEqualityComparer.Default.Equals(property.Type, enumType) && !property.IsIgnored())
         {
            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems,
               property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, context.CancellationToken),
               property.Name);
         }
      }
   }

   private static void ValidateEnumDerivedTypes(
      SymbolAnalysisContext context,
      INamedTypeSymbol type)
   {
      var derivedTypes = type.FindDerivedInnerTypes();
      var typesToLeaveOpen = ImmutableArray.Create<INamedTypeSymbol>();

      for (var i = 0; i < derivedTypes.Count; i++)
      {
         var (derivedType, _, level) = derivedTypes[i];

         if (level == 1)
         {
            if (derivedType.DeclaredAccessibility != Accessibility.Private)
               ReportDiagnostic(context, DiagnosticsDescriptors.InnerSmartEnumOnFirstLevelMustBePrivate, derivedType.GetTypeIdentifierLocation(context.CancellationToken), derivedType);
         }
         else if (derivedType.DeclaredAccessibility != Accessibility.Public)
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.InnerSmartEnumOnNonFirstLevelMustBePublic, derivedType.GetTypeIdentifierLocation(context.CancellationToken), derivedType);
         }

         if (!derivedType.BaseType.IsNullOrDotnetBaseType())
            typesToLeaveOpen = typesToLeaveOpen.Add(derivedType.BaseType);
      }

      for (var i = 0; i < derivedTypes.Count; i++)
      {
         var derivedType = derivedTypes[i];

         if (derivedType.Type is { IsSealed: false, IsAbstract: false } && !typesToLeaveOpen.Contains(derivedType.Type, SymbolEqualityComparer.Default))
            ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumWithoutDerivedTypesMustBeSealed, derivedType.Type.GetTypeIdentifierLocation(context.CancellationToken), derivedType.Type);
      }
   }

   private static void ValidateUnionDerivedTypes(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      var derivedTypes = type.FindDerivedInnerTypes();

      for (var i = 0; i < derivedTypes.Count; i++)
      {
         var (derivedType, _, _) = derivedTypes[i];

         if (derivedType.Arity != 0)
            ReportDiagnostic(context, DiagnosticsDescriptors.UnionDerivedTypesMustNotBeGeneric, derivedType.GetTypeIdentifierLocation(context.CancellationToken), derivedType);

         if (!derivedType.IsAbstract && derivedType.HasLowerAccessibility(type.DeclaredAccessibility, type))
            ReportDiagnostic(context, DiagnosticsDescriptors.NonAbstractDerivedUnionIsLessAccessibleThanBaseUnion, derivedType.GetTypeIdentifierLocation(context.CancellationToken), derivedType, type);

         if (!derivedType.IsSealed)
         {
            if (derivedType.IsRecord)
            {
               ReportDiagnostic(context, DiagnosticsDescriptors.UnionRecordMustBeSealed, derivedType.GetTypeIdentifierLocation(context.CancellationToken), derivedType);
            }
            else if (derivedType.Constructors.Any(ctor => ctor.DeclaredAccessibility != Accessibility.Private))
            {
               ReportDiagnostic(context, DiagnosticsDescriptors.UnionMustBeSealedOrHavePrivateConstructorsOnly, derivedType.GetTypeIdentifierLocation(context.CancellationToken), derivedType);
            }
         }
      }
   }

   private static void CheckForNonDerivedUnionTypes(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      var allInnerTypes = type.GetTypeMembers();
      var baseTypeTuple = (type, type.GetGenericTypeDefinition());

      for (var i = 0; i < allInnerTypes.Length; i++)
      {
         var innerType = allInnerTypes[i];

         // Skip non-class types (enums, delegates, etc.)
         if (innerType.TypeKind != TypeKind.Class)
            continue;

         if (!innerType.IsDerivedFrom(baseTypeTuple))
         {
            ReportDiagnostic(
               context,
               DiagnosticsDescriptors.InnerTypeDoesNotDeriveFromUnion,
               innerType.GetTypeIdentifierLocation(context.CancellationToken),
               innerType,
               type);
         }
      }
   }

   private static void EnumKeyMemberNameMustNotBeItems(
      SymbolAnalysisContext context,
      AttributeData smartEnumAttribute,
      Location tdsLocation)
   {
      var keyMemberName = smartEnumAttribute.FindKeyMemberName();

      if (!StringComparer.OrdinalIgnoreCase.Equals(keyMemberName, "Items"))
         return;

      var attributeSyntax = (AttributeSyntax?)smartEnumAttribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken);

      ReportDiagnostic(
         context,
         DiagnosticsDescriptors.SmartEnumKeyMemberNameNotAllowed,
         attributeSyntax?.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.Identifier.Text == Constants.Attributes.Properties.KEY_MEMBER_NAME)?.GetLocation()
         ?? attributeSyntax?.GetLocation()
         ?? tdsLocation,
         keyMemberName);
   }

   private static void EnumItemsMustBePublic(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      ImmutableArray<IFieldSymbol> items)
   {
      if (items.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < items.Length; i++)
      {
         var item = items[i];

         if (item.DeclaredAccessibility == Accessibility.Public)
            continue;

         ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumItemMustBePublic,
                          item.GetFieldLocation(context.CancellationToken),
                          item.Name, BuildTypeName(type));
      }
   }

   private static void TypeMustBePartial(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      var references = type.DeclaringSyntaxReferences;

      if (references.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < references.Length; i++)
      {
         var syntaxRef = references[i];

         if (syntaxRef.GetSyntax(context.CancellationToken) is not TypeDeclarationSyntax tds)
            continue;

         if (!tds.IsPartial())
            ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBePartial, tds.Identifier.GetLocation(), type);
      }
   }

   private static void CheckConstructors(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      bool mustBePrivate,
      bool canHavePrimaryConstructor)
   {
      if (type.Constructors.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < type.Constructors.Length; i++)
      {
         var ctor = type.Constructors[i];

         if (ctor.IsImplicitlyDeclared || ctor.DeclaringSyntaxReferences.IsDefaultOrEmpty)
            continue;

         for (var j = 0; j < ctor.DeclaringSyntaxReferences.Length; j++)
         {
            var declarationSyntax = ctor.DeclaringSyntaxReferences[j].GetSyntax(context.CancellationToken);

            switch (declarationSyntax)
            {
               // regular ctor
               case ConstructorDeclarationSyntax constructorDeclarationSyntax:
               {
                  if (!mustBePrivate || ctor.DeclaredAccessibility == Accessibility.Private)
                     continue;

                  var location = constructorDeclarationSyntax.Identifier.GetLocation();
                  ReportDiagnostic(context, DiagnosticsDescriptors.ConstructorsMustBePrivate, location, type);
                  return;
               }

               // primary ctor
               case ClassDeclarationSyntax classDeclarationSyntax:
               {
                  if (canHavePrimaryConstructor)
                     continue;

                  var location = classDeclarationSyntax.Identifier.GetLocation();
                  ReportDiagnostic(context, DiagnosticsDescriptors.PrimaryConstructorNotAllowed, location, type);
                  break;
               }

               // primary ctor
               case StructDeclarationSyntax structDeclarationSyntax:
               {
                  if (canHavePrimaryConstructor)
                     continue;

                  var location = structDeclarationSyntax.Identifier.GetLocation();
                  ReportDiagnostic(context, DiagnosticsDescriptors.PrimaryConstructorNotAllowed, location, type);
                  break;
               }

               // primary ctor
               case RecordDeclarationSyntax recordDeclarationSyntax:
               {
                  if (canHavePrimaryConstructor)
                     continue;

                  var location = recordDeclarationSyntax.Identifier.GetLocation();
                  ReportDiagnostic(context, DiagnosticsDescriptors.PrimaryConstructorNotAllowed, location, type);
                  break;
               }
            }
         }
      }
   }

   private static void CheckForConstructorWithArgument(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      ITypeSymbol argumentType)
   {
      if (type.Constructors.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < type.Constructors.Length; i++)
      {
         var ctor = type.Constructors[i];

         if (ctor.Parameters.IsDefaultOrEmpty || ctor.Parameters.Length != 1)
            continue;

         if (SymbolEqualityComparer.Default.Equals(ctor.Parameters[0].Type, argumentType))
            return;
      }

      var location = type.GetTypeIdentifierLocation(context.CancellationToken);

      ReportDiagnostic(context, DiagnosticsDescriptors.ObjectFactoryMustHaveCorrespondingConstructor, location, type, argumentType);
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0));
   }

   private static void ReportDiagnostic(in SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0));
   }

   private static void ReportDiagnostic(in SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0, ITypeSymbol arg1)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0), BuildTypeName(arg1));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location));
   }

   /// <summary>
   /// Resolves TypeParamRef markers in <paramref name="keyType"/> to the type parameters of <paramref name="type"/>
   /// and reports TTRESG074 if the resolved type parameter may be null. The resolved key type is provided via
   /// <paramref name="resolvedKeyType"/>, so callers do not have to resolve a second time.
   /// </summary>
   private static bool ReportIfTypeParamRefMissingNotnullConstraint(
      in SymbolAnalysisContext context,
      ITypeSymbol keyType,
      INamedTypeSymbol type,
      Location tdsLocation,
      out ITypeSymbol resolvedKeyType)
   {
      var maxTypeParamRefIndex = keyType.GetMaxTypeParamRefIndex();

      if (maxTypeParamRefIndex <= 0 || type.Arity == 0 || maxTypeParamRefIndex > type.Arity)
      {
         resolvedKeyType = keyType;
         return false;
      }

      var (resolved, _) = keyType.ResolveTypeParamRefs(type.TypeParameters, context.Compilation);
      resolvedKeyType = resolved;

      if (resolved is ITypeParameterSymbol { HasNotNullConstraint: false, HasReferenceTypeConstraint: false, HasValueTypeConstraint: false } resolvedTypeParam
          && !HasNonNullableTypeConstraint(resolvedTypeParam))
      {
         var properties = ImmutableDictionary.Create<string, string?>().Add("TypeParamName", resolvedTypeParam.Name);
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeParamRefRequiresNotnullConstraint, tdsLocation, properties, resolvedTypeParam.Name, type.ToMinimallyQualifiedDisplayString());
         return true;
      }

      return false;
   }

   private static bool HasNonNullableTypeConstraint(ITypeParameterSymbol typeParam)
   {
      foreach (var constraintType in typeParam.ConstraintTypes)
      {
         if (constraintType.NullableAnnotation == NullableAnnotation.NotAnnotated)
            return true;
      }

      return false;
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0));
   }

   private static void ReportDiagnostic(in SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0));
   }

   private static void ReportDiagnostic(in SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0, string arg1)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0, arg1));
   }

   private static void ReportDiagnostic(in SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ImmutableDictionary<string, string?> properties, string arg0, string arg1)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties, arg0, arg1));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0, string arg1, string arg2)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0, arg1, arg2));
   }

   private static void ReportDiagnostic(in SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0, string arg1, string arg2, string arg3, string arg4)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0, arg1, arg2, arg3, arg4));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0, string arg1)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0), arg1);
   }

   private static string BuildTypeName(ITypeSymbol type)
   {
      return type.ToMinimallyQualifiedDisplayString();
   }
}
