namespace Thinktecture.CodeAnalysis;

[Flags]
public enum ImplementedComparisonOperators
{
   None = 0,
   GreaterThan = 1 << 0,
   GreaterThanOrEqual = 1 << 1,
   LessThan = 1 << 2,
   LessThanOrEqual = 1 << 3,

   All = GreaterThan | GreaterThanOrEqual | LessThan | LessThanOrEqual
}
