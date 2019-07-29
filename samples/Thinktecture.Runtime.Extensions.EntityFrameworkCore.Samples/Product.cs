using System;
using Thinktecture.Runtime.Extensions.Samples.EnumLikeClass;

namespace Thinktecture
{
	public class Product
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public ProductCategory Category { get; set; }
	}
}
