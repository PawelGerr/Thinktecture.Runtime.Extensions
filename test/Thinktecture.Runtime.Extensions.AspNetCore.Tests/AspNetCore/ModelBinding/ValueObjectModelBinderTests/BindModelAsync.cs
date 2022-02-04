using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.Internal;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.AspNetCore.ModelBinding.ValueObjectModelBinderTests;

// ReSharper disable once InconsistentNaming
public class BindModelAsync
{
   [Fact]
   public async Task Should_bind_int_based_enum()
   {
      var ctx = await BindAsync<IntegerEnum, int>(IntegerEnum.Validate, "1");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(IntegerEnum.Item1);
   }

   [Fact]
   public async Task Should_return_null_if_value_is_empty_string()
   {
      var ctx = await BindAsync<IntegerEnum, int>(IntegerEnum.Validate, String.Empty);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_not_bind_if_value_is_null()
   {
      var ctx = await BindAsync<IntegerEnum, int>(IntegerEnum.Validate, null);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_return_error_if_value_not_exists_in_int_based_enum()
   {
      var ctx = await BindAsync<IntegerEnum, int>(IntegerEnum.Validate, "42");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName].Errors.Should().BeEquivalentTo(new[] { new ModelError("The enumeration item of type \"IntegerEnum\" with identifier \"42\" is not valid.") });
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_return_error_if_value_not_matching_key_type_of_an_int_based_enum()
   {
      var ctx = await BindAsync<IntegerEnum, int>(IntegerEnum.Validate, "item1");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName].Errors.Should().BeEquivalentTo(new[] { new ModelError("The value 'item1' is not valid.") });
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_string_based_enum()
   {
      var ctx = await BindAsync<TestEnum, string>(TestEnum.Validate, "item1");
      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(TestEnum.Item1);

      ctx = await BindAsync<ExtensibleTestEnum, string>(ExtensibleTestEnum.Validate, "item1");
      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(ExtensibleTestEnum.Item1);

      ctx = await BindAsync<ExtendedTestEnum, string>(ExtendedTestEnum.Validate, "item1");
      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(ExtendedTestEnum.Item1);

      ctx = await BindAsync<DifferentAssemblyExtendedTestEnum, string>(DifferentAssemblyExtendedTestEnum.Validate, "item1");
      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(DifferentAssemblyExtendedTestEnum.Item1);
   }

   [Fact]
   public async Task Should_bind_string_based_value_type()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObject, string>(StringBasedReferenceValueObject.TryCreate, "Value");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(StringBasedReferenceValueObject.Create("Value"));
   }

   [Fact]
   public async Task Should_return_error_if_value_violates_validation_rules()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObject, string>(StringBasedReferenceValueObject.TryCreate, "A");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName].Errors.Should().BeEquivalentTo(new[] { new ModelError("Property cannot be 1 character long.") });
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   private static async Task<DefaultModelBindingContext> BindAsync<T, TKey>(
      Validate<T, TKey> validate,
      string value)
   {
      var binder = new ValueObjectModelBinder<T, TKey>(NullLoggerFactory.Instance, validate);
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
