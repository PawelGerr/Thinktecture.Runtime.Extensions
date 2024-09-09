namespace Thinktecture;

/// <summary>
/// Access modifer.
/// </summary>
public enum UnionConstructorAccessModifier
{
   /// <summary>
   /// Access modifer "private".
   /// </summary>
   Private = 1 << 0,

   /// <summary>
   /// Access modifer "internal".
   /// </summary>
   Internal = 1 << 2,

   /// <summary>
   /// Access modifer "public".
   /// </summary>
   Public = 1 << 3
}
