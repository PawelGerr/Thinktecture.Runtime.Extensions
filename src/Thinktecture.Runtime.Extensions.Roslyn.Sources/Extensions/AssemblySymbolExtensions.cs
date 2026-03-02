namespace Thinktecture;

public static class AssemblySymbolExtensions
{
   public static bool HasInternalsVisibleToFor(this IAssemblySymbol assembly, IAssemblySymbol targetAssembly)
   {
      var attributes = assembly.GetAttributes();

      for (var i = 0; i < attributes.Length; i++)
      {
         var attribute = attributes[i];

         if (!attribute.AttributeClass.IsInternalsVisibleToAttribute())
            continue;

         if (attribute.ConstructorArguments.Length != 1
             || attribute.ConstructorArguments[0].Kind != TypedConstantKind.Primitive
             || attribute.ConstructorArguments[0].Value is not string assemblyName
             || !ExtractAssemblyName(assemblyName).Equals(targetAssembly.Name.AsSpan(), StringComparison.OrdinalIgnoreCase))
            continue;

         return true;
      }

      return false;
   }

   private static ReadOnlySpan<char> ExtractAssemblyName(string assemblyString)
   {
      // InternalsVisibleTo can be in format: "AssemblyName" or "AssemblyName, PublicKey=..."
      var assembly = assemblyString.AsSpan();
      var commaIndex = assembly.IndexOf(',');
      return commaIndex >= 0 ? assembly.Slice(0, commaIndex).Trim() : assembly.Trim();
   }
}
