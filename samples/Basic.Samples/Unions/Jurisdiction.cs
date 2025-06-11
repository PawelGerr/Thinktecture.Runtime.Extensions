using System.Text.Json.Serialization;

namespace Thinktecture.Unions;

[Union]
[JsonConverter(typeof(JurisdictionJsonConverter))]
public abstract partial class Jurisdiction
{
   [ValueObject<string>(KeyMemberName = "_isoCode")]
   [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>] // case-insensitive comparison
   [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
   public partial class Country : Jurisdiction
   {
      static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string isoCode)
      {
         isoCode = isoCode.Trim();

         if (isoCode.Length != 2)
            validationError = new ValidationError("ISO code must be exactly 2 characters long.");
      }
   }

   /// <summary>
   /// Let's assume that the federal state is represented by a number.
   /// </summary>
   [ValueObject<int>]
   public partial class FederalState : Jurisdiction;

   [ValueObject<string>(KeyMemberName = "_name")]
   [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>] // case-insensitive comparison
   [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
   public partial class District : Jurisdiction;

   /// <summary>
   /// The complex type adds appropriate equality comparison(i.e., it checks for type only).
   /// </summary>
   [ComplexValueObject(SkipFactoryMethods = true)]
   public partial class Unknown : Jurisdiction
   {
      public static readonly Unknown Instance = new();
   }

   /// <summary>
   /// Calling a continent a jurisdiction makes no sense, but you get the idea.
   /// </summary>
   [SmartEnum<string>]
   public partial class Continent : Jurisdiction
   {
      public static readonly Continent Africa = new("Africa");
      public static readonly Continent Antarctica = new("Antarctica");
      public static readonly Continent Asia = new("Asia");
      public static readonly Continent Australia = new("Australia");
      public static readonly Continent Europe = new("Europe");
      public static readonly Continent NorthAmerica = new("North America");
      public static readonly Continent SouthAmerica = new("South America");
   }
}
