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
                      SmartEnum_StringBased = SmartEnum_StringBased.Item1,
                      TestSmartEnum_CustomError = TestSmartEnum_CustomError.Item1,
                      IntBasedReferenceValueObject = IntBasedReferenceValueObject.Create(42),
                      IntBasedStructValueObject = IntBasedStructValueObject.Create(43),
                      StringBasedReferenceValueObject = StringBasedReferenceValueObject.Create("value 1"),
                      StringBasedStructValueObject = StringBasedStructValueObject.Create("value 2"),
                      StringBasedReferenceValueObjectWithCustomError = StringBasedReferenceValueObjectWithCustomError.Create("value 3"),
                      Boundary = Boundary.Create(10, 20),
                      BoundaryWithCustomError = BoundaryWithCustomError.Create(11, 21),
                      BoundaryWithCustomFactoryNames = BoundaryWithCustomFactoryNames.Get(11, 21),
                      IntBasedReferenceValueObjectWitCustomFactoryName = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1),
                      TestComplexValueObject_ObjectFactory = TestComplexValueObject_ObjectFactory.Create("value 4", "value 5"),
                      TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("value 6", "value 7"),
                      CustomObject_ObjectFactory = new CustomObject_ObjectFactory("value 8", "value 9"),
                      SmartEnum_IntBased = SmartEnum_IntBased.Item2,
                      CollectionOfIntBasedReferenceValueObject =
                      [
                         IntBasedReferenceValueObject.Create(1),
                         IntBasedReferenceValueObject.Create(2)
                      ],
                   };
      _ctx.Add(entity);
      await _ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

      _ctx.ChangeTracker.Clear();
      (await _ctx.TestEntities_with_Enum_and_ValueObjects.SingleAsync(TestContext.Current.CancellationToken))
         .Should().BeEquivalentTo(entity);
   }

   [Fact]
   public async Task Should_use_ctor_of_value_types_instead_of_factory_because_DB_is_source_of_truth()
   {
      var entity = new TestEntity_with_Enum_and_ValueObjects
                   {
                      Id = new Guid("A53F60CD-B53E-40E3-B16F-05E9A223E238"),
                      StringBasedReferenceValueObject = StringBasedReferenceValueObject.Create("value 1"),
                      StringBasedStructValueObject = StringBasedStructValueObject.Create("value 2"),
                      Boundary = Boundary.Create(10, 20),
                      TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("value 3", "value 4"),
                   };
      _ctx.Add(entity);
      await _ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

      await using var command = _ctx.Database.GetDbConnection().CreateCommand();
      command.CommandText = @"
UPDATE TestEntities_with_Enum_and_ValueObjects
SET
    StringBasedStructValueObject = '',
    Boundary_Lower = 30,
    TestComplexValueObject_ObjectFactory_and_Constructor = ''
";
      await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);

      _ctx.ChangeTracker.Clear();
      var loadedEntity = await _ctx.TestEntities_with_Enum_and_ValueObjects.SingleAsync(TestContext.Current.CancellationToken);
      loadedEntity.StringBasedStructValueObject.Property.Should().Be(String.Empty);
      loadedEntity.Boundary.Lower.Should().Be(30);
      loadedEntity.Boundary.Upper.Should().Be(20);
      loadedEntity.TestComplexValueObject_ObjectFactory_and_Constructor.Property1.Should().BeEmpty();
      loadedEntity.TestComplexValueObject_ObjectFactory_and_Constructor.Property2.Should().BeEmpty();
   }

   [Fact]
   public async Task Should_not_roundtrip_convert_underlying_type_to_value_object_and_back_to_underlying_type()
   {
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
                .Where(e => e.IntBasedStructValueObject == valueObj
                            && e.IntBasedStructValueObject == nullableValueObj
                            && e.IntBasedStructValueObject == int64
                            && e.IntBasedStructValueObject == nullableInt64
                            && e.IntBasedStructValueObject == int32
                            && e.IntBasedStructValueObject == nullableInt32
                            && e.IntBasedStructValueObject == int16
                            && e.IntBasedStructValueObject == nullableInt16
                            && e.IntBasedStructValueObject == deci
                            && e.IntBasedStructValueObject == nullableDecimal)
                .ToListAsync(TestContext.Current.CancellationToken);
   }

   [Fact]
   public async Task Should_roundtrip_complex_value_object_with_complex_property()
   {
      var entity = ComplexValueObjectWithComplexType.Create(
         new Guid("00B7B411-A95E-41F7-91C8-29384431E21A"),
         new TestComplexType { TestEnum = SmartEnum_StringBased.Item1 });

      _ctx.Add(entity);
      await _ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

      _ctx.ChangeTracker.Clear();

      var loadedEntity = await _ctx.ComplexValueObject_with_ComplexType.SingleAsync(TestContext.Current.CancellationToken);
      loadedEntity.Id.Should().Be(entity.Id);
      loadedEntity.TestComplexType.Should().BeEquivalentTo(entity.TestComplexType);
   }

   public void Dispose()
   {
      _ctx.Dispose();
   }
}
