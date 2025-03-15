namespace Thinktecture.Unions;

[Union]
public abstract partial class Jurisdiction
{
   [ValueObject<string>(KeyMemberName = "IsoCode")]
   [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>] // case-insensitive comparison
   [ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
   public partial class Country : Jurisdiction
   {
      static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string isoCode)
      {
         if (string.IsNullOrWhiteSpace(isoCode))
         {
            validationError = new ValidationError("ISO code is required.");
            isoCode = string.Empty;
            return;
         }

         isoCode = isoCode.Trim();

         if (isoCode.Length != 2)
            validationError = new ValidationError("ISO code must be exactly 2 characters long.");
      }
   }

   /// <summary>
   /// Let's assume that the federal state is represented by an number.
   /// </summary>
   [ValueObject<int>(KeyMemberName = "Number")]
   public partial class FederalState : Jurisdiction;

   [ValueObject<string>(KeyMemberName = "Name")]
   [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>] // case-insensitive comparison
   [ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
   public partial class District : Jurisdiction;

   /// <summary>
   /// The complex type adds appropriate equality comparison(i.e. it checks for type only).
   /// </summary>
   [ComplexValueObject]
   public partial class Unknown : Jurisdiction
   {
      public static readonly Unknown Instance = new();
   }
}
