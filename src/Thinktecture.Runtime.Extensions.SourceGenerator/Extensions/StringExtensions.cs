namespace Thinktecture;

public static class StringExtensions
{
   public static string MakeArgumentName(this string name)
   {
      return name.Length switch
      {
         1 => name.ToLowerInvariant(),
         _ => name.StartsWith("_", StringComparison.Ordinal)
                 ? $"{Char.ToLowerInvariant(name[1])}{name.Substring(2)}"
                 : $"{Char.ToLowerInvariant(name[0])}{name.Substring(1)}"
      };
   }
}
