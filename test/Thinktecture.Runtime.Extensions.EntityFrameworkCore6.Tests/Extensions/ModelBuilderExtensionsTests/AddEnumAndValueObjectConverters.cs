using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Runtime.Tests.TestEntities;
using Thinktecture.Runtime.Tests.TestEnums;

#if EFCORE5
using Microsoft.EntityFrameworkCore;
#endif

// ReSharper disable InconsistentNaming
namespace Thinktecture.Runtime.Tests.Extensions.ModelBuilderExtensionsTests
{
   public class AddEnumAndValueObjectConverters : IDisposable
   {
      private static readonly Type _validateableConverterType = typeof(ValueObjectValueConverterFactory).GetNestedTypes(BindingFlags.NonPublic)
                                                                                                        .Single(t => t.Name.StartsWith("ValidatableEnumValueConverter", StringComparison.Ordinal));

      private static readonly Type _testEnumConverterType = _validateableConverterType.MakeGenericType(typeof(TestEnum), typeof(string));

      private static readonly Type _converterType = typeof(ValueObjectValueConverterFactory).GetNestedTypes(BindingFlags.NonPublic)
                                                                                            .Single(t => t.Name.StartsWith("ValueObjectValueConverter", StringComparison.Ordinal));

      private readonly TestDbContext _ctx = new(true);

      [Fact]
      public void Should_add_converters_for_structs_and_classes()
      {
         var entityType = _ctx.Model.FindEntityType(typeof(TestEntity_with_Enum_and_ValueObjects));

         ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestEnum), _testEnumConverterType);

         ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestSmartEnum_Class_IntBased), _converterType.MakeGenericType(typeof(TestSmartEnum_Class_IntBased), typeof(int)));
         ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestSmartEnum_Class_StringBased), _converterType.MakeGenericType(typeof(TestSmartEnum_Class_StringBased), typeof(string)));
         ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestSmartEnum_Struct_IntBased), _validateableConverterType.MakeGenericType(typeof(TestSmartEnum_Struct_IntBased), typeof(int)));
         ValidateConverter(entityType, nameof(TestEntity_with_Enum_and_ValueObjects.TestSmartEnum_Struct_StringBased), _validateableConverterType.MakeGenericType(typeof(TestSmartEnum_Struct_StringBased), typeof(string)));
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

      private static void ValidateConverter(IEntityType entityType, string propertyName, Type converterType = null)
      {
         entityType.FindProperty(propertyName).GetValueConverter().Should().BeOfType(converterType ?? _testEnumConverterType);
      }

      public void Dispose()
      {
         _ctx.Dispose();
      }
   }
}