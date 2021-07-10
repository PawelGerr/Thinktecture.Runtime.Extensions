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
               EmitFile(context, state.EnumType.Name, generatedCode);
            }
            catch (Exception ex)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                          state.EnumType.DeclaringSyntaxReferences.First().GetSyntax().GetLocation(), // pick one location as the representative,
                                                          state.EnumType.Name, ex.Message));
            }
         }

         foreach (var state in PrepareValueObjects(context.Compilation, receiver.ValueObjects))
         {
            try
            {
               var generatedCode = GenerateValueObject(state);
               EmitFile(context, state.Type.Name, generatedCode);
            }
            catch (Exception ex)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                          state.Type.DeclaringSyntaxReferences.First().GetSyntax().GetLocation(), // pick one location as the representative
                                                          state.Type.Name, ex.Message));
            }
         }
      }

      private static IReadOnlyList<EnumSourceGeneratorState> PrepareEnums(
         Compilation compilation,
         IReadOnlyList<TypeDeclarationSyntax> enums)
      {
         if (enums.Count == 0)
            return Array.Empty<EnumSourceGeneratorState>();

         var states = new HashSet<EnumSourceGeneratorState>();

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

         return states.OrderBy(s => s).ToList();
      }

      private static IReadOnlyCollection<ValueObjectSourceGeneratorState> PrepareValueObjects(
         Compilation compilation,
         IReadOnlyList<TypeDeclarationSyntax> valueObjects)
      {
         if (valueObjects.Count == 0)
            return Array.Empty<ValueObjectSourceGeneratorState>();

         var states = new HashSet<ValueObjectSourceGeneratorState>();

         foreach (var tds in valueObjects)
         {
            var state = GetValueObjectState(compilation, tds);

            if (state is not null)
               states.Add(state);
         }

         return states;
      }

      private static ValueObjectSourceGeneratorState? GetValueObjectState(Compilation compilation, TypeDeclarationSyntax declaration)
      {
         var model = compilation.GetSemanticModel(declaration.SyntaxTree, true);
         var type = model.GetDeclaredSymbol(declaration);

         if (type is null)
            return null;

         if (!type.HasValueObjectAttribute(out var valueObjectAttribute))
            return null;

         if (type.ContainingType is not null)
            return null;

         return new ValueObjectSourceGeneratorState(model, type, valueObjectAttribute);
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

         var enumInterface = enumInterfaces.GetValidEnumInterface(type, declaration.Identifier.GetLocation());

         if (enumInterface is null)
            return null;

         return new EnumSourceGeneratorState(model, type, enumInterface);
      }

      private void EmitFile(
         GeneratorExecutionContext context,
         string typeName,
         string? generatedCode)
      {
         if (!String.IsNullOrWhiteSpace(generatedCode))
         {
            context.AddSource($"{typeName}{_generatedFileInfix}_Generated.cs", $@"// <auto-generated />
#nullable enable
{generatedCode}");
         }
      }

      protected virtual string? GenerateValueObject(ValueObjectSourceGeneratorState state)
      {
         return null;
      }

      protected virtual string? GenerateEnum(EnumSourceGeneratorState state)
      {
         return null;
      }
   }
}
