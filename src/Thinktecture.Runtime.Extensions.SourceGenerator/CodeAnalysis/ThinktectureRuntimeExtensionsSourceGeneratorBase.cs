using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis
{
   /// <summary>
   /// Base class for source generator for enum-like classes.
   /// </summary>
   public abstract class ThinktectureRuntimeExtensionsSourceGeneratorBase : ISourceGenerator
   {
      /// <inheritdoc />
      public void Initialize(GeneratorInitializationContext context)
      {
         context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
      }

      /// <inheritdoc />
      public void Execute(GeneratorExecutionContext context)
      {
         var receiver = (SyntaxReceiver)(context.SyntaxReceiver ?? throw new Exception($"Syntax receiver must be of type '{nameof(SyntaxReceiver)}' but found '{context.SyntaxReceiver?.GetType().Name}'."));

         foreach (var declaration in receiver.Enums)
         {
            GenerateEnum(context, declaration);
         }

         foreach (var declaration in receiver.ValueTypes)
         {
            GenerateValueType(context, declaration);
         }
      }

      private void GenerateValueType(GeneratorExecutionContext context, TypeDeclarationSyntax declaration)
      {
         var model = context.Compilation.GetSemanticModel(declaration.SyntaxTree, true);
         var type = model.GetDeclaredSymbol(declaration);

         if (type is null)
            return;

         if (!type.HasValueTypeAttribute(out var valueTypeAttribute))
            return;

         if (type.ContainingType is not null)
            return;

         var state = new ValueTypeSourceGeneratorState(model, declaration, type, valueTypeAttribute);
         var generatedCode = GenerateValueType(state);

         if (!String.IsNullOrWhiteSpace(generatedCode))
         {
            context.AddSource($"{declaration.Identifier}_Generated.cs", $@"// <auto-generated />
#nullable enable
{generatedCode}");
         }
      }

      private void GenerateEnum(GeneratorExecutionContext context, TypeDeclarationSyntax declaration)
      {
         var model = context.Compilation.GetSemanticModel(declaration.SyntaxTree, true);
         var type = model.GetDeclaredSymbol(declaration);

         if (type is null)
            return;

         if (!type.IsEnum(out var enumInterfaces))
            return;

         if (type.ContainingType is not null)
            return;

         var enumInterface = enumInterfaces.GetValidEnumInterface(type);

         if (enumInterface is not null)
         {
            var state = new EnumSourceGeneratorState(model, declaration, type, enumInterface);
            var generatedCode = GenerateEnum(state);

            if (!String.IsNullOrWhiteSpace(generatedCode))
            {
               context.AddSource($"{declaration.Identifier}_Generated.cs", $@"// <auto-generated />
#nullable enable
{generatedCode}");
            }
         }
      }

      protected virtual string? GenerateValueType(ValueTypeSourceGeneratorState state)
      {
         return null;
      }

      protected virtual string? GenerateEnum(EnumSourceGeneratorState state)
      {
         return null;
      }
   }
}