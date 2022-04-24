using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Thinktecture.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ThinktectureRuntimeExtensionsAnalyzer : DiagnosticAnalyzer
{
   /// <inheritdoc />
   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial,
                                                                                                              DiagnosticsDescriptors.StructMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.TypeMustBeClassOrStruct,
                                                                                                              DiagnosticsDescriptors.NonValidatableEnumsMustBeClass,
                                                                                                              DiagnosticsDescriptors.ConstructorsMustBePrivate,
                                                                                                              DiagnosticsDescriptors.NoItemsWarning,
                                                                                                              DiagnosticsDescriptors.FieldMustBePublic,
                                                                                                              DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.PropertyMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                                                                                                              DiagnosticsDescriptors.InvalidSignatureOfCreateInvalidItem,
                                                                                                              DiagnosticsDescriptors.KeyPropertyNameNotAllowed,
                                                                                                              DiagnosticsDescriptors.MultipleIncompatibleEnumInterfaces,
                                                                                                              DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces,
                                                                                                              DiagnosticsDescriptors.FirstLevelInnerTypeMustBePrivate,
                                                                                                              DiagnosticsDescriptors.NonFirstLevelInnerTypeMustBePublic,
                                                                                                              DiagnosticsDescriptors.TypeCannotBeNestedClass,
                                                                                                              DiagnosticsDescriptors.KeyMemberShouldNotBeNullable,
                                                                                                              DiagnosticsDescriptors.ExtensibleEnumMemberMustBePublicOrHaveMapping,
                                                                                                              DiagnosticsDescriptors.MemberNotFound,
                                                                                                              DiagnosticsDescriptors.MultipleMembersWithSameName,
                                                                                                              DiagnosticsDescriptors.MappedMemberMustBePublic,
                                                                                                              DiagnosticsDescriptors.MappedMethodMustBeNotBeGeneric,
                                                                                                              DiagnosticsDescriptors.ExtendedEnumCannotBeValidatableIfBaseEnumIsNot,
                                                                                                              DiagnosticsDescriptors.DerivedEnumMustNotBeExtensible,
                                                                                                              DiagnosticsDescriptors.BaseEnumMustBeExtensible,
                                                                                                              DiagnosticsDescriptors.ExtensibleEnumCannotBeStruct,
                                                                                                              DiagnosticsDescriptors.ExtensibleEnumCannotBeAbstract,
                                                                                                              DiagnosticsDescriptors.ExtensibleEnumMustNotHaveVirtualMembers,
                                                                                                              DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems,
                                                                                                              DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty,
                                                                                                              DiagnosticsDescriptors.KeyComparerOfExtensibleEnumMustBeProtectedOrPublic,
                                                                                                              DiagnosticsDescriptors.ComparerApplicableOnKeyMemberOnly,
                                                                                                              DiagnosticsDescriptors.ExtendedEnumMustHaveSameKeyPropertyName,
                                                                                                              DiagnosticsDescriptors.EnumsAndValueObjectsMustNotBeGeneric);

   /// <inheritdoc />
   public override void Initialize(AnalysisContext context)
   {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
   }

   private static void AnalyzeSymbol(SymbolAnalysisContext context)
   {
      var type = (INamedTypeSymbol)context.Symbol;

      if (type.DeclaringSyntaxReferences.Length == 0)
         return;

      var declarations = new TypeDeclarationSyntax[type.DeclaringSyntaxReferences.Length];

      for (var i = 0; i < type.DeclaringSyntaxReferences.Length; i++)
      {
         var syntaxRef = type.DeclaringSyntaxReferences[i];

         if (syntaxRef.GetSyntax() is not TypeDeclarationSyntax tds)
            return;

         declarations[i] = tds;
      }

      if (type.IsEnum(out var enumInterfaces))
         ValidateEnum(context, declarations, type, enumInterfaces);

      if (type.HasValueObjectAttribute(out _))
         ValidateValueObject(context, declarations, type);
   }

   private static void ValidateValueObject(
      SymbolAnalysisContext context,
      IReadOnlyList<TypeDeclarationSyntax> declarations,
      INamedTypeSymbol type)
   {
      var locationOfFirstDeclaration = declarations[0].Identifier.GetLocation(); // a representative for all

      if (type.IsRecord ||
          type.TypeKind != TypeKind.Class && type.TypeKind != TypeKind.Struct)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, type);
         return;
      }

      if (type.ContainingType is not null) // is nested class
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeCannotBeNestedClass, locationOfFirstDeclaration, type);
         return;
      }

      TypeMustBePartial(context, type, declarations);
      TypeMustNotBeGeneric(context, type, locationOfFirstDeclaration, "Value Object");
      StructMustBeReadOnly(context, type, locationOfFirstDeclaration);

      var assignableMembers = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.ReportDiagnostic)
                                  .Where(m => !m.IsStatic)
                                  .ToList();

      if (assignableMembers.Count == 1)
      {
         var keyMember = assignableMembers[0];

         if (keyMember.NullableAnnotation == NullableAnnotation.Annotated || keyMember.IsNullableStruct)
            ReportDiagnostic(context, DiagnosticsDescriptors.KeyMemberShouldNotBeNullable, keyMember.GetIdentifierLocation(), keyMember.Name);
      }
      else
      {
         CheckAssignableMembers(context, assignableMembers);
      }
   }

   private static void CheckAssignableMembers(SymbolAnalysisContext context, IReadOnlyList<InstanceMemberInfo> assignableMembers)
   {
      foreach (var assignableMember in assignableMembers)
      {
         var comparer = assignableMember.ValueObjectMemberSettings.Comparer;

         if (comparer is not null)
         {
            ReportDiagnostic(context,
                             DiagnosticsDescriptors.ComparerApplicableOnKeyMemberOnly,
                             assignableMember.ValueObjectMemberSettings.GetAttributeLocationOrNull(context.CancellationToken) ?? assignableMember.GetIdentifierLocation(),
                             comparer);
         }
      }
   }

   private static void ValidateEnum(
      SymbolAnalysisContext context,
      IReadOnlyList<TypeDeclarationSyntax> declarations,
      INamedTypeSymbol enumType,
      IReadOnlyList<INamedTypeSymbol> enumInterfaces)
   {
      var locationOfFirstDeclaration = declarations[0].Identifier.GetLocation(); // a representative for all

      if (enumType.IsRecord
          || (enumType.TypeKind != TypeKind.Class && enumType.TypeKind != TypeKind.Struct))
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, enumType);
         return;
      }

      if (enumType.ContainingType is not null) // is nested class
      {
         if (!enumType.BaseType.IsSelfOrBaseTypesAnEnum()) // base class is not an enum because in this case "DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces" kicks in
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.TypeCannotBeNestedClass, locationOfFirstDeclaration, enumType);
            return;
         }
      }

      TypeMustBePartial(context, enumType, declarations);
      TypeMustNotBeGeneric(context, enumType, locationOfFirstDeclaration, "Enumeration");

      var validEnumInterface = enumInterfaces.GetValidEnumInterface(enumType, locationOfFirstDeclaration, context.ReportDiagnostic);

      if (validEnumInterface is null)
         return;

      var isValidatable = validEnumInterface.IsValidatableEnumInterface();

      StructMustBeReadOnly(context, enumType, locationOfFirstDeclaration);

      if (enumType.IsValueType && !isValidatable)
         ReportDiagnostic(context, DiagnosticsDescriptors.NonValidatableEnumsMustBeClass, locationOfFirstDeclaration, enumType);

      ConstructorsMustBePrivate(context, enumType);

      var items = enumType.EnumerateEnumItems().ToList();

      if (items.Count == 0)
         ReportDiagnostic(context, DiagnosticsDescriptors.NoItemsWarning, locationOfFirstDeclaration, enumType);

      Check_ItemLike_StaticProperties(context, enumType);
      FieldsMustBePublic(context, enumType, items);

      if (isValidatable)
         ValidateCreateInvalidItem(context, enumType, validEnumInterface, locationOfFirstDeclaration);

      var assignableMembers = enumType.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.ReportDiagnostic).ToList();
      var enumAttr = enumType.FindEnumGenerationAttribute();

      // don't validate base-enum of nested classes because
      // a) the base class will be either another nested class OR
      // b) the public enum, which is validated separately
      var hasBaseEnum = enumType.ContainingType is null && ValidateBaseEnum(context, enumType, enumAttr, locationOfFirstDeclaration, isValidatable);

      if (enumAttr is not null)
      {
         EnumKeyPropertyNameMustNotBeItem(context, enumAttr, locationOfFirstDeclaration);

         var comparer = enumAttr.FindKeyComparer();
         var comparerMembers = comparer is null ? Array.Empty<ISymbol>() : enumType.GetNonIgnoredMembers(comparer);

         var isExtensible = enumAttr.IsExtensible() ?? false;

         CheckKeyComparer(context, comparerMembers, isExtensible);

         if (isExtensible)
         {
            AssignableMembersMustBePublicOrBeMapped(context, enumType, assignableMembers);
            InstanceMembersMustNotBeVirtual(context, enumType, locationOfFirstDeclaration);

            if (hasBaseEnum)
            {
               ReportDiagnostic(context, DiagnosticsDescriptors.DerivedEnumMustNotBeExtensible,
                                enumAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? locationOfFirstDeclaration, enumType);
            }

            if (enumType.IsValueType)
            {
               ReportDiagnostic(context, DiagnosticsDescriptors.ExtensibleEnumCannotBeStruct,
                                enumAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? locationOfFirstDeclaration, enumType);
            }

            if (enumType.IsAbstract)
            {
               ReportDiagnostic(context, DiagnosticsDescriptors.ExtensibleEnumCannotBeAbstract,
                                enumAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? locationOfFirstDeclaration, enumType);
            }
         }
      }

      ValidateDerivedTypes(context, enumType);
   }

   private static void TypeMustNotBeGeneric(SymbolAnalysisContext context, INamedTypeSymbol type, Location locationOfFirstDeclaration, string typeKind)
   {
      if (type.TypeParameters.Length > 0)
         ReportDiagnostic(context, DiagnosticsDescriptors.EnumsAndValueObjectsMustNotBeGeneric, locationOfFirstDeclaration, typeKind, BuildTypeName(type));
   }

   private static void CheckKeyComparer(SymbolAnalysisContext context, IEnumerable<ISymbol> comparerMembers, bool isExtensible)
   {
      foreach (var comparerMember in comparerMembers)
      {
         switch (comparerMember)
         {
            case IFieldSymbol field:

               if (!field.IsStatic)
                  ReportDiagnostic(context, DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, field.GetIdentifier().GetLocation(), field.Name);

               if (isExtensible && !field.DeclaredAccessibility.IsAtLeastProtected())
                  ReportDiagnostic(context, DiagnosticsDescriptors.KeyComparerOfExtensibleEnumMustBeProtectedOrPublic, field.GetIdentifier().GetLocation(), field.Name);

               break;

            case IPropertySymbol property:

               if (!property.IsStatic)
                  ReportDiagnostic(context, DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, property.GetIdentifier().GetLocation(), property.Name);

               if (isExtensible && !property.DeclaredAccessibility.IsAtLeastProtected())
                  ReportDiagnostic(context, DiagnosticsDescriptors.KeyComparerOfExtensibleEnumMustBeProtectedOrPublic, property.GetIdentifier().GetLocation(), property.Name);

               break;

            default:
               ReportDiagnostic(context, DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, comparerMember.DeclaringSyntaxReferences.Single().GetSyntax().GetLocation(), comparerMember.Name);
               break;
         }
      }
   }

   private static void Check_ItemLike_StaticProperties(SymbolAnalysisContext context, INamedTypeSymbol enumType)
   {
      foreach (var member in enumType.GetNonIgnoredMembers())
      {
         if (member.IsStatic && member is IPropertySymbol property && SymbolEqualityComparer.Default.Equals(property.Type, enumType))
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems, property.GetIdentifier().GetLocation(), property.Name);
         }
      }
   }

   private static void InstanceMembersMustNotBeVirtual(SymbolAnalysisContext context, INamedTypeSymbol enumType, Location location)
   {
      foreach (var member in enumType.GetMembers())
      {
         if (!member.IsVirtual)
            continue;

         var virtualKeyword = member.DeclaringSyntaxReferences
                                    .Select(r =>
                                            {
                                               var node = (MemberDeclarationSyntax)r.GetSyntax();
                                               var keyword = node.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.VirtualKeyword));

                                               return keyword == default ? (SyntaxToken?)null : keyword;
                                            })
                                    .SingleOrDefault(n => n is not null);

         ReportDiagnostic(context, DiagnosticsDescriptors.ExtensibleEnumMustNotHaveVirtualMembers,
                          virtualKeyword?.GetLocation() ?? location, enumType);
      }
   }

   private static bool ValidateBaseEnum(
      SymbolAnalysisContext context,
      INamedTypeSymbol enumType,
      AttributeData? enumAttribute,
      Location location,
      bool isValidatable)
   {
      if (!enumType.BaseType.IsEnum(out var baseEnumInterfaces))
         return false;

      var isBaseEnumValidatable = baseEnumInterfaces.GetValidEnumInterface(enumType.BaseType)?.IsValidatableEnumInterface() ?? false;

      if (isValidatable != isBaseEnumValidatable)
         ReportDiagnostic(context, DiagnosticsDescriptors.ExtendedEnumCannotBeValidatableIfBaseEnumIsNot, location, enumType);

      var baseEnumAttr = enumType.BaseType.FindEnumGenerationAttribute();
      var isBaseEnumExtensible = baseEnumAttr?.IsExtensible() ?? false;

      if (!isBaseEnumExtensible)
         ReportDiagnostic(context, DiagnosticsDescriptors.BaseEnumMustBeExtensible, location, enumType.BaseType);

      var baseKeyPropName = baseEnumAttr?.FindKeyPropertyName();
      var enumKeyPropName = enumAttribute?.FindKeyPropertyName();

      if (!String.IsNullOrWhiteSpace(baseKeyPropName)
          && !String.IsNullOrWhiteSpace(enumKeyPropName)
          && baseKeyPropName != enumKeyPropName)
         ReportDiagnostic(context, DiagnosticsDescriptors.ExtendedEnumMustHaveSameKeyPropertyName, location, enumType, enumType.BaseType);

      return true;
   }

   private static void AssignableMembersMustBePublicOrBeMapped(
      SymbolAnalysisContext context,
      INamedTypeSymbol enumType,
      IReadOnlyList<InstanceMemberInfo> assignableMembers)
   {
      foreach (var memberInfo in assignableMembers)
      {
         if (memberInfo.ReadAccessibility == Accessibility.Public || memberInfo.IsStatic)
            continue;

         if (HasMemberMapping(context, enumType, memberInfo))
            continue;

         ReportDiagnostic(context, DiagnosticsDescriptors.ExtensibleEnumMemberMustBePublicOrHaveMapping,
                          memberInfo.GetIdentifierLocation(), memberInfo.Name);
      }
   }

   private static bool HasMemberMapping(
      SymbolAnalysisContext context,
      INamedTypeSymbol enumType,
      InstanceMemberInfo memberInfo)
   {
      var mappedMemberName = memberInfo.EnumMemberSettings.MappedMemberName;

      if (mappedMemberName is null)
         return false;

      var mappedMembers = enumType.GetMembers()
                                  .Where(m => m switch
                                  {
                                     IFieldSymbol field => field.Name == mappedMemberName,
                                     IPropertySymbol property => property.Name == mappedMemberName,
                                     IMethodSymbol method => method.Name == mappedMemberName,
                                     _ => false
                                  })
                                  .ToList();

      if (mappedMembers.Count == 0)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.MemberNotFound,
                          memberInfo.EnumMemberSettings.GetAttributeLocationOrNull(context.CancellationToken) ?? memberInfo.GetIdentifierLocation(), mappedMemberName);
      }
      else if (mappedMembers.Count > 1)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.MultipleMembersWithSameName,
                          memberInfo.EnumMemberSettings.GetAttributeLocationOrNull(context.CancellationToken) ?? memberInfo.GetIdentifierLocation(), mappedMemberName);
      }
      else
      {
         var mappedMember = mappedMembers[0];

         if (mappedMember.DeclaredAccessibility != Accessibility.Public)
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.MappedMemberMustBePublic,
                             memberInfo.EnumMemberSettings.GetAttributeLocationOrNull(context.CancellationToken) ?? memberInfo.GetIdentifierLocation(), mappedMemberName);
         }

         if (mappedMember is IMethodSymbol { TypeParameters: { Length: > 0 } })
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.MappedMethodMustBeNotBeGeneric,
                             memberInfo.EnumMemberSettings.GetAttributeLocationOrNull(context.CancellationToken) ?? memberInfo.GetIdentifierLocation(), mappedMemberName);
         }
      }

      return true;
   }

   private static void ValidateDerivedTypes(SymbolAnalysisContext context, INamedTypeSymbol enumType)
   {
      var derivedTypes = enumType.FindDerivedInnerEnums();

      foreach (var derivedType in derivedTypes)
      {
         if (derivedType.Type.IsEnum())
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces,
                             ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(), derivedType.Type);
         }

         if (derivedType.Level == 1)
         {
            if (derivedType.Type.DeclaredAccessibility != Accessibility.Private)
            {
               ReportDiagnostic(context, DiagnosticsDescriptors.FirstLevelInnerTypeMustBePrivate,
                                ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(), derivedType.Type);
            }
         }
         else if (derivedType.Type.DeclaredAccessibility != Accessibility.Public)
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.NonFirstLevelInnerTypeMustBePublic,
                             ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(), derivedType.Type);
         }
      }
   }

   private static void ValidateCreateInvalidItem(SymbolAnalysisContext context, INamedTypeSymbol enumType, INamedTypeSymbol validEnumInterface, Location location)
   {
      var keyType = validEnumInterface.TypeArguments[0];
      var hasCreateInvalidImplementation = enumType.HasCreateInvalidImplementation(keyType, context.ReportDiagnostic);

      if (!hasCreateInvalidImplementation && enumType.IsAbstract)
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

      var attributeSyntax = (AttributeSyntax?)enumSettingsAttr.ApplicationSyntaxReference?.GetSyntax();

      ReportDiagnostic(context, DiagnosticsDescriptors.KeyPropertyNameNotAllowed,
                       attributeSyntax?.ArgumentList?.GetLocation() ?? location, keyPropName);
   }

   private static void FieldsMustBePublic(SymbolAnalysisContext context, INamedTypeSymbol type, IReadOnlyList<IFieldSymbol> items)
   {
      foreach (var item in items)
      {
         if (item.DeclaredAccessibility != Accessibility.Public)
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.FieldMustBePublic,
                             item.GetIdentifier().GetLocation(),
                             item.Name, BuildTypeName(type));
         }
      }
   }

   private static void TypeMustBePartial(SymbolAnalysisContext context, INamedTypeSymbol type, IReadOnlyList<TypeDeclarationSyntax> declarations)
   {
      foreach (var tds in declarations)
      {
         if (!tds.IsPartial())
            ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBePartial, tds.Identifier.GetLocation(), type);
      }
   }

   private static void StructMustBeReadOnly(SymbolAnalysisContext context, INamedTypeSymbol type, Location location)
   {
      if (type.IsValueType && !type.IsReadOnly)
         ReportDiagnostic(context, DiagnosticsDescriptors.StructMustBeReadOnly, location, type);
   }

   private static void ConstructorsMustBePrivate(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      foreach (var ctor in type.Constructors)
      {
         if (ctor.IsImplicitlyDeclared || ctor.DeclaredAccessibility == Accessibility.Private)
            continue;

         var location = ((ConstructorDeclarationSyntax)ctor.DeclaringSyntaxReferences.Single().GetSyntax()).Identifier.GetLocation();
         ReportDiagnostic(context, DiagnosticsDescriptors.ConstructorsMustBePrivate, location, type);
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

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0, string arg1)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0, arg1));
   }

   private static string BuildTypeName(ITypeSymbol type)
   {
      return type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
   }
}
