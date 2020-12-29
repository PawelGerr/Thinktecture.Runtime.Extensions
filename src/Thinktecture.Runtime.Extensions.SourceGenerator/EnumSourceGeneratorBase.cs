using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   /// <summary>
   /// Base class for source generator for enum-like classes.
   /// </summary>
   public abstract class EnumSourceGeneratorBase : ISourceGenerator
   {
      private static readonly DiagnosticDescriptor _fieldMustBeReadOnly = new("TTRESG001", "Field must be read-only", "The field '{0}' of the class '{1}' must be read-only", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);
      private static readonly DiagnosticDescriptor _fieldMustBePublic = new("TTRESG002", "Field must be public", "The field '{0}' of the class '{1}' must be public", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);

      /// <inheritdoc />
      public void Initialize(GeneratorInitializationContext context)
      {
         context.RegisterForSyntaxNotifications(() => new EnumSyntaxReceiver());
      }

      /// <inheritdoc />
      public void Execute(GeneratorExecutionContext context)
      {
         var receiver = (EnumSyntaxReceiver)(context.SyntaxReceiver ?? throw new Exception($"Syntax receiver must be of type '{nameof(EnumSyntaxReceiver)}' but found '{context.SyntaxReceiver?.GetType().Name}'."));

         foreach (var enumDeclaration in receiver.Enums)
         {
            var model = context.Compilation.GetSemanticModel(enumDeclaration.TypeDeclarationSyntax.SyntaxTree, true);

            if (IsValid(enumDeclaration, context, model))
            {
               var generatedCode = GenerateCode(enumDeclaration, context, model);
               context.AddSource($"{enumDeclaration.TypeDeclarationSyntax.Identifier}_Generated.cs", $@"
// <auto-generated />
#nullable enable

{generatedCode}
");
            }
         }
      }

      protected abstract string GenerateCode(EnumSourceGeneratorState state);

      protected virtual bool IsValid(
         EnumDeclaration enumDeclaration,
         GeneratorExecutionContext context,
         SemanticModel model)
      {
         if (enumDeclaration is null)
            throw new ArgumentNullException(nameof(enumDeclaration));

         var typeInfo = model.GetTypeInfo(enumDeclaration.BaseType).Type;

         if (typeInfo is null)
            return false;

         if (typeInfo.ContainingNamespace.Name != "Thinktecture")
            return false;

         return true;
      }

      private string GenerateCode(
         EnumDeclaration enumDeclaration,
         GeneratorExecutionContext context,
         SemanticModel model)
      {
         var classTypeInfo = model.GetDeclaredSymbol(enumDeclaration.TypeDeclarationSyntax);

         if (classTypeInfo is null)
            return String.Empty;

         var keyType = GetKeyType(enumDeclaration.BaseType, model);

         if (keyType is null)
            return String.Empty;

         var items = GetItems(enumDeclaration, context, model, classTypeInfo);
         var state = new EnumSourceGeneratorState(context, model, enumDeclaration, classTypeInfo, keyType, items);

         return GenerateCode(state);
      }

      private static IReadOnlyList<FieldDeclarationSyntax> GetItems(
         EnumDeclaration enumDeclaration,
         GeneratorExecutionContext context,
         SemanticModel model,
         INamedTypeSymbol classTypeInfo)
      {
         return enumDeclaration.TypeDeclarationSyntax.Members
                               .Select(m =>
                                       {
                                          if (m.IsStatic() && m is FieldDeclarationSyntax fds)
                                          {
                                             var fieldTypeInfo = model.GetTypeInfo(fds.Declaration.Type).Type;

                                             if (SymbolEqualityComparer.Default.Equals(fieldTypeInfo, classTypeInfo))
                                             {
                                                if (!m.IsPublic())
                                                {
                                                   context.ReportDiagnostic(Diagnostic.Create(_fieldMustBePublic,
                                                                                              fds.GetLocation(),
                                                                                              fds.Declaration.Variables[0].Identifier,
                                                                                              enumDeclaration.TypeDeclarationSyntax.Identifier));
                                                   return null;
                                                }

                                                if (!m.IsReadOnly())
                                                {
                                                   context.ReportDiagnostic(Diagnostic.Create(_fieldMustBeReadOnly,
                                                                                              fds.GetLocation(),
                                                                                              fds.Declaration.Variables[0].Identifier,
                                                                                              enumDeclaration.TypeDeclarationSyntax.Identifier));

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

      private static ITypeSymbol? GetKeyType(GenericNameSyntax enumBaseType, SemanticModel model)
      {
         var type = enumBaseType.TypeArgumentList.Arguments[0];
         return model.GetTypeInfo(type).Type;
      }
   }
}
