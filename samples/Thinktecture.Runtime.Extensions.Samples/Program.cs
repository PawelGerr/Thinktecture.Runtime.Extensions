using System;
using Newtonsoft.Json;
using Serilog;

namespace Thinktecture.Runtime.Extensions.Samples
{
	public class Program
	{
		public static void Main()
		{
			var logger = GetLogger();

			var categories = ProductCategory.GetAll();
			logger.Information("Catgories: {categories}", categories);

			var category = ProductCategory.Get("Fruits");
			logger.Information("Category: {category}", category);

			string keyOfTheCategory = category;
			logger.Information("Implicit conversion of Category -> string: {key}", keyOfTheCategory);

			// Throws "InvalidOperationException" if not valid
			category.EnsureValid();

			var unknownCategory = ProductCategory.Get("Grains");
			logger.Information("unknownCategory.Key: {categoryKey}", unknownCategory.Key);
			logger.Information("unknownCategory.isValid: {IsValid}", unknownCategory.IsValid);

			DoJsonSerialization(logger, ProductCategory.Fruits);
			DoJsonSerialization(logger, ProductKind.Apple);

			logger.Information("Press ENTER to exit");
			Console.ReadLine();
		}

		private static void DoJsonSerialization(ILogger logger, ProductCategory category)
		{
			var json = JsonConvert.SerializeObject(category);
			logger.Information("ProductCategory -> json: {json}", json);

			var deserializedCategory = JsonConvert.DeserializeObject<ProductCategory>(json);
			logger.Information("json -> ProductCategory: {category}", deserializedCategory);

			logger.Information("Category == Deserialized Category: {bool}", ReferenceEquals(deserializedCategory, category));

			var invalidItem = JsonConvert.DeserializeObject<ProductCategory>("\"invalid item\"");
			logger.Information("Invalid category: {invalid}", invalidItem);
		}

		private static void DoJsonSerialization(ILogger logger, ProductKind kind)
		{
			var json = JsonConvert.SerializeObject(kind);
			logger.Information("ProductKind -> json: {json}", json);

			var deserializedKind = JsonConvert.DeserializeObject<ProductKind>(json);
			logger.Information("json -> ProductKind: {kind}", deserializedKind);
		}

		private static ILogger GetLogger()
		{
			return new LoggerConfiguration()
					.WriteTo.Console()
					.CreateLogger();
		}
	}
}
