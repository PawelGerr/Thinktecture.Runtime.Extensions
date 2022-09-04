using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class TypeSymbolExtensions
{
   public static bool IsNullOrObject([NotNullWhen(false)] this ITypeSymbol? type)
   {
      return type is null || type.SpecialType == SpecialType.System_Object;
   }

   public static bool IsSelfOrBaseTypesAnEnum(this ITypeSymbol? type)
   {
      while (type is not null)
      {
         if (type.IsEnum())
            return true;

         type = type.BaseType;
      }

      return false;
   }

   public static bool HasValueObjectAttribute(
      [NotNullWhen(true)] this ITypeSymbol? type,
      [MaybeNullWhen(false)] out AttributeData valueObjectAttribute)
   {
      if (type is null)
      {
         valueObjectAttribute = null;
         return false;
      }

      valueObjectAttribute = type.FindValueObjectAttribute();

      return valueObjectAttribute is not null;
   }

   public static bool IsEnum([NotNullWhen(true)] this ITypeSymbol? enumType)
   {
      return enumType.IsEnum(false, out _);
   }

   public static bool IsEnum(
      [NotNullWhen(true)] this ITypeSymbol? enumType,
      [MaybeNullWhen(false)] out IReadOnlyList<INamedTypeSymbol> enumInterfaces)
   {
      return enumType.IsEnum(true, out enumInterfaces);
   }

   private static bool IsEnum(
      [NotNullWhen(true)] this ITypeSymbol? enumType,
      bool collectFoundInterfaces,
      [MaybeNullWhen(false)] out IReadOnlyList<INamedTypeSymbol> foundEnumInterfaces)
   {
      // The type is an enum, if
      // a) it directly implements IEnum/IValidatableEnum OR
      // b) it has EnumGenerationAttribute and one of its base classes is an enum

      if (enumType.IsNullOrObject())
      {
         foundEnumInterfaces = null;
         return false;
      }

      List<INamedTypeSymbol>? implementedInterfaces = null;

      if (SearchForEnumInterfaces(enumType.Interfaces, collectFoundInterfaces, ref implementedInterfaces))
      {
         if (!enumType.BaseType.IsNullOrObject())
            SearchForEnumInterfaces(enumType.BaseType.AllInterfaces, collectFoundInterfaces, ref implementedInterfaces);

         foundEnumInterfaces = implementedInterfaces ?? (IReadOnlyList<INamedTypeSymbol>)Array.Empty<INamedTypeSymbol>();
         return true;
      }

      var enumGenerationAttr = enumType.FindEnumGenerationAttribute();

      if (enumGenerationAttr is null)
      {
         foundEnumInterfaces = null;
         return false;
      }

      return enumType.BaseType.IsEnum(collectFoundInterfaces, out foundEnumInterfaces);
   }

   private static bool SearchForEnumInterfaces(ImmutableArray<INamedTypeSymbol> interfaces, bool collectFoundInterfaces, ref List<INamedTypeSymbol>? foundEnumInterfaces)
   {
      foreach (var @interface in interfaces)
      {
         if (@interface.IsNonValidatableEnumInterface() || @interface.IsValidatableEnumInterface())
         {
            if (!collectFoundInterfaces)
               return true;

            (foundEnumInterfaces ??= new List<INamedTypeSymbol>()).Add(@interface);
         }
      }

      return foundEnumInterfaces?.Count > 0;
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

            // if the type implements both, IEnum and IValidatableEnum
            // then it is considered an IValidatableEnum.
            if (enumInterface.IsValidatableEnumInterface())
               validInterface = enumInterface;
         }
      }

      return validInterface;
   }

   public static bool IsNonValidatableEnumInterface(this ITypeSymbol type)
   {
      return type.Name == "IEnum"
             && type.ContainingNamespace is { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true };
   }

   public static bool IsValidatableEnumInterface(this ITypeSymbol type)
   {
      return type.Name == "IValidatableEnum"
             && type.ContainingNamespace is { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true };
   }

   public static bool IsMessagePackFormatterAttribute(this ITypeSymbol type)
   {
      return type.Name == "MessagePackFormatterAttribute"
             && type.ContainingNamespace is { Name: "MessagePack", ContainingNamespace.IsGlobalNamespace: true };
   }

   public static bool IsStructLayoutAttribute(this ITypeSymbol type)
   {
      return type.Name == "StructLayoutAttribute"
             && type.ContainingNamespace is
             {
                Name: "InteropServices",
                ContainingNamespace:
                {
                   Name: "Runtime",
                   ContainingNamespace:
                   {
                      Name: "System",
                      ContainingNamespace.IsGlobalNamespace: true
                   }
                }
             };
   }

   public static bool IsNewtonsoftJsonConverterAttribute(this ITypeSymbol type)
   {
      return type.Name == "JsonConverterAttribute"
             && type.ContainingNamespace is
             {
                Name: "Json",
                ContainingNamespace:
                {
                   Name: "Newtonsoft",
                   ContainingNamespace.IsGlobalNamespace: true
                }
             };
   }

   public static bool IsJsonConverterAttribute(this ITypeSymbol type)
   {
      return type.Name == "JsonConverterAttribute"
             && type.ContainingNamespace is
             {
                Name: "Serialization",
                ContainingNamespace:
                {
                   Name: "Json",
                   ContainingNamespace:
                   {
                      Name: "Text",
                      ContainingNamespace:
                      {
                         Name: "System",
                         ContainingNamespace.IsGlobalNamespace: true
                      }
                   }
                }
             };
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

   public static bool IsFormattableInterface(this INamedTypeSymbol @interface)
   {
      return @interface.Name == "IFormattable"
             && @interface.ContainingNamespace is { Name: "System", ContainingNamespace.IsGlobalNamespace: true };
   }

   public static bool IsComparableInterface(this INamedTypeSymbol @interface, ITypeSymbol genericTypeParameter)
   {
      return @interface.Name == "IComparable"
             && @interface.ContainingNamespace is { Name: "System", ContainingNamespace.IsGlobalNamespace: true }
             && (!@interface.IsGenericType || (@interface.IsGenericType && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], genericTypeParameter)));
   }

   public static AttributeData? FindEnumGenerationAttribute(this ITypeSymbol type)
   {
      return type.FindAttribute(static attrType => attrType.Name == "EnumGenerationAttribute" && attrType.ContainingNamespace is { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true });
   }

   public static AttributeData? FindValueObjectAttribute(this ITypeSymbol type)
   {
      return type.FindAttribute(static attrType => attrType.Name == "ValueObjectAttribute" && attrType.ContainingNamespace is { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true });
   }

   public static IReadOnlyList<(INamedTypeSymbol Type, int Level)> FindDerivedInnerEnums(this ITypeSymbol enumType)
   {
      List<(INamedTypeSymbol, int Level)>? derivedTypes = null;

      FindDerivedInnerEnums(enumType, 0, enumType, ref derivedTypes);

      return derivedTypes ?? (IReadOnlyList<(INamedTypeSymbol Type, int Level)>)Array.Empty<(INamedTypeSymbol Type, int Level)>();
   }

   private static void FindDerivedInnerEnums(ITypeSymbol typeToCheck, int currentLevel, ITypeSymbol enumType, ref List<(INamedTypeSymbol, int Level)>? derivedTypes)
   {
      currentLevel++;

      foreach (var innerType in typeToCheck.GetTypeMembers())
      {
         // enums can be structs or classes only
         if (innerType.TypeKind is not (TypeKind.Class or TypeKind.Struct))
            continue;

         if ((!innerType.IsGenericType || innerType.IsUnboundGenericType) && IsDerivedFrom(innerType, enumType))
            (derivedTypes ??= new List<(INamedTypeSymbol, int Level)>()).Add((innerType, currentLevel));

         FindDerivedInnerEnums(innerType, currentLevel, enumType, ref derivedTypes);
      }
   }

   private static bool IsDerivedFrom(this ITypeSymbol? type, ITypeSymbol baseType)
   {
      while (!type.IsNullOrObject())
      {
         if (baseType.TypeKind == TypeKind.Interface)
         {
            foreach (var @interface in type.Interfaces)
            {
               if (SymbolEqualityComparer.Default.Equals(@interface, baseType))
                  return true;
            }
         }
         else if (SymbolEqualityComparer.Default.Equals(type.BaseType, baseType))
         {
            return true;
         }

         type = type.BaseType;
      }

      return false;
   }

   public static IEnumerable<ISymbol> GetNonIgnoredMembers(this ITypeSymbol type, string? name = null)
   {
      return (name is not null ? type.GetMembers(name) : type.GetMembers())
         .Where(m => !m.HasAttribute(static attrType => attrType.Name == "ValueObjectIgnoreAttribute" && attrType.ContainingNamespace is { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true }));
   }

   public static IEnumerable<InstanceMemberInfo> GetAssignableFieldsAndPropertiesAndCheckForReadOnly(
      this ITypeSymbol type,
      bool instanceMembersOnly,
      Action<Diagnostic>? reportDiagnostic = null)
   {
      return type.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(instanceMembersOnly, null, reportDiagnostic)
                 .Select(tuple =>
                         {
                            return tuple switch
                            {
                               ({ } field, _) => InstanceMemberInfo.CreateFrom(field),
                               (_, { } property) => InstanceMemberInfo.CreateFrom(property),
                               _ => throw new Exception("Either field or property must be set.")
                            };
                         });
   }

   public static IEnumerable<(IFieldSymbol? Field, IPropertySymbol? Property)> IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(
      this ITypeSymbol type,
      bool instanceMembersOnly,
      Location? locationOfDerivedType = null,
      Action<Diagnostic>? reportDiagnostic = null)
   {
      void ReportField(IFieldSymbol field)
      {
         if (reportDiagnostic is null)
            return;

         DiagnosticDescriptor descriptor;
         Location location;

         if (locationOfDerivedType is null)
         {
            descriptor = DiagnosticsDescriptors.FieldMustBeReadOnly;
            location = field.GetIdentifier().GetLocation();
         }
         else
         {
            descriptor = DiagnosticsDescriptors.BaseClassFieldMustBeReadOnly;
            location = locationOfDerivedType;
         }

         reportDiagnostic.Invoke(Diagnostic.Create(descriptor, location, field.Name, type.Name));
      }

      void ReportProperty(IPropertySymbol property, DiagnosticSeverity? severity = null)
      {
         if (reportDiagnostic is null)
            return;

         DiagnosticDescriptor descriptor;
         Location location;

         if (locationOfDerivedType is null)
         {
            descriptor = DiagnosticsDescriptors.PropertyMustBeReadOnly;
            location = property.GetIdentifier().GetLocation();
         }
         else
         {
            descriptor = DiagnosticsDescriptors.BaseClassPropertyMustBeReadOnly;
            location = locationOfDerivedType;
         }

         reportDiagnostic.Invoke(Diagnostic.Create(descriptor, location, effectiveSeverity: severity ?? descriptor.DefaultSeverity, null, null, messageArgs: new object?[] { property.Name, type.Name }));
      }

      return type.GetNonIgnoredMembers()
                 .Select(member =>
                         {
                            if ((instanceMembersOnly && member.IsStatic) || !member.CanBeReferencedByName)
                               return ((IFieldSymbol?, IPropertySymbol?)?)null;

                            switch (member)
                            {
                               case IFieldSymbol field:
                               {
                                  if (!field.IsReadOnly && !field.IsConst)
                                     ReportField(field);

                                  return (field, null);
                               }

                               case IPropertySymbol property:
                               {
                                  // other assembly
                                  if (property.DeclaringSyntaxReferences.Length == 0)
                                  {
                                     if (!property.IsReadOnly && !property.IsWriteOnly)
                                        ReportProperty(property, DiagnosticSeverity.Warning);
                                  }
                                  // same assembly
                                  else
                                  {
                                     var syntax = (PropertyDeclarationSyntax)property.DeclaringSyntaxReferences.Single().GetSyntax();

                                     if (syntax.ExpressionBody is not null) // public int Foo => 42;
                                        return null;

                                     var getter = syntax.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));

                                     if (!IsDefaultImplementation(getter)) // public int Foo { get { return 42; } } OR public int Foo { get => 42; }
                                        return null;

                                     if (property.SetMethod is not null)
                                     {
                                        var setter = syntax.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));

                                        if (!IsDefaultImplementation(setter))
                                           return null;

                                        ReportProperty(property);
                                     }
                                  }

                                  return (null, property);
                               }

                               default:
                                  return null;
                            }
                         })
                 .Where(m => m is not null)
                 .Select(m => m!.Value);
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
