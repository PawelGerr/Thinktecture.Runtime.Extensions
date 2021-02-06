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
      private readonly string? _generatedFileInfix;

      protected ThinktectureRuntimeExtensionsSourceGeneratorBase(string? generatedFileInfix)
      {
         _generatedFileInfix = generatedFileInfix;
      }

      /// <inheritdoc />
      public void Initialize(GeneratorInitializationContext context)
      {
         context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
      }

      /// <inheritdoc />
      public void Execute(GeneratorExecutionContext context)
      {
         var receiver = (SyntaxReceiver)(context.SyntaxReceiver ?? throw new Exception($"Syntax receiver must be of type '{nameof(SyntaxReceiver)}' but found '{context.SyntaxReceiver?.GetType().Name}'."));

         foreach (var state in PrepareEnums(context.Compilation, receiver.Enums))
         {
            try
            {
               var generatedCode = GenerateEnum(state);
               EmitFile(context, state.Declaration, generatedCode);
            }
            catch (Exception ex)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration, state.Declaration.GetLocation(), state.EnumIdentifier, ex.Message));
            }
         }

         foreach (var declaration in receiver.ValueTypes)
         {
            try
            {
               var generatedCode = GenerateValueType(context.Compilation, declaration);
               EmitFile(context, declaration, generatedCode);
            }
            catch (Exception ex)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration, declaration.GetLocation(), declaration.Identifier, ex.Message));
            }
         }
      }

      private static IReadOnlyList<EnumSourceGeneratorState> PrepareEnums(
         Compilation compilation,
         IReadOnlyList<TypeDeclarationSyntax> enums)
      {
         if (enums.Count == 0)
            return Array.Empty<EnumSourceGeneratorState>();

         var states = new List<EnumSourceGeneratorState>();

         foreach (var tds in enums)
         {
            var state = GetEnumState(compilation, tds);

            if (state is not null)
               states.Add(state);
         }

         foreach (var state in states)
         {
            if (state.EnumType.BaseType is null || state.EnumType.BaseType.SpecialType == SpecialType.System_Object)
               continue;

            var baseEnum = states.FirstOrDefault(s => SymbolEqualityComparer.Default.Equals(state.EnumType.BaseType, s.EnumType));

            if (baseEnum is not null)
               state.SetBaseType(baseEnum);
         }

         states.Sort((state, other) =>
                     {
                        if (SymbolEqualityComparer.Default.Equals(state.EnumType.BaseType, other.EnumType))
                           return 1;

                        if (SymbolEqualityComparer.Default.Equals(other.EnumType.BaseType, state.EnumType))
                           return -1;

                        if (state.EnumType.BaseType is null)
                           return other.EnumType.BaseType is null ? 0 : -1;

                        return other.EnumType.BaseType is null ? 1 : 0;
                     });

         return states;
      }

      private string? GenerateValueType(Compilation compilation, TypeDeclarationSyntax declaration)
      {
         var model = compilation.GetSemanticModel(declaration.SyntaxTree, true);
         var type = model.GetDeclaredSymbol(declaration);

         if (type is null)
            return null;

         if (!type.HasValueTypeAttribute(out var valueTypeAttribute))
            return null;

         if (type.ContainingType is not null)
            return null;

         var state = new ValueTypeSourceGeneratorState(model, declaration, type, valueTypeAttribute);
         return GenerateValueType(state);
      }

      private static EnumSourceGeneratorState? GetEnumState(Compilation compilation, TypeDeclarationSyntax declaration)
      {
         var model = compilation.GetSemanticModel(declaration.SyntaxTree, true);
         var type = model.GetDeclaredSymbol(declaration);

         if (type is null)
            return null;

         if (!type.IsEnum(out var enumInterfaces))
            return null;

         if (type.ContainingType is not null)
            return null;

         var enumInterface = enumInterfaces.GetValidEnumInterface(type);

         if (enumInterface is null)
            return null;

         return new EnumSourceGeneratorState(model, declaration, type, enumInterface);
      }

      private void EmitFile(
         GeneratorExecutionContext context,
         TypeDeclarationSyntax declaration,
         string? generatedCode)
      {
         if (!String.IsNullOrWhiteSpace(generatedCode))
         {
            context.AddSource($"{declaration.Identifier}{_generatedFileInfix}_Generated.cs", $@"// <auto-generated />
#nullable enable
{generatedCode}");
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
