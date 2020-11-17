namespace Thinktecture.TestEnums.Isolated
{
	/// <summary>
	/// This enum may be used in 1 test only.
	/// Otherwise it is initialized and the test is invalid.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class StaticCtorTestEnum_Get : Enum<StaticCtorTestEnum_Get>
	{
		// ReSharper disable once UnusedMember.Global
		public static readonly StaticCtorTestEnum_Get Item = new("item");

		private StaticCtorTestEnum_Get(string key)
			: base(key)
		{
		}

		protected override StaticCtorTestEnum_Get CreateInvalid(string key)
		{
			return new(key);
		}
	}
}
