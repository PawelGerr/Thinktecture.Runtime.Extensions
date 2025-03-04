using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.AspNetCore.ModelBinding.ValueObjectModelBinderTests;

// ReSharper disable once InconsistentNaming
public class BindModelAsync
{
   [Fact]
   public async Task Should_bind_int_based_enum()
   {
      var ctx = await BindAsync<IntegerEnum, int>("1");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(IntegerEnum.Item1);
   }

   [Fact]
   public async Task Should_return_null_if_value_is_empty_string()
   {
      var ctx = await BindAsync<IntegerEnum, int>(String.Empty);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_not_bind_if_value_is_null()
   {
      var ctx = await BindAsync<IntegerEnum, int>(null);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_validatable_enum_even_if_key_is_unknown()
   {
      var ctx = await BindAsync<IntegerEnum, int>("42");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();

      var value = (IntegerEnum)ctx.Result.Model;
      value!.IsValid.Should().BeFalse();
      value!.Key.Should().Be(42);
   }

   [Fact]
   public async Task Should_not_bind_validatable_enum_if_value_not_matching_key_type()
   {
      var ctx = await BindAsync<IntegerEnum, int>("item1");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("The value 'item1' is not valid.")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_not_bind_enum_if_key_is_unknown()
   {
      var ctx = await BindAsync<ValidIntegerEnum, int>("42");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("There is no item of type 'ValidIntegerEnum' with the identifier '42'.")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_not_bind_enum_if_value_not_matching_key_type()
   {
      var ctx = await BindAsync<ValidIntegerEnum, int>("item1");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("The value 'item1' is not valid.")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_string_based_enum()
   {
      var ctx = await BindAsync<TestEnum, string>("item1");
      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(TestEnum.Item1);
   }

   [Fact]
   public async Task Should_bind_string_based_value_type()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObject, string>("Value");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(StringBasedReferenceValueObject.Create("Value"));
   }

   [Fact]
   public async Task Should_bind_string_based_value_type_with_NullInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>("Value");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.Create("Value"));
   }

   [Fact]
   public async Task Should_bind_null_with_NullInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>(null);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeFalse();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_empty_string_with_NullInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>(String.Empty);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_whitespaces_with_NullInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>(" ");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_string_based_value_type_with_StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>("Value");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.Create("Value"));
   }

   [Fact]
   public async Task Should_bind_null_with_StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>(null);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeFalse();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_empty_string_with_StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>(String.Empty);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_whitespaces_with_StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>(" ");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_return_error_if_value_violates_validation_rules()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObject, string>("A");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("Property cannot be 1 character long.")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_successfully_value_object_having_string_base_factory_specified_by_ValueObjectFactoryAttribute()
   {
      var ctx = await BindAsync<BoundaryWithFactories, string>("1:2");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeEquivalentTo(BoundaryWithFactories.Create(1, 2));
   }

   [Fact]
   public async Task Should_return_error_when_binding_value_object_having_string_base_factory_specified_by_ValueObjectFactoryAttribute()
   {
      var ctx = await BindAsync<BoundaryWithFactories, string>("1");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("Invalid format.")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_successfully_smart_enum_having_string_base_factory_specified_by_ValueObjectFactoryAttribute()
   {
      var ctx = await BindAsync<EnumWithFactory, string>("=1=");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(EnumWithFactory.Item1);
   }

   [Fact]
   public async Task Should_return_error_when_binding_smart_enum_having_string_base_factory_specified_by_ValueObjectFactoryAttribute()
   {
      var ctx = await BindAsync<EnumWithFactory, string>("A");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("Unknown item 'A'")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_successfully_keyless_enum_with_factory()
   {
      var ctx = await BindAsync<KeylessTestEnumWithFactory, string>("1");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(KeylessTestEnumWithFactory.Item1);
   }

   [Fact]
   public async Task Should_return_error_when_binding_keyless_enum_with_factories_with_invalid_intput()
   {
      var ctx = await BindAsync<KeylessTestEnumWithFactory, string>("A");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("Unknown item 'A'")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_simple_value_object_having_custom_factory_name()
   {
      var ctx = await BindAsync<IntBasedReferenceValueObjectWithCustomFactoryNames, int>("1");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEmpty();
      ctx.Result.Model.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1));
   }

   private static async Task<DefaultModelBindingContext> BindAsync<T, TKey>(
      string value)
      where T : IValueObjectFactory<T, TKey, ValidationError>
   {
      var binder = new ValueObjectModelBinder<T, TKey, ValidationError>(NullLoggerFactory.Instance);
      var query = new Dictionary<string, StringValues> { { "name", value } };

      var ctx = new DefaultModelBindingContext
                {
                   ModelName = "name",
                   ValueProvider = new QueryStringValueProvider(BindingSource.Query, new QueryCollection(query), CultureInfo.InvariantCulture),
                   ModelState = new ModelStateDictionary(),
                   ModelMetadata = BindingContextHelper.CreateModelMetadata<T>()
                };

      await binder.BindModelAsync(ctx);

      return ctx;
   }
}
