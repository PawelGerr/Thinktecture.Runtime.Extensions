using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class TypeSymbolExtensions
   {
      public static bool IsSelfOrBaseTypesAnEnum(this ITypeSymbol? type)
      {
         while (type is not null)
         {
            if (type.IsEnum(out _))
               return true;

            type = type.BaseType;
         }

         return false;
      }

      public static bool HasValueObjectAttribute(this ITypeSymbol? type, [MaybeNullWhen(false)] out AttributeData valueObjectAttribute)
      {
         if (type is null)
         {
            valueObjectAttribute = null;
            return false;
         }

         valueObjectAttribute = type.FindValueObjectAttribute();

         return valueObjectAttribute is not null;
      }

      public static bool IsEnum(this ITypeSymbol enumType, out IReadOnlyList<INamedTypeSymbol> enumInterfaces)
      {
         var implementedInterfaces = new List<INamedTypeSymbol>();
         enumInterfaces = implementedInterfaces!;

         foreach (var @interface in enumType.Interfaces)
         {
            if (@interface.IsNonValidatableEnumInterface() || @interface.IsValidatableEnumInterface())
               implementedInterfaces.Add(@interface);
         }

         if (implementedInterfaces.Count > 0)
         {
            if (enumType.BaseType is not null)
            {
               foreach (var @interface in enumType.BaseType.AllInterfaces)
               {
                  if (@interface.IsNonValidatableEnumInterface() || @interface.IsValidatableEnumInterface())
                     implementedInterfaces.Add(@interface);
               }
            }

            return true;
         }

         var enumGenerationAttr = enumType.FindEnumGenerationAttribute();

         if (enumGenerationAttr is null)
            return false;

         if (enumType.BaseType is null)
            return false;

         return enumType.BaseType.IsEnum(out enumInterfaces);
      }

      public static INamedTypeSymbol? GetValidEnumInterface(
         this IReadOnlyList<INamedTypeSymbol> enumInterfaces,
         ITypeSymbol enumType,
         Location? location = null,
         Action<Diagnostic>? reportDiagnostic = null)
      {
         INamedTypeSymbol? validInterface = null;
         ITypeSymbol? validKeyType = null;

         foreach (var enumInterface in enumInterfaces)
         {
            if (!enumInterface.IsGenericType || enumInterface.TypeArguments.Length != 1)
               continue;

            var keyType = enumInterface.TypeArguments[0];

            if (validInterface == null)
            {
               validInterface = enumInterface;
               validKeyType = keyType;
            }
            else
            {
               if (!SymbolEqualityComparer.Default.Equals(validKeyType, keyType))
               {
                  location ??= ((TypeDeclarationSyntax)enumType.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation();
                  reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.MultipleIncompatibleEnumInterfaces, location, enumType.Name));
                  return null;
               }

               if (enumInterface.IsValidatableEnumInterface())
               {
                  validInterface = enumInterface;
                  validKeyType = keyType;
               }
            }
         }

         return validInterface;
      }

      public static bool IsNonValidatableEnumInterface(this ITypeSymbol type)
      {
         return type.ContainingNamespace?.Name == "Thinktecture"
                && type.Name == "IEnum";
      }

      public static bool IsValidatableEnumInterface(this ITypeSymbol type)
      {
         return type.ContainingNamespace?.Name == "Thinktecture"
                && type.Name == "IValidatableEnum";
      }

      public static IReadOnlyList<IFieldSymbol> GetEnumItems(this ITypeSymbol enumType)
      {
         return enumType.EnumerateEnumItems().ToList()!;
      }

      public static IEnumerable<IFieldSymbol> EnumerateEnumItems(this ITypeSymbol enumType)
      {
         return enumType.GetNonIgnoredMembers()
                        .Select(m =>
                                {
                                   if (!m.IsStatic || m is not IFieldSymbol field || field.IsPropertyBackingField(out _))
                                      return null;

                                   if (SymbolEqualityComparer.Default.Equals(field.Type, enumType))
                                      return field;

                                   return null;
                                })
                        .Where(field => field is not null)!;
      }

      public static bool IsFormattable(this ITypeSymbol type)
      {
         return type.AllInterfaces.Any(i => i.ContainingNamespace?.Name == "System" && i.Name == "IFormattable");
      }

      public static bool IsComparable(this ITypeSymbol type)
      {
         return type.AllInterfaces.Any(i => i.ContainingNamespace?.Name == "System" && i.Name == "IComparable" &&
                                            (!i.IsGenericType || i.IsGenericType && SymbolEqualityComparer.Default.Equals(i.TypeArguments[0], type)));
      }

      public static AttributeData? FindEnumGenerationAttribute(this ITypeSymbol type)
      {
         return type.FindAttribute("Thinktecture.EnumGenerationAttribute");
      }

      public static AttributeData? FindValueObjectAttribute(this ITypeSymbol type)
      {
         return type.FindAttribute("Thinktecture.ValueObjectAttribute");
      }

      public static bool HasStructLayoutAttribute(this ITypeSymbol enumType)
      {
         return enumType.HasAttribute("System.Runtime.InteropServices.StructLayoutAttribute");
      }

      public static IReadOnlyList<(INamedTypeSymbol Type, int Level)> FindDerivedInnerTypes(this ITypeSymbol enumType)
      {
         var derivedTypes = new List<(INamedTypeSymbol, int Level)>();

         FindDerivedTypes(enumType, 0, enumType, derivedTypes);

         return derivedTypes;
      }

      private static void FindDerivedTypes(ITypeSymbol typeToCheck, int currentLevel, ITypeSymbol enumType, List<(INamedTypeSymbol, int Level)> derivedTypes)
      {
         currentLevel++;

         foreach (var innerType in typeToCheck.GetTypeMembers())
         {
            if (IsDerivedFrom(innerType, enumType))
               derivedTypes.Add((innerType, currentLevel));

            FindDerivedTypes(innerType, currentLevel, enumType, derivedTypes);
         }
      }

      public static bool IsDerivedFrom(this ITypeSymbol? type, ITypeSymbol baseType)
      {
         while (type is not null)
         {
            if (SymbolEqualityComparer.Default.Equals(type.BaseType, baseType))
               return true;

            foreach (var @interface in type.Interfaces)
            {
               if (SymbolEqualityComparer.Default.Equals(@interface, baseType))
                  return true;
            }

            type = type.BaseType;
         }

         return false;
      }

      public static IEnumerable<ISymbol> GetNonIgnoredMembers(this ITypeSymbol type, string? name = null)
      {
         return (name is not null ? type.GetMembers(name) : type.GetMembers())
            .Where(m => !m.HasAttribute("Thinktecture.ValueObjectIgnoreAttribute"));
      }

      public static IReadOnlyList<InstanceMemberInfo> GetAssignableFieldsAndPropertiesAndCheckForReadOnly(
         this ITypeSymbol type,
         bool instanceMembersOnly,
         Action<Diagnostic>? reportDiagnostic = null)
      {
         return type.GetNonIgnoredMembers()
                    .Select(m =>
                            {
                               if (instanceMembersOnly && m.IsStatic
                                   || !m.CanBeReferencedByName)
                                  return null;

                               if (m is IFieldSymbol fds)
                               {
                                  var identifier = fds.GetIdentifier();

                                  if (!fds.IsReadOnly && !fds.IsConst)
                                  {
                                     reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                identifier.GetLocation(),
                                                                                fds.Name,
                                                                                type.Name));
                                  }

                                  return InstanceMemberInfo.CreateFrom(fds);
                               }

                               if (m is IPropertySymbol pds)
                               {
                                  var syntax = (PropertyDeclarationSyntax)pds.DeclaringSyntaxReferences.Single().GetSyntax();

                                  if (syntax.ExpressionBody is not null) // public int Foo => 42;
                                     return null;

                                  var getter = syntax.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));

                                  if (!IsDefaultImplementation(getter)) // public int Foo { get { return 42; } } OR public int Foo { get => 42; }
                                     return null;

                                  var identifier = pds.GetIdentifier();

                                  if (pds.SetMethod is not null)
                                  {
                                     var setter = syntax.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));

                                     if (!IsDefaultImplementation(setter))
                                        return null;

                                     reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.PropertyMustBeReadOnly,
                                                                                identifier.GetLocation(),
                                                                                pds.Name,
                                                                                type.Name));
                                  }

                                  return InstanceMemberInfo.CreateFrom(pds);
                               }

                               return null;
                            })
                    .Where(m => m is not null)
                    .ToList()!;
      }

      public static IReadOnlyList<InstanceMemberInfo> GetReadableInstanceFieldsAndProperties(this ITypeSymbol type)
      {
         return type.GetNonIgnoredMembers()
                    .Select(m =>
                            {
                               if (m.IsStatic || !m.CanBeReferencedByName)
                                  return null;

                               switch (m)
                               {
                                  case IFieldSymbol fds:
                                     return InstanceMemberInfo.CreateFrom(fds);

                                  case IPropertySymbol pds:
                                  {
                                     if (!pds.IsWriteOnly)
                                        return InstanceMemberInfo.CreateFrom(pds);

                                     break;
                                  }
                               }

                               return null;
                            })
                    .Where(m => m is not null)
                    .ToList()!;
      }

      private static bool IsDefaultImplementation(AccessorDeclarationSyntax? accessor)
      {
         if (accessor is null)
            return false;

         return accessor.Body is null && accessor.ExpressionBody is null;
      }

      public static bool HasCreateInvalidImplementation(
         this ITypeSymbol enumType,
         ITypeSymbol keyType,
         Action<Diagnostic>? reportDiagnostic = null)
      {
         foreach (var member in enumType.GetNonIgnoredMembers())
         {
            if (member is not IMethodSymbol method)
               continue;

            if (method.Name != "CreateInvalidItem")
               continue;

            if (method.Parameters.Length == 1)
            {
               var parameterType = method.Parameters[0].Type;
               var returnType = method.ReturnType;

               if (member.IsStatic &&
                   member.DeclaredAccessibility == Accessibility.Private &&
                   SymbolEqualityComparer.Default.Equals(parameterType, keyType) &&
                   SymbolEqualityComparer.Default.Equals(returnType, enumType))
               {
                  return true;
               }
            }

            reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.InvalidSignatureOfCreateInvalidItem,
                                                       method.GetIdentifier().GetLocation(),
                                                       enumType.Name,
                                                       keyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

            return true;
         }

         return false;
      }
   }
}
