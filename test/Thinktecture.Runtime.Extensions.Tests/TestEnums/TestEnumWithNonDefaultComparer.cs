﻿using System;

namespace Thinktecture.TestEnums
{
	public class TestEnumWithNonDefaultComparer : Enum<TestEnumWithNonDefaultComparer>
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
