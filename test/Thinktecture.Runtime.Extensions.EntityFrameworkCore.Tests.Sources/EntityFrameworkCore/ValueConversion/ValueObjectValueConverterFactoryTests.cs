using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEntities;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.ValueConversion;

public class ValueObjectValueConverterFactoryTests : IDisposable
{
   private readonly TestDbContext _ctx;

   public ValueObjectValueConverterFactoryTests()
   {
      _ctx = new(
         new DbContextOptionsBuilder<TestDbContext>()
            .EnableServiceProviderCaching(false)
            .Options,
         ValueConverterRegistration.OnModelCreating);
      _ctx.Database.OpenConnection();
      _ctx.Database.EnsureCreated();
   }

   [Fact]
   public async Task Should_write_and_read_enums_and_value_types()
   {
      var entity = new TestEntity_with_Enum_and_ValueObjects
                   {
                      Id = new Guid("A53F60CD-B53E-40E3-B16F-05E9A223E238"),
                      TestEnum = TestEnum.Item1,
                      TestEnumWithCustomError = TestEnumWithCustomError.Item1,
                      IntBasedReferenceValueObject = IntBasedReferenceValueObject.Create(42),
                      IntBasedStructValueObject = IntBasedStructValueObject.Create(43),
                      StringBasedReferenceValueObject = StringBasedReferenceValueObject.Create("value 1"),
                      StringBasedStructValueObject = StringBasedStructValueObject.Create("value 2"),
                      StringBasedReferenceValueObjectWithCustomError = StringBasedReferenceValueObjectWithCustomError.Create("value 3"),
                      TestSmartEnum_Struct_IntBased = TestSmartEnum_Struct_IntBased.Value1,
                      TestSmartEnum_Struct_StringBased = TestSmartEnum_Struct_StringBased.Value1,
                      NullableTestSmartEnum_Struct_StringBased = TestSmartEnum_Struct_StringBased.Value1,
                      Boundary = Boundary.Create(10, 20),
                      BoundaryWithCustomError = BoundaryWithCustomError.Create(11, 21),
                      BoundaryWithCustomFactoryNames = BoundaryWithCustomFactoryNames.Get(11, 21),
                      IntBasedReferenceValueObjectWitCustomFactoryName = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1)
                   };
      _ctx.Add(entity);
      await _ctx.SaveChangesAsync();

      _ctx.ChangeTracker.Clear();
      (await _ctx.TestEntities_with_Enum_and_ValueObjects.SingleAsync())
         .Should().BeEquivalentTo(entity);
   }

   [Fact]
   public async Task Should_use_ctor_of_value_types_instead_of_factory_because_DB_is_source_of_truth()
   {
      var entity = new TestEntity_with_Enum_and_ValueObjects
                   {
                      Id = new Guid("A53F60CD-B53E-40E3-B16F-05E9A223E238"),
                      StringBasedReferenceValueObject = StringBasedReferenceValueObject.Create("value"),
                      StringBasedStructValueObject = StringBasedStructValueObject.Create("other value"),
                      TestSmartEnum_Struct_IntBased = TestSmartEnum_Struct_IntBased.Value1,
                      TestSmartEnum_Struct_StringBased = TestSmartEnum_Struct_StringBased.Value1,
                      NullableTestSmartEnum_Struct_StringBased = TestSmartEnum_Struct_StringBased.Value1,
                      Boundary = Boundary.Create(10, 20)
                   };
      _ctx.Add(entity);
      await _ctx.SaveChangesAsync();

      await using var command = _ctx.Database.GetDbConnection().CreateCommand();
      command.CommandText = @"
UPDATE TestEntities_with_Enum_and_ValueObjects
SET
    StringBasedStructValueObject = '',
    Boundary_Lower = 30
";
      await command.ExecuteNonQueryAsync();

      _ctx.ChangeTracker.Clear();
      var loadedEntity = await _ctx.TestEntities_with_Enum_and_ValueObjects.SingleAsync();
      loadedEntity.StringBasedStructValueObject.Property.Should().Be(String.Empty);
      loadedEntity.Boundary.Lower.Should().Be(30);
      loadedEntity.Boundary.Upper.Should().Be(20);
   }

   [Fact]
   public async Task Should_not_roundtrip_convert_underlying_type_to_value_object_and_back_to_underlying_type()
   {
      var item = TestSmartEnum_Struct_IntBased.Value1;
      TestSmartEnum_Struct_IntBased? nullableItem = item;

      var valueObj = IntBasedStructValueObject.Create(42);
      IntBasedStructValueObject? nullableValueObj = valueObj;

      long int64 = 42;
      long? nullableInt64 = int64;
      int int32 = 42;
      int? nullableInt32 = int32;
      short int16 = 42;
      short? nullableInt16 = int16;
      decimal deci = 42;
      decimal? nullableDecimal = deci;

      await _ctx.TestEntities_with_Enum_and_ValueObjects
                .Where(e => e.TestSmartEnum_Struct_IntBased == item
                            && e.TestSmartEnum_Struct_IntBased == nullableItem
                            && e.IntBasedStructValueObject == valueObj
                            && e.IntBasedStructValueObject == nullableValueObj
                            && e.IntBasedStructValueObject == int64
                            && e.TestSmartEnum_Struct_IntBased == int64
                            && e.IntBasedStructValueObject == nullableInt64
                            && e.TestSmartEnum_Struct_IntBased == nullableInt64
                            && e.IntBasedStructValueObject == int32
                            && e.TestSmartEnum_Struct_IntBased == int32
                            && e.IntBasedStructValueObject == nullableInt32
                            && e.TestSmartEnum_Struct_IntBased == nullableInt32
                            && e.IntBasedStructValueObject == int16
                            && e.TestSmartEnum_Struct_IntBased == int16
                            && e.IntBasedStructValueObject == nullableInt16
                            && e.TestSmartEnum_Struct_IntBased == nullableInt16
                            && e.IntBasedStructValueObject == deci
                            && e.TestSmartEnum_Struct_IntBased == deci
                            && e.IntBasedStructValueObject == nullableDecimal
                            && e.TestSmartEnum_Struct_IntBased == nullableDecimal)
                .ToListAsync();
   }

#if COMPLEX_TYPES
   [Fact]
   public async Task Should_roundtrip_complex_value_object_with_complex_property()
   {
      var entity = ComplexValueObjectWithComplexType.Create(
         new Guid("00B7B411-A95E-41F7-91C8-29384431E21A"),
         new TestComplexType { TestEnum = TestEnum.Item1 });

      _ctx.Add(entity);
      await _ctx.SaveChangesAsync();

      _ctx.ChangeTracker.Clear();

      var loadedEntity = await _ctx.ComplexValueObject_with_ComplexType.SingleAsync();
      loadedEntity.Id.Should().Be(entity.Id);
      loadedEntity.TestComplexType.Should().BeEquivalentTo(entity.TestComplexType);
   }
#endif

   public void Dispose()
   {
      _ctx.Dispose();
   }
}
