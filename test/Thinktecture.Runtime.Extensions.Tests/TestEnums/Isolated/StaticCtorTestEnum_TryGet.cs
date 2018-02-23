namespace Thinktecture.TestEnums.Isolated
{
	/// <summary>
	/// This enum may be used in 1 test only.
	/// Otherwise it is initialized and the test is invalid.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class StaticCtorTestEnum_TryGet : Enum<StaticCtorTestEnum_TryGet>
	{
		// ReSharper disable once UnusedMember.Global
		public static readonly StaticCtorTestEnum_TryGet Item = new StaticCtorTestEnum_TryGet("item");

		private StaticCtorTestEnum_TryGet(string key)
			: base(key)
		{
		}

		protected override StaticCtorTestEnum_TryGet CreateInvalid(string key)
		{
			return new StaticCtorTestEnum_TryGet(key);
		}
	}
}
