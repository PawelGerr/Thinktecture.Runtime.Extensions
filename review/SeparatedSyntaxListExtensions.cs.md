Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SeparatedSyntaxListExtensions.cs

1) Unnecessarily public API surface (design)
- The extensions class is declared public but appears to be used only internally by the source generator.
- Public Roslyn-specific extension methods increase the visible API surface of the generator assembly unnecessarily.
- Recommendation: Change the class to internal.

Suggested change:
- public static class SeparatedSyntaxListExtensions
+ internal static class SeparatedSyntaxListExtensions

2) Struct copy of SeparatedSyntaxList (performance)
- SeparatedSyntaxList<T> is a struct. Passing it by value copies the struct on each call.
- In hot paths, prefer passing by readonly reference to avoid copies.
- Recommendation: Use the &#34;in&#34; modifier on the extension &#34;this&#34; parameter.

Suggested change:
- public static T? FirstOrDefault<T>(this SeparatedSyntaxList<T> list, Func<T, bool> predicate)
+ public static T? FirstOrDefault<T>(this in SeparatedSyntaxList<T> list, Func<T, bool> predicate)

3) Minor micro-optimization: cache Count in local (performance, low impact)
- Accessing list.Count in each loop iteration is inexpensive but can be cached to avoid repeated property access.
- Recommendation: Cache Count before the loop.

Suggested code (combined):
internal static class SeparatedSyntaxListExtensions
{
   public static T? FirstOrDefault<T>(this in SeparatedSyntaxList<T> list, Func<T, bool> predicate)
      where T : SyntaxNode
   {
      var count = list.Count;

      for (var i = 0; i < count; i++)
      {
         var node = list[i];

         if (predicate(node))
            return node;
      }

      return null;
   }
}
