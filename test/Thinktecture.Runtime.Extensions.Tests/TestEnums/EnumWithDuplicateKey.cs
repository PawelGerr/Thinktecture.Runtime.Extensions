namespace Thinktecture.TestEnums
{
	public partial class EnumWithDuplicateKey : Enum<EnumWithDuplicateKey>
	{
		public static readonly EnumWithDuplicateKey Item = new("Item");
		public static readonly EnumWithDuplicateKey Duplicate = new("item");

		public EnumWithDuplicateKey(string key)
			: base(key)
		{
		}

		protected override EnumWithDuplicateKey CreateInvalid(string key)
		{
			return new(key);
		}
	}
}
