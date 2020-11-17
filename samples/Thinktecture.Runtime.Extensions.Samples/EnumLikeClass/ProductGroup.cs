namespace Thinktecture.EnumLikeClass
{
	public sealed class ProductGroup : Enum<ProductGroup, int>
	{
		public static readonly ProductGroup Apple = new(1, "Apple", ProductCategory.Fruits);
		public static readonly ProductGroup Orange = new(2, "Orange", ProductCategory.Fruits);

		public string DisplayName { get; }
		public ProductCategory Category { get; }

		private ProductGroup(int key, string displayName, ProductCategory category)
			: base(key)
		{
			DisplayName = displayName;
			Category = category;
		}

		public int Do(string foo)
		{
			EnsureValid(); // "Do()" is not allowed for invalid items

			// do something

			return 42;
		}

		protected override ProductGroup CreateInvalid(int key)
		{
			// the values can be anything besides the key,
			// the key must not be null
			return new(key, "Unknown product group", ProductCategory.Get("Unknown"));
		}
	}
}
