using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.TestEnums
{
	public class EnumWithNonReadOnlyField : Enum<EnumWithNonReadOnlyField>
	{
		[SuppressMessage("ReSharper", "CA2211")]
		public static EnumWithNonReadOnlyField Item = new EnumWithNonReadOnlyField("Item");

		private EnumWithNonReadOnlyField(string key)
			: base(key)
		{
		}

		protected override EnumWithNonReadOnlyField CreateInvalid(string key)
		{
			return new EnumWithNonReadOnlyField(key);
		}
	}
}
