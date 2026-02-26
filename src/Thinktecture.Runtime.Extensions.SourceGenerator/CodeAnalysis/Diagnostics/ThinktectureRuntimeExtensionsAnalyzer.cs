using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ThinktectureRuntimeExtensionsAnalyzer : DiagnosticAnalyzer
{
   private const string _SWITCH_PARTIALLY = "SwitchPartially";
   private const string _MAP_PARTIALLY = "MapPartially";

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
      DiagnosticsDescriptors.AdHocUnionsMustNotBeGeneric,
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
      DiagnosticsDescriptors.TypeMustNotBeInsideGenericType,
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
      DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneSmartEnumAttribute,
      DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneValueObjectAttribute,
      DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneDiscriminatedUnionAttribute,
      DiagnosticsDescriptors.AdHocUnionMustHaveAtLeastTwoMemberTypes,
      DiagnosticsDescriptors.ComparisonAndEqualityOperatorsMismatch,
      DiagnosticsDescriptors.UseSwitchMapWithStaticLambda,
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
            return;
         }

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
               CheckAssignableMembers(context, assignableMembers, type, complexValueObjectAttribute, tdsLocation);
         }

         if (regularUnionAttribute is not null)
         {
            ValidateRegularUnion(context, type);
            ValidateObjectFactories(context, type, objectFactoryAttributes, false);

            needsObjectFactoryHandling = false;
         }

         if (adHocUnionAttribute is not null)
         {
            ValidateAdHocUnion(context, type);
            ValidateObjectFactories(context, type, objectFactoryAttributes, false);

            needsObjectFactoryHandling = false;
         }

         if (needsObjectFactoryHandling)
            ValidateObjectFactories(context, type, objectFactoryAttributes, false);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    Location.None,
                                                    type.ToFullyQualifiedDisplayString(), ex.ToString()));
      }
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

      MemberDisallowingDefaultValuesMustBeRequired(context, propertyDeclarationSyntax, propertySymbol.Type, "property", propertySymbol.Name);
   }

   private static void AnalyzeFieldDisallowingDefaultValues(SyntaxNodeAnalysisContext context)
   {
      if (context.Node is not FieldDeclarationSyntax fieldDeclarationSyntax
          || (fieldDeclarationSyntax.Declaration.Variables.Count == 1 && fieldDeclarationSyntax.Declaration.Variables[0].Initializer is not null) // public MyStruct Member = ...;
          || context.ContainingSymbol is not IFieldSymbol fieldSymbol
          || fieldSymbol.IsReadOnly
          || fieldSymbol.IsStatic
          || fieldSymbol.DeclaredAccessibility < fieldSymbol.ContainingType.DeclaredAccessibility) // required members must not be less visible than the containing type
         return;

      MemberDisallowingDefaultValuesMustBeRequired(context, fieldDeclarationSyntax, fieldSymbol.Type, "field", fieldSymbol.Name);
   }

   private static void MemberDisallowingDefaultValuesMustBeRequired(
      SyntaxNodeAnalysisContext context,
      MemberDeclarationSyntax memberDeclarationSyntax,
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
                                  context.Node.GetLocation(),
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
          || (operation.TargetMethod.Name != "Switch"
              && operation.TargetMethod.Name != _SWITCH_PARTIALLY
              && operation.TargetMethod.Name != "Map"
              && operation.TargetMethod.Name != _MAP_PARTIALLY))
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

      if (operation.TargetMethod.Name is "Switch" or _SWITCH_PARTIALLY)
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
                              + (operation.TargetMethod.Name is _SWITCH_PARTIALLY or _MAP_PARTIALLY ? 1 : 0);

      AnalyzeSwitchMap(context, args, operation, numberOfCallbacks);
   }

   private static void AnalyzeAnyUnionSwitchMap(
      OperationAnalysisContext context,
      ImmutableArray<IArgumentOperation> args,
      IInvocationOperation operation)
   {
      var numberOfCallbacks = operation.TargetMethod.Parameters.IsDefaultOrEmpty ? 0 : operation.TargetMethod.Parameters.Length;

      if (numberOfCallbacks > 0
          && operation.TargetMethod.Parameters[0].Name == "state")
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
      INamedTypeSymbol type)
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
      TypeMustNotBeGeneric(context, type, type.GetTypeIdentifierLocation(context.CancellationToken));
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

      if (keyType.NullableAnnotation == NullableAnnotation.Annotated || keyType.SpecialType == SpecialType.System_Nullable_T)
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.KeyMemberShouldNotBeNullable,
            keyedValueObjectAttribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? tdsLocation);
         return;
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
      TypeMustNotBeInsideGenericType(context, type, tdsLocation);

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

   private static void CheckObjectFactoryUseWithEntityFrameworkConflicts(
      SymbolAnalysisContext context,
      INamedTypeSymbol type,
      ImmutableArray<AttributeData> objectFactoryAttributes)
   {
      var countWithEntityFramework = 0;

      for (var i = 0; i < objectFactoryAttributes.Length; i++)
      {
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
      TypeMustNotBeInsideGenericType(context, enumType, tdsLocation);

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

      if (keyType.NullableAnnotation == NullableAnnotation.Annotated || keyType.SpecialType == SpecialType.System_Nullable_T)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumKeyShouldNotBeNullable, tdsLocation);
         return;
      }

      ValidateKeyMemberComparers(context, enumType, keyType, smartEnumAttribute, factory, tdsLocation, false);
   }

   private static void TypeMustNotBeGeneric(SymbolAnalysisContext context, INamedTypeSymbol type, Location tdsLocation)
   {
      if (!type.TypeParameters.IsDefaultOrEmpty)
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.AdHocUnionsMustNotBeGeneric,
            tdsLocation,
            BuildTypeName(type));
      }
   }

   private static void TypeMustNotBeInsideGenericType(SymbolAnalysisContext context, INamedTypeSymbol type, Location tdsLocation)
   {
      if (type.IsNestedInGenericClass())
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustNotBeInsideGenericType, tdsLocation, BuildTypeName(type));
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
