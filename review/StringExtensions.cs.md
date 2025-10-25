Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/StringExtensions.cs

Errors
1) ToCamelCase duplicates characters and drops leading underscore for names starting with '_' when the first convertible letter is at index > 0.
   - Examples:
     - Input: "_A" → Actual: "Aa" (expected: "_a")
     - Input: "_ABc" → Actual: "AaABc" (expected: "_aBc")
   - Root cause:
     - Uses name.Substring(startsWithUnderscore ? 1 : 0, i) where i is the absolute index in the original string, causing inclusion of the current character and duplication when appending the lowered char.
     - Suffix uses name.Substring(i) instead of i + 1, reintroducing the current character.
     - Prefix logic omits the original underscore when startsWithUnderscore is true, effectively removing it from the result.
   - Impact:
     - MakeBackingFieldName(true) can return incorrect field names for inputs that already start with '_', violating the expected “single leading underscore + camelCase” convention.

Warnings
2) Fragile reliance on implicit usings for String.Empty and Char APIs.
   - If implicit usings are disabled, this file will not compile without a using System; directive (or fully-qualified names / keyword forms like string.Empty).
