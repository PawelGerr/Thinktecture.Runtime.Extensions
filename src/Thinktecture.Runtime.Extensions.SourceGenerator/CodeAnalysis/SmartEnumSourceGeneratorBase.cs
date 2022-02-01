using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Base class for source generator for Smart Enums.
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
                              .Where(static state => state.HasValue)
                              .Select((state, _) => state!.Value)
                              .Collect();

      context.RegisterSourceOutput(candidates, GenerateCode);
   }

   private static bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      try
      {
         return syntaxNode switch
         {
            ClassDeclarationSyntax classDeclaration when IsEnumCandidate(classDeclaration) => true,
            StructDeclarationSyntax structDeclaration when IsEnumCandidate(structDeclaration) => true,
            _ => false
         };
      }
      catch (Exception ex)
      {
         Debug.Write(ex);
         return false;
      }
   }

   private static bool IsEnumCandidate(TypeDeclarationSyntax typeDeclaration)
   {
      return typeDeclaration.IsPartial()
             && !typeDeclaration.IsGeneric()
             && typeDeclaration.IsEnumCandidate();
   }

   private static SourceGenState<EnumSourceGeneratorState>? GetEnumStateOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      try
      {
         var tds = (TypeDeclarationSyntax)context.Node;
         var type = context.SemanticModel.GetDeclaredSymbol(tds);

         if (type is null)
            return null;

         if (!type.IsEnum(out var enumInterfaces))
            return null;

         if (type.ContainingType is not null)
            return null;

         var enumInterface = enumInterfaces.GetValidEnumInterface(type);

         if (enumInterface is null)
            return null;

         return new SourceGenState<EnumSourceGeneratorState>(new EnumSourceGeneratorState(type, enumInterface), null);
      }
      catch (Exception ex)
      {
         return new SourceGenState<EnumSourceGeneratorState>(null, ex);
      }
   }

   private void GenerateCode(SourceProductionContext context, ImmutableArray<SourceGenState<EnumSourceGeneratorState>> states)
   {
      if (states.IsDefaultOrEmpty)
         return;

      IReadOnlyList<EnumSourceGeneratorState> enumStates;

      try
      {
         enumStates = states.GetDistinctInnerStates(context);

         if (enumStates.Count == 0)
            return;

         Prepare(enumStates);
      }
      catch (Exception ex)
      {
         context.ReportException(ex);
         return;
      }

      foreach (var enumState in enumStates)
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
                                                       type.GetLocationOrNullSafe(context),
                                                       type.Name, ex.Message));
         }
      }
   }

   private static void Prepare(IReadOnlyList<EnumSourceGeneratorState> states)
   {
      foreach (var enumState in states)
      {
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
