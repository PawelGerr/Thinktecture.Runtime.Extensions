namespace Thinktecture.EnumClassTests.Enums
{
	public class InvalidImplementationEnum : EnumClass<TestEnum>
	{
		public InvalidImplementationEnum(string key)
			: base(key)
		{
		}

		protected override TestEnum CreateInvalid(string key)
		{
			return null;
		}
	}
}
