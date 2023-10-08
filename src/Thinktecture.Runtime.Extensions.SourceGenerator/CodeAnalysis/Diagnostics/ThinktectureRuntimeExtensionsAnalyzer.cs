using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ThinktectureRuntimeExtensionsAnalyzer : DiagnosticAnalyzer
{
   /// <inheritdoc />
   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial,
                                                                                                              DiagnosticsDescriptors.StructMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.TypeMustBeClassOrStruct,
                                                                                                              DiagnosticsDescriptors.NonValidatableEnumsMustBeClass,
                                                                                                              DiagnosticsDescriptors.EnumConstructorsMustBePrivate,
                                                                                                              DiagnosticsDescriptors.EnumerationHasNoItems,
                                                                                                              DiagnosticsDescriptors.EnumItemMustBePublic,
                                                                                                              DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.PropertyMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                                                                                                              DiagnosticsDescriptors.InvalidSignatureOfCreateInvalidItem,
                                                                                                              DiagnosticsDescriptors.EnumKeyPropertyNameNotAllowed,
                                                                                                              DiagnosticsDescriptors.InnerEnumOnFirstLevelMustBePrivate,
                                                                                                              DiagnosticsDescriptors.InnerEnumOnNonFirstLevelMustBePublic,
                                                                                                              DiagnosticsDescriptors.TypeCannotBeNestedClass,
                                                                                                              DiagnosticsDescriptors.KeyMemberShouldNotBeNullable,
                                                                                                              DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems,
                                                                                                              DiagnosticsDescriptors.ComparerApplicableOnKeyMemberOnly,
                                                                                                              DiagnosticsDescriptors.EnumsAndValueObjectsMustNotBeGeneric,
                                                                                                              DiagnosticsDescriptors.BaseClassFieldMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.BaseClassPropertyMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.EnumKeyShouldNotBeNullable,
                                                                                                              DiagnosticsDescriptors.EnumWithoutDerivedTypesMustBeSealed,
                                                                                                              DiagnosticsDescriptors.ValueObjectMustBeSealed,
                                                                                                              DiagnosticsDescriptors.SwitchAndMapMustCoverAllItems,
                                                                                                              DiagnosticsDescriptors.ComparerTypeMustMatchMemberType,
                                                                                                              DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                                                                              DiagnosticsDescriptors.InitAccessorMustBePrivate,
                                                                                                              DiagnosticsDescriptors.PrimaryConstructorNotAllowed);

   /// <inheritdoc />
   public override void Initialize(AnalysisContext context)
   {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterSymbolAction(AnalyzeSmartEnum, SymbolKind.NamedType);
      context.RegisterSymbolAction(AnalyzeValueObject, SymbolKind.NamedType);

      context.RegisterOperationAction(AnalyzeMethodCall, OperationKind.Invocation);
   }

   private static void AnalyzeMethodCall(OperationAnalysisContext context)
   {
      var operation = (IInvocationOperation)context.Operation;

      if (operation.Instance is null
          || operation.Arguments.IsDefaultOrEmpty
          || operation.Arguments.Length % 2 != 0
          || operation.TargetMethod.IsStatic
          || (operation.TargetMethod.Name != "Switch" && operation.TargetMethod.Name != "Map"))
      {
         return;
      }

      if (!operation.Instance.Type.IsEnum())
         return;

      var missingItemNames = ImmutableArray.Create<string>();

      foreach (var item in operation.Instance.Type.EnumerateEnumItems())
      {
         var args = operation.Arguments;

         if (args.IsDefaultOrEmpty)
            continue;

         var found = false;

         for (var i = 0; i < args.Length; i += 2)
         {
            var argument = args[i];

            if (argument.Value is not IFieldReferenceOperation fieldReferenceOperation)
               continue;

            if (!SymbolEqualityComparer.Default.Equals(fieldReferenceOperation.Field, item))
               continue;

            found = true;
            break;
         }

         if (!found)
            missingItemNames = missingItemNames.Add(item.Name);
      }

      if (!missingItemNames.IsDefaultOrEmpty)
         ReportDiagnostic(context, DiagnosticsDescriptors.SwitchAndMapMustCoverAllItems, operation.Syntax.GetLocation(), operation.Instance.Type, String.Join(", ", missingItemNames));
   }

   private static void AnalyzeSmartEnum(SymbolAnalysisContext context)
   {
      if (context.Symbol is not INamedTypeSymbol type || type.TypeKind == TypeKind.Error)
         return;

      try
      {
         if (type.DeclaringSyntaxReferences.IsDefaultOrEmpty)
            return;

         if (!type.IsEnum(out var smartEnumAttribute))
            return;

         ValidateEnum(context, type, smartEnumAttribute);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    Location.None,
                                                    type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), ex.Message));
      }
   }

   private static void AnalyzeValueObject(SymbolAnalysisContext context)
   {
      if (context.Symbol is not INamedTypeSymbol type || type.TypeKind == TypeKind.Error)
         return;

      try
      {
         if (type.DeclaringSyntaxReferences.IsDefaultOrEmpty)
            return;

         if (!type.HasValueObjectAttribute())
            return;

         ValidateValueObject(context, type);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    Location.None,
                                                    type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), ex.Message));
      }
   }

   private static void ValidateValueObject(
      SymbolAnalysisContext context,
      INamedTypeSymbol type)
   {
      var locationOfFirstDeclaration = type.Locations.IsDefaultOrEmpty ? Location.None : type.Locations[0]; // a representative for all

      if (type.IsRecord || type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, type);
         return;
      }

      if (type.ContainingType is not null) // is nested class
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeCannotBeNestedClass, locationOfFirstDeclaration, type);
         return;
      }

      var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.Compilation, new SelfLogErrorLogger(nameof(ThinktectureRuntimeExtensionsAnalyzer)));

      if (factory is null)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    locationOfFirstDeclaration,
                                                    type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                                                    "Could not fetch type information for analysis of the value object."));
         return;
      }

      EnsureNoPrimaryConstructor(context, type);
      TypeMustBePartial(context, type);
      TypeMustNotBeGeneric(context, type, locationOfFirstDeclaration, "Value Object");
      StructMustBeReadOnly(context, type, locationOfFirstDeclaration);

      var assignableMembers = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, false, true, context.CancellationToken, context.ReportDiagnostic)
                                  .Where(m => !m.IsStatic)
                                  .ToList();

      var baseClass = type.BaseType;

      while (!baseClass.IsNullOrObject())
      {
         baseClass.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, locationOfFirstDeclaration, context.ReportDiagnostic).Enumerate();

         baseClass = baseClass.BaseType;
      }

      if (assignableMembers.Count == 1)
      {
         var keyMember = assignableMembers[0];

         if (keyMember.NullableAnnotation == NullableAnnotation.Annotated)
            ReportDiagnostic(context, DiagnosticsDescriptors.KeyMemberShouldNotBeNullable, keyMember.GetIdentifierLocation(context.CancellationToken), keyMember.Name);

         CheckComparerTypes(context, keyMember);
      }
      else
      {
         CheckAssignableMembers(context, assignableMembers);
      }

      if (type is { IsSealed: false, IsAbstract: false })
         ReportDiagnostic(context, DiagnosticsDescriptors.ValueObjectMustBeSealed, locationOfFirstDeclaration, type);
   }

   private static void CheckAssignableMembers(SymbolAnalysisContext context, IReadOnlyList<InstanceMemberInfo> assignableMembers)
   {
      foreach (var assignableMember in assignableMembers)
      {
         if (!assignableMember.ValueObjectMemberSettings.IsExplicitlyDeclared)
            continue;

         CheckComparerTypes(context, assignableMember);

         var comparerAccessor = assignableMember.ValueObjectMemberSettings.ComparerAccessor;

         if (comparerAccessor is not null)
         {
            ReportDiagnostic(context,
                             DiagnosticsDescriptors.ComparerApplicableOnKeyMemberOnly,
                             assignableMember.ValueObjectMemberSettings.GetComparerAttributeLocationOrNull(context.CancellationToken) ?? assignableMember.GetIdentifierLocation(context.CancellationToken),
                             comparerAccessor);
         }
      }
   }

   private static void CheckComparerTypes(SymbolAnalysisContext context, InstanceMemberInfo member)
   {
      if (member.ValueObjectMemberSettings is { HasInvalidEqualityComparerType: true, EqualityComparerAccessor: { } })
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ComparerTypeMustMatchMemberType,
                          member.ValueObjectMemberSettings.GetEqualityComparerAttributeLocationOrNull(context.CancellationToken) ?? member.GetIdentifierLocation(context.CancellationToken),
                          member.ValueObjectMemberSettings.EqualityComparerAccessor);
      }

      if (member.ValueObjectMemberSettings is { HasInvalidComparerType: true, ComparerAccessor: { } })
      {
         ReportDiagnostic(context,
                          DiagnosticsDescriptors.ComparerTypeMustMatchMemberType,
                          member.ValueObjectMemberSettings.GetComparerAttributeLocationOrNull(context.CancellationToken) ?? member.GetIdentifierLocation(context.CancellationToken),
                          member.ValueObjectMemberSettings.ComparerAccessor);
      }
   }

   private static void ValidateEnum(
      SymbolAnalysisContext context,
      INamedTypeSymbol enumType,
      AttributeData smartEnumAttribute)
   {
      var locationOfFirstDeclaration = enumType.Locations.IsDefaultOrEmpty ? Location.None : enumType.Locations[0]; // a representative for all

      if (enumType.IsRecord || enumType.TypeKind is not (TypeKind.Class or TypeKind.Struct))
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, enumType);
         return;
      }

      if (enumType.ContainingType is not null) // is nested class
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeCannotBeNestedClass, locationOfFirstDeclaration, enumType);
         return;
      }

      var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.Compilation, new SelfLogErrorLogger(nameof(ThinktectureRuntimeExtensionsAnalyzer)));

      if (factory is null)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringCodeAnalysis,
                                                    locationOfFirstDeclaration,
                                                    enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                                                    "Could not fetch type information for analysis of the smart enum."));
         return;
      }

      ConstructorsMustBePrivate(context, enumType);
      TypeMustBePartial(context, enumType);
      TypeMustNotBeGeneric(context, enumType, locationOfFirstDeclaration, "Enumeration");

      var keyType = smartEnumAttribute.AttributeClass?.TypeArguments[0];

      if (keyType is null || keyType.TypeKind == TypeKind.Error)
         return;

      if (keyType.NullableAnnotation == NullableAnnotation.Annotated || keyType.SpecialType == SpecialType.System_Nullable_T)
         ReportDiagnostic(context, DiagnosticsDescriptors.EnumKeyShouldNotBeNullable, locationOfFirstDeclaration);

      var isValidatable = smartEnumAttribute.FindIsValidatable() ?? false;

      StructMustBeReadOnly(context, enumType, locationOfFirstDeclaration);

      if (enumType.IsValueType && !isValidatable)
         ReportDiagnostic(context, DiagnosticsDescriptors.NonValidatableEnumsMustBeClass, locationOfFirstDeclaration, enumType);

      var items = enumType.EnumerateEnumItems().ToList();

      if (items.Count == 0)
         ReportDiagnostic(context, DiagnosticsDescriptors.EnumerationHasNoItems, locationOfFirstDeclaration, enumType);

      Check_ItemLike_StaticProperties(context, enumType);
      EnumItemsMustBePublic(context, enumType, items);

      if (isValidatable)
         ValidateCreateInvalidItem(context, enumType, keyType, locationOfFirstDeclaration);

      enumType.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, false, false, context.CancellationToken, context.ReportDiagnostic).Enumerate();
      var baseClass = enumType.BaseType;

      while (!baseClass.IsNullOrObject())
      {
         baseClass.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, locationOfFirstDeclaration, context.ReportDiagnostic).Enumerate();

         baseClass = baseClass.BaseType;
      }

      EnumKeyPropertyNameMustNotBeItem(context, smartEnumAttribute, locationOfFirstDeclaration);

      var derivedTypes = ValidateDerivedTypes(context, enumType);

      if (enumType is { IsSealed: false, IsAbstract: false } && derivedTypes.Count == 0)
         ReportDiagnostic(context, DiagnosticsDescriptors.EnumWithoutDerivedTypesMustBeSealed, locationOfFirstDeclaration, enumType);
   }

   private static void TypeMustNotBeGeneric(SymbolAnalysisContext context, INamedTypeSymbol type, Location locationOfFirstDeclaration, string typeKind)
   {
      if (!type.TypeParameters.IsDefaultOrEmpty)
         ReportDiagnostic(context, DiagnosticsDescriptors.EnumsAndValueObjectsMustNotBeGeneric, locationOfFirstDeclaration, typeKind, BuildTypeName(type));
   }

   private static void Check_ItemLike_StaticProperties(SymbolAnalysisContext context, INamedTypeSymbol enumType)
   {
      foreach (var member in enumType.GetNonIgnoredMembers())
      {
         if (member.IsStatic && member is IPropertySymbol property && SymbolEqualityComparer.Default.Equals(property.Type, enumType))
            ReportDiagnostic(context, DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems, property.GetIdentifier(context.CancellationToken)?.GetLocation() ?? Location.None, property.Name);
      }
   }

   private static IReadOnlyList<(INamedTypeSymbol Type, int Level)> ValidateDerivedTypes(SymbolAnalysisContext context, INamedTypeSymbol enumType)
   {
      var derivedTypes = enumType.FindDerivedInnerEnums();
      var typesToLeaveOpen = ImmutableArray.Create<INamedTypeSymbol>();

      for (var i = 0; i < derivedTypes.Count; i++)
      {
         var (type, level) = derivedTypes[i];

         if (level == 1)
         {
            if (type.DeclaredAccessibility != Accessibility.Private)
               ReportDiagnostic(context, DiagnosticsDescriptors.InnerEnumOnFirstLevelMustBePrivate, GetDerivedTypeLocation(type, context), type);
         }
         else if (type.DeclaredAccessibility != Accessibility.Public)
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.InnerEnumOnNonFirstLevelMustBePublic, GetDerivedTypeLocation(type, context), type);
         }

         if (!type.BaseType.IsNullOrObject())
            typesToLeaveOpen = typesToLeaveOpen.Add(type.BaseType);
      }

      for (var i = 0; i < derivedTypes.Count; i++)
      {
         var derivedType = derivedTypes[i];

         if (!derivedType.Type.IsSealed && !derivedType.Type.IsAbstract && !typesToLeaveOpen.Contains(derivedType.Type, SymbolEqualityComparer.Default))
            ReportDiagnostic(context, DiagnosticsDescriptors.EnumWithoutDerivedTypesMustBeSealed, GetDerivedTypeLocation(derivedType.Type, context), derivedType.Type);
      }

      return derivedTypes;
   }

   private static Location GetDerivedTypeLocation(INamedTypeSymbol derivedType, SymbolAnalysisContext context)
   {
      return ((TypeDeclarationSyntax)derivedType.DeclaringSyntaxReferences.First().GetSyntax(context.CancellationToken)).Identifier.GetLocation();
   }

   private static void ValidateCreateInvalidItem(SymbolAnalysisContext context, INamedTypeSymbol enumType, ITypeSymbol keyType, Location location)
   {
      var hasCreateInvalidItemImplementation = enumType.HasCreateInvalidItemImplementation(keyType, context.CancellationToken, context.ReportDiagnostic);

      if (!hasCreateInvalidItemImplementation && enumType.IsAbstract)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                          location,
                          enumType,
                          keyType);
      }
   }

   private static void EnumKeyPropertyNameMustNotBeItem(SymbolAnalysisContext context, AttributeData enumSettingsAttr, Location location)
   {
      var keyPropName = enumSettingsAttr.FindKeyPropertyName();

      if (!StringComparer.OrdinalIgnoreCase.Equals(keyPropName, "Item"))
         return;

      var attributeSyntax = (AttributeSyntax?)enumSettingsAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken);

      ReportDiagnostic(context, DiagnosticsDescriptors.EnumKeyPropertyNameNotAllowed,
                       attributeSyntax?.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.Identifier.Text == Constants.Attributes.SmartEnum.Properties.KEY_PROPERTY_NAME)?.GetLocation() ?? location, keyPropName);
   }

   private static void EnumItemsMustBePublic(SymbolAnalysisContext context, INamedTypeSymbol type, IReadOnlyList<IFieldSymbol> items)
   {
      for (var i = 0; i < items.Count; i++)
      {
         var item = items[i];

         if (item.DeclaredAccessibility == Accessibility.Public)
            continue;

         ReportDiagnostic(context, DiagnosticsDescriptors.EnumItemMustBePublic,
                          item.GetIdentifier(context.CancellationToken)?.GetLocation() ?? Location.None,
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

   private static void StructMustBeReadOnly(SymbolAnalysisContext context, INamedTypeSymbol type, Location location)
   {
      if (type is { IsValueType: true, IsReadOnly: false })
         ReportDiagnostic(context, DiagnosticsDescriptors.StructMustBeReadOnly, location, type);
   }

   private static void ConstructorsMustBePrivate(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      if (type.Constructors.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < type.Constructors.Length; i++)
      {
         var ctor = type.Constructors[i];

         if (ctor.IsImplicitlyDeclared)
            continue;

         var declarationSyntax = ctor.DeclaringSyntaxReferences.Single().GetSyntax(context.CancellationToken);

         switch (declarationSyntax)
         {
            case ConstructorDeclarationSyntax constructorDeclarationSyntax:
            {
               if (ctor.DeclaredAccessibility != Accessibility.Private)
               {
                  var location = constructorDeclarationSyntax.Identifier.GetLocation();
                  ReportDiagnostic(context, DiagnosticsDescriptors.EnumConstructorsMustBePrivate, location, type);
               }

               break;
            }
            case ClassDeclarationSyntax classDeclarationSyntax:
            {
               var location = classDeclarationSyntax.Identifier.GetLocation();
               ReportDiagnostic(context, DiagnosticsDescriptors.PrimaryConstructorNotAllowed, location, type);
               break;
            }
            case StructDeclarationSyntax structDeclarationSyntax:
            {
               var location = structDeclarationSyntax.Identifier.GetLocation();
               ReportDiagnostic(context, DiagnosticsDescriptors.PrimaryConstructorNotAllowed, location, type);
               break;
            }
         }
      }
   }

   private static void EnsureNoPrimaryConstructor(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      if (type.Constructors.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < type.Constructors.Length; i++)
      {
         var ctor = type.Constructors[i];

         if (ctor.IsImplicitlyDeclared)
            continue;

         var declarationSyntax = ctor.DeclaringSyntaxReferences.Single().GetSyntax(context.CancellationToken);

         switch (declarationSyntax)
         {
            case ClassDeclarationSyntax classDeclarationSyntax:
            {
               var location = classDeclarationSyntax.Identifier.GetLocation();
               ReportDiagnostic(context, DiagnosticsDescriptors.PrimaryConstructorNotAllowed, location, type);
               break;
            }
            case StructDeclarationSyntax structDeclarationSyntax:
            {
               var location = structDeclarationSyntax.Identifier.GetLocation();
               ReportDiagnostic(context, DiagnosticsDescriptors.PrimaryConstructorNotAllowed, location, type);
               break;
            }
         }
      }
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0, ITypeSymbol arg1)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0), BuildTypeName(arg1));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0, string arg1)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0, arg1));
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0, string arg1)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0), arg1);
   }

   private static void ReportDiagnostic(OperationAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0, string arg1)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0, arg1));
   }

   private static string BuildTypeName(ITypeSymbol type)
   {
      return type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
   }
}
