namespace Thinktecture.TestEnums
{
	public partial class EmptyEnum : Enum<EmptyEnum>
	{
		private EmptyEnum(string key)
			: base(key)
		{
		}

		protected override EmptyEnum CreateInvalid(string key)
		{
			return new(key);
		}
	}
}
