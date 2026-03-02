namespace Thinktecture;

public static class SyntaxTreeExtensions
{
   public static bool IsGeneratedTree(this SyntaxTree tree, CancellationToken cancellationToken)
   {
      return IsGeneratedFilePath(tree.FilePath);
   }

   private static bool IsGeneratedFilePath(string? path)
   {
      if (string.IsNullOrEmpty(path))
         return false;

      return path!.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
             path.EndsWith(".g.i.cs", StringComparison.OrdinalIgnoreCase) ||
             path.EndsWith(".designer.cs", StringComparison.OrdinalIgnoreCase) ||
             path.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase);
   }
}
