Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ObjectCreationOperationExtensions.cs

- Error (conditional): Missing required Roslyn/system usings.
  - The file uses OperationKind and IOperation (namespace Microsoft.CodeAnalysis) and ImmutableArray<T> (namespace System.Collections.Immutable) but only imports Microsoft.CodeAnalysis.Operations.
  - Without project/global usings, this will not compile.
  - Fix: Add:
    - `using Microsoft.CodeAnalysis;`
    - `using System.Collections.Immutable;`

- Error (runtime risk): Potential InvalidCastException in GetIntegerParameterValue.
  - Code: `return (int?)initializer?.Initializers.FindInitialization(name);`
  - If FindInitialization returns a boxed enum value (e.g., AccessModifier) or a different integral type (byte, short, long), the direct `(int?)` cast can throw.
  - Safer approach:
    - ```
      private static int? GetIntegerParameterValue(IObjectOrCollectionInitializerOperation? initializer, string name)
      {
         var obj = initializer?.Initializers.FindInitialization(name);
         return obj switch
         {
            int i => i,
            byte b => b,
            short s => s,
            long l when l >= int.MinValue && l <= int.MaxValue => (int)l,
            Enum e => Convert.ToInt32(e),
            _ => null
         };
      }
      ```
    - Or, if only enums and ints are expected: handle `Enum` via `Convert.ToInt32`.

- Warning (conditional/style + implicit using dependency): `String.IsNullOrWhiteSpace`.
  - Using the framework type name `String` requires `using System;` (or full qualification).
  - To avoid dependency on implicit usings and align with common style in the project, prefer `string.IsNullOrWhiteSpace(value)`.
  - Also, after the null/whitespace check, `value` cannot be null, so `value?.Trim()` can be simplified to `value.Trim()`.

- Note to verify intent: `HasDefaultStringComparison` returns true if the initializer sets the property, regardless of value.
  - If `DEFAULT_STRING_COMPARISON` is an enum (e.g., StringComparison) and the goal is to check presence, this is correct.
  - If it could be a boolean flag, consider evaluating the actual constant value instead of only presence.
