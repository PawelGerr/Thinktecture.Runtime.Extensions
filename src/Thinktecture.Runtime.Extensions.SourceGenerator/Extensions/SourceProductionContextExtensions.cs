using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class SourceProductionContextExtensions
{
   public static void EmitFile(
      this SourceProductionContext context,
      string? typeNamespace,
      IReadOnlyList<ContainingTypeState> containingTypes,
      string typeName,
      int numberOfGenerics,
      string? generatedCode,
      string? fileNameSuffix)
   {
      if (String.IsNullOrWhiteSpace(generatedCode))
         return;

      var containingTypeNames = containingTypes.Count == 0 ? null : String.Join(".", containingTypes.Select(t => t.Name)) + ".";
      var hintName = $"{(typeNamespace is null ? null : $"{typeNamespace}.")}{containingTypeNames}{typeName}{(numberOfGenerics > 0 ? $"`{numberOfGenerics}" : null)}{fileNameSuffix}.g.cs";
      context.AddSource(hintName, generatedCode!);
   }

   public static void ReportError(this SourceProductionContext context, Location location, string name, string message)
   {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                 location,
                                                 name, message));
   }
}
