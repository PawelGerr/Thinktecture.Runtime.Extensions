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
      _ctx = new(true);
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
                      IntBasedReferenceValueObject = IntBasedReferenceValueObject.Create(42),
                      IntBasedStructValueObject = IntBasedStructValueObject.Create(43),
                      StringBasedReferenceValueObject = StringBasedReferenceValueObject.Create("value 1"),
                      StringBasedStructValueObject = StringBasedStructValueObject.Create("value 2"),
                      TestSmartEnum_Struct_IntBased = TestSmartEnum_Struct_IntBased.Value1,
                      TestSmartEnum_Struct_StringBased = TestSmartEnum_Struct_StringBased.Value1,
                      Boundary = Boundary.Create(10, 20)
                   };
      _ctx.Add(entity);
      await _ctx.SaveChangesAsync();

      _ctx.ChangeTracker.Clear();
      (await _ctx.TestEntities_with_Enum_and_ValueObjects.SingleAsync())
         .Should().BeEquivalentTo(entity);
   }

   [Fact]
   public async Task Should_use_ctor_of_value_types_instead_of_factory_because_EF_is_source_of_truth()
   {
      var entity = new TestEntity_with_Enum_and_ValueObjects
                   {
                      Id = new Guid("A53F60CD-B53E-40E3-B16F-05E9A223E238"),
                      StringBasedReferenceValueObject = StringBasedReferenceValueObject.Create("value"),
                      StringBasedStructValueObject = StringBasedStructValueObject.Create("other value"),
                      TestSmartEnum_Struct_IntBased = TestSmartEnum_Struct_IntBased.Value1,
                      TestSmartEnum_Struct_StringBased = TestSmartEnum_Struct_StringBased.Value1,
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

   public void Dispose()
   {
      _ctx.Dispose();
   }
}
