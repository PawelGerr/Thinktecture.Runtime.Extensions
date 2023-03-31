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
      while (!type.IsNullOrObject())
      {
         if (type.IsEnum())
            return true;

         type = type.BaseType;
      }

      return false;
   }

   public static bool HasValueObjectAttribute(this ITypeSymbol type)
   {
      var valueObjectAttribute = type.FindValueObjectAttribute();

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
      while (true)
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

         enumType = enumType.BaseType;
      }
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
      (Action<Diagnostic> ReportDiagnostic, Location Location)? diagnostics = null)
   {
      INamedTypeSymbol? validInterface = null;
      ITypeSymbol? validKeyType = null;

      var emitDontImplementEnumInterfaceWithTwoGenerics = false;
      INamedTypeSymbol? enumInterfaceWithTwoGenerics = null;

      for (var i = 0; i < enumInterfaces.Count; i++)
      {
         var enumInterface = enumInterfaces[i];
         var enumInterfaceTypeArgs = enumInterface.TypeArguments;

         if (!enumInterface.IsGenericType || enumInterfaceTypeArgs.IsDefaultOrEmpty)
            continue;

         if (enumInterfaceTypeArgs.Length != 1)
         {
            if (diagnostics is not null && enumInterfaceTypeArgs.Length == 2)
            {
               if (enumInterfaceWithTwoGenerics is null)
               {
                  enumInterfaceWithTwoGenerics = enumInterface;
               }
               // forbid 2 different implementations of IEnum<TKey, T>
               else if (!SymbolEqualityComparer.Default.Equals(enumInterfaceWithTwoGenerics.TypeArguments[0], enumInterfaceTypeArgs[0])
                        || !SymbolEqualityComparer.Default.Equals(enumInterfaceWithTwoGenerics.TypeArguments[1], enumInterfaceTypeArgs[1]))
               {
                  emitDontImplementEnumInterfaceWithTwoGenerics = true;
               }
            }

            continue;
         }

         var keyType = enumInterfaceTypeArgs[0];

         if (validInterface == null)
         {
            validInterface = enumInterface;
            validKeyType = keyType;
         }
         else
         {
            if (!SymbolEqualityComparer.Default.Equals(validKeyType, keyType))
            {
               diagnostics?.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.MultipleIncompatibleEnumInterfaces, diagnostics.Value.Location, enumType.Name));

               return null;
            }

            // if the type implements both, IEnum and IValidatableEnum
            // then it is considered an IValidatableEnum.
            if (enumInterface.IsValidatableEnumInterface())
               validInterface = enumInterface;
         }
      }

      // check the validity of IEnum<TKey, T>
      if (diagnostics is not null && validInterface is not null && validKeyType is not null && enumInterfaceWithTwoGenerics is not null
          && (emitDontImplementEnumInterfaceWithTwoGenerics
              || !SymbolEqualityComparer.Default.Equals(enumInterfaceWithTwoGenerics.TypeArguments[0], validKeyType)
              || !SymbolEqualityComparer.Default.Equals(enumInterfaceWithTwoGenerics.TypeArguments[1], enumType)))
      {
         diagnostics.Value.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.DontImplementEnumInterfaceWithTwoGenerics,
                                                              diagnostics.Value.Location,
                                                              validKeyType,
                                                              enumType.Name));
      }

      return validInterface;
   }

   public static bool IsNonValidatableEnumInterface(this INamedTypeSymbol type)
   {
      return type is { Name: "IEnum", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsValidatableEnumInterface(this INamedTypeSymbol type)
   {
      return type is { Name: "IValidatableEnum", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsMessagePackFormatterAttribute(this ITypeSymbol type)
   {
      return type is { Name: "MessagePackFormatterAttribute", ContainingNamespace: { Name: "MessagePack", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsStructLayoutAttribute(this ITypeSymbol type)
   {
      return type is
      {
         Name: "StructLayoutAttribute",
         ContainingNamespace:
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
         }
      };
   }

   public static bool IsNewtonsoftJsonConverterAttribute(this ITypeSymbol type)
   {
      return type is
      {
         Name: "JsonConverterAttribute",
         ContainingNamespace:
         {
            Name: "Json",
            ContainingNamespace:
            {
               Name: "Newtonsoft",
               ContainingNamespace.IsGlobalNamespace: true
            }
         }
      };
   }

   public static bool IsJsonConverterAttribute(this ITypeSymbol type)
   {
      return type is
      {
         Name: "JsonConverterAttribute",
         ContainingNamespace:
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
         }
      };
   }

   public static IEnumerable<IFieldSymbol> EnumerateEnumItems(this ITypeSymbol enumType)
   {
      return enumType.GetNonIgnoredMembers()
                     .Select(m =>
                             {
                                if (!m.IsStatic || m is not IFieldSymbol field || field.IsPropertyBackingField())
                                   return null;

                                if (SymbolEqualityComparer.Default.Equals(field.Type, enumType))
                                   return field;

                                return null;
                             })
                     .Where(field => field is not null)!;
   }

   public static bool IsFormattableInterface(this INamedTypeSymbol @interface)
   {
      return @interface is { Name: "IFormattable", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsComparableInterface(this INamedTypeSymbol @interface, ITypeSymbol genericTypeParameter)
   {
      return @interface is { Name: "IComparable", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } }
             && (!@interface.IsGenericType || (@interface.IsGenericType && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], genericTypeParameter)));
   }

   public static bool IsParsableInterface(this INamedTypeSymbol @interface, ITypeSymbol genericTypeParameter)
   {
      return @interface is
             {
                Name: "IParsable",
                ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true },
                IsGenericType: true
             }
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], genericTypeParameter);
   }

   public static bool IsIAdditionOperators(this INamedTypeSymbol @interface, ITypeSymbol genericTypeParameter)
   {
      return @interface is
             {
                Name: "IAdditionOperators",
                ContainingNamespace: { Name: "Numerics", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } },
                IsGenericType: true,
                TypeArguments: { IsDefaultOrEmpty: false, Length: 3 }
             }
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], genericTypeParameter)
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[1], genericTypeParameter)
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[2], genericTypeParameter);
   }

   public static bool IsISubtractionOperators(this INamedTypeSymbol @interface, ITypeSymbol genericTypeParameter)
   {
      return @interface is
             {
                Name: "ISubtractionOperators",
                ContainingNamespace: { Name: "Numerics", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } },
                IsGenericType: true,
                TypeArguments: { IsDefaultOrEmpty: false, Length: 3 }
             }
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], genericTypeParameter)
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[1], genericTypeParameter)
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[2], genericTypeParameter);
   }

   public static bool IsIMultiplyOperators(this INamedTypeSymbol @interface, ITypeSymbol genericTypeParameter)
   {
      return @interface is
             {
                Name: "IMultiplyOperators",
                ContainingNamespace: { Name: "Numerics", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } },
                IsGenericType: true,
                TypeArguments: { IsDefaultOrEmpty: false, Length: 3 }
             }
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], genericTypeParameter)
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[1], genericTypeParameter)
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[2], genericTypeParameter);
   }

   public static bool IsIDivisionOperators(this INamedTypeSymbol @interface, ITypeSymbol genericTypeParameter)
   {
      return @interface is
             {
                Name: "IDivisionOperators",
                ContainingNamespace: { Name: "Numerics", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } },
                IsGenericType: true,
                TypeArguments: { IsDefaultOrEmpty: false, Length: 3 }
             }
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], genericTypeParameter)
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[1], genericTypeParameter)
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[2], genericTypeParameter);
   }

   public static bool IsIComparisonOperators(this INamedTypeSymbol @interface, ITypeSymbol genericTypeParameter)
   {
      return @interface is
             {
                Name: "IComparisonOperators",
                ContainingNamespace: { Name: "Numerics", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } },
                IsGenericType: true,
                TypeArguments: { IsDefaultOrEmpty: false, Length: 3 }
             }
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[0], genericTypeParameter)
             && SymbolEqualityComparer.Default.Equals(@interface.TypeArguments[1], genericTypeParameter)
             && @interface.TypeArguments[2].SpecialType == SpecialType.System_Boolean;
   }

   public static AttributeData? FindEnumGenerationAttribute(this ITypeSymbol type)
   {
      return type.FindAttribute(static attrType => attrType is { Name: "EnumGenerationAttribute", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } });
   }

   private static AttributeData? FindValueObjectAttribute(this ITypeSymbol type)
   {
      return type.FindAttribute(static attrType => attrType is { Name: "ValueObjectAttribute", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } });
   }

   public static IReadOnlyList<(INamedTypeSymbol Type, int Level)> FindDerivedInnerEnums(
      this ITypeSymbol enumType)
   {
      List<(INamedTypeSymbol, int Level)>? derivedTypes = null;

      FindDerivedInnerEnums(enumType, 0, enumType, ref derivedTypes);

      return derivedTypes ?? (IReadOnlyList<(INamedTypeSymbol Type, int Level)>)Array.Empty<(INamedTypeSymbol Type, int Level)>();
   }

   private static void FindDerivedInnerEnums(
      ITypeSymbol typeToCheck,
      int currentLevel,
      ITypeSymbol enumType,
      ref List<(INamedTypeSymbol, int Level)>? derivedTypes)
   {
      currentLevel++;

      var types = typeToCheck.GetTypeMembers();

      if (types.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < types.Length; i++)
      {
         var innerType = types[i];

         // derived enums can be classes only
         if (innerType.TypeKind is not TypeKind.Class)
            continue;

         if (IsDerivedFrom(innerType, enumType))
         {
            var derivedType = innerType;

            if (derivedType is { IsGenericType: true, IsUnboundGenericType: false })
               derivedType = derivedType.ConstructUnboundGenericType();

            (derivedTypes ??= new List<(INamedTypeSymbol, int Level)>()).Add((derivedType, currentLevel));
         }

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
         .Where(m => !m.HasAttribute(static attrType => attrType.Name == "ValueObjectMemberIgnoreAttribute" && attrType.ContainingNamespace is { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true }));
   }

   public static IEnumerable<InstanceMemberInfo> GetAssignableFieldsAndPropertiesAndCheckForReadOnly(
      this ITypeSymbol type,
      TypedMemberStateFactory factory,
      bool instanceMembersOnly,
      bool populateValueObjectMemberSettings,
      CancellationToken cancellationToken,
      Action<Diagnostic>? reportDiagnostic = null)
   {
      var allowedCaptureSymbols = reportDiagnostic is not null;

      return type.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(instanceMembersOnly, cancellationToken, null, reportDiagnostic)
                 .Select(tuple =>
                         {
                            return tuple switch
                            {
                               ({ } field, _) => InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings, allowedCaptureSymbols),
                               (_, { } property) => InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings, allowedCaptureSymbols),
                               _ => throw new Exception("Either field or property must be set.")
                            };
                         })
                 .Where(i => i is not null)!;
   }

   public static IEnumerable<(IFieldSymbol? Field, IPropertySymbol? Property)> IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(
      this ITypeSymbol type,
      bool instanceMembersOnly,
      CancellationToken cancellationToken,
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
            location = field.GetIdentifier(cancellationToken)?.GetLocation() ?? Location.None;
         }
         else
         {
            descriptor = DiagnosticsDescriptors.BaseClassFieldMustBeReadOnly;
            location = locationOfDerivedType;
         }

         reportDiagnostic.Invoke(Diagnostic.Create(descriptor, location, field.Name, type.Name));
      }

      void ReportPropertyMustBeReadOnly(IPropertySymbol property, DiagnosticSeverity? severity = null)
      {
         if (reportDiagnostic is null)
            return;

         DiagnosticDescriptor descriptor;
         Location location;

         if (locationOfDerivedType is null)
         {
            descriptor = DiagnosticsDescriptors.PropertyMustBeReadOnly;
            location = property.GetIdentifier(cancellationToken)?.GetLocation() ?? Location.None;
         }
         else
         {
            descriptor = DiagnosticsDescriptors.BaseClassPropertyMustBeReadOnly;
            location = locationOfDerivedType;
         }

         reportDiagnostic.Invoke(Diagnostic.Create(descriptor, location, effectiveSeverity: severity ?? descriptor.DefaultSeverity, null, null, messageArgs: new object?[] { property.Name, type.Name }));
      }

      void ReportPropertyInitAccessorMustBePrivate(IPropertySymbol property)
      {
         if (reportDiagnostic is null)
            return;

         var descriptor = DiagnosticsDescriptors.InitAccessorMustBePrivate;
         var location = property.GetIdentifier(cancellationToken)?.GetLocation() ?? Location.None;

         reportDiagnostic.Invoke(Diagnostic.Create(descriptor, location, effectiveSeverity: descriptor.DefaultSeverity, null, null, messageArgs: new object?[] { property.Name, type.Name }));
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
                                  if (property.DeclaringSyntaxReferences.IsDefaultOrEmpty)
                                  {
                                     if (property is { IsReadOnly: false, IsWriteOnly: false })
                                        ReportPropertyMustBeReadOnly(property, DiagnosticSeverity.Warning);
                                  }
                                  // same assembly
                                  else
                                  {
                                     var syntax = (PropertyDeclarationSyntax)property.DeclaringSyntaxReferences.Single().GetSyntax(cancellationToken);

                                     if (syntax.ExpressionBody is not null) // public int Foo => 42;
                                        return null;

                                     if (syntax.AccessorList is null)
                                        return null;

                                     AccessorDeclarationSyntax? getter = null;
                                     AccessorDeclarationSyntax? setter = null;
                                     AccessorDeclarationSyntax? init = null;

                                     foreach (var accessor in syntax.AccessorList.Accessors)
                                     {
                                        switch ((SyntaxKind)accessor.RawKind)
                                        {
                                           case SyntaxKind.GetAccessorDeclaration:
                                              getter = accessor;
                                              break;
                                           case SyntaxKind.SetAccessorDeclaration:
                                              setter = accessor;
                                              break;
                                           case SyntaxKind.InitAccessorDeclaration:
                                              init = accessor;
                                              break;
                                        }
                                     }

                                     // public int Foo { get { return 42; } }
                                     // public int Foo { get => 42; }
                                     // public int Foo { get => _foo; }
                                     // If we have 'init' then continue checks
                                     if (!IsDefaultImplementation(getter) && init is null)
                                        return null;

                                     if (property.SetMethod is not null)
                                     {
                                        // there can be init, setter or none of them
                                        if (init is not null)
                                        {
                                           if (property.DeclaredAccessibility != Accessibility.Private
                                               && property.SetMethod.DeclaredAccessibility != Accessibility.Private)
                                           {
                                              ReportPropertyInitAccessorMustBePrivate(property);
                                           }
                                        }
                                        else if (setter is not null)
                                        {
                                           if (!IsDefaultImplementation(setter))
                                              return null;

                                           ReportPropertyMustBeReadOnly(property);
                                        }
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

   public static bool HasCreateInvalidItemImplementation(
      this ITypeSymbol enumType,
      ITypeSymbol keyType,
      CancellationToken cancellationToken,
      Action<Diagnostic>? reportDiagnostic = null)
   {
      foreach (var member in enumType.GetNonIgnoredMembers())
      {
         if (member is not IMethodSymbol { Name: Constants.Methods.CREATE_INVALID_ITEM } method)
            continue;

         if (method.Parameters is { IsDefaultOrEmpty: false, Length: 1 })
         {
            var parameterType = method.Parameters[0].Type;
            var returnType = method.ReturnType;

            if (member is { IsStatic: true, DeclaredAccessibility: Accessibility.Private } &&
                SymbolEqualityComparer.Default.Equals(parameterType, keyType) &&
                SymbolEqualityComparer.Default.Equals(returnType, enumType))
            {
               return true;
            }
         }

         reportDiagnostic?.Invoke(Diagnostic.Create(DiagnosticsDescriptors.InvalidSignatureOfCreateInvalidItem,
                                                    method.GetIdentifier(cancellationToken).GetLocation(),
                                                    enumType.Name,
                                                    keyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

         return true;
      }

      return false;
   }
}
