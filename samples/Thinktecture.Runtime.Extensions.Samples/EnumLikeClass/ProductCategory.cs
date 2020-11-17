namespace Thinktecture.EnumLikeClass
{
	public sealed class ProductCategory : Enum<ProductCategory>
	{
		public static readonly ProductCategory Fruits = new("Fruits");
		public static readonly ProductCategory Dairy = new("Dairy");

		private ProductCategory(string key)
			: base(key)
		{
		}

		protected override ProductCategory CreateInvalid(string key)
		{
			return new(key);
		}
	}
}
