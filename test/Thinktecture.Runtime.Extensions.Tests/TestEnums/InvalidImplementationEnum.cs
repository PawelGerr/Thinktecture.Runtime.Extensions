namespace Thinktecture.TestEnums
{
	public class InvalidImplementationEnum : EnumClass<TestEnum>
	{
		public InvalidImplementationEnum(string key)
			: base(key)
		{
		}

		protected override TestEnum CreateInvalid(string key)
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			return null;
		}
	}
}
