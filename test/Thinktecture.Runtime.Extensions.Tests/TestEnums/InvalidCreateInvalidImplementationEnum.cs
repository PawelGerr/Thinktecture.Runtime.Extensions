namespace Thinktecture.TestEnums
{
	public class InvalidCreateInvalidImplementationEnum : EnumClass<InvalidCreateInvalidImplementationEnum>
	{
		public InvalidCreateInvalidImplementationEnum(string key)
			: base(key)
		{
		}

		protected override InvalidCreateInvalidImplementationEnum CreateInvalid(string key)
		{
			return null;
		}
	}
}
