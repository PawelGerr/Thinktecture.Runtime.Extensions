using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

public abstract class SmartEnumSourceGeneratorBase<T, TBaseEnumExtension> : ThinktectureSourceGeneratorBase, IIncrementalGenerator
   where T : EnumSourceGeneratorStateBase<TBaseEnumExtension>, IEquatable<T>
   where TBaseEnumExtension : IEquatable<TBaseEnumExtension>
{
   protected SmartEnumSourceGeneratorBase(string? generatedFileSuffix)
      : base(generatedFileSuffix)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var candidates = context.SyntaxProvider.CreateSyntaxProvider(IsCandidate, GetEnumStateOrNull)
                              .Where(static state => state.HasValue)
                              .Select((state, _) => state!.Value)
                              .Collect()
                              .SelectMany((states, _) => states.Distinct());

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

   private SourceGenState<T>? GetEnumStateOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
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

         return new SourceGenState<T>(CreateState(type, enumInterface), null);
      }
      catch (Exception ex)
      {
         return new SourceGenState<T>(null, ex);
      }
   }

   protected abstract T CreateState(INamedTypeSymbol type, INamedTypeSymbol enumInterface);

   private void GenerateCode(SourceProductionContext context, SourceGenState<T> state)
   {
      if (state.Exception is not null)
      {
         context.ReportException(state.Exception);
         return;
      }

      var enumState = state.State;

      if (enumState is null)
         return;

      var stringBuilderProvider = new StringBuilderProvider();

      try
      {
         var generatedCode = GenerateEnum(enumState, stringBuilderProvider);

         EmitFile(context, enumState.Namespace, enumState.Name, generatedCode);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                    enumState.GetLocationOrNullSafe(context),
                                                    enumState.Name, ex.Message));
      }
   }

   protected virtual string? GenerateEnum(T state, StringBuilderProvider stringBuilderProvider)
   {
      return null;
   }
}
