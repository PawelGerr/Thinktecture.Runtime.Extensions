using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class MetadataReferenceExtensions
{
   public static IEnumerable<ModuleInfo> GetModules(this MetadataReference metadataReference)
   {
      // Project reference (ISymbol)
      if (metadataReference is CompilationReference compilationReference)
         return compilationReference.Compilation.Assembly.Modules.Select(m => new ModuleInfo(m.Name));

      // DLL
      if (metadataReference is PortableExecutableReference portable && portable.GetMetadata() is AssemblyMetadata assemblyMetadata)
         return assemblyMetadata.GetModules().Select(m => new ModuleInfo(m.Name));

      return Array.Empty<ModuleInfo>();
   }
}
