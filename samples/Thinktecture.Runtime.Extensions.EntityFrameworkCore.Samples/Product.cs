using System;
using Thinktecture.EnumLikeClasses;
using Thinktecture.ValueTypes;

#nullable disable

namespace Thinktecture
{
	public class Product
	{
		public Guid Id { get; set; }
		public ProductName Name { get; set; }
		public ProductCategory Category { get; set; }
	}
}
