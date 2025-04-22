using System;
using System.Diagnostics.CodeAnalysis;
using Thinktecture.SmartEnums;
using Thinktecture.ValueObjects;

namespace Thinktecture;

public class Product
{
   public Guid Id { get; }
   public ProductName Name { get; private set; }
   public ProductCategory Category { get; private set; }
   public ProductType ProductType { get; private set; }
   public OpenEndDate EndDate { get; set; }
   public required DayMonth ScheduledDeliveryDate { get; set; }

   private Boundary? _boundary;
   public Boundary Boundary => _boundary ?? throw new InvalidOperationException("Boundary is not loaded.");

   [SetsRequiredMembers]
   private Product(
      Guid id,
      ProductName name,
      ProductCategory category,
      ProductType productType,
      OpenEndDate endDate,
      DayMonth scheduledDeliveryDate)
   {
      Id = id;
      Name = name;
      Category = category;
      ProductType = productType;
      EndDate = endDate;
      ScheduledDeliveryDate = scheduledDeliveryDate;
   }

   [SetsRequiredMembers]
   public Product(
      Guid id,
      ProductName name,
      ProductCategory category,
      ProductType productType,
      DayMonth scheduledDeliveryDate,
      Boundary boundary,
      OpenEndDate endDate = default)
      : this(id, name, category, productType, endDate, scheduledDeliveryDate)
   {
      _boundary = boundary;
   }
}
