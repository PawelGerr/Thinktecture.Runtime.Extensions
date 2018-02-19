namespace Thinktecture.TestEnums
{
	public class EmptyEnum : EnumClass<EmptyEnum>
	{
		private EmptyEnum(string key)
			: base(key)
		{
		}

		protected override EmptyEnum CreateInvalid(string key)
		{
			return new EmptyEnum(key);
		}
	}
}
