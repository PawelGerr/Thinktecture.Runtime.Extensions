namespace Thinktecture.Json.ValueTypeNewtonsoftJsonConverterTests.TestClasses
{
   public partial class StringBasedEnum : IValidatableEnum<string>
   {
      public static readonly StringBasedEnum ValueA = new("A");
      public static readonly StringBasedEnum ValueB = new("B");
   }
}
