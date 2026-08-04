namespace Thinktecture;

/// <summary>
/// Controls how the default value of an ad-hoc union struct is treated.
/// </summary>
public enum UnionDefaultValueHandling
{
   /// <summary>
   /// The default value of the union is invalid. The analyzer reports TTRESG047 on
   /// usage of <c>default</c> and <c>new()</c>, and the generated members throw at runtime.
   /// This is the current behavior and the default.
   /// </summary>
   Disallow = 0,

   /// <summary>
   /// <c>default(TUnion)</c> represents the first member (T1). The value is fully
   /// functional and freely usable.
   /// </summary>
   MapToFirstMember = 1
}
