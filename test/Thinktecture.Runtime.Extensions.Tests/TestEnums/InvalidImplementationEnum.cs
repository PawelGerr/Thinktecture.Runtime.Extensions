using System;

namespace Thinktecture.TestEnums
{
	public partial class InvalidImplementationEnum : Enum<TestEnum>
	{
		public InvalidImplementationEnum(string key)
			: base(key)
		{
		}

		protected override TestEnum CreateInvalid(string key)
		{
			throw new NotImplementedException();
		}
	}
}
