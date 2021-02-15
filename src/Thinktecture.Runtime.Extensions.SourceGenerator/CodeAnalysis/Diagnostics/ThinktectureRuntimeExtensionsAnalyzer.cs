using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Thinktecture.CodeAnalysis.Diagnostics
{
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
                                                                                                                 DiagnosticsDescriptors.ExtendedEnumCannotBeValidatableEnumIfBaseEnumIsNot,
                                                                                                                 DiagnosticsDescriptors.DerivedEnumMustNotBeExtensible,
                                                                                                                 DiagnosticsDescriptors.BaseEnumMustBeExtensible,
                                                                                                                 DiagnosticsDescriptors.ExtensibleEnumCannotBeStruct,
                                                                                                                 DiagnosticsDescriptors.ExtensibleEnumCannotBeAbstract,
                                                                                                                 DiagnosticsDescriptors.ExtensibleEnumMustNotHaveVirtualMembers,
                                                                                                                 DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems,
                                                                                                                 DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty,
                                                                                                                 DiagnosticsDescriptors.KeyComparerOfExtensibleEnumMustBeProtectedOrPublic,
                                                                                                                 DiagnosticsDescriptors.ComparerApplicableOnKeyMemberOnly);

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
         var declaration = type.DeclaringSyntaxReferences.First().GetSyntax();

         if (declaration is not TypeDeclarationSyntax tds)
            return;

         if (type.IsEnum(out var enumInterfaces))
            ValidateEnum(context, declaration, tds, type, enumInterfaces);

         if (type.HasValueTypeAttribute(out var valueTypeAttribute))
            ValidateValueType(context, declaration, tds, type, valueTypeAttribute);
      }

      private static void ValidateValueType(
         SymbolAnalysisContext context,
         SyntaxNode declaration,
         TypeDeclarationSyntax tds,
         INamedTypeSymbol type,
         AttributeData valueTypeAttribute)
      {
         if (!declaration.IsKind(SyntaxKind.ClassDeclaration) && !declaration.IsKind(SyntaxKind.StructDeclaration))
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBeClassOrStruct, tds.Identifier.GetLocation(), tds.Identifier));
            return;
         }

         if (type.ContainingType is not null) // is nested class
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeCannotBeNestedClass, tds.Identifier.GetLocation(), tds.Identifier));
            return;
         }

         TypeMustBePartial(context, tds);
         StructMustBeReadOnly(context, tds, type);

         var assignableMembers = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.ReportDiagnostic)
                                     .Where(m => !m.Symbol.IsStatic)
                                     .ToList();

         if (assignableMembers.Count == 1)
         {
            var keyMember = assignableMembers[0];

            if (keyMember.Type.NullableAnnotation == NullableAnnotation.Annotated || keyMember.IsNullableStruct)
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyMemberShouldNotBeNullable, keyMember.Identifier.GetLocation(), keyMember.Identifier));
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
            var memberAttr = assignableMember.Symbol.FindValueTypeEqualityMemberAttribute();

            if (memberAttr is null)
               continue;

            var comparer = memberAttr.FindComparer();

            if (comparer is not null)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ComparerApplicableOnKeyMemberOnly,
                                                          memberAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? assignableMember.Identifier.GetLocation(),
                                                          comparer));
            }
         }
      }

      private static void ValidateEnum(SymbolAnalysisContext context, SyntaxNode declaration, TypeDeclarationSyntax tds, INamedTypeSymbol enumType, IReadOnlyList<INamedTypeSymbol> enumInterfaces)
      {
         if (!declaration.IsKind(SyntaxKind.ClassDeclaration) && !declaration.IsKind(SyntaxKind.StructDeclaration))
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBeClassOrStruct, tds.Identifier.GetLocation(), tds.Identifier));
            return;
         }

         if (enumType.ContainingType is not null) // is nested class
         {
            if (!enumType.BaseType.IsSelfOrBaseTypesAnEnum()) // base class is not an enum because in this case "DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces" kicks in
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeCannotBeNestedClass, tds.Identifier.GetLocation(), tds.Identifier));
               return;
            }
         }

         TypeMustBePartial(context, tds);

         var validEnumInterface = enumInterfaces.GetValidEnumInterface(enumType, context.ReportDiagnostic);

         if (validEnumInterface is null)
            return;

         var isValidatable = validEnumInterface.IsValidatableEnumInterface();

         StructMustBeReadOnly(context, tds, enumType);

         if (enumType.IsValueType && !isValidatable)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NonValidatableEnumsMustBeClass, tds.Identifier.GetLocation(), tds.Identifier));

         ConstructorsMustBePrivate(context, tds, enumType);

         var items = enumType.GetEnumItems();

         if (items.Count == 0)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NoItemsWarning, tds.Identifier.GetLocation(), tds.Identifier));

         Check_ItemLike_StaticProperties(context, enumType);
         FieldsMustBePublic(context, enumType, items);

         if (isValidatable)
            ValidateCreateInvalidItem(context, tds, enumType, validEnumInterface);

         var assignableMembers = enumType.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.ReportDiagnostic);

         var hasBaseEnum = ValidateBaseEnum(context, tds, enumType, isValidatable);

         var enumAttr = enumType.FindEnumGenerationAttribute();

         if (enumAttr is not null)
         {
            EnumKeyPropertyNameMustNotBeItem(context, tds, enumAttr);

            var comparer = enumAttr.FindKeyComparer();
            var comparerMembers = comparer is null ? (IReadOnlyList<ISymbol>)Array.Empty<ISymbol>() : enumType.GetMembers(comparer);

            var isExtensible = enumAttr.IsExtensible() ?? false;

            CheckKeyComparer(context, comparerMembers, isExtensible);

            if (isExtensible)
            {
               AssignableMembersMustBePublicOrBeMapped(context, tds, assignableMembers);
               InstanceMembersMustNotBeVirtual(context, tds);

               if (hasBaseEnum)
               {
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.DerivedEnumMustNotBeExtensible,
                                                             enumAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? tds.Identifier.GetLocation(),
                                                             tds.Identifier));
               }

               if (enumType.IsValueType)
               {
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtensibleEnumCannotBeStruct,
                                                             enumAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? tds.Identifier.GetLocation(),
                                                             tds.Identifier));
               }

               if (enumType.IsAbstract)
               {
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtensibleEnumCannotBeAbstract,
                                                             enumAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? tds.Identifier.GetLocation(),
                                                             tds.Identifier));
               }
            }
         }

         ValidateDerivedTypes(context, enumType);
      }

      private static void CheckKeyComparer(SymbolAnalysisContext context, IReadOnlyList<ISymbol> comparerMembers, bool isExtensible)
      {
         foreach (var comparerMember in comparerMembers)
         {
            switch (comparerMember)
            {
               case IFieldSymbol field:

                  if (!field.IsStatic)
                     context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, field.GetIdentifier().GetLocation(), field.Name));

                  if (isExtensible && !field.DeclaredAccessibility.IsAtLeastProtected())
                     context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyComparerOfExtensibleEnumMustBeProtectedOrPublic, field.GetIdentifier().GetLocation(), field.Name));

                  break;

               case IPropertySymbol property:

                  if (!property.IsStatic)
                     context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, property.GetIdentifier().GetLocation(), property.Name));

                  if (isExtensible && !property.DeclaredAccessibility.IsAtLeastProtected())
                     context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyComparerOfExtensibleEnumMustBeProtectedOrPublic, property.GetIdentifier().GetLocation(), property.Name));

                  break;

               default:
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, comparerMember.DeclaringSyntaxReferences.First().GetSyntax().GetLocation(), comparerMember.Name));
                  break;
            }
         }
      }

      private static void Check_ItemLike_StaticProperties(SymbolAnalysisContext context, INamedTypeSymbol enumType)
      {
         foreach (var member in enumType.GetMembers())
         {
            if (member.IsStatic && member is IPropertySymbol property && SymbolEqualityComparer.Default.Equals(property.Type, enumType))
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems, property.GetIdentifier().GetLocation(), property.Name));
            }
         }
      }

      private static void InstanceMembersMustNotBeVirtual(SymbolAnalysisContext context, TypeDeclarationSyntax tds)
      {
         foreach (var member in tds.Members)
         {
            var virtualKeyword = member.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.VirtualKeyword));

            if (virtualKeyword != default)
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtensibleEnumMustNotHaveVirtualMembers, virtualKeyword.GetLocation(), tds.Identifier));
         }
      }

      private static bool ValidateBaseEnum(
         SymbolAnalysisContext context,
         TypeDeclarationSyntax enumDeclaration,
         INamedTypeSymbol enumType,
         bool isValidatable)
      {
         if (enumType.BaseType is null)
            return false;

         if (enumType.ContainingType is not null) // inner enum
            return false;

         if (!enumType.BaseType.IsEnum(out var baseEnumInterfaces))
            return false;

         var isBaseEnumValidatable = baseEnumInterfaces.GetValidEnumInterface(enumType.BaseType)?.IsValidatableEnumInterface() ?? false;

         if (isValidatable != isBaseEnumValidatable)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtendedEnumCannotBeValidatableEnumIfBaseEnumIsNot, enumDeclaration.Identifier.GetLocation(), enumDeclaration.Identifier));

         var baseEnumAttr = enumType.BaseType.FindEnumGenerationAttribute();
         var isBaseEnumExtensible = baseEnumAttr?.IsExtensible() ?? false;

         if (!isBaseEnumExtensible)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.BaseEnumMustBeExtensible,
                                                       enumDeclaration.Identifier.GetLocation(),
                                                       enumType.BaseType.Name));
         }

         return true;
      }

      private static void AssignableMembersMustBePublicOrBeMapped(
         SymbolAnalysisContext context,
         TypeDeclarationSyntax enumDeclaration,
         IReadOnlyList<InstanceMemberInfo> assignableMembers)
      {
         foreach (var memberInfo in assignableMembers)
         {
            if (memberInfo.ReadAccessibility == Accessibility.Public || memberInfo.IsStatic)
               continue;

            if (HasMemberMapping(context, memberInfo, enumDeclaration))
               continue;

            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtensibleEnumMemberMustBePublicOrHaveMapping,
                                                       memberInfo.Identifier.GetLocation(),
                                                       memberInfo.Identifier));
         }
      }

      private static bool HasMemberMapping(
         SymbolAnalysisContext context,
         InstanceMemberInfo memberInfo,
         TypeDeclarationSyntax enumDeclaration)
      {
         var enumMemberAttr = memberInfo.Symbol.FindEnumGenerationMemberAttribute();

         if (enumMemberAttr is null)
            return false;

         var mappedMemberName = enumMemberAttr.FindMapsToMember();

         if (mappedMemberName is null)
            return false;

         var mappedMembers = enumDeclaration.Members.Where(m =>
                                                           {
                                                              if (m is FieldDeclarationSyntax field)
                                                                 return field.Declaration.Variables[0].Identifier.ToString() == mappedMemberName;

                                                              if (m is PropertyDeclarationSyntax property)
                                                                 return property.Identifier.ToString() == mappedMemberName;

                                                              if (m is MethodDeclarationSyntax method)
                                                                 return method.Identifier.ToString() == mappedMemberName;

                                                              return false;
                                                           })
                                            .ToList();

         if (mappedMembers.Count == 0)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.MemberNotFound,
                                                       enumMemberAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? memberInfo.Identifier.GetLocation(),
                                                       mappedMemberName));
         }
         else if (mappedMembers.Count > 1)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.MultipleMembersWithSameName,
                                                       enumMemberAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? memberInfo.Identifier.GetLocation(),
                                                       mappedMemberName));
         }
         else
         {
            var mappedMember = mappedMembers[0];

            if (!mappedMember.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.MappedMemberMustBePublic,
                                                          enumMemberAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? memberInfo.Identifier.GetLocation(),
                                                          mappedMemberName));
            }

            if (mappedMember is MethodDeclarationSyntax method &&
                method.TypeParameterList?.Parameters.Count > 0)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.MappedMethodMustBeNotBeGeneric,
                                                          enumMemberAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? memberInfo.Identifier.GetLocation(),
                                                          mappedMemberName));
            }
         }

         return true;
      }

      private static void ValidateDerivedTypes(SymbolAnalysisContext context, INamedTypeSymbol enumType)
      {
         var derivedTypes = enumType.FindDerivedInnerTypes();

         foreach (var derivedType in derivedTypes)
         {
            if (derivedType.Type.IsEnum(out _))
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces,
                                                          ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(),
                                                          derivedType.Type.Name));
            }

            if (derivedType.Level == 1)
            {
               if (derivedType.Type.DeclaredAccessibility != Accessibility.Private)
               {
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.FirstLevelInnerTypeMustBePrivate,
                                                             ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(),
                                                             derivedType.Type.Name));
               }
            }
            else if (derivedType.Type.DeclaredAccessibility != Accessibility.Public)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NonFirstLevelInnerTypeMustBePublic,
                                                          ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(),
                                                          derivedType.Type.Name));
            }
         }
      }

      private static void ValidateCreateInvalidItem(SymbolAnalysisContext context, TypeDeclarationSyntax tds, INamedTypeSymbol enumType, INamedTypeSymbol validEnumInterface)
      {
         var keyType = validEnumInterface.TypeArguments[0];
         var hasCreateInvalidImplementation = enumType.HasCreateInvalidImplementation(keyType, context.ReportDiagnostic);

         if (!hasCreateInvalidImplementation && enumType.IsAbstract)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                                                       tds.Identifier.GetLocation(),
                                                       enumType.Name,
                                                       keyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
         }
      }

      private static void EnumKeyPropertyNameMustNotBeItem(SymbolAnalysisContext context, TypeDeclarationSyntax tds, AttributeData enumSettingsAttr)
      {
         var keyPropName = enumSettingsAttr.FindKeyPropertyName();

         if (StringComparer.OrdinalIgnoreCase.Equals(keyPropName, "Item"))
         {
            var attributeSyntax = (AttributeSyntax?)enumSettingsAttr.ApplicationSyntaxReference?.GetSyntax();

            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyPropertyNameNotAllowed,
                                                       attributeSyntax?.ArgumentList?.GetLocation() ?? tds.Identifier.GetLocation(),
                                                       keyPropName));
         }
      }

      private static void FieldsMustBePublic(SymbolAnalysisContext context, INamedTypeSymbol type, IReadOnlyList<IFieldSymbol> items)
      {
         foreach (var item in items)
         {
            if (item.DeclaredAccessibility != Accessibility.Public)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.FieldMustBePublic,
                                                          item.GetIdentifier().GetLocation(),
                                                          item.Name, type.Name));
            }
         }
      }

      private static void TypeMustBePartial(SymbolAnalysisContext context, TypeDeclarationSyntax tds)
      {
         if (!tds.IsPartial())
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBePartial, tds.Identifier.GetLocation(), tds.Identifier));
      }

      private static void StructMustBeReadOnly(SymbolAnalysisContext context, TypeDeclarationSyntax tds, INamedTypeSymbol type)
      {
         if (type.IsValueType && !type.IsReadOnly)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.StructMustBeReadOnly, tds.Identifier.GetLocation(), tds.Identifier));
      }

      private static void ConstructorsMustBePrivate(SymbolAnalysisContext context, TypeDeclarationSyntax tds, INamedTypeSymbol type)
      {
         foreach (var ctor in type.Constructors)
         {
            if (!ctor.IsImplicitlyDeclared && ctor.DeclaredAccessibility != Accessibility.Private)
            {
               var location = ((ConstructorDeclarationSyntax)ctor.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation();
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ConstructorsMustBePrivate, location, tds.Identifier));
            }
         }
      }
   }
}
