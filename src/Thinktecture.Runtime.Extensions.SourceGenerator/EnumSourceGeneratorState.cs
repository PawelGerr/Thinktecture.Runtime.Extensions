using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   public class EnumSourceGeneratorState
   {
      public GeneratorExecutionContext Context { get; }
      public SemanticModel Model { get; }

      public TypeDeclarationSyntax EnumSyntax { get; }
      public GenericNameSyntax EnumInterfaceSyntax { get; }

      public string? Namespace { get; }
      public INamedTypeSymbol EnumType { get; }
      public ITypeSymbol KeyType { get; }
      public bool IsKeyARefType => KeyType.TypeKind != TypeKind.Struct;
      public bool IsEnumARefType => EnumType.TypeKind != TypeKind.Struct;
      public SyntaxToken EnumIdentifier => EnumSyntax.Identifier;

      public string KeyPropertyName { get; }
      public string KeyArgumentName { get; }
      public string KeyComparerMember { get; }
      public bool NeedsDefaultComparer { get; }
      public bool IsValidatable { get; }

      public string? NullableQuestionMarkEnum { get; }
      public string? NullableQuestionMarkKey { get; }

      private IReadOnlyList<FieldDeclarationSyntax>? _items;
      public IReadOnlyList<FieldDeclarationSyntax> Items => _items ??= GetItems();

      private IReadOnlyList<EnumMemberInfo>? _assignableInstanceFieldsAndProperties;
      public IReadOnlyList<EnumMemberInfo> AssignableInstanceFieldsAndProperties => _assignableInstanceFieldsAndProperties ??= GetAssignableInstanceFieldsAndProperties();

      public AttributeSyntax? EnumSettings { get; }

      public EnumSourceGeneratorState(
         GeneratorExecutionContext context,
         SemanticModel model,
         EnumDeclaration enumDeclaration,
         INamedTypeSymbol enumType,
         EnumInterfaceInfo enumInterfaceInfo)
      {
         if (enumDeclaration is null)
            throw new ArgumentNullException(nameof(enumDeclaration));
         if (enumInterfaceInfo is null)
            throw new ArgumentNullException(nameof(enumInterfaceInfo));

         Context = context;
         Model = model ?? throw new ArgumentNullException(nameof(model));

         EnumSyntax = enumDeclaration.TypeDeclarationSyntax;
         EnumInterfaceSyntax = enumInterfaceInfo.Syntax;

         EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
         Namespace = enumType.ContainingNamespace.ToString();
         KeyType = enumInterfaceInfo.KeyType;
         IsValidatable = enumInterfaceInfo.IsValidatable;

         NullableQuestionMarkEnum = IsEnumARefType ? "?" : null;
         NullableQuestionMarkKey = IsKeyARefType ? "?" : null;

         EnumSettings = enumDeclaration.TypeDeclarationSyntax.AttributeLists.SelectMany(a => a.Attributes).FirstOrDefault(a => ModelExtensions.GetTypeInfo(model, a).Type?.ToString() == "Thinktecture.EnumGenerationAttribute");

         KeyComparerMember = GetKeyComparerMember(model, EnumSettings, out var needsDefaultComparer);
         KeyPropertyName = GetKeyPropertyName(model, EnumSettings);
         KeyArgumentName = MakeArgumentName(KeyPropertyName);
         NeedsDefaultComparer = needsDefaultComparer;
      }

      private static string MakeArgumentName(string name)
      {
         return $"{Char.ToLowerInvariant(name[0])}{name.Substring(1)}";
      }

      private IReadOnlyList<EnumMemberInfo> GetAssignableInstanceFieldsAndProperties()
      {
         return EnumSyntax.Members
                          .Select(m =>
                                  {
                                     if (m.IsStatic())
                                        return null;

                                     EnumMemberInfo? member = null;

                                     if (m is FieldDeclarationSyntax fds)
                                     {
                                        if (!m.IsReadOnly())
                                        {
                                           Context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                      fds.GetLocation(),
                                                                                      fds.Declaration.Variables[0].Identifier,
                                                                                      EnumIdentifier));
                                        }

                                        var identifier = fds.Declaration.Variables[0].Identifier;
                                        var type = Model.GetTypeInfo(fds.Declaration.Type).Type;

                                        if (type is null)
                                        {
                                           Context.ReportTypeCouldNotBeResolved(fds.Declaration.Type, identifier);
                                           return null;
                                        }

                                        member = new EnumMemberInfo(fds, type, identifier, MakeArgumentName(identifier.ToString()));
                                     }

                                     if (m is PropertyDeclarationSyntax pds && pds.AccessorList is not null)
                                     {
                                        var getter = pds.AccessorList.Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword));

                                        if (getter?.Body is not null)
                                           return null;

                                        if (pds.AccessorList.Accessors.Any(a => a.Keyword.IsKind(SyntaxKind.SetKeyword)))
                                        {
                                           Context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.PropertyMustBeReadOnly,
                                                                                      pds.GetLocation(),
                                                                                      pds.Identifier,
                                                                                      EnumIdentifier));
                                        }

                                        var type = Model.GetTypeInfo(pds.Type).Type;

                                        if (type is null)
                                        {
                                           Context.ReportTypeCouldNotBeResolved(pds.Type, pds.Identifier);
                                           return null;
                                        }

                                        member = new EnumMemberInfo(pds, type, pds.Identifier, MakeArgumentName(pds.Identifier.ToString()));
                                     }

                                     return member;
                                  })
                          .Where(m => m is not null)
                          .ToList()!;
      }

      private IReadOnlyList<FieldDeclarationSyntax> GetItems()
      {
         return EnumSyntax.Members
                          .Select(m =>
                                  {
                                     if (m.IsStatic() && m is FieldDeclarationSyntax fds)
                                     {
                                        var fieldTypeInfo = ModelExtensions.GetTypeInfo(Model, fds.Declaration.Type).Type;

                                        if (SymbolEqualityComparer.Default.Equals(fieldTypeInfo, EnumType))
                                        {
                                           if (!m.IsPublic())
                                           {
                                              Context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.FieldMustBePublic,
                                                                                         fds.GetLocation(),
                                                                                         fds.Declaration.Variables[0].Identifier,
                                                                                         EnumIdentifier));
                                              return null;
                                           }

                                           if (!m.IsReadOnly())
                                           {
                                              Context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                         fds.GetLocation(),
                                                                                         fds.Declaration.Variables[0].Identifier,
                                                                                         EnumIdentifier));

                                              return null;
                                           }

                                           return fds;
                                        }
                                     }

                                     return null;
                                  })
                          .Where(fds => fds is not null)
                          .ToList()!;
      }

      private static string GetKeyComparerMember(SemanticModel model, AttributeSyntax? enumSettingsAttribute, out bool needsDefaultComparer)
      {
         var comparerMemberName = GetParameterValue(model, enumSettingsAttribute, "KeyComparerProvidingMember");

         needsDefaultComparer = comparerMemberName is null;
         return comparerMemberName ?? "_defaultKeyComparerMember";
      }

      private string GetKeyPropertyName(SemanticModel model, AttributeSyntax? enumSettingsAttribute)
      {
         var name = GetParameterValue(model, enumSettingsAttribute, "KeyPropertyName");

         if (name is not null)
         {
            if (!StringComparer.OrdinalIgnoreCase.Equals(name, "item"))
               return name;

            Context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyPropertyNameNotAllowed,
                                                       EnumSyntax.GetLocation(),
                                                       name));
         }

         return "Key";
      }

      private static string? GetParameterValue(SemanticModel model, AttributeSyntax? enumSettingsAttribute, string parameterName)
      {
         var keyName = enumSettingsAttribute?.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.Identifier.ToString() == parameterName);

         if (keyName is not null)
         {
            var value = model.GetConstantValue(keyName.Expression);

            if (value.HasValue && value.Value is string name && !String.IsNullOrWhiteSpace(name))
               return name.Trim();
         }

         return null;
      }

      public IReadOnlyList<TypeDeclarationSyntax> FindDerivedTypes()
      {
         var types = new List<TypeDeclarationSyntax>();

         FindDerivedTypes(types, EnumSyntax);

         return types;
      }

      private void FindDerivedTypes(
         List<TypeDeclarationSyntax> types,
         TypeDeclarationSyntax typeToAnalyse)
      {
         foreach (var member in typeToAnalyse.Members)
         {
            if (member is TypeDeclarationSyntax innerClass)
            {
               if (innerClass.BaseList?.Types.Count > 0)
               {
                  foreach (var baseType in innerClass.BaseList.Types)
                  {
                     var baseTypeInfo = ModelExtensions.GetTypeInfo(Model, baseType.Type).Type;

                     if (SymbolEqualityComparer.Default.Equals(baseTypeInfo, EnumType))
                        types.Add(innerClass);
                  }
               }

               FindDerivedTypes(types, innerClass);
            }
         }
      }
   }
}
