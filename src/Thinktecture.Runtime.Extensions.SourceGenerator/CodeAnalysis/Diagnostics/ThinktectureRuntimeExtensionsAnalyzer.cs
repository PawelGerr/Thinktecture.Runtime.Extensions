using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ThinktectureRuntimeExtensionsAnalyzer : DiagnosticAnalyzer
{
   private const string _SWITCH_PARTIALLY = "SwitchPartially";
   private const string _MAP_PARTIALLY = "MapPartially";

   private static readonly ILogger _errorLogger = new SelfLogErrorLogger(nameof(ThinktectureRuntimeExtensionsAnalyzer));

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
      DiagnosticsDescriptors.SmartEnumsValueObjectsAndAdHocUnionsMustNotBeGeneric,
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
      DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfValueObjectIsStructButKeyTypeIsClass,
      DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfSomeMembersDisallowDefaultValues,
      DiagnosticsDescriptors.MembersDisallowingDefaultValuesMustBeRequired,
   ];

   /// <inheritdoc />
   public override void Initialize(AnalysisContext context)
   {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterOperationAction(AnalyzeSmartEnum, OperationKind.Attribute);
      context.RegisterOperationAction(AnalyzeValueObject, OperationKind.Attribute);
      context.RegisterOperationAction(AnalyzeAdHocUnion, OperationKind.Attribute);
      context.RegisterOperationAction(AnalyzeUnion, OperationKind.Attribute);
      context.RegisterOperationAction(AnalyzeMethodWithUseDelegateFromConstructor, OperationKind.Attribute);

      context.RegisterOperationAction(AnalyzeMethodCall, OperationKind.Invocation);
      context.RegisterOperationAction(AnalyzeDefaultValueAssignment, OperationKind.DefaultValue);
      context.RegisterOperationAction(AnalyzeObjectCreation, OperationKind.ObjectCreation);

      context.RegisterSyntaxNodeAction(AnalyzeFieldDisallowingDefaultValues, SyntaxKind.FieldDeclaration);
      context.RegisterSyntaxNodeAction(AnalyzePropertyDisallowingDefaultValues, SyntaxKind.PropertyDeclaration);
   }

   private static void AnalyzePropertyDisallowingDefaultValues(SyntaxNodeAnalysisContext context)
   {
      if (context.Node is not PropertyDeclarationSyntax propertyDeclarationSyntax
          || propertyDeclarationSyntax.ExpressionBody is not null // public MyStruct Member => ...;
          || propertyDeclarationSyntax.Initializer is not null    // public MyStruct Member { get; } = ...;
          || context.ContainingSymbol is not IPropertySymbol propertySymbol
          || propertySymbol.SetMethod is null // public MyStruct Member { get; }
          || propertySymbol.IsStatic
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

         var syntaxRef = method.DeclaringSyntaxReferences[0];

         if (syntaxRef.GetSyntax(context.CancellationToken) is not MethodDeclarationSyntax mds)
            return;

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
               operation = operation.Parent;
               break;

            case OperationKind.VariableInitializer:
            case OperationKind.ParameterInitializer: // void MyMethod(MyUnion u = default)
            case OperationKind.SimpleAssignment:
            case OperationKind.CoalesceAssignment:
            case OperationKind.CompoundAssignment:
            case OperationKind.DeconstructionAssignment:
            case OperationKind.Argument:
            case OperationKind.Tuple: // (42, default(MyUnion))
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

      if (declaredType.IsEnum(out var attribute))
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

         if (argument.Syntax is not ArgumentSyntax argSyntax)
            continue;

         if (argSyntax.NameColon is not null)
            continue;

         hasNonNamedParameters = true;
         break;
      }

      if (hasNonNamedParameters)
         ReportDiagnostic(context, DiagnosticsDescriptors.IndexBasedSwitchAndMapMustUseNamedParameters, operation.Syntax.GetLocation(), operation.TargetMethod.ContainingType);
   }

   private static void AnalyzeSmartEnum(OperationAnalysisContext context)
   {
      if (context.ContainingSymbol.Kind != SymbolKind.NamedType
          || context.Operation is not IAttributeOperation { Operation: IObjectCreationOperation attrCreation }
          || !attrCreation.Type.IsSmartEnumAttribute()
          || context.ContainingSymbol is not INamedTypeSymbol type
          || type.TypeKind == TypeKind.Error)
      {
         return;
      }

      try
      {
         if (type.DeclaringSyntaxReferences.IsDefaultOrEmpty)
            return;

         ValidateEnum(context, type, attrCreation);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    Location.None,
                                                    type.ToFullyQualifiedDisplayString(), ex.ToString()));
      }
   }

   private static void AnalyzeValueObject(OperationAnalysisContext context)
   {
      if (context.ContainingSymbol.Kind != SymbolKind.NamedType
          || context.Operation is not IAttributeOperation { Operation: IObjectCreationOperation attrCreation }
          || context.ContainingSymbol is not INamedTypeSymbol type
          || type.TypeKind == TypeKind.Error
          || type.DeclaringSyntaxReferences.IsDefaultOrEmpty)
      {
         return;
      }

      try
      {
         var isKeyed = attrCreation.Type.IsKeyedValueObjectAttribute();
         var isComplex = attrCreation.Type.IsComplexValueObjectAttribute();

         if (!isKeyed && !isComplex)
            return;

         var locationOfFirstDeclaration = type.Locations.IsDefaultOrEmpty ? Location.None : type.Locations[0]; // a representative for all

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.Compilation, _errorLogger);

         if (factory is null)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                       locationOfFirstDeclaration,
                                                       type.ToFullyQualifiedDisplayString(),
                                                       "Could not fetch type information for analysis of the value object."));
            return;
         }

         var assignableMembers = ValidateSharedValueObject(context, type, locationOfFirstDeclaration, factory);

         if (isKeyed)
            ValidateKeyedValueObject(context, assignableMembers, type, attrCreation, locationOfFirstDeclaration, factory);

         if (isComplex && assignableMembers is not null)
            ValidateComplexValueObject(context, assignableMembers, type, attrCreation, locationOfFirstDeclaration);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    Location.None,
                                                    type.ToFullyQualifiedDisplayString(), ex.ToString()));
      }
   }

   private static void AnalyzeAdHocUnion(OperationAnalysisContext context)
   {
      if (context.ContainingSymbol.Kind != SymbolKind.NamedType
          || context.Operation is not IAttributeOperation { Operation: IObjectCreationOperation attrCreation }
          || context.ContainingSymbol is not INamedTypeSymbol type
          || type.TypeKind == TypeKind.Error
          || type.DeclaringSyntaxReferences.IsDefaultOrEmpty)
      {
         return;
      }

      try
      {
         if (!attrCreation.Type.IsAdHocUnionAttribute())
            return;

         var locationOfFirstDeclaration = type.Locations.IsDefaultOrEmpty ? Location.None : type.Locations[0]; // a representative for all

         ValidateAdHocUnion(context, type, locationOfFirstDeclaration);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    Location.None,
                                                    type.ToFullyQualifiedDisplayString(), ex.ToString()));
      }
   }

   private static void AnalyzeUnion(OperationAnalysisContext context)
   {
      if (context.ContainingSymbol.Kind != SymbolKind.NamedType
          || context.Operation is not IAttributeOperation { Operation: IObjectCreationOperation attrCreation }
          || context.ContainingSymbol is not INamedTypeSymbol type
          || type.TypeKind == TypeKind.Error
          || type.DeclaringSyntaxReferences.IsDefaultOrEmpty)
      {
         return;
      }

      try
      {
         if (!attrCreation.Type.IsRegularUnionAttribute())
            return;

         ValidateUnion(context, type);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    Location.None,
                                                    type.ToFullyQualifiedDisplayString(), ex.ToString()));
      }
   }

   private static void ValidateAdHocUnion(
      OperationAnalysisContext context,
      INamedTypeSymbol type,
      Location locationOfFirstDeclaration)
   {
      if (type.IsRecord || type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, type);
         return;
      }

      CheckConstructors(context, type, mustBePrivate: false, canHavePrimaryConstructor: false);
      TypeMustBePartial(context, type);
      TypeMustNotBeGeneric(context, type, locationOfFirstDeclaration);
   }

   private static void ValidateUnion(
      OperationAnalysisContext context,
      INamedTypeSymbol type)
   {
      CheckConstructors(context, type, mustBePrivate: true, canHavePrimaryConstructor: false);
      TypeMustBePartial(context, type);
      ValidateUnionDerivedTypes(context, type);
   }

   private static void ValidateKeyedValueObject(
      OperationAnalysisContext context,
      IReadOnlyList<InstanceMemberInfo>? assignableMembers,
      INamedTypeSymbol type,
      IObjectCreationOperation attribute,
      Location locationOfFirstDeclaration,
      TypedMemberStateFactory factory)
   {
      var keyType = (attribute.Type as INamedTypeSymbol)?.TypeArguments.FirstOrDefault();

      if (keyType is null)
         return;

      if (keyType.TypeKind == TypeKind.Error)
         return;

      if (keyType.NullableAnnotation == NullableAnnotation.Annotated || keyType.SpecialType == SpecialType.System_Nullable_T)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.KeyMemberShouldNotBeNullable, attribute.Syntax.GetLocation());
         return;
      }

      if (attribute.FindSkipKeyMember() == true)
         ValidateValueObjectCustomKeyMemberImplementation(context, keyType, assignableMembers, attribute, locationOfFirstDeclaration);

      ValidateKeyMemberComparers(context, type, keyType, attribute, locationOfFirstDeclaration, factory, true);

      var allowDefaultStructs = attribute.FindAllowDefaultStructs() ?? false;

      if (allowDefaultStructs)
      {
         if (type.IsValueType && keyType.IsReferenceType)
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfValueObjectIsStructButKeyTypeIsClass, attribute.Syntax.GetLocation(), type, keyType);
         }
         else if (keyType.ImplementsIDisallowDefaultValue())
         {
            ReportDiagnostic(context,
                             DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfSomeMembersDisallowDefaultValues,
                             locationOfFirstDeclaration,
                             type,
                             keyType.Name);
         }
      }
   }

   private static void ValidateKeyMemberComparers(
      OperationAnalysisContext context,
      INamedTypeSymbol type,
      ITypeSymbol keyType,
      IObjectCreationOperation attibute,
      Location locationOfFirstDeclaration,
      TypedMemberStateFactory factory,
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

      ValidateComparer(context, keyType, keyMemberComparerAttr);
      ValidateComparer(context, keyType, keyMemberEqualityComparerAttr);

      if (keyMemberComparerAttr is not null && keyMemberEqualityComparerAttr is null)
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ExplicitComparerWithoutEqualityComparer,
                          locationOfFirstDeclaration,
                          BuildTypeName(type));
      }
      else if (stringBasedRequiresEqualityComparer
               && keyType.SpecialType == SpecialType.System_String
               && keyMemberEqualityComparerAttr is null)
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.StringBasedValueObjectNeedsEqualityComparer,
                          locationOfFirstDeclaration);
      }

      if (keyMemberEqualityComparerAttr is not null
          && keyMemberComparerAttr is null
          && attibute.FindSkipIComparable() != true
          && factory.Create(keyType).IsComparable)
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ExplicitEqualityComparerWithoutComparer,
                          locationOfFirstDeclaration,
                          BuildTypeName(type));
      }
   }

   private static void ValidateComparer(OperationAnalysisContext context, ITypeSymbol keyType, AttributeData? keyMemberComparerAttr)
   {
      var comparerGenericTypes = keyMemberComparerAttr?.GetComparerTypes();

      if (comparerGenericTypes is null || SymbolEqualityComparer.Default.Equals(comparerGenericTypes.Value.ItemType, keyType))
         return;

      ReportDiagnostic(context,
                       DiagnosticsDescriptors.ComparerTypeMustMatchMemberType,
                       keyMemberComparerAttr?.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None,
                       BuildTypeName(comparerGenericTypes.Value.ComparerType),
                       BuildTypeName(keyType));
   }

   private static void ValidateValueObjectCustomKeyMemberImplementation(
      OperationAnalysisContext context,
      ITypeSymbol keyType,
      IReadOnlyList<InstanceMemberInfo>? assignableMembers,
      IObjectCreationOperation attribute,
      Location locationOfFirstDeclaration)
   {
      var keyMemberAccessModifier = attribute.FindKeyMemberAccessModifier() ?? Constants.ValueObject.DEFAULT_KEY_MEMBER_ACCESS_MODIFIER;
      var keyMemberKind = attribute.FindKeyMemberKind() ?? Constants.ValueObject.DEFAULT_KEY_MEMBER_KIND;
      var keyMemberName = attribute.FindKeyMemberName() ?? Helper.GetDefaultValueObjectKeyMemberName(keyMemberAccessModifier, keyMemberKind);

      ValidateCustomKeyMemberImplementation(context, keyType, assignableMembers, keyMemberName, locationOfFirstDeclaration);
   }

   private static void ValidateCustomKeyMemberImplementation(
      OperationAnalysisContext context,
      ITypeSymbol keyType,
      IReadOnlyList<InstanceMemberInfo>? assignableMembers,
      string keyMemberName,
      Location locationOfFirstDeclaration)
   {
      var keyMember = assignableMembers?.FirstOrDefault(m => !m.IsStatic && m.Name == keyMemberName);

      if (keyMember is null)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.CustomKeyMemberImplementationNotFound, locationOfFirstDeclaration, keyMemberName);
         return;
      }

      if (!keyMember.IsOfType(keyType))
      {
         ReportDiagnostic(
            context,
            DiagnosticsDescriptors.CustomKeyMemberImplementationTypeMismatch,
            keyMember.GetIdentifierLocation(context.CancellationToken) ?? locationOfFirstDeclaration,
            keyMemberName,
            keyMember.TypeMinimallyQualified,
            BuildTypeName(keyType));
      }
   }

   private static IReadOnlyList<InstanceMemberInfo>? ValidateSharedValueObject(
      OperationAnalysisContext context,
      INamedTypeSymbol type,
      Location locationOfFirstDeclaration,
      TypedMemberStateFactory factory)
   {
      if (type.IsRecord || type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, type);
         return null;
      }

      CheckConstructors(context, type, mustBePrivate: false, canHavePrimaryConstructor: false);
      TypeMustBePartial(context, type);
      TypeMustNotBeGeneric(context, type, locationOfFirstDeclaration);
      TypeMustNotBeInsideGenericType(context, type, locationOfFirstDeclaration);

      var assignableMembers = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, false, true, context.CancellationToken, context)
                                  .Where(m => !m.IsStatic)
                                  .ToList();

      var baseClass = type.BaseType;

      while (!baseClass.IsNullOrObject())
      {
         baseClass.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, locationOfFirstDeclaration, context).Enumerate();

         baseClass = baseClass.BaseType;
      }

      return assignableMembers;
   }

   private static void ValidateComplexValueObject(
      OperationAnalysisContext context,
      IReadOnlyList<InstanceMemberInfo> assignableMembers,
      INamedTypeSymbol type,
      IObjectCreationOperation attribute,
      Location locationOfFirstDeclaration)
   {
      CheckAssignableMembers(context, assignableMembers, type, attribute, locationOfFirstDeclaration);
   }

   private static void CheckAssignableMembers(
      OperationAnalysisContext context,
      IReadOnlyList<InstanceMemberInfo> assignableMembers,
      INamedTypeSymbol type,
      IObjectCreationOperation attribute,
      Location locationOfFirstDeclaration)
   {
      var allowDefaultStructs = attribute.FindAllowDefaultStructs() ?? false;

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

         CheckComparerTypes(context, assignableMember);
      }

      if (hasStringMembersWithoutComparer && !attribute.HasDefaultStringComparison())
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ComplexValueObjectWithStringMembersNeedsDefaultEqualityComparer,
                          locationOfFirstDeclaration);
      }

      if (membersWithDisallowDefaultValue is not null)
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.AllowDefaultStructsCannotBeTrueIfSomeMembersDisallowDefaultValues,
                          locationOfFirstDeclaration,
                          type,
                          String.Join(", ", membersWithDisallowDefaultValue)
         );
      }
   }

   private static void CheckComparerTypes(OperationAnalysisContext context, InstanceMemberInfo member)
   {
      if (member.ValueObjectMemberSettings is { HasInvalidEqualityComparerType: true, EqualityComparerAccessor: not null })
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ComparerTypeMustMatchMemberType,
                          member.ValueObjectMemberSettings.GetEqualityComparerAttributeLocationOrNull(context.CancellationToken) ?? member.GetIdentifierLocation(context.CancellationToken) ?? Location.None,
                          member.ValueObjectMemberSettings.EqualityComparerAccessor,
                          member.TypeMinimallyQualified);
      }
   }

   private static void ValidateEnum(
      OperationAnalysisContext context,
      INamedTypeSymbol enumType,
      IObjectCreationOperation attribute)
   {
      var locationOfFirstDeclaration = enumType.Locations.IsDefaultOrEmpty ? Location.None : enumType.Locations[0]; // a representative for all

      if (enumType.IsRecord || enumType.TypeKind is not (TypeKind.Class or TypeKind.Struct))
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, enumType);
         return;
      }

      var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.Compilation, _errorLogger);

      if (factory is null)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    locationOfFirstDeclaration,
                                                    enumType.ToFullyQualifiedDisplayString(),
                                                    "Could not fetch type information for analysis of the smart enum."));
         return;
      }

      CheckConstructors(context, enumType, mustBePrivate: true, canHavePrimaryConstructor: false);
      TypeMustBePartial(context, enumType);
      TypeMustNotBeGeneric(context, enumType, locationOfFirstDeclaration);
      TypeMustNotBeInsideGenericType(context, enumType, locationOfFirstDeclaration);

      var items = enumType.GetEnumItems();

      if (items.IsDefaultOrEmpty)
         ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumHasNoItems, locationOfFirstDeclaration, enumType);

      Check_ItemLike_StaticProperties(context, enumType);
      EnumItemsMustBePublic(context, enumType, items);

      _ = enumType.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, false, false, context.CancellationToken, context).ToList();

      var baseClass = enumType.BaseType;

      while (!baseClass.IsNullOrObject())
      {
         baseClass.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, locationOfFirstDeclaration, context).Enumerate();

         baseClass = baseClass.BaseType;
      }

      ValidateEnumDerivedTypes(context, enumType);

      EnumKeyMemberNameMustNotBeItem(context, attribute, locationOfFirstDeclaration);

      ValidateKeyedSmartEnum(context, enumType, attribute, locationOfFirstDeclaration, factory);
   }

   private static void ValidateKeyedSmartEnum(
      OperationAnalysisContext context,
      INamedTypeSymbol enumType,
      IObjectCreationOperation attribute,
      Location locationOfFirstDeclaration,
      TypedMemberStateFactory factory)
   {
      var keyType = (attribute.Type as INamedTypeSymbol)?.TypeArguments.FirstOrDefault();

      if (keyType is null)
         return;

      if (keyType.TypeKind == TypeKind.Error)
         return;

      if (keyType.NullableAnnotation == NullableAnnotation.Annotated || keyType.SpecialType == SpecialType.System_Nullable_T)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumKeyShouldNotBeNullable, locationOfFirstDeclaration);
         return;
      }

      ValidateKeyMemberComparers(context, enumType, keyType, attribute, locationOfFirstDeclaration, factory, false);
   }

   private static void TypeMustNotBeGeneric(OperationAnalysisContext context, INamedTypeSymbol type, Location locationOfFirstDeclaration)
   {
      if (!type.TypeParameters.IsDefaultOrEmpty)
         ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumsValueObjectsAndAdHocUnionsMustNotBeGeneric, locationOfFirstDeclaration, BuildTypeName(type));
   }

   private static void TypeMustNotBeInsideGenericType(OperationAnalysisContext context, INamedTypeSymbol type, Location locationOfFirstDeclaration)
   {
      if (type.IsNestedInGenericClass())
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustNotBeInsideGenericType, locationOfFirstDeclaration, BuildTypeName(type));
   }

   private static void Check_ItemLike_StaticProperties(
      OperationAnalysisContext context,
      INamedTypeSymbol enumType)
   {
      var members = enumType.GetMembers();

      if (members.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < members.Length; i++)
      {
         var member = members[i];

         if (member.IsStatic && member is IPropertySymbol property && SymbolEqualityComparer.Default.Equals(property.Type, enumType) && !property.IsIgnored())
            ReportDiagnostic(context, DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems, property.GetIdentifier(context.CancellationToken)?.GetLocation() ?? Location.None, property.Name);
      }
   }

   private static void ValidateEnumDerivedTypes(OperationAnalysisContext context, INamedTypeSymbol type)
   {
      var derivedTypes = type.FindDerivedInnerTypes();
      var typesToLeaveOpen = ImmutableArray.Create<INamedTypeSymbol>();

      for (var i = 0; i < derivedTypes.Count; i++)
      {
         var (derivedType, _, level) = derivedTypes[i];

         if (level == 1)
         {
            if (derivedType.DeclaredAccessibility != Accessibility.Private)
               ReportDiagnostic(context, DiagnosticsDescriptors.InnerSmartEnumOnFirstLevelMustBePrivate, GetDerivedTypeLocation(context, derivedType), derivedType);
         }
         else if (derivedType.DeclaredAccessibility != Accessibility.Public)
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.InnerSmartEnumOnNonFirstLevelMustBePublic, GetDerivedTypeLocation(context, derivedType), derivedType);
         }

         if (!derivedType.BaseType.IsNullOrObject())
            typesToLeaveOpen = typesToLeaveOpen.Add(derivedType.BaseType);
      }

      for (var i = 0; i < derivedTypes.Count; i++)
      {
         var derivedType = derivedTypes[i];

         if (!derivedType.Type.IsSealed && !derivedType.Type.IsAbstract && !typesToLeaveOpen.Contains(derivedType.Type, SymbolEqualityComparer.Default))
            ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumWithoutDerivedTypesMustBeSealed, GetDerivedTypeLocation(context, derivedType.Type), derivedType.Type);
      }
   }

   private static void ValidateUnionDerivedTypes(OperationAnalysisContext context, INamedTypeSymbol type)
   {
      var derivedTypes = type.FindDerivedInnerTypes();

      for (var i = 0; i < derivedTypes.Count; i++)
      {
         var (derivedType, _, _) = derivedTypes[i];

         if (derivedType.Arity != 0)
            ReportDiagnostic(context, DiagnosticsDescriptors.UnionDerivedTypesMustNotBeGeneric, GetDerivedTypeLocation(context, derivedType), derivedType);

         if (!derivedType.IsAbstract && derivedType.HasLowerAccessibility(type.DeclaredAccessibility, type))
            ReportDiagnostic(context, DiagnosticsDescriptors.NonAbstractDerivedUnionIsLessAccessibleThanBaseUnion, GetDerivedTypeLocation(context, derivedType), derivedType, type);

         if (!derivedType.IsSealed)
         {
            if (derivedType.IsRecord)
            {
               ReportDiagnostic(context, DiagnosticsDescriptors.UnionRecordMustBeSealed, GetDerivedTypeLocation(context, derivedType), derivedType);
            }
            else if (derivedType.Constructors.Any(ctor => ctor.DeclaredAccessibility != Accessibility.Private))
            {
               ReportDiagnostic(context, DiagnosticsDescriptors.UnionMustBeSealedOrHavePrivateConstructorsOnly, GetDerivedTypeLocation(context, derivedType), derivedType);
            }
         }
      }
   }

   private static Location GetDerivedTypeLocation(OperationAnalysisContext context, INamedTypeSymbol derivedType)
   {
      if (derivedType.DeclaringSyntaxReferences.IsDefaultOrEmpty)
         return Location.None;

      return ((TypeDeclarationSyntax)derivedType.DeclaringSyntaxReferences[0].GetSyntax(context.CancellationToken)).Identifier.GetLocation();
   }

   private static void EnumKeyMemberNameMustNotBeItem(OperationAnalysisContext context, IObjectCreationOperation enumSettingsAttr, Location location)
   {
      var keyMemberName = enumSettingsAttr.FindKeyMemberName();

      if (!StringComparer.OrdinalIgnoreCase.Equals(keyMemberName, "Item"))
         return;

      var attributeSyntax = (AttributeSyntax?)enumSettingsAttr.Syntax;

      ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumKeyMemberNameNotAllowed,
                       attributeSyntax?.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.Identifier.Text == Constants.Attributes.Properties.KEY_MEMBER_NAME)?.GetLocation() ?? location, keyMemberName);
   }

   private static void EnumItemsMustBePublic(OperationAnalysisContext context, INamedTypeSymbol type, ImmutableArray<IFieldSymbol> items)
   {
      if (items.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < items.Length; i++)
      {
         var item = items[i];

         if (item.DeclaredAccessibility == Accessibility.Public)
            continue;

         ReportDiagnostic(context, DiagnosticsDescriptors.SmartEnumItemMustBePublic,
                          item.GetIdentifier(context.CancellationToken)?.GetLocation() ?? Location.None,
                          item.Name, BuildTypeName(type));
      }
   }

   private static void TypeMustBePartial(OperationAnalysisContext context, INamedTypeSymbol type)
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
      OperationAnalysisContext context,
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

         var declarationSyntax = ctor.DeclaringSyntaxReferences[0].GetSyntax(context.CancellationToken);

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

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0));
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0, ITypeSymbol arg1)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0), BuildTypeName(arg1));
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location));
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0));
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0, string arg1)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0, arg1));
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0, string arg1, string arg2)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0, arg1, arg2));
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0, string arg1)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0), arg1);
   }

   private static string BuildTypeName(ITypeSymbol type)
   {
      return type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
   }
}
