using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEntities;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueTypes;
using Xunit;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.ValueConversion
{
   public class ValueTypeValueConverterFactoryTests : IDisposable
   {
      private readonly TestDbContext _ctx;

      public ValueTypeValueConverterFactoryTests()
      {
         _ctx = new();
         _ctx.Database.OpenConnection();
         _ctx.Database.EnsureCreated();
      }

      [Fact]
      public async Task Should_write_and_read_enums_and_value_types()
      {
         var entity = new TestEntity_with_Enum_and_ValueTypes
                      {
                         Id = new Guid("A53F60CD-B53E-40E3-B16F-05E9A223E238"),
                         TestEnum = TestEnum.Item1,
                         IntBasedReferenceValueType = IntBasedReferenceValueType.Create(42),
                         IntBasedStructValueType = IntBasedStructValueType.Create(43),
                         StringBasedReferenceValueType = StringBasedReferenceValueType.Create("value 1"),
                         StringBasedStructValueType = StringBasedStructValueType.Create("value 2"),
                         Boundary = Boundary.Create(10, 20)
                      };
         _ctx.Add(entity);
         await _ctx.SaveChangesAsync();

         _ctx.ChangeTracker.Clear();
         (await _ctx.TestEntities_with_Enum_and_ValueTypes.SingleAsync())
            .Should().BeEquivalentTo(entity);
      }

      [Fact]
      public async Task Should_use_ctor_of_value_types_instead_of_factory_because_EF_is_source_of_truth()
      {
         var entity = new TestEntity_with_Enum_and_ValueTypes
                      {
                         Id = new Guid("A53F60CD-B53E-40E3-B16F-05E9A223E238"),
                         StringBasedReferenceValueType = StringBasedReferenceValueType.Create("value"),
                         StringBasedStructValueType = StringBasedStructValueType.Create("other value"),
                         Boundary = Boundary.Create(10, 20)
                      };
         _ctx.Add(entity);
         await _ctx.SaveChangesAsync();

         await using var command = _ctx.Database.GetDbConnection().CreateCommand();
         command.CommandText = @"
UPDATE TestEntities_with_Enum_and_ValueTypes
SET
    StringBasedStructValueType = '',
    Boundary_Lower = 30
";
         await command.ExecuteNonQueryAsync();

         _ctx.ChangeTracker.Clear();
         var loadedEntity = await _ctx.TestEntities_with_Enum_and_ValueTypes.SingleAsync();
         loadedEntity.StringBasedStructValueType.Property.Should().Be(String.Empty);
         loadedEntity.Boundary.Lower.Should().Be(30);
         loadedEntity.Boundary.Upper.Should().Be(20);
      }

      public void Dispose()
      {
         _ctx.Dispose();
      }
   }
}
