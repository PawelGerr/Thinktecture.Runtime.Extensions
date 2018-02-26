using System;

namespace Thinktecture.TestEnums
{
	public class TestEnumWithNonDefaultComparer : Enum<TestEnumWithNonDefaultComparer>
	{
		public static readonly TestEnumWithNonDefaultComparer Item = new TestEnumWithNonDefaultComparer("item");
		public static readonly TestEnumWithNonDefaultComparer AnotherItem = new TestEnumWithNonDefaultComparer("Item");

		static TestEnumWithNonDefaultComparer()
		{
			KeyEqualityComparer = StringComparer.Ordinal;
		}

		private TestEnumWithNonDefaultComparer(string key)
			: base(key)
		{
		}

		protected override TestEnumWithNonDefaultComparer CreateInvalid(string key)
		{
			return new TestEnumWithNonDefaultComparer(key);
		}
	}
}
