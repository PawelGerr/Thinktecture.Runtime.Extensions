using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class StringExtensions
{
   private static readonly HashSet<string> _reservedIdentifiers = new(StringComparer.Ordinal)
                                                                  {
                                                                     "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
                                                                     "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
                                                                     "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit",
                                                                     "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object",
                                                                     "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return",
                                                                     "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this",
                                                                     "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
                                                                     "void", "volatile", "while"
                                                                  };

   public static ArgumentName MakeArgumentName(this string name)
   {
      name = name.Length switch
      {
         1 => name.ToLowerInvariant(),
         _ => name.StartsWith("_", StringComparison.Ordinal)
                 ? $"{Char.ToLowerInvariant(name[1])}{name.Substring(2)}"
                 : $"{Char.ToLowerInvariant(name[0])}{name.Substring(1)}"
      };

      var escaped = _reservedIdentifiers.Contains(name) ? $"@{name}" : name;

      return new ArgumentName(name, escaped);
   }

   public static string? TrimAndNullify(this string? text)
   {
      if (String.IsNullOrWhiteSpace(text))
         return null;

      return text!.Trim();
   }
}
