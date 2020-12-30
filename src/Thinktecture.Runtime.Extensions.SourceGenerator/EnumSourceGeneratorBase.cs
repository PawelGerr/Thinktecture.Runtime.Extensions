using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture
{
   /// <summary>
   /// Base class for source generator for enum-like classes.
   /// </summary>
   public abstract class EnumSourceGeneratorBase : ISourceGenerator
   {
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
            var enumInterface = GetValidEnumInterface(enumDeclaration, context, model);

            if (enumInterface != null)
            {
               var generatedCode = GenerateCode(enumDeclaration, enumInterface, context, model);
               context.AddSource($"{enumDeclaration.TypeDeclarationSyntax.Identifier}_Generated.cs", $@"
// <auto-generated />
#nullable enable

{generatedCode}
");
            }
         }
      }

      protected abstract string GenerateCode(EnumSourceGeneratorState state);

      protected virtual EnumInterfaceInfo? GetValidEnumInterface(
         EnumDeclaration enumDeclaration,
         GeneratorExecutionContext context,
         SemanticModel model)
      {
         if (enumDeclaration is null)
            throw new ArgumentNullException(nameof(enumDeclaration));

         EnumInterfaceInfo? validInterface = null;

         foreach (var enumInterface in enumDeclaration.EnumInterfaces)
         {
            var typeInfo = model.GetTypeInfo(enumInterface).Type;

            if (typeInfo is null)
               continue;

            if (typeInfo.ContainingNamespace.Name != "Thinktecture")
               continue;

            var type = enumInterface.TypeArgumentList.Arguments[0];
            var keyType = model.GetTypeInfo(type).Type;

            if (keyType is null)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeCouldNotBeResolved,
                                                          type.GetLocation(),
                                                          type));

               return null;
            }

            var candicate = new EnumInterfaceInfo(enumInterface, typeInfo, keyType, typeInfo.Name == "IValidatableEnum");

            if (validInterface == null)
            {
               validInterface = candicate;
            }
            else
            {
               if (!SymbolEqualityComparer.Default.Equals(validInterface.KeyType, candicate.KeyType))
               {
                  context.ReportTypeCouldNotBeResolved(enumDeclaration.TypeDeclarationSyntax);
                  return null;
               }

               if (candicate.IsValidatable)
                  validInterface = candicate;
            }
         }

         return validInterface;
      }

      private string GenerateCode(
         EnumDeclaration enumDeclaration,
         EnumInterfaceInfo enumInterfaceInfo,
         GeneratorExecutionContext context,
         SemanticModel model)
      {
         var classTypeInfo = model.GetDeclaredSymbol(enumDeclaration.TypeDeclarationSyntax);

         if (classTypeInfo is null)
            return String.Empty;

         var state = new EnumSourceGeneratorState(context, model, enumDeclaration, classTypeInfo, enumInterfaceInfo);

         return GenerateCode(state);
      }
   }
}
