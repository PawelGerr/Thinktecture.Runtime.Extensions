using System;
using Thinktecture.EnumLikeClasses;
using Thinktecture.ValueTypes;

namespace Thinktecture
{
   public class Product
   {
      public Guid Id { get; private set; }
      public ProductName Name { get; private set; }
      public ProductCategory Category { get; private set; }

      public Product(Guid id, ProductName name, ProductCategory category)
      {
         Id = id;
         Name = name;
         Category = category;
      }
   }
}
