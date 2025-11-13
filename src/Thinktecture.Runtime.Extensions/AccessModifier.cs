namespace Thinktecture;

/// <summary>
/// Access modifer.
/// </summary>
public enum AccessModifier
{
   /// <summary>
   /// Access modifer "private".
   /// </summary>
   Private = 1 << 0,

   /// <summary>
   /// Access modifer "protected".
   /// </summary>
   Protected = 1 << 1,

   /// <summary>
   /// Access modifer "internal".
   /// </summary>
   Internal = 1 << 2,

   /// <summary>
   /// Access modifer "public".
   /// </summary>
   Public = 1 << 3,

   /// <summary>
   /// Access modifer "private protected".
   /// </summary>
   PrivateProtected = Private | Protected,

   /// <summary>
   /// Access modifer "protected internal".
   /// </summary>
   ProtectedInternal = Protected | Internal
}
