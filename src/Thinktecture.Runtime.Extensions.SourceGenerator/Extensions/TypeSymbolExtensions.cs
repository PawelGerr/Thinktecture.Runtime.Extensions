using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class TypeSymbolExtensions
{
   private static readonly SymbolDisplayFormat _fullyQualifiedDisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat
                                                                                                 .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

   public static string ToFullyQualifiedDisplayString(this ITypeSymbol type)
   {
      return type.ToDisplayString(_fullyQualifiedDisplayFormat);
   }

   public static bool IsNullOrObject([NotNullWhen(false)] this ITypeSymbol? type)
   {
      return type is null || type.SpecialType == SpecialType.System_Object;
   }

   /// <summary>
   /// Keyed or complex value object.
   /// </summary>
   public static bool IsValueObjectType(
      [NotNullWhen(true)] this ITypeSymbol? valueObjectType,
      [NotNullWhen(true)] out AttributeData? valueObjectAttributeBase)
   {
      if (valueObjectType is null || valueObjectType.SpecialType != SpecialType.None)
      {
         valueObjectAttributeBase = null;
         return false;
      }

      valueObjectAttributeBase = valueObjectType.FindAttribute(static attributeClass => attributeClass.IsKeyedValueObjectAttribute()
                                                                                        || attributeClass.IsComplexValueObjectAttribute());

      return valueObjectAttributeBase is not null;
   }

   public static bool IsKeyedValueObjectAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.ValueObject.KEYED_NAME, ContainingNamespace: { Name: Constants.Attributes.ValueObject.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsComplexValueObjectAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.ValueObject.COMPLEX_NAME, ContainingNamespace: { Name: Constants.Attributes.ValueObject.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsSmartEnumAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.SmartEnum.NAME, ContainingNamespace: { Name: Constants.Attributes.SmartEnum.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsAdHocUnionAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null
          || attributeType.TypeKind == TypeKind.Error
          || attributeType is not INamedTypeSymbol namedType
          || namedType.Arity == 0)
         return false;

      return attributeType is { Name: Constants.Attributes.Union.NAME, ContainingNamespace: { Name: Constants.Attributes.Union.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsUnionAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null
          || attributeType.TypeKind == TypeKind.Error
          || attributeType is not INamedTypeSymbol namedType
          || namedType.Arity != 0)
         return false;

      return attributeType is { Name: Constants.Attributes.Union.NAME, ContainingNamespace: { Name: Constants.Attributes.Union.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsAnyUnionAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null
          || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.Union.NAME, ContainingNamespace: { Name: Constants.Attributes.Union.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsAdHocUnionType(
      [NotNullWhen(true)] this ITypeSymbol? unionType,
      [NotNullWhen(true)] out AttributeData? unionAttribute)
   {
      if (unionType is null || unionType.SpecialType != SpecialType.None)
      {
         unionAttribute = null;
         return false;
      }

      unionAttribute = unionType.FindAttribute(static attributeClass => attributeClass.IsAdHocUnionAttribute());

      return unionAttribute is not null;
   }

   public static bool IsAnyUnionType(
      [NotNullWhen(true)] this ITypeSymbol? unionType,
      [NotNullWhen(true)] out AttributeData? unionAttribute)
   {
      if (unionType is null || unionType.SpecialType != SpecialType.None)
      {
         unionAttribute = null;
         return false;
      }

      unionAttribute = unionType.FindAttribute(static attributeClass => attributeClass.IsAnyUnionAttribute());

      return unionAttribute is not null;
   }

   public static bool IsValueObjectMemberEqualityComparerAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: "ValueObjectMemberEqualityComparerAttribute", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsValueObjectMemberIgnoreAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: "ValueObjectMemberIgnoreAttribute", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsUseDelegateFromConstructorAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: "UseDelegateFromConstructorAttribute", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsEnum(
      [NotNullWhen(true)] this ITypeSymbol? enumType)
   {
      return IsEnum(enumType, out _);
   }

   public static bool IsEnum(
      [NotNullWhen(true)] this ITypeSymbol? enumType,
      [NotNullWhen(true)] out AttributeData? smartEnumAttribute)
   {
      if (enumType is null || enumType.SpecialType != SpecialType.None)
      {
         smartEnumAttribute = null;
         return false;
      }

      smartEnumAttribute = enumType.FindAttribute(static attributeClass => attributeClass.IsSmartEnumAttribute());

      return smartEnumAttribute is not null;
   }

   public static bool IsMessagePackFormatterAttribute(this ITypeSymbol type)
   {
      return type is { Name: "MessagePackFormatterAttribute", ContainingNamespace: { Name: "MessagePack", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsValueObjectFactoryAttribute(this INamedTypeSymbol type)
   {
      return type is { Name: "ValueObjectFactoryAttribute", TypeArguments.Length: 1, ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsValueObjectValidationErrorAttribute(this INamedTypeSymbol type)
   {
      return type is { Name: "ValueObjectValidationErrorAttribute", TypeArguments.Length: 1, ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsValueObjectKeyMemberComparerAttribute(this INamedTypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: "ValueObjectKeyMemberComparerAttribute", TypeArguments.Length: 2, ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsValueObjectKeyMemberEqualityComparerAttribute(this INamedTypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: "ValueObjectKeyMemberEqualityComparerAttribute", TypeArguments.Length: 2, ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
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

   public static ImmutableArray<IFieldSymbol> GetEnumItems(this ITypeSymbol enumType)
   {
      return enumType.GetMembers()
                     .SelectWhere(static (ISymbol m, ITypeSymbol type, [MaybeNullWhen(false)] out IFieldSymbol result) =>
                     {
                        if (!m.IsStatic || m is not IFieldSymbol field || field.IsPropertyBackingField())
                        {
                           result = null;
                           return false;
                        }

                        if (SymbolEqualityComparer.Default.Equals(field.Type, type)
                            && !field.IsIgnored())
                        {
                           result = field;
                           return true;
                        }

                        result = null;
                        return false;
                     }, enumType);
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

   public static IReadOnlyList<(INamedTypeSymbol Type, int Level)> FindDerivedInnerTypes(
      this ITypeSymbol baseType)
   {
      List<(INamedTypeSymbol, int Level)>? derivedTypes = null;

      FindDerivedInnerTypes(baseType, 0, baseType, ref derivedTypes);

      return derivedTypes ?? (IReadOnlyList<(INamedTypeSymbol Type, int Level)>)Array.Empty<(INamedTypeSymbol Type, int Level)>();
   }

   private static void FindDerivedInnerTypes(
      ITypeSymbol typeToCheck,
      int currentLevel,
      ITypeSymbol baseType,
      ref List<(INamedTypeSymbol, int Level)>? derivedTypes)
   {
      currentLevel++;

      var types = typeToCheck.GetTypeMembers();

      if (types.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < types.Length; i++)
      {
         var innerType = types[i];

         // derived types can be classes only
         if (innerType.TypeKind is not TypeKind.Class)
            continue;

         if (IsDerivedFrom(innerType, baseType))
         {
            var derivedType = innerType;

            if (derivedType is { IsGenericType: true, IsUnboundGenericType: false })
               derivedType = derivedType.ConstructUnboundGenericType();

            (derivedTypes ??= []).Add((derivedType, currentLevel));
         }

         FindDerivedInnerTypes(innerType, currentLevel, baseType, ref derivedTypes);
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

   public static IEnumerable<InstanceMemberInfo> GetAssignableFieldsAndPropertiesAndCheckForReadOnly(
      this ITypeSymbol type,
      TypedMemberStateFactory factory,
      bool instanceMembersOnly,
      bool populateValueObjectMemberSettings,
      CancellationToken cancellationToken,
      OperationAnalysisContext? reportDiagnostic = null)
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
                 .Where(m => m is not null)!;
   }

   public static IEnumerable<(IFieldSymbol? Field, IPropertySymbol? Property)> IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(
      this ITypeSymbol type,
      bool instanceMembersOnly,
      CancellationToken cancellationToken,
      Location? locationOfDerivedType = null,
      OperationAnalysisContext? reportDiagnostic = null)
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

         reportDiagnostic.Value.ReportDiagnostic(Diagnostic.Create(descriptor, location, field.Name, type.Name));
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

         reportDiagnostic.Value.ReportDiagnostic(Diagnostic.Create(descriptor, location, effectiveSeverity: severity ?? descriptor.DefaultSeverity, null, null, messageArgs: [property.Name, type.Name]));
      }

      void ReportPropertyInitAccessorMustBePrivate(IPropertySymbol property)
      {
         if (reportDiagnostic is null)
            return;

         var descriptor = DiagnosticsDescriptors.InitAccessorMustBePrivate;
         var location = property.GetIdentifier(cancellationToken)?.GetLocation() ?? Location.None;

         reportDiagnostic.Value.ReportDiagnostic(Diagnostic.Create(descriptor, location, effectiveSeverity: descriptor.DefaultSeverity, null, null, messageArgs: [property.Name, type.Name]));
      }

      return type.GetMembers()
             .Select(member =>
             {
                if ((instanceMembersOnly && member.IsStatic) || !member.CanBeReferencedByName)
                   return ((IFieldSymbol?, IPropertySymbol?)?)null;

                switch (member)
                {
                   case IFieldSymbol field:
                   {
                      if (field.IsIgnored())
                         return null;

                      if (!field.IsReadOnly && !field.IsConst)
                         ReportField(field);

                      return (field, null);
                   }

                   case IPropertySymbol property:
                   {
                      if (property.IsIgnored())
                         return null;

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
      OperationAnalysisContext? reportDiagnostic = null)
   {
      var members = enumType.GetMembers(Constants.Methods.CREATE_INVALID_ITEM);

      foreach (var member in members)
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

         reportDiagnostic?.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.InvalidSignatureOfCreateInvalidItem,
                                                              method.GetIdentifier(cancellationToken).GetLocation(),
                                                              enumType.Name,
                                                              keyType.ToFullyQualifiedDisplayString()));

         return true;
      }

      return false;
   }

   public static IReadOnlyList<DelegateMethodState> GetDelegateMethods(
      this INamedTypeSymbol typeSymbol)
   {
      List<DelegateMethodState>? methodStates = null;

      foreach (var member in typeSymbol.GetMembers())
      {
         if (member is not IMethodSymbol methodSymbol)
            continue;

         if (!methodSymbol.IsPartialDefinition)
            continue;

         var useDelegateFromConstructorAttribute = methodSymbol.FindAttribute(a => a.IsUseDelegateFromConstructorAttribute());

         if (useDelegateFromConstructorAttribute == null)
            continue;

         var methodName = methodSymbol.Name;
         var returnType = methodSymbol.ReturnType.SpecialType == SpecialType.System_Void
                             ? null
                             : methodSymbol.ReturnType.ToFullyQualifiedDisplayString();

         var parameters = methodSymbol.Parameters.Length == 0
                             ? (IReadOnlyList<DelegateMethodState.ParameterState>) []
                             : methodSymbol.Parameters
                                           .Select(p => new DelegateMethodState.ParameterState(
                                                      p.Name,
                                                      p.Type.ToFullyQualifiedDisplayString(),
                                                      p.RefKind))
                                           .ToList();

         var methodState = new DelegateMethodState(methodSymbol.DeclaredAccessibility, methodName, returnType, parameters);

         (methodStates ??= []).Add(methodState);
      }

      return methodStates ?? (IReadOnlyList<DelegateMethodState>) [];
   }
}
