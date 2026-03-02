namespace Thinktecture.CodeAnalysis;

[Flags]
public enum ImplementedOperators
{
   None = 0,
   Default = 1 << 0,
   Checked = 1 << 1,

   All = Default | Checked
}
