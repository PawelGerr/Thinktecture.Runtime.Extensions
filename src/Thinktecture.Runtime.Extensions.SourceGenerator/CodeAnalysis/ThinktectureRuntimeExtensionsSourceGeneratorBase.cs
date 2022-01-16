using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Base class for source generator for enum-like classes.
/// </summary>
public abstract class ThinktectureRuntimeExtensionsSourceGeneratorBase : IIncrementalGenerator
{
   private readonly string? _generatedFileInfix;

   protected ThinktectureRuntimeExtensionsSourceGeneratorBase(string? generatedFileInfix)
   {
      _generatedFileInfix = generatedFileInfix;
   }

   /// <inheritdoc />
   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var candidates = context.SyntaxProvider.CreateSyntaxProvider(IsCandidate, GetCandidate)
                              .Where(c => !c.IsEmpty)
                              .Collect();

      context.RegisterSourceOutput(context.CompilationProvider.Combine(candidates), GenerateCode);
   }

   private static ThinktectureRuntimeExtensionsStates GetCandidate(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.Node;

      EnumSourceGeneratorState? enumState = null;
      ValueObjectSourceGeneratorState? valueObjectState = null;

      if (tds.IsEnumCandidate())
         enumState = GetEnumState(context.SemanticModel, tds);

      if (tds.IsValueObjectCandidate())
         valueObjectState = GetValueObjectState(context.SemanticModel, tds);

      return new ThinktectureRuntimeExtensionsStates(enumState, valueObjectState);
   }

   private static bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      return syntaxNode switch
      {
         ClassDeclarationSyntax classDeclaration when classDeclaration.IsPartial() => true,
         StructDeclarationSyntax structDeclaration when structDeclaration.IsPartial() => true,
         _ => false
      };
   }

   private void GenerateCode(SourceProductionContext context, (Compilation Compilation, ImmutableArray<ThinktectureRuntimeExtensionsStates> States) compilationAndCandidates)
   {
      if (compilationAndCandidates.States.IsDefaultOrEmpty)
         return;

      try
      {
         Prepare(compilationAndCandidates.States);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration, null, new object?[] { null, ex.Message }));
         return;
      }

      foreach (var states in compilationAndCandidates.States.Distinct())
      {
         INamedTypeSymbol? type = null;

         try
         {
            string? generatedCode;
            string? typeName;

            if (states.EnumState is not null)
            {
               type = states.EnumState.EnumType;
               typeName = states.EnumState.EnumType.Name;
               generatedCode = GenerateEnum(states.EnumState);
            }
            else if (states.ValueObjectState is not null)
            {
               type = states.ValueObjectState.Type;
               typeName = states.ValueObjectState.Type.Name;
               generatedCode = GenerateValueObject(states.ValueObjectState);
            }
            else
            {
               continue;
            }

            EmitFile(context, type.ContainingNamespace, typeName, generatedCode);
         }
         catch (Exception ex)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                       type?.DeclaringSyntaxReferences.First().GetSyntax().GetLocation(), // pick one location as the representative,
                                                       type?.Name, ex.Message));
         }
      }
   }

   private static void Prepare(ImmutableArray<ThinktectureRuntimeExtensionsStates> states)
   {
      foreach (var (enumState, _) in states)
      {
         if (enumState is null)
            continue;

         if (enumState.EnumType.BaseType is null || enumState.EnumType.BaseType.SpecialType == SpecialType.System_Object)
            continue;

         var baseEnum = states.FirstOrDefault(s => SymbolEqualityComparer.Default.Equals(enumState.EnumType.BaseType, s.EnumState?.EnumType)).EnumState;

         if (baseEnum is not null)
            enumState.SetBaseType(baseEnum);
      }
   }

   private static ValueObjectSourceGeneratorState? GetValueObjectState(SemanticModel model, TypeDeclarationSyntax declaration)
   {
      var type = model.GetDeclaredSymbol(declaration);

      if (type is null)
         return null;

      if (!type.HasValueObjectAttribute(out var valueObjectAttribute))
         return null;

      if (type.ContainingType is not null)
         return null;

      return new ValueObjectSourceGeneratorState(model, type, valueObjectAttribute);
   }

   private static EnumSourceGeneratorState? GetEnumState(SemanticModel model, TypeDeclarationSyntax declaration)
   {
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

      return new EnumSourceGeneratorState(type, enumInterface);
   }

   private void EmitFile(SourceProductionContext context, INamespaceSymbol typeNamespace, string typeName, string? generatedCode)
   {
      if (!String.IsNullOrWhiteSpace(generatedCode))
      {
         context.AddSource($"{(typeNamespace.IsGlobalNamespace ? null : $"{typeNamespace}.")}{typeName}{_generatedFileInfix}_Generated.cs", $@"// <auto-generated />
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
