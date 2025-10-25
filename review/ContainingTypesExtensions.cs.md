Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ContainingTypesExtensions.cs

1) Bug: Incorrect camelCase conversion for leading uppercase runs (acronyms, interface prefixes)
- Current behavior lowercases the entire initial consecutive uppercase run, producing incorrect names:
  - URLValue -> urlvalue (expected: urlValue)
  - IPAddress -> ipaddress (expected: ipAddress)
  - ICode -> icode (expected: iCode)
- Impact: Generated identifiers deviate from common .NET naming conventions and may trigger analyzers; readability suffers.

Root cause
- The loop lowercases every character in the initial consecutive uppercase run:
  for (var j = i + 1; j < len && char.IsUpper(sb[j]); j++) { sb[j] = char.ToLowerInvariant(sb[j]); }
- This also lowercases the last uppercase letter of an acronym that precedes a lowercase letter (e.g., the V in URLValue).

Proposed fix (look-ahead rule, align with well-known ToCamelCase algorithms)
- Lowercase:
  - the first uppercase letter, and
  - subsequent uppercase letters only while the next character is also uppercase.
- Stop before the last uppercase that is immediately followed by a lowercase.

Code change (replace the inner lowercasing loop):
for (var i = originalLen; i < len; i++)
{
   var ch = sb[i];

   if (!char.IsLetter(ch))
      continue;

   if (char.IsUpper(ch))
   {
      // Lowercase the first letter
      sb[i] = char.ToLowerInvariant(ch);

      // Lowercase subsequent uppercase letters only if the next char is also uppercase.
      for (var j = i + 1; j < len; j++)
      {
         if (char.IsUpper(sb[j]) && j + 1 < len && char.IsUpper(sb[j + 1]))
         {
            sb[j] = char.ToLowerInvariant(sb[j]);
         }
         else
         {
            break;
         }
      }
   }

   // Either handled uppercase or first letter was already lowercase.
   break;
}

Examples with the fix
- URLValue -> urlValue
- IPAddress -> ipAddress
- ICode -> iCode
- XmlReader -> xmlReader (unchanged from current)

Suggested tests (to prevent regressions)
- MakeFullyQualifiedArgumentName([], "URLValue", false) == "urlValue"
- MakeFullyQualifiedArgumentName([], "IPAddress", false) == "ipAddress"
- MakeFullyQualifiedArgumentName([], "ICode", false) == "iCode"
- MakeFullyQualifiedArgumentName([Outer, Inner], "Value", false) == "outerInnerValue"
- MakeFullyQualifiedArgumentName([Outer], "Value", true) == "value"
