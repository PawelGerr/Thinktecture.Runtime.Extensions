using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class TypeSymbolExtensions
{
   private static readonly SymbolDisplayFormat _fullyQualifiedDisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat
                                                                                                 .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier)
                                                                                                 .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.ExpandValueTuple);

   public static string ToFullyQualifiedDisplayString(this ITypeSymbol type)
   {
      return type.ToDisplayString(_fullyQualifiedDisplayFormat);
   }

   public static string ToMinimallyQualifiedDisplayString(this ITypeSymbol type)
   {
      return type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
   }

   public static bool IsNullOrDotnetBaseType([NotNullWhen(false)] this ITypeSymbol? type)
   {
      return type is null
             || type.SpecialType == SpecialType.System_Object
             || type.SpecialType == SpecialType.System_ValueType
             || type.SpecialType == SpecialType.System_Enum
             || type.SpecialType == SpecialType.System_MulticastDelegate;
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

   public static bool IsInternalsVisibleToAttribute([NotNullWhen(true)] this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is
      {
         Name: "InternalsVisibleToAttribute",
         ContainingNamespace:
         {
            Name: "CompilerServices",
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

   /// <summary>
   /// Keyed or keyless Smart Enum.
   /// </summary>
   public static bool IsSmartEnumType(
      [NotNullWhen(true)] this ITypeSymbol? type,
      [NotNullWhen(true)] out AttributeData? smartEnumAttribute)
   {
      if (type is null || type.SpecialType != SpecialType.None)
      {
         smartEnumAttribute = null;
         return false;
      }

      smartEnumAttribute = type.FindAttribute(static attributeClass => attributeClass.IsSmartEnumAttribute());

      return smartEnumAttribute is not null;
   }

   public static bool IsKeyedValueObjectAttribute([NotNullWhen(true)] this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.ValueObject.KEYED_NAME, ContainingNamespace: { Name: Constants.Attributes.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsComplexValueObjectAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.ValueObject.COMPLEX_NAME, ContainingNamespace: { Name: Constants.Attributes.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsSmartEnumAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.SmartEnum.NAME, ContainingNamespace: { Name: Constants.Attributes.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsObjectFactoryAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.ObjectFactory.NAME, ContainingNamespace: { Name: Constants.Attributes.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsAdHocUnionAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null
          || attributeType.TypeKind == TypeKind.Error
          || attributeType is not INamedTypeSymbol namedType)
         return false;

      return attributeType.ContainingNamespace is { Name: Constants.Attributes.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true }
             && (
                   attributeType.Name == Constants.Attributes.Union.NAME && namedType.Arity > 0
                   || attributeType.Name == Constants.Attributes.Union.NAME_AD_HOC && namedType.Arity == 0
                );
   }

   public static bool IsRegularUnionAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null
          || attributeType.TypeKind == TypeKind.Error
          || attributeType is not INamedTypeSymbol namedType
          || namedType.Arity != 0)
         return false;

      return attributeType is
      {
         Name: Constants.Attributes.Union.NAME,
         ContainingNamespace: { Name: Constants.Attributes.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true }
      };
   }

   public static bool IsAnyUnionAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is
      {
         Name: Constants.Attributes.Union.NAME or Constants.Attributes.Union.NAME_AD_HOC,
         ContainingNamespace: { Name: Constants.Attributes.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true }
      };
   }

   public static bool IsUnionSwitchMapOverloadAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.UnionSwitchMapOverload.NAME, ContainingNamespace: { Name: Constants.Attributes.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsThinktectureComponentAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is
      {
         Name: Constants.Attributes.SmartEnum.NAME
         or Constants.Attributes.ValueObject.KEYED_NAME
         or Constants.Attributes.ValueObject.COMPLEX_NAME
         or Constants.Attributes.Union.NAME         // both regular and generic ad-hoc
         or Constants.Attributes.Union.NAME_AD_HOC, // non-generic ad-hoc
         ContainingNamespace: { Name: Constants.Attributes.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true }
      };
   }

   public static bool IsMessagePackKeyAttribute([NotNullWhen(true)] this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: "KeyAttribute", ContainingNamespace: { Name: "MessagePack", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsJsonIgnoreAttribute([NotNullWhen(true)] this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is
      {
         Name: "JsonIgnoreAttribute",
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

   public static bool IsMemberEqualityComparerAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: "MemberEqualityComparerAttribute" or "ValueObjectMemberEqualityComparerAttribute", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsMemberIgnoreAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: "IgnoreMemberAttribute" or "ValueObjectMemberIgnoreAttribute", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsUseDelegateFromConstructorAttribute(this ITypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is { Name: Constants.Attributes.UseDelegateFromConstructor.NAME, ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } };
   }

   public static bool IsSmartEnum(
      [NotNullWhen(true)] this ITypeSymbol? enumType)
   {
      return IsSmartEnum(enumType, out _);
   }

   public static bool IsSmartEnum(
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
      return type is
      {
         Name: "MessagePackFormatterAttribute",
         ContainingNamespace: { Name: "MessagePack", ContainingNamespace.IsGlobalNamespace: true }
      };
   }

   public static bool IsObjectFactoryAttribute(this INamedTypeSymbol type)
   {
      return type is
      {
         Name: Constants.Attributes.ObjectFactory.NAME,
         TypeArguments.Length: 1,
         ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true }
      };
   }

   public static bool IsValidationErrorAttribute(this INamedTypeSymbol type)
   {
      return type is
      {
         Name: "ValidationErrorAttribute" or "ValueObjectValidationErrorAttribute",
         TypeArguments.Length: 1,
         ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true }
      };
   }

   public static bool IsKeyMemberComparerAttribute(this INamedTypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is
      {
         Name: "KeyMemberComparerAttribute" or "ValueObjectKeyMemberComparerAttribute",
         TypeArguments.Length: 2,
         ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true }
      };
   }

   public static bool IsKeyMemberEqualityComparerAttribute(this INamedTypeSymbol? attributeType)
   {
      if (attributeType is null || attributeType.TypeKind == TypeKind.Error)
         return false;

      return attributeType is
      {
         Name: "KeyMemberEqualityComparerAttribute" or "ValueObjectKeyMemberEqualityComparerAttribute",
         TypeArguments.Length: 2,
         ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true }
      };
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
                        if (!m.IsStatic || m is not IFieldSymbol field || field.IsImplicitlyDeclared) // skip backing fields
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

   public static INamedTypeSymbol GetGenericTypeDefinition(
      this INamedTypeSymbol type)
   {
      return type is { IsGenericType: true, IsUnboundGenericType: false }
                ? type.ConstructUnboundGenericType()
                : type;
   }

   public static IReadOnlyList<DerivedTypeInfo> FindDerivedInnerTypes(
      this INamedTypeSymbol baseType)
   {
      List<DerivedTypeInfo>? derivedTypes = null;

      FindDerivedInnerTypes(
         baseType,
         0,
         (baseType, baseType.GetGenericTypeDefinition()),
         ref derivedTypes);

      return derivedTypes ?? (IReadOnlyList<DerivedTypeInfo>)[];
   }

   private static void FindDerivedInnerTypes(
      INamedTypeSymbol typeToCheck,
      int currentLevel,
      (INamedTypeSymbol Type, INamedTypeSymbol TypeDef) baseType,
      ref List<DerivedTypeInfo>? derivedTypes)
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
            (derivedTypes ??= []).Add(new(innerType, innerType.GetGenericTypeDefinition(), currentLevel));
         }

         FindDerivedInnerTypes(innerType, currentLevel, baseType, ref derivedTypes);
      }
   }

   private static bool IsDerivedFrom(
      this ITypeSymbol? type,
      (INamedTypeSymbol Type, INamedTypeSymbol TypeDef) baseType)
   {
      while (!type.IsNullOrDotnetBaseType())
      {
         if (baseType.Type.TypeKind == TypeKind.Interface)
         {
            foreach (var @interface in type.Interfaces)
            {
               if (SymbolEqualityComparer.Default.Equals(@interface, baseType.Type))
                  return true;
            }
         }
         else if (SymbolEqualityComparer.Default.Equals(type.BaseType?.GetGenericTypeDefinition(), baseType.TypeDef))
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
            location = field.GetFieldLocation(cancellationToken);
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
            location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, cancellationToken);
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
         var location = property.GetPropertyLocation(PropertyDeclarationSyntaxKind.All, cancellationToken);

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
                          if (field.IsConst || field.IsIgnored())
                             return null;

                          if (field is { IsReadOnly: false })
                             ReportField(field);

                          return (field, null);
                       }

                       case IPropertySymbol property:
                       {
                          if (property.IsIgnored())
                             return null;

                          // Skip EqualityContract property from records
                          if (property.Name == "EqualityContract" && property.IsVirtual && property.ContainingType.IsRecord)
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

                             // public int Foo => 42; (exclude) OR public int Foo => field; (include)
                             if (syntax.ExpressionBody is not null)
                             {
                                var hasFieldKeyword = syntax.ExpressionBody.DescendantTokens().Any(t => t.IsKind(SyntaxKind.FieldKeyword) || (t.IsKind(SyntaxKind.IdentifierToken) && t.ValueText == "field"));

                                if (!hasFieldKeyword)
                                   return null;

                                // Has field keyword in expression body, include it
                                return (null, property);
                             }

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
                             // If we have 'init' or use the keyword "field" ({ get => field; }) then continue checks
                             if (!IsDefaultAccessorOrWithFieldKeyword(getter) && init is null)
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
                                   if (!IsDefaultAccessorOrWithFieldKeyword(setter))
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

   private static bool IsDefaultAccessorOrWithFieldKeyword(AccessorDeclarationSyntax? accessor)
   {
      if (accessor is null)
         return false;

      // Check for field keyword in regular body (if present)
      var hasBodyWithField = accessor.Body is not null && accessor.Body.DescendantTokens().Any(t => t.IsKind(SyntaxKind.FieldKeyword) || (t.IsKind(SyntaxKind.IdentifierToken) && t.ValueText == "field"));

      // Check for field keyword in expression body (if present)
      var hasExpressionBodyWithField = accessor.ExpressionBody is not null && accessor.ExpressionBody.DescendantTokens().Any(t => t.IsKind(SyntaxKind.FieldKeyword) || (t.IsKind(SyntaxKind.IdentifierToken) && t.ValueText == "field"));

      // Check for default accessor (no body and no expression body - auto-implemented)
      var isDefaultAccessor = accessor.Body is null && accessor.ExpressionBody is null;

      return isDefaultAccessor || hasBodyWithField || hasExpressionBodyWithField;
   }

   public static ImmutableArray<DelegateMethodState> GetDelegateMethods(
      this INamedTypeSymbol typeSymbol)
   {
      ImmutableArray<DelegateMethodState>.Builder? methodStates = null;

      foreach (var member in typeSymbol.GetMembers())
      {
         if (member is not IMethodSymbol methodSymbol)
            continue;

         if (!methodSymbol.IsPartialDefinition)
            continue;

         if (!methodSymbol.TypeParameters.IsDefaultOrEmpty)
            continue;

         var useDelegateFromConstructorAttribute = methodSymbol.FindAttribute(a => a.IsUseDelegateFromConstructorAttribute());

         if (useDelegateFromConstructorAttribute == null)
            continue;

         var methodName = methodSymbol.Name;
         var returnType = methodSymbol.ReturnType.SpecialType == SpecialType.System_Void
                             ? null
                             : methodSymbol.ReturnType.ToFullyQualifiedDisplayString();

         var parameters = methodSymbol.Parameters.Length == 0
                             ? []
                             : ImmutableArray.CreateRange(methodSymbol.Parameters,
                                                          static p => new ParameterState(
                                                             p.Name,
                                                             p.Type.ToFullyQualifiedDisplayString(),
                                                             p.RefKind));

         var customDelegateName = useDelegateFromConstructorAttribute.FindDelegateName();

         var methodState = new DelegateMethodState(
            methodSymbol.DeclaredAccessibility,
            methodName,
            returnType,
            parameters,
            customDelegateName);

         (methodStates ??= ImmutableArray.CreateBuilder<DelegateMethodState>()).Add(methodState);
      }

      return methodStates?.DrainToImmutable() ?? [];
   }

   public static bool HasRequiredMembers(this INamedTypeSymbol type)
   {
      var typeToCheck = type;

      while (typeToCheck is not null)
      {
         var members = typeToCheck.GetMembers();

         for (var i = 0; i < members.Length; i++)
         {
            switch (members[i])
            {
               case IPropertySymbol { IsRequired: true }:
               case IFieldSymbol { IsRequired: true }:
                  return true;
            }
         }

         typeToCheck = typeToCheck.BaseType;
      }

      return false;
   }

   public static bool TryBuildMemberName(
      this INamedTypeSymbol type,
      [MaybeNullWhen(false)] out string name)
   {
      if (type.TypeKind == TypeKind.Error)
      {
         name = null;
         return false;
      }

      if (!type.IsGenericType)
      {
         if (type.Name.Length == 0)
         {
            name = null;
            return false;
         }

         name = type.Name;
         return true;
      }

      var typeArguments = type.TypeArguments;

      // Perf optimization
      if (typeArguments.Length == 1)
      {
         if (!typeArguments[0].TryBuildMemberName(out var typeArgumentName))
         {
            name = null;
            return false;
         }

         name = $"{type.Name}Of{typeArgumentName}";
         return true;
      }

      var typeArgumentNames = new string[typeArguments.Length];

      for (var i = 0; i < typeArguments.Length; i++)
      {
         var typeArgument = typeArguments[i];

         if (!typeArgument.TryBuildMemberName(out var typeArgumentName))
         {
            name = null;
            return false;
         }

         typeArgumentNames[i] = typeArgumentName;
      }

      name = $"{type.Name}Of{String.Join(String.Empty, typeArgumentNames)}";
      return true;
   }

   public static bool TryBuildMemberName(
      this IArrayTypeSymbol type,
      [MaybeNullWhen(false)] out string name)
   {
      if (!type.ElementType.TryBuildMemberName(out var elementName))
      {
         name = null;
         return false;
      }

      name = type.Rank switch
      {
         1 => $"{elementName}Array",
         _ => $"{elementName}Array{type.Rank}D"
      };
      return true;
   }

   public static bool TryBuildMemberName(
      this ITypeParameterSymbol type,
      [MaybeNullWhen(false)] out string name)
   {
      name = type.Name;
      return true;
   }

   public static bool TryBuildMemberName(
      this ITypeSymbol type,
      [MaybeNullWhen(false)] out string name)
   {
      name = null;

      return type switch
      {
         IArrayTypeSymbol arrayTypeSymbol => TryBuildMemberName(arrayTypeSymbol, out name),
         INamedTypeSymbol namedTypeSymbol => TryBuildMemberName(namedTypeSymbol, out name),
         ITypeParameterSymbol typeParameterSymbol => TryBuildMemberName(typeParameterSymbol, out name),
         _ => false
      };
   }

   public static bool IsIDisallowDefaultValue(this ITypeSymbol? type)
   {
      if (type is not INamedTypeSymbol namedType)
         return false;

      return namedType is
      {
         TypeKind: TypeKind.Interface,
         Arity: 0,             // not generic
         ContainingType: null, // not nested
         Name: Constants.Interfaces.IDisallowDefaultValue.NAME,
         ContainingNamespace: { Name: Constants.Interfaces.IDisallowDefaultValue.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true }
      };
   }

   public static bool ImplementsIDisallowDefaultValue(this ITypeSymbol type)
   {
      for (var i = 0; i < type.AllInterfaces.Length; i++)
      {
         var @interface = type.AllInterfaces[i];

         if (@interface.IsIDisallowDefaultValue())
            return true;
      }

      return false;
   }
}
