namespace Thinktecture;

/// <summary>
/// Member kind.
/// </summary>
[Obsolete("Use 'MemberKind' instead.")]
public enum ValueObjectMemberKind
{
   /// <summary>
   /// Field.
   /// </summary>
   Field = 0,

   /// <summary>
   /// Property.
   /// </summary>
   Property = 1
}


/// <summary>
/// Member kind.
/// </summary>
public enum MemberKind
{
   /// <summary>
   /// Field.
   /// </summary>
   Field = 0,

   /// <summary>
   /// Property.
   /// </summary>
   Property = 1
}
