using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Base class for source generator for smart enums.
/// </summary>
public abstract class SmartEnumSourceGeneratorBase : ThinktectureSourceGeneratorBase, IIncrementalGenerator
{
   protected SmartEnumSourceGeneratorBase(string? generatedFileSuffix)
      : base(generatedFileSuffix)
   {
   }

   /// <inheritdoc />
   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var candidates = context.SyntaxProvider.CreateSyntaxProvider(IsCandidate, GetEnumStateOrNull)
                              .Where(static state => state is not null)!
                              .Collect<EnumSourceGeneratorState>();

      context.RegisterSourceOutput(candidates, GenerateCode);
   }

   private static bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      return syntaxNode switch
      {
         ClassDeclarationSyntax classDeclaration when classDeclaration.IsPartial() && classDeclaration.IsEnumCandidate() => true,
         StructDeclarationSyntax structDeclaration when structDeclaration.IsPartial() && structDeclaration.IsEnumCandidate() => true,
         _ => false
      };
   }

   private static EnumSourceGeneratorState? GetEnumStateOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.Node;
      var type = context.SemanticModel.GetDeclaredSymbol(tds);

      if (type is null)
         return null;

      if (!type.IsEnum(out var enumInterfaces))
         return null;

      if (type.ContainingType is not null)
         return null;

      var enumInterface = enumInterfaces.GetValidEnumInterface(type, tds.Identifier.GetLocation());

      if (enumInterface is null)
         return null;

      return new EnumSourceGeneratorState(type, enumInterface);
   }

   private void GenerateCode(SourceProductionContext context, ImmutableArray<EnumSourceGeneratorState> enumStates)
   {
      if (enumStates.IsDefaultOrEmpty)
         return;

      try
      {
         Prepare(enumStates);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration, null, new object?[] { null, ex.Message }));
         return;
      }

      foreach (var enumState in enumStates.Distinct())
      {
         var type = enumState.EnumType;

         try
         {
            var generatedCode = GenerateEnum(enumState);

            EmitFile(context, enumState.Namespace, type.Name, generatedCode);
         }
         catch (Exception ex)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                       type.DeclaringSyntaxReferences.First().GetSyntax().GetLocation(), // pick one location as the representative,
                                                       type.Name, ex.Message));
         }
      }
   }

   private static void Prepare(ImmutableArray<EnumSourceGeneratorState> states)
   {
      foreach (var enumState in states)
      {
         if (enumState is null)
            continue;

         if (enumState.EnumType.BaseType is null || enumState.EnumType.BaseType.SpecialType == SpecialType.System_Object)
            continue;

         var baseEnum = states.FirstOrDefault(s => SymbolEqualityComparer.Default.Equals(enumState.EnumType.BaseType, s.EnumType));

         if (baseEnum is not null)
            enumState.SetBaseType(baseEnum);
      }
   }

   protected virtual string? GenerateEnum(EnumSourceGeneratorState state)
   {
      return null;
   }
}
