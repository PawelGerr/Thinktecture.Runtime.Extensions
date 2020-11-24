namespace Thinktecture.TestEnums
{
	public partial class InvalidCreateInvalidImplementationEnum : Enum<InvalidCreateInvalidImplementationEnum>
	{
		public InvalidCreateInvalidImplementationEnum(string key)
			: base(key)
		{
		}

		protected override InvalidCreateInvalidImplementationEnum CreateInvalid(string key)
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			return null;
		}
	}
}
