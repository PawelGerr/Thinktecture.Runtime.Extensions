using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Runtime.Tests.TestEntities;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Extensions.DbContextOptionsBuilderExtensionsTests;

// ReSharper disable InconsistentNaming
public class UseThinktectureValueConverters : IDisposable
{
   private static readonly Type _validateableConverterType = typeof(ThinktectureValueConverterFactory).GetNestedTypes(BindingFlags.NonPublic)
                                                                                                      .Single(t => t.Name.StartsWith("ValidatableEnumValueConverter", StringComparison.Ordinal));

   private static readonly Type _testEnumConverterType = _validateableConverterType.MakeGenericType(typeof(TestEnum), typeof(string));

   private static readonly Type _converterType = typeof(ThinktectureValueConverterFactory).GetNestedTypes(BindingFlags.NonPublic)
                                                                                          .Single(t => t.Name.StartsWith("ThinktectureValueConverter", StringComparison.Ordinal));

   private readonly TestDbContext _ctx;

   public UseThinktectureValueConverters()
   {
      var options = new DbContextOptionsBuilder<TestDbContext>()
                    .UseSqlite("DataSource=:memory:")
                    .EnableServiceProviderCaching(false)
                    .UseThinktectureValueConverters()
                    .Options;

      _ctx = new TestDbContext(options);
   }

   [Fact]
   public void Should_add_converters_for_structs_and_classes()
   {
      var entityType = _ctx.Model.FindEntityType(typeof(TestEntity_with_Enum_and_ValueObjects));

      ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestEnum), _testEnumConverterType);

      ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestSmartEnum_Class_IntBased), _converterType.MakeGenericType(typeof(TestSmartEnum_Class_IntBased), typeof(int)));
      ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestSmartEnum_Class_StringBased), _converterType.MakeGenericType(typeof(TestSmartEnum_Class_StringBased), typeof(string)));
      ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestSmartEnum_Struct_IntBased), _validateableConverterType.MakeGenericType(typeof(TestSmartEnum_Struct_IntBased_Validatable), typeof(int)));
      ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestSmartEnum_Struct_StringBased), _validateableConverterType.MakeGenericType(typeof(TestSmartEnum_Struct_StringBased_Validatable), typeof(string)));
      ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.NullableTestSmartEnum_Struct_StringBased), _validateableConverterType.MakeGenericType(typeof(TestSmartEnum_Struct_StringBased_Validatable), typeof(string)));
   }

#if COMPLEX_TYPES
   [Fact]
   public void Should_add_converters_for_complex_types()
   {
      var entityType = _ctx.Model.FindEntityType(typeof(TestEntityWithComplexType)) ?? throw new Exception("Entity not found");
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
#endif

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
#if COMPLEX_TYPES
      ITypeBase
#else
      IEntityType
#endif
         entityType,
      string propertyName,
      Type converterType = null)
   {
      var property = entityType.FindProperty(propertyName) ?? throw new Exception($"Property with the name '{propertyName}' not found.");
      property.GetValueConverter().Should().BeOfType(converterType ?? _testEnumConverterType);
   }

   public void Dispose()
   {
      _ctx.Dispose();
   }
}
