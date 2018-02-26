namespace Thinktecture.TestEnums
{
	public class EnumWithDuplicateKey : Enum<EnumWithDuplicateKey>
	{
		public static readonly EnumWithDuplicateKey Item = new EnumWithDuplicateKey("Item");
		public static readonly EnumWithDuplicateKey Duplicate = new EnumWithDuplicateKey("item");

		public EnumWithDuplicateKey(string key)
			: base(key)
		{
		}

		protected override EnumWithDuplicateKey CreateInvalid(string key)
		{
			return new EnumWithDuplicateKey(key);
		}
	}
}
