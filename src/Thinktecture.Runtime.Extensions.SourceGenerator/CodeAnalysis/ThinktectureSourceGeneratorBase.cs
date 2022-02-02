using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public abstract class ThinktectureSourceGeneratorBase
{
   public const string GENERATED_CODE_PREFIX = @"// <auto-generated />
#nullable enable
";

   private readonly string? _generatedFileSuffix;

   protected ThinktectureSourceGeneratorBase(string? generatedFileSuffix)
   {
      _generatedFileSuffix = generatedFileSuffix;
   }

   protected void EmitFile(SourceProductionContext context, string? typeNamespace, string typeName, string? generatedCode)
   {
      if (String.IsNullOrWhiteSpace(generatedCode))
         return;

      var hintName = $"{(typeNamespace is null ? null : $"{typeNamespace}.")}{typeName}{_generatedFileSuffix}.g.cs";
      context.AddSource(hintName, generatedCode!);
   }
}
