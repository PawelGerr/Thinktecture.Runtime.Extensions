namespace Thinktecture.Runtime.Extensions.Samples
{
	public class ProductKind : Enum<ProductKind, int>
	{
		public static readonly ProductKind Apple = new ProductKind(1, "Apple", ProductCategory.Fruits);
		public static readonly ProductKind Orange = new ProductKind(2, "Orange", ProductCategory.Fruits);

		public string DisplayName { get; }
		public ProductCategory Category { get; }

		private ProductKind(int key, string displayName, ProductCategory category)
			: base(key)
		{
			DisplayName = displayName;
			Category = category;
		}

		protected override ProductKind CreateInvalid(int key)
		{
			return new ProductKind(key, "Unknown product kind", ProductCategory.Get("Unknown"));
		}
	}
}
