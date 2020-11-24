using System;

namespace Thinktecture.TestEnums
{
	public partial class TestEnumWithNonDefaultComparer : Enum<TestEnumWithNonDefaultComparer>
	{
		public static readonly TestEnumWithNonDefaultComparer Item = new("item");
		public static readonly TestEnumWithNonDefaultComparer AnotherItem = new("Item");

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
			return new(key);
		}
	}
}
