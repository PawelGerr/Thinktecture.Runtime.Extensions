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
      // Project reference (ISymbol)
      if (metadataReference is CompilationReference compilationReference)
         return compilationReference.Compilation.Assembly.Modules.Select(m => new ModuleInfo(m.Name));

      // DLL
      if (metadataReference is PortableExecutableReference portable && portable.GetMetadata() is AssemblyMetadata assemblyMetadata)
         return assemblyMetadata.GetModules().Select(m => new ModuleInfo(m.Name));

      return [];
   }
}
