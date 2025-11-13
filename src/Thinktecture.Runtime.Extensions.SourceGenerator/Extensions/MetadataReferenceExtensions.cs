using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class MetadataReferenceExtensions
{
   /// <summary>
   /// Can throw if file (DLL) could not be loaded.
   /// </summary>
   /// <exception cref="FileNotFoundException">MissingMetadataReference throws if DLL could not be loaded.</exception>
   public static IEnumerable<ModuleInfo> GetModules(this MetadataReference metadataReference)
   {
      return metadataReference switch
      {
         // Project reference (ISymbol)
         CompilationReference compilationReference => compilationReference.Compilation.Assembly.Modules.Select(m => new ModuleInfo(m.Name)),
         // DLL (assembly with potentially multiple modules)
         PortableExecutableReference portable when portable.GetMetadata() is AssemblyMetadata assemblyMetadata => assemblyMetadata.GetModules().Select(m => new ModuleInfo(m.Name)),
         // Netmodule (single module reference)
         PortableExecutableReference portable when portable.GetMetadata() is ModuleMetadata moduleMetadata => [new ModuleInfo(moduleMetadata.Name)],
         _ => []
      };
   }
}
