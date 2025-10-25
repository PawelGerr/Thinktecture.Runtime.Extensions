Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SerializationFrameworksExtensions.cs

Summary
- Errors: 0
- Warnings: 1

Warnings
1) Zero-flag semantics may be surprising
- What: The extension method returns true if flag == 0 for any value because (value & 0) == 0 evaluates to true.
- Why it matters: If SerializationFrameworks defines None = 0 (common for [Flags] enums), then value.Has(SerializationFrameworks.None) will always be true regardless of whether value actually represents “no frameworks”. This can hide logic bugs and lead to unintended control flow.
- Evidence:
  public static bool Has(this SerializationFrameworks value, SerializationFrameworks flag)
  {
     return (value & flag) == flag;
  }
- Recommendation: Handle the zero flag explicitly to match the intended semantics in this project:
  Option A (treat None as only true when value == None):
  public static bool Has(this SerializationFrameworks value, SerializationFrameworks flag)
  {
      if (flag == 0)
         return value == 0;
      return (value & flag) == flag;
  }

  Option B (disallow zero flag):
  public static bool Has(this SerializationFrameworks value, SerializationFrameworks flag)
  {
      if (flag == 0)
         throw new ArgumentException("flag must be non-zero.", nameof(flag));
      return (value & flag) == flag;
  }

  Note: Enum.HasFlag in .NET returns true for zero, which is often considered surprising; choose behavior explicitly to avoid ambiguity.
