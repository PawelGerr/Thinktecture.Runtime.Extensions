using System;
using System.Collections;
using System.Collections.Generic;

namespace Thinktecture.EmptyClass
{
	public class EmptyCollectionsDemos
	{
		public static void Demo()
		{
			Method_1(Empty.Collection());
			Method_2(Empty.Collection<string>());
			Method_3(Empty.Collection<int>());
			Method_4(Empty.Collection<int>());

			Method_5(Empty.Dictionary<string, int>());
		}

		public static void Method_1(IEnumerable collection)
		{
		}

		public static void Method_2(IEnumerable<string> collection)
		{
		}

		public static void Method_3(IReadOnlyCollection<int> collection)
		{
		}

		public static void Method_4(IReadOnlyList<int> collection)
		{
		}

		public static void Method_5(IReadOnlyDictionary<string, int> dictionary)
		{
		}
	}
}
