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

      foreach (var state in PrepareEnums(compilationAndCandidates.States))
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

      foreach (var state in PrepareValueObjects(compilationAndCandidates.States))
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

   private static IReadOnlyList<EnumSourceGeneratorState> PrepareEnums(ImmutableArray<ThinktectureRuntimeExtensionsStates> states)
   {
      HashSet<EnumSourceGeneratorState>? enumStates = null;

      foreach (var state in states)
      {
         if (state.EnumState is not null)
            (enumStates ??= new HashSet<EnumSourceGeneratorState>()).Add(state.EnumState);
      }

      if (enumStates is null)
         return Array.Empty<EnumSourceGeneratorState>();

      foreach (var state in enumStates)
      {
         if (state.EnumType.BaseType is null || state.EnumType.BaseType.SpecialType == SpecialType.System_Object)
            continue;

         var baseEnum = enumStates.FirstOrDefault(s => SymbolEqualityComparer.Default.Equals(state.EnumType.BaseType, s.EnumType));

         if (baseEnum is not null)
            state.SetBaseType(baseEnum);
      }

      return enumStates.OrderBy(s => s).ToList();
   }

   private static IReadOnlyCollection<ValueObjectSourceGeneratorState> PrepareValueObjects(ImmutableArray<ThinktectureRuntimeExtensionsStates> states)
   {
      HashSet<ValueObjectSourceGeneratorState>? valueObjectStates = null;

      foreach (var state in states)
      {
         if (state.ValueObjectState is not null)
            (valueObjectStates ??= new HashSet<ValueObjectSourceGeneratorState>()).Add(state.ValueObjectState);
      }

      return (IReadOnlyCollection<ValueObjectSourceGeneratorState>?)valueObjectStates ?? Array.Empty<ValueObjectSourceGeneratorState>();
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

      return new EnumSourceGeneratorState(model, type, enumInterface);
   }

   private void EmitFile(SourceProductionContext context, string typeName, string? generatedCode)
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
