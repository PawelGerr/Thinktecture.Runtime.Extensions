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
      public SpecialProductType ProductType { get; private set; }
      public Boundary Boundary { get; private set; }

      // For EF (see also https://github.com/dotnet/efcore/issues/12078)
#pragma warning disable 8618
      private Product(Guid id, ProductName name, ProductCategory category, SpecialProductType productType)
      {
         Id = id;
         Name = name;
         Category = category;
         ProductType = productType;
      }
#pragma warning restore 8618

      public Product(Guid id, ProductName name, ProductCategory category, SpecialProductType productType, Boundary boundary)
         : this(id, name, category, productType)
      {
         Boundary = boundary;
      }
   }
}
