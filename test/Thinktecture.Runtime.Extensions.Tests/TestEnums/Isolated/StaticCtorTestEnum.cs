namespace Thinktecture.TestEnums.Isolated
{
	/// <summary>
	/// This enum may be used in 1 test only.
	/// Otherwise it is initialized and the test is invalid.
	/// </summary>
	public class StaticCtorTestEnum : Enum<StaticCtorTestEnum>
	{
		// ReSharper disable once UnusedMember.Global
		public static readonly StaticCtorTestEnum Item = new StaticCtorTestEnum("item");

		private StaticCtorTestEnum(string key)
			: base(key)
		{
		}

		protected override StaticCtorTestEnum CreateInvalid(string key)
		{
			return new StaticCtorTestEnum(key);
		}
	}
}
