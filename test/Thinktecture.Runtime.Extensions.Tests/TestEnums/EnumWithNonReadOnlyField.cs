namespace Thinktecture.TestEnums
{
	public class EnumWithNonReadOnlyField : Enum<EnumWithNonReadOnlyField>
	{
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
