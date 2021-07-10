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
                                                                                                                 DiagnosticsDescriptors.ExtendedEnumCannotBeValidatableIfBaseEnumIsNot,
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
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, type.Name));
            return;
         }

         if (type.ContainingType is not null) // is nested class
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeCannotBeNestedClass, locationOfFirstDeclaration, type.Name));
            return;
         }

         TypeMustBePartial(context, type, declarations);
         StructMustBeReadOnly(context, type, locationOfFirstDeclaration);

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
            var memberAttr = assignableMember.Symbol.FindValueObjectEqualityMemberAttribute();

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

      private static void ValidateEnum(
         SymbolAnalysisContext context,
         IReadOnlyList<TypeDeclarationSyntax> declarations,
         INamedTypeSymbol enumType,
         IReadOnlyList<INamedTypeSymbol> enumInterfaces)
      {
         var locationOfFirstDeclaration = declarations[0].Identifier.GetLocation(); // a representative for all

         if (enumType.IsRecord ||
             enumType.TypeKind != TypeKind.Class && enumType.TypeKind != TypeKind.Struct)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, enumType.Name));
            return;
         }

         if (enumType.ContainingType is not null) // is nested class
         {
            if (!enumType.BaseType.IsSelfOrBaseTypesAnEnum()) // base class is not an enum because in this case "DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces" kicks in
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeCannotBeNestedClass, locationOfFirstDeclaration, enumType.Name));
               return;
            }
         }

         TypeMustBePartial(context, enumType, declarations);

         var validEnumInterface = enumInterfaces.GetValidEnumInterface(enumType, locationOfFirstDeclaration, context.ReportDiagnostic);

         if (validEnumInterface is null)
            return;

         var isValidatable = validEnumInterface.IsValidatableEnumInterface();

         StructMustBeReadOnly(context, enumType, locationOfFirstDeclaration);

         if (enumType.IsValueType && !isValidatable)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NonValidatableEnumsMustBeClass, locationOfFirstDeclaration, enumType.Name));

         ConstructorsMustBePrivate(context, enumType);

         var items = enumType.GetEnumItems();

         if (items.Count == 0)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NoItemsWarning, locationOfFirstDeclaration, enumType.Name));

         Check_ItemLike_StaticProperties(context, enumType);
         FieldsMustBePublic(context, enumType, items);

         if (isValidatable)
            ValidateCreateInvalidItem(context, enumType, validEnumInterface, locationOfFirstDeclaration);

         var assignableMembers = enumType.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.ReportDiagnostic);

         var hasBaseEnum = ValidateBaseEnum(context, enumType, locationOfFirstDeclaration, isValidatable);

         var enumAttr = enumType.FindEnumGenerationAttribute();

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
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.DerivedEnumMustNotBeExtensible,
                                                             enumAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? locationOfFirstDeclaration,
                                                             enumType.Name));
               }

               if (enumType.IsValueType)
               {
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtensibleEnumCannotBeStruct,
                                                             enumAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? locationOfFirstDeclaration,
                                                             enumType.Name));
               }

               if (enumType.IsAbstract)
               {
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtensibleEnumCannotBeAbstract,
                                                             enumAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? locationOfFirstDeclaration,
                                                             enumType.Name));
               }
            }
         }

         ValidateDerivedTypes(context, enumType);
      }

      private static void CheckKeyComparer(SymbolAnalysisContext context, IEnumerable<ISymbol> comparerMembers, bool isExtensible)
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
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, comparerMember.DeclaringSyntaxReferences.Single().GetSyntax().GetLocation(), comparerMember.Name));
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
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems, property.GetIdentifier().GetLocation(), property.Name));
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

            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtensibleEnumMustNotHaveVirtualMembers,
                                                       virtualKeyword?.GetLocation() ?? location,
                                                       enumType.Name));
         }
      }

      private static bool ValidateBaseEnum(
         SymbolAnalysisContext context,
         INamedTypeSymbol enumType,
         Location location,
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
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtendedEnumCannotBeValidatableIfBaseEnumIsNot, location, enumType.Name));

         var baseEnumAttr = enumType.BaseType.FindEnumGenerationAttribute();
         var isBaseEnumExtensible = baseEnumAttr?.IsExtensible() ?? false;

         if (!isBaseEnumExtensible)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.BaseEnumMustBeExtensible, location, enumType.BaseType.Name));

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

            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ExtensibleEnumMemberMustBePublicOrHaveMapping,
                                                       memberInfo.Identifier.GetLocation(),
                                                       memberInfo.Identifier));
         }
      }

      private static bool HasMemberMapping(
         SymbolAnalysisContext context,
         INamedTypeSymbol enumType,
         InstanceMemberInfo memberInfo)
      {
         var enumMemberAttr = memberInfo.Symbol.FindEnumGenerationMemberAttribute();

         if (enumMemberAttr is null)
            return false;

         var mappedMemberName = enumMemberAttr.FindMapsToMember();

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

            if (mappedMember.DeclaredAccessibility != Accessibility.Public)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.MappedMemberMustBePublic,
                                                          enumMemberAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? memberInfo.Identifier.GetLocation(),
                                                          mappedMemberName));
            }

            if (mappedMember is IMethodSymbol { TypeParameters: { Length: > 0 } })
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

      private static void ValidateCreateInvalidItem(SymbolAnalysisContext context, INamedTypeSymbol enumType, INamedTypeSymbol validEnumInterface, Location location)
      {
         var keyType = validEnumInterface.TypeArguments[0];
         var hasCreateInvalidImplementation = enumType.HasCreateInvalidImplementation(keyType, context.ReportDiagnostic);

         if (!hasCreateInvalidImplementation && enumType.IsAbstract)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                                                       location,
                                                       enumType.Name,
                                                       keyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
         }
      }

      private static void EnumKeyPropertyNameMustNotBeItem(SymbolAnalysisContext context, AttributeData enumSettingsAttr, Location location)
      {
         var keyPropName = enumSettingsAttr.FindKeyPropertyName();

         if (!StringComparer.OrdinalIgnoreCase.Equals(keyPropName, "Item"))
            return;

         var attributeSyntax = (AttributeSyntax?)enumSettingsAttr.ApplicationSyntaxReference?.GetSyntax();

         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyPropertyNameNotAllowed,
                                                    attributeSyntax?.ArgumentList?.GetLocation() ?? location,
                                                    keyPropName));
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

      private static void TypeMustBePartial(SymbolAnalysisContext context, INamedTypeSymbol type, IReadOnlyList<TypeDeclarationSyntax> declarations)
      {
         foreach (var tds in declarations)
         {
            if (!tds.IsPartial())
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBePartial, tds.Identifier.GetLocation(), type.Name));
         }
      }

      private static void StructMustBeReadOnly(SymbolAnalysisContext context, INamedTypeSymbol type, Location location)
      {
         if (type.IsValueType && !type.IsReadOnly)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.StructMustBeReadOnly, location, type.Name));
      }

      private static void ConstructorsMustBePrivate(SymbolAnalysisContext context, INamedTypeSymbol type)
      {
         foreach (var ctor in type.Constructors)
         {
            if (ctor.IsImplicitlyDeclared || ctor.DeclaredAccessibility == Accessibility.Private)
               continue;

            var location = ((ConstructorDeclarationSyntax)ctor.DeclaringSyntaxReferences.Single().GetSyntax()).Identifier.GetLocation();
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ConstructorsMustBePrivate, location, type.Name));
         }
      }
   }
}
