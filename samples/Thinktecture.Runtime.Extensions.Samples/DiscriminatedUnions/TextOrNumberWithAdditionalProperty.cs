namespace Thinktecture.DiscriminatedUnions;

[Union<string, int>(T1Name = "Text",
                    T2Name = "Number",
                    SkipImplicitConversionFromValue = true,
                    ConstructorAccessModifier = UnionConstructorAccessModifier.Private)]
public partial class TextOrNumberExtended
{
   public required string AdditionalProperty { get; init; }

   public TextOrNumberExtended(string text, string additionalProperty)
      : this(text)
   {
      AdditionalProperty = additionalProperty;
   }

   public TextOrNumberExtended(int number, string additionalProperty)
      : this(number)
   {
      AdditionalProperty = additionalProperty;
   }
}
