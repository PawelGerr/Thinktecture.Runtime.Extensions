namespace Thinktecture.CodeAnalysis;

public abstract class CodeGeneratorBase
{
   protected const string GENERATED_CODE_PREFIX = @"// <auto-generated />
#nullable enable";

   public abstract string? FileNameSuffix { get; }
   public abstract void Generate();
}
