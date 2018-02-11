using System;

namespace Thinktecture.EnumClassTests.Enums
{
	public class TestEnumWithNonDefaultComparer : EnumClass<TestEnumWithNonDefaultComparer>
	{
		public static readonly TestEnumWithNonDefaultComparer Item1 = new TestEnumWithNonDefaultComparer("item1");

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