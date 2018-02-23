namespace Thinktecture.Runtime.Extensions.Samples
{
	public class ProductCategory : Enum<ProductCategory>
	{
		public static readonly ProductCategory Fruits = new ProductCategory("Fruits");
		public static readonly ProductCategory Dairy = new ProductCategory("Dairy");

		private ProductCategory(string key)
			: base(key)
		{
		}

		protected override ProductCategory CreateInvalid(string key)
		{
			return new ProductCategory(key);
		}
	}
}
