using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class TypeSymbolExtensions
   {
      public static bool IsEnum(this ITypeSymbol enumType, out IReadOnlyList<INamedTypeSymbol> enumInterfaces)
      {
         var implementedInterfaces = new List<INamedTypeSymbol>();
         enumInterfaces = implementedInterfaces!;

         foreach (var @interface in enumType.Interfaces)
         {
            if (@interface.IsNonValidatableEnumInterface() || @interface.IsValidatableEnumInterface())
               implementedInterfaces.Add(@interface);
         }

         return implementedInterfaces.Count > 0;
      }

      public static INamedTypeSymbol? GetValidEnumInterface(
         this IReadOnlyList<INamedTypeSymbol> enumInterfaces,
         ITypeSymbol enumType,
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
                  reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.MultipleIncompatibleEnumInterfaces,
                                                             ((TypeDeclarationSyntax)enumType.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(),
                                                             enumType.Name));
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

      public static IReadOnlyList<IFieldSymbol> GetEnumItems(
         this ITypeSymbol enumType,
         Action<Diagnostic>? reportDiagnostic = null)
      {
         return enumType.GetMembers()
                        .Select(m =>
                                {
                                   if (!m.IsStatic || m is not IFieldSymbol field)
                                      return null;

                                   if (SymbolEqualityComparer.Default.Equals(field.Type, enumType))
                                   {
                                      if (m.DeclaredAccessibility != Accessibility.Public)
                                      {
                                         reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.FieldMustBePublic,
                                                                                    GetLocation(field),
                                                                                    m.Name, enumType.Name));
                                      }

                                      if (!field.IsReadOnly)
                                      {
                                         reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                    GetLocation(field),
                                                                                    m.Name, enumType.Name));
                                      }

                                      return field;
                                   }

                                   return null;
                                })
                        .Where(field => field is not null)
                        .ToList()!;
      }

      public static AttributeData? FindEnumGenerationAttribute(this ITypeSymbol enumType)
      {
         return enumType.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToString() == "Thinktecture.EnumGenerationAttribute");
      }

      public static IReadOnlyList<EnumMemberInfo> GetAssignableInstanceFieldsAndProperties(
         this ITypeSymbol enumType,
         Action<Diagnostic>? reportDiagnostic = null)
      {
         return enumType.GetMembers()
                        .Select(m =>
                                {
                                   if (m.IsStatic || !m.CanBeReferencedByName)
                                      return null;

                                   EnumMemberInfo? member = null;

                                   if (m is IFieldSymbol fds)
                                   {
                                      if (!fds.IsReadOnly)
                                      {
                                         reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                    GetLocation(fds),
                                                                                    fds.Name,
                                                                                    enumType.Name));
                                      }

                                      member = new EnumMemberInfo(fds, fds.Type);
                                   }

                                   if (m is IPropertySymbol pds)
                                   {
                                      var syntax = (PropertyDeclarationSyntax)pds.DeclaringSyntaxReferences.First().GetSyntax();

                                      if (syntax.ExpressionBody is not null) // public int Foo => 42;
                                         return null;

                                      var getter = syntax.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));

                                      if (!IsDefaultImplementation(getter)) // public int Foo { get { return 42; } } OR public int Foo { get => 42; }
                                         return null;

                                      if (pds.SetMethod is not null)
                                      {
                                         var setter = syntax.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));

                                         if (!IsDefaultImplementation(setter))
                                            return null;

                                         reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.PropertyMustBeReadOnly,
                                                                                    GetLocation(pds),
                                                                                    pds.Name,
                                                                                    enumType.Name));
                                      }

                                      member = new EnumMemberInfo(pds, pds.Type);
                                   }

                                   return member;
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
         foreach (var member in enumType.GetMembers())
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
                                                       GetLocation(method),
                                                       enumType.Name,
                                                       keyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

            return true;
         }

         return false;
      }

      private static Location GetLocation(IFieldSymbol field)
      {
         var syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences.First().GetSyntax();
         return syntax.Identifier.GetLocation();
      }

      private static Location GetLocation(IPropertySymbol field)
      {
         var syntax = (PropertyDeclarationSyntax)field.DeclaringSyntaxReferences.First().GetSyntax();
         return syntax.Identifier.GetLocation();
      }

      private static Location GetLocation(IMethodSymbol method)
      {
         var syntax = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences.First().GetSyntax();
         return syntax.Identifier.GetLocation();
      }
   }
}
