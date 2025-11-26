using System;
using System.Linq;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Thinktecture.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Runtime.Tests.TestEntities;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable InconsistentNaming
namespace Thinktecture.Runtime.Tests.Extensions.EntityTypeBuilderExtensionsTests
{
   public class AddThinktectureValueConverters : IDisposable
   {
      private static readonly Type _converterType = typeof(ThinktectureValueConverterFactory).GetNestedTypes(BindingFlags.NonPublic)
                                                                                             .Single(t => t.Name.StartsWith("ThinktectureValueConverter", StringComparison.Ordinal));
      private static readonly Type _stringSmartEnumConverterType = _converterType.MakeGenericType(typeof(SmartEnum_StringBased), typeof(string), typeof(ValidationError));

      private readonly TestDbContext _ctx = new(new DbContextOptionsBuilder<TestDbContext>()
                                                .UseSqlite("DataSource=:memory:")
                                                .EnableServiceProviderCaching(false)
                                                .Options,
                                                ValueConverterRegistration.EntityConfiguration);

      [Fact]
      public void Should_add_converters_for_structs_and_classes()
      {
         var entityType = _ctx.Model.FindEntityType(typeof(TestEntity_with_Enum_and_ValueObjects));

         ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.SmartEnum_IntBased), _converterType.MakeGenericType(typeof(SmartEnum_IntBased), typeof(int), typeof(ValidationError)));
         ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.SmartEnum_StringBased));
      }

      [Fact]
      public void Should_add_converters_for_type_with_object_factories()
      {
         var entityType = _ctx.Model.FindEntityType(typeof(TestEntity_with_Types_having_ObjectFactories));

         ValidateConverter(
            entityType,
            nameof(TestEntity_with_Types_having_ObjectFactories.TestComplexValueObject_ObjectFactory),
            _converterType.MakeGenericType(typeof(TestComplexValueObject_ObjectFactory), typeof(string), typeof(ValidationError)));

         ValidateConverter(
            entityType,
            nameof(TestEntity_with_Types_having_ObjectFactories.TestComplexValueObject_ObjectFactory_and_Constructor),
            _converterType.MakeGenericType(typeof(TestComplexValueObject_ObjectFactory_and_Constructor), typeof(string), typeof(ValidationError)));

         ValidateConverter(
            entityType,
            nameof(TestEntity_with_Types_having_ObjectFactories.CustomObject_ObjectFactory),
            _converterType.MakeGenericType(typeof(CustomObject_ObjectFactory), typeof(string), typeof(ValidationError)));
      }

      [Fact]
      public void Should_add_converters_for_complex_types()
      {
         var entityType = _ctx.Model.FindEntityType(typeof(TestEntityWithComplexType)) ?? throw new Exception("Entity not found");
         var complexProperty = entityType.FindComplexProperty(nameof(TestEntityWithComplexType.TestComplexType)) ?? throw new Exception("Complex type property not found");

         ValidateConverter(complexProperty.ComplexType, nameof(TestComplexType.TestEnum));
      }

      [Fact]
      public void Should_add_converters_for_complex_types_inside_complex_type_configuration()
      {
         using var ctx = new TestDbContext(
            new DbContextOptionsBuilder<TestDbContext>()
               .UseSqlite("DataSource=:memory:")
               .EnableServiceProviderCaching(false)
               .Options,
            ValueConverterRegistration.ComplexTypeConfiguration);

         var entityType = ctx.Model.FindEntityType(typeof(TestEntityWithComplexType)) ?? throw new Exception("Entity not found");
         var complexProperty = entityType.FindComplexProperty(nameof(TestEntityWithComplexType.TestComplexType)) ?? throw new Exception("Complex type property not found");

         ValidateConverter(complexProperty.ComplexType, nameof(TestComplexType.TestEnum));
      }

      [Fact]
      public void Should_add_converters_for_complex_types_inside_complex_value_object()
      {
         var entityType = _ctx.Model.FindEntityType(typeof(ComplexValueObjectWithComplexType)) ?? throw new Exception("Entity not found");
         var complexProperty = entityType.FindComplexProperty(nameof(ComplexValueObjectWithComplexType.TestComplexType)) ?? throw new Exception("Complex type property not found");

         ValidateConverter(complexProperty.ComplexType, nameof(TestComplexType.TestEnum));
      }

      [Fact]
      public void Should_add_converters_for_complex_value_object_as_complex_type()
      {
         var entityType = _ctx.Model.FindEntityType(typeof(TestEntityWithComplexValueObjectAsComplexType)) ?? throw new Exception("Entity not found");
         var complexProperty = entityType.FindComplexProperty(nameof(TestEntityWithComplexValueObjectAsComplexType.TestComplexType)) ?? throw new Exception("Complex type property not found");

         ValidateConverter(complexProperty.ComplexType, nameof(TestComplexValueObject.TestEnum));
      }

      [Fact]
      public void Should_add_converters_for_owned_types()
      {
         var entityType = _ctx.Model.FindEntityType(typeof(TestEntity_with_OwnedTypes));
         ValidateConverter(entityType, nameof(TestEntity_with_OwnedTypes.TestEnum));

         var inline_inline = entityType.FindNavigation(nameof(TestEntity_with_OwnedTypes.Inline_Inline)).TargetEntityType;
         var inline_inline_inner = inline_inline.FindNavigation(nameof(OwnedEntity_Owns_Inline.InlineEntity)).TargetEntityType;
         ValidateConverter(inline_inline, nameof(TestEntity_with_OwnedTypes.TestEnum));
         ValidateConverter(inline_inline_inner, nameof(TestEntity_with_OwnedTypes.TestEnum));

         var inline_separateOne = entityType.FindNavigation(nameof(TestEntity_with_OwnedTypes.Inline_SeparateOne)).TargetEntityType;
         var inline_separateOne_inner = inline_separateOne.FindNavigation(nameof(OwnedEntity_Owns_SeparateOne.SeparateEntity)).TargetEntityType;
         ValidateConverter(inline_separateOne, nameof(TestEntity_with_OwnedTypes.TestEnum));
         ValidateConverter(inline_separateOne_inner, nameof(TestEntity_with_OwnedTypes.TestEnum));

         var inline_SeparateMany = entityType.FindNavigation(nameof(TestEntity_with_OwnedTypes.Inline_SeparateMany)).TargetEntityType;
         var inline_SeparateMany_inner = inline_SeparateMany.FindNavigation(nameof(OwnedEntity_Owns_SeparateMany.SeparateEntities)).TargetEntityType;
         ValidateConverter(inline_SeparateMany, nameof(TestEntity_with_OwnedTypes.TestEnum));
         ValidateConverter(inline_SeparateMany_inner, nameof(TestEntity_with_OwnedTypes.TestEnum));

         var separateMany_Inline = entityType.FindNavigation(nameof(TestEntity_with_OwnedTypes.SeparateMany_Inline)).TargetEntityType;
         var separateMany_Inline_inner = separateMany_Inline.FindNavigation(nameof(OwnedEntity_Owns_Inline.InlineEntity)).TargetEntityType;
         ValidateConverter(separateMany_Inline, nameof(TestEntity_with_OwnedTypes.TestEnum));
         ValidateConverter(separateMany_Inline_inner, nameof(TestEntity_with_OwnedTypes.TestEnum));

         var separateMany_SeparateOne = entityType.FindNavigation(nameof(TestEntity_with_OwnedTypes.SeparateMany_SeparateOne)).TargetEntityType;
         var separateMany_SeparateOne_inner = separateMany_SeparateOne.FindNavigation(nameof(OwnedEntity_Owns_SeparateOne.SeparateEntity)).TargetEntityType;
         ValidateConverter(separateMany_SeparateOne, nameof(TestEntity_with_OwnedTypes.TestEnum));
         ValidateConverter(separateMany_SeparateOne_inner, nameof(TestEntity_with_OwnedTypes.TestEnum));

         var separateMany_SeparateMany = entityType.FindNavigation(nameof(TestEntity_with_OwnedTypes.SeparateMany_SeparateMany)).TargetEntityType;
         var separateMany_SeparateMany_inner = separateMany_SeparateMany.FindNavigation(nameof(OwnedEntity_Owns_SeparateMany.SeparateEntities)).TargetEntityType;
         ValidateConverter(separateMany_SeparateMany, nameof(TestEntity_with_OwnedTypes.TestEnum));
         ValidateConverter(separateMany_SeparateMany_inner, nameof(TestEntity_with_OwnedTypes.TestEnum));

         var separateOne_Inline = entityType.FindNavigation(nameof(TestEntity_with_OwnedTypes.SeparateOne_Inline)).TargetEntityType;
         var separateOne_Inline_inner = separateOne_Inline.FindNavigation(nameof(OwnedEntity_Owns_Inline.InlineEntity)).TargetEntityType;
         ValidateConverter(separateOne_Inline, nameof(TestEntity_with_OwnedTypes.TestEnum));
         ValidateConverter(separateOne_Inline_inner, nameof(TestEntity_with_OwnedTypes.TestEnum));

         var separateOne_SeparateOne = entityType.FindNavigation(nameof(TestEntity_with_OwnedTypes.SeparateOne_SeparateOne)).TargetEntityType;
         var separateOne_SeparateOne_inner = separateOne_SeparateOne.FindNavigation(nameof(OwnedEntity_Owns_SeparateOne.SeparateEntity)).TargetEntityType;
         ValidateConverter(separateOne_SeparateOne, nameof(TestEntity_with_OwnedTypes.TestEnum));
         ValidateConverter(separateOne_SeparateOne_inner, nameof(TestEntity_with_OwnedTypes.TestEnum));

         var separateOne_SeparateMany = entityType.FindNavigation(nameof(TestEntity_with_OwnedTypes.SeparateOne_SeparateMany)).TargetEntityType;
         var separateOne_SeparateMany_inner = separateOne_SeparateMany.FindNavigation(nameof(OwnedEntity_Owns_SeparateMany.SeparateEntities)).TargetEntityType;
         ValidateConverter(separateOne_SeparateMany, nameof(TestEntity_with_OwnedTypes.TestEnum));
         ValidateConverter(separateOne_SeparateMany_inner, nameof(TestEntity_with_OwnedTypes.TestEnum));
      }

      private static void ValidateConverter(
         ITypeBase entityType, string propertyName, Type converterType = null)
      {
         var property = entityType.FindProperty(propertyName) ?? throw new Exception($"Property with the name '{propertyName}' not found.");
         property.GetValueConverter().Should().BeOfType(converterType ?? _stringSmartEnumConverterType);
      }

      [Fact]
      public void Should_persist_and_retrieve_smart_enums_with_default_configuration()
      {
         using var sqliteConnection = new SqliteConnection("DataSource=:memory:;cache=shared");
         sqliteConnection.Open();

         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite(sqliteConnection)
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters()
                       .Options;

         using var ctx = new TestDbContext(options, setConnectionString: false);
         ctx.Database.EnsureCreated();

         var entity = new TestEntity_with_Enum_and_ValueObjects
                      {
                         Id = Guid.NewGuid(),
                         SmartEnum_StringBased = SmartEnum_StringBased.Item1,
                         SmartEnum_IntBased = SmartEnum_IntBased.Item2,
                         StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest1"),
                         TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop1", "entityprop2")
                      };

         ctx.TestEntities_with_Enum_and_ValueObjects.Add(entity);
         ctx.SaveChanges();

         ctx.ChangeTracker.Clear();

         var retrieved = ctx.TestEntities_with_Enum_and_ValueObjects.Single(e => e.Id == entity.Id);
         retrieved.SmartEnum_StringBased.Should().Be(SmartEnum_StringBased.Item1);
         retrieved.SmartEnum_IntBased.Should().Be(SmartEnum_IntBased.Item2);
      }

      [Fact]
      public void Should_apply_default_max_length_for_string_based_smart_enums()
      {
         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite("DataSource=:memory:")
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters()
                       .Options;

         using var ctx = new TestDbContext(options);
         var entityType = ctx.Model.FindEntityType(typeof(TestEntity_with_Enum_and_ValueObjects));
         var property = entityType.FindProperty(nameof(TestEntity_with_Enum_and_ValueObjects.SmartEnum_StringBased));

         property.GetMaxLength().Should().Be(10);
      }

      [Fact]
      public void Should_not_apply_max_length_to_int_based_smart_enums()
      {
         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite("DataSource=:memory:")
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters()
                       .Options;

         using var ctx = new TestDbContext(options);
         var entityType = ctx.Model.FindEntityType(typeof(TestEntity_with_Enum_and_ValueObjects));
         var property = entityType.FindProperty(nameof(TestEntity_with_Enum_and_ValueObjects.SmartEnum_IntBased));

         // Int-based smart enums should not have max length
         property.GetMaxLength().Should().BeNull();
      }

      [Fact]
      public void Should_not_apply_max_length_when_using_NoMaxLength_configuration()
      {
         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite("DataSource=:memory:")
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters(Configuration.NoMaxLength)
                       .Options;

         using var ctx = new TestDbContext(options);
         var entityType = ctx.Model.FindEntityType(typeof(TestEntity_with_Enum_and_ValueObjects));
         var property = entityType.FindProperty(nameof(TestEntity_with_Enum_and_ValueObjects.SmartEnum_StringBased));

         // With NoMaxLength configuration, no max length should be set
         property.GetMaxLength().Should().BeNull();
      }

      [Fact]
      public void Should_persist_and_retrieve_with_NoMaxLength_configuration()
      {
         using var sqliteConnection = new SqliteConnection("DataSource=:memory:;cache=shared");
         sqliteConnection.Open();

         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite(sqliteConnection)
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters(Configuration.NoMaxLength)
                       .Options;

         using var ctx = new TestDbContext(options, setConnectionString: false);
         ctx.Database.EnsureCreated();

         var entity = new TestEntity_with_Enum_and_ValueObjects
                      {
                         Id = Guid.NewGuid(),
                         SmartEnum_StringBased = SmartEnum_StringBased.Item2,
                         SmartEnum_IntBased = SmartEnum_IntBased.Item3,
                         StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest2"),
                         TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop3", "entityprop4")
                      };

         ctx.TestEntities_with_Enum_and_ValueObjects.Add(entity);
         ctx.SaveChanges();

         ctx.ChangeTracker.Clear();

         var retrieved = ctx.TestEntities_with_Enum_and_ValueObjects.Single(e => e.Id == entity.Id);
         retrieved.SmartEnum_StringBased.Should().Be(SmartEnum_StringBased.Item2);
         retrieved.SmartEnum_IntBased.Should().Be(SmartEnum_IntBased.Item3);
      }

      [Fact]
      public void Should_apply_fixed_max_length_with_custom_configuration()
      {
         var customConfig = new Configuration
                            {
                               SmartEnums = new SmartEnumConfiguration
                                            {
                                               MaxLengthStrategy = new FixedSmartEnumMaxLengthStrategy(100)
                                            },
                               KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength
                            };

         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite("DataSource=:memory:")
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters(customConfig)
                       .Options;

         using var ctx = new TestDbContext(options);
         var entityType = ctx.Model.FindEntityType(typeof(TestEntity_with_Enum_and_ValueObjects));
         var property = entityType.FindProperty(nameof(TestEntity_with_Enum_and_ValueObjects.SmartEnum_StringBased));

         // Custom configuration with FixedSmartEnumMaxLengthStrategy(100) should set max length to 100
         property.GetMaxLength().Should().Be(100);
      }

      [Fact]
      public void Should_persist_and_retrieve_with_custom_fixed_max_length()
      {
         using var sqliteConnection = new SqliteConnection("DataSource=:memory:;cache=shared");
         sqliteConnection.Open();

         var customConfig = new Configuration
                            {
                               SmartEnums = new SmartEnumConfiguration
                                            {
                                               MaxLengthStrategy = new FixedSmartEnumMaxLengthStrategy(50)
                                            },
                               KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength
                            };

         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite(sqliteConnection)
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters(customConfig)
                       .Options;

         using var ctx = new TestDbContext(options, setConnectionString: false);
         ctx.Database.EnsureCreated();

         var entity = new TestEntity_with_Enum_and_ValueObjects
                      {
                         Id = Guid.NewGuid(),
                         SmartEnum_StringBased = SmartEnum_StringBased.Item1,
                         SmartEnum_IntBased = SmartEnum_IntBased.Item4,
                         StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest3"),
                         TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop5", "entityprop6")
                      };

         ctx.TestEntities_with_Enum_and_ValueObjects.Add(entity);
         ctx.SaveChanges();

         ctx.ChangeTracker.Clear();

         var retrieved = ctx.TestEntities_with_Enum_and_ValueObjects.Single(e => e.Id == entity.Id);
         retrieved.SmartEnum_StringBased.Should().Be(SmartEnum_StringBased.Item1);
         retrieved.SmartEnum_IntBased.Should().Be(SmartEnum_IntBased.Item4);
      }

      [Fact]
      public void Should_handle_null_smart_enum_values()
      {
         using var sqliteConnection = new SqliteConnection("DataSource=:memory:;cache=shared");
         sqliteConnection.Open();

         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite(sqliteConnection)
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters()
                       .Options;

         using var ctx = new TestDbContext(options, setConnectionString: false);
         ctx.Database.EnsureCreated();

         var entity = new TestEntity_with_Enum_and_ValueObjects
                      {
                         Id = Guid.NewGuid(),
                         SmartEnum_StringBased = null,
                         SmartEnum_IntBased = SmartEnum_IntBased.Item1,
                         StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest4"),
                         TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop7", "entityprop8")
                      };

         ctx.TestEntities_with_Enum_and_ValueObjects.Add(entity);
         ctx.SaveChanges();

         ctx.ChangeTracker.Clear();

         var retrieved = ctx.TestEntities_with_Enum_and_ValueObjects.Single(e => e.Id == entity.Id);
         retrieved.SmartEnum_StringBased.Should().BeNull();
         retrieved.SmartEnum_IntBased.Should().Be(SmartEnum_IntBased.Item1);
      }

      [Fact]
      public void Should_work_with_multiple_entities_in_same_context()
      {
         using var sqliteConnection = new SqliteConnection("DataSource=:memory:;cache=shared");
         sqliteConnection.Open();

         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite(sqliteConnection)
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters()
                       .Options;

         using var ctx = new TestDbContext(options, setConnectionString: false);
         ctx.Database.EnsureCreated();

         var entity1 = new TestEntity_with_Enum_and_ValueObjects
                       {
                          Id = Guid.NewGuid(),
                          SmartEnum_StringBased = SmartEnum_StringBased.Item1,
                          SmartEnum_IntBased = SmartEnum_IntBased.Item1,
                          StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest5"),
                          TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop9", "entityprop10")
                       };

         var entity2 = new TestEntity_with_Enum_and_ValueObjects
                       {
                          Id = Guid.NewGuid(),
                          SmartEnum_StringBased = SmartEnum_StringBased.Item2,
                          SmartEnum_IntBased = SmartEnum_IntBased.Item2,
                          StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest6"),
                          TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop11", "entityprop12")
                       };

         ctx.TestEntities_with_Enum_and_ValueObjects.AddRange(entity1, entity2);
         ctx.SaveChanges();

         ctx.ChangeTracker.Clear();

         var retrieved = ctx.TestEntities_with_Enum_and_ValueObjects.OrderBy(e => e.Id).ToList();
         retrieved.Should().HaveCount(2);
      }

      [Fact]
      public void Should_apply_custom_max_length_strategy()
      {
         var customConfig = new Configuration
                            {
                               SmartEnums = new SmartEnumConfiguration
                                            {
                                               MaxLengthStrategy = new CustomSmartEnumMaxLengthStrategy((_, keyType, _) =>
                                               {
                                                  // Custom logic: always return 42 for string-based enums
                                                  if (keyType == typeof(string))
                                                     return new MaxLengthChange(42);

                                                  return MaxLengthChange.None;
                                               })
                                            },
                               KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength
                            };

         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite("DataSource=:memory:")
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters(customConfig)
                       .Options;

         using var ctx = new TestDbContext(options);
         var entityType = ctx.Model.FindEntityType(typeof(TestEntity_with_Enum_and_ValueObjects));
         var property = entityType.FindProperty(nameof(TestEntity_with_Enum_and_ValueObjects.SmartEnum_StringBased));

         // Custom strategy should set max length to 42
         property.GetMaxLength().Should().Be(42);
      }

      [Fact]
      public void Should_update_existing_entities()
      {
         using var sqliteConnection = new SqliteConnection("DataSource=:memory:;cache=shared");
         sqliteConnection.Open();

         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite(sqliteConnection)
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters()
                       .Options;

         using var ctx = new TestDbContext(options, setConnectionString: false);
         ctx.Database.EnsureCreated();

         // Create and save initial entity
         var entity = new TestEntity_with_Enum_and_ValueObjects
                      {
                         Id = Guid.NewGuid(),
                         SmartEnum_StringBased = SmartEnum_StringBased.Item1,
                         SmartEnum_IntBased = SmartEnum_IntBased.Item1,
                         StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest7"),
                         TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop13", "entityprop14")
                      };

         ctx.TestEntities_with_Enum_and_ValueObjects.Add(entity);
         ctx.SaveChanges();

         ctx.ChangeTracker.Clear();

         // Update the entity
         var toUpdate = ctx.TestEntities_with_Enum_and_ValueObjects.Single(e => e.Id == entity.Id);
         toUpdate.SmartEnum_StringBased = SmartEnum_StringBased.Item2;
         toUpdate.SmartEnum_IntBased = SmartEnum_IntBased.Item2;
         ctx.SaveChanges();

         ctx.ChangeTracker.Clear();

         // Verify update
         var retrieved = ctx.TestEntities_with_Enum_and_ValueObjects.Single(e => e.Id == entity.Id);
         retrieved.SmartEnum_StringBased.Should().Be(SmartEnum_StringBased.Item2);
         retrieved.SmartEnum_IntBased.Should().Be(SmartEnum_IntBased.Item2);
      }

      [Fact]
      public void Should_work_with_queries()
      {
         using var sqliteConnection = new SqliteConnection("DataSource=:memory:;cache=shared");
         sqliteConnection.Open();

         var options = new DbContextOptionsBuilder<TestDbContext>()
                       .UseSqlite(sqliteConnection)
                       .EnableServiceProviderCaching(false)
                       .UseThinktectureValueConverters()
                       .Options;

         using var ctx = new TestDbContext(options, setConnectionString: false);
         ctx.Database.EnsureCreated();

         var entities = new[]
                        {
                           new TestEntity_with_Enum_and_ValueObjects
                           {
                              Id = Guid.NewGuid(),
                              SmartEnum_StringBased = SmartEnum_StringBased.Item1,
                              SmartEnum_IntBased = SmartEnum_IntBased.Item1,
                              StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest8"),
                              TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop15", "entityprop16")
                           },
                           new TestEntity_with_Enum_and_ValueObjects
                           {
                              Id = Guid.NewGuid(),
                              SmartEnum_StringBased = SmartEnum_StringBased.Item2,
                              SmartEnum_IntBased = SmartEnum_IntBased.Item2,
                              StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest9"),
                              TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop17", "entityprop18")
                           },
                           new TestEntity_with_Enum_and_ValueObjects
                           {
                              Id = Guid.NewGuid(),
                              SmartEnum_StringBased = SmartEnum_StringBased.Item1,
                              SmartEnum_IntBased = SmartEnum_IntBased.Item3,
                              StringBasedStructValueObject = StringBasedStructValueObject.Create("entitytest10"),
                              TestComplexValueObject_ObjectFactory_and_Constructor = TestComplexValueObject_ObjectFactory_and_Constructor.Create("entityprop19", "entityprop20")
                           }
                        };

         ctx.TestEntities_with_Enum_and_ValueObjects.AddRange(entities);
         ctx.SaveChanges();

         ctx.ChangeTracker.Clear();

         // Query with Where clause
         var item1Results = ctx.TestEntities_with_Enum_and_ValueObjects
                               .Where(e => e.SmartEnum_StringBased == SmartEnum_StringBased.Item1)
                               .ToList();

         item1Results.Should().HaveCount(2);
         item1Results.Should().AllSatisfy(e => e.SmartEnum_StringBased.Should().Be(SmartEnum_StringBased.Item1));
      }

      public void Dispose()
      {
         _ctx.Dispose();
      }
   }
}
