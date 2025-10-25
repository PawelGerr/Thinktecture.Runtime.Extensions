Issues found

1) Public API surface in SourceGenerator assembly
- Severity: Warning
- Details: The extension class and method are public. This assembly is an analyzer/source generator; exposing helper extensions publicly increases the surface area unintentionally for consumers of the analyzer package.
- Recommendation: Make the class and method internal to keep the public API minimal.

Suggested change:
------- PATCH
- public static class EnumerableExtension
+ internal static class EnumerableExtensions

-   public static void Enumerate<T>(this IEnumerable<T> enumerable)
+   internal static void Enumerate<T>(this IEnumerable<T> enumerable)
+++++++ END


2) Naming inconsistency: use plural "Extensions"
- Severity: Warning
- Details: Other helper types follow the "Extensions" plural naming convention (e.g., ReadOnlyCollectionExtensions). This class is named "EnumerableExtension" (singular), which is inconsistent with common .NET conventions and with the project naming.
- Recommendation: Rename the class to EnumerableExtensions and the file to EnumerableExtensions.cs.

Suggested change:
- File rename: src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/EnumerableExtension.cs → EnumerableExtensions.cs
- Class rename:
------- PATCH
- public static class EnumerableExtension
+ internal static class EnumerableExtensions
+++++++ END


3) Potential missing using for IEnumerable<T>
- Severity: Warning (conditional)
- Details: The file relies on IEnumerable<T> but has no explicit using System.Collections.Generic;. If implicit or global usings are disabled for this project, this will fail to compile.
- Recommendation: Ensure a global using exists or add an explicit using.

Suggested change (if needed):
------- PATCH
+ using System.Collections.Generic;
 
 namespace Thinktecture;
+++++++ END


Notes
- Null-check for enumerable is not strictly required due to NRT and project guidance (nullable reference types enabled). If defensive checks are desired, add ArgumentNullException.ThrowIfNull(enumerable); at the start of the method.
