#pragma warning disable RCS1213 // Remove unused member declaration.
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Thinktecture.TestEnums
{
	public class TestEnum : EnumClass<TestEnum>
	{
		public static readonly TestEnum Item1 = new TestEnum("item1");
		public static readonly TestEnum Item2 = new TestEnum("item2");
		protected static readonly TestEnum Item3 = new TestEnum("item3");
		internal static readonly TestEnum Item4 = new TestEnum("item4");
		private static readonly TestEnum Item5 = new TestEnum("item5");
		public static TestEnum Item6 { get; } = new TestEnum("item6");
		public static readonly IntegerEnum Item7 = IntegerEnum.Item1;
		public static readonly IntegerEnum Item8 = IntegerEnum.Get(42);

		private TestEnum(string key)
			: base(key)
		{
		}

		protected override TestEnum CreateInvalid(string key)
		{
			return new TestEnum(key);
		}
	}
}
