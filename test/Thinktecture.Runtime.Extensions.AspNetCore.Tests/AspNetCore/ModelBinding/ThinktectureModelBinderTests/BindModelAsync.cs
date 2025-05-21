using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.AspNetCore.ModelBinding.ThinktectureModelBinderTests;

// ReSharper disable once InconsistentNaming
public class BindModelAsync
{
   [Fact]
   public void Should_try_bind_enum_when_value_is_null_or_default()
   {
      // class - int
      BindNot<SmartEnum_IntBased>(null);
      FluentActions.Invoking(() => Bind<SmartEnum_IntBased>("0")).Should().Throw<Exception>().WithMessage("There is no item of type 'SmartEnum_IntBased' with the identifier '0'.");

      // class - string
      BindNot<SmartEnum_StringBased>(null);

      // class - class
      BindNot<SmartEnum_ClassBased>(null);
   }

   [Fact]
   public void Should_try_bind_keyed_value_object_when_value_is_null_or_default()
   {
      // class - int
      BindNot<IntBasedReferenceValueObject>(null);

      // class - string
      BindNot<StringBasedReferenceValueObject>(null);

      // class - class
      BindNot<ClassBasedReferenceValueObject>(null);

      // nullable struct - int
      BindNot<IntBasedStructValueObject?>(null);
      Bind<IntBasedStructValueObject?>("0").Should().Be(IntBasedStructValueObject.Create(0));

      // struct - int
      Bind<IntBasedStructValueObject>("0").Should().Be(IntBasedStructValueObject.Create(0)); // AllowDefaultStructs = true
      BindNot<IntBasedStructValueObject>(null);

      Bind<IntBasedStructValueObjectDoesNotAllowDefaultStructs>("0").Should().Be(IntBasedStructValueObjectDoesNotAllowDefaultStructs.Create(0)); // AllowDefaultStructs = true

      // nullable struct - string
      BindNot<StringBasedStructValueObject?>(null);

      // struct - string
      BindNot<StringBasedStructValueObject>(null); // AllowDefaultStructs = false;

      // struct - class
      BindNot<ReferenceTypeBasedStructValueObjectDoesNotAllowDefaultStructs>(null); // AllowDefaultStructs = false
   }

   [Fact]
   public async Task Should_bind_int_based_enum()
   {
      var ctx = await BindAsync<SmartEnum_IntBased>("1");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(SmartEnum_IntBased.Item1);
   }

   [Fact]
   public async Task Should_return_null_if_value_is_empty_string()
   {
      var ctx = await BindAsync<SmartEnum_IntBased>(String.Empty);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_not_bind_enum_if_key_is_unknown()
   {
      var ctx = await BindAsync<SmartEnum_IntBased>("42");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("There is no item of type 'SmartEnum_IntBased' with the identifier '42'.")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_not_bind_enum_if_value_not_matching_key_type()
   {
      var ctx = await BindAsync<SmartEnum_IntBased>("item1");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("The value 'item1' is not valid.")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_string_based_enum()
   {
      var ctx = await BindAsync<SmartEnum_StringBased>("item1");
      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public async Task Should_bind_string_based_value_type()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObject>("Value");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(StringBasedReferenceValueObject.Create("Value"));
   }

   [Fact]
   public async Task Should_bind_string_based_value_type_with_NullInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull>("Value");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.Create("Value"));
   }

   [Fact]
   public async Task Should_not_set_model_when_trying_to_bind_null()
   {
      // ASP.NET or rather IQueryCollection doesn't distinguish between null and none
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull>(null);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeFalse();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_empty_string_with_NullInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull>(String.Empty);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_whitespaces_with_NullInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull>(" ");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_string_based_value_type_with_StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull>("Value");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.Create("Value"));
   }

   [Fact]
   public async Task Should_bind_null_with_StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull()
   {
      // ASP.NET or rather IQueryCollection doesn't distinguish between null and none
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull>(null);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeFalse();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_empty_string_with_StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull>(String.Empty);

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_bind_whitespaces_with_StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull>(" ");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeNull();
   }

   [Fact]
   public async Task Should_return_error_if_value_violates_validation_rules()
   {
      var ctx = await BindAsync<StringBasedReferenceValueObject>("A");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("Property cannot be 1 character long.")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_successfully_value_object_having_string_base_factory_specified_by_ObjectFactoryAttribute()
   {
      var ctx = await BindAsync<BoundaryWithFactories>("1:2");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().BeEquivalentTo(BoundaryWithFactories.Create(1, 2));
   }

   [Fact]
   public async Task Should_return_error_when_binding_value_object_having_string_base_factory_specified_by_ObjectFactoryAttribute()
   {
      var ctx = await BindAsync<BoundaryWithFactories>("1");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("Invalid format.")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_successfully_smart_enum_having_string_base_factory_specified_by_ObjectFactoryAttribute()
   {
      var ctx = await BindAsync<SmartEnum_Factory>("=1=");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(SmartEnum_Factory.Item1);
   }

   [Fact]
   public async Task Should_return_error_when_binding_smart_enum_having_string_base_factory_specified_by_ObjectFactoryAttribute()
   {
      var ctx = await BindAsync<SmartEnum_Factory>("A");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("Unknown item 'A'")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_successfully_keyless_enum_with_factory()
   {
      var ctx = await BindAsync<SmartEnum_Keyless_ObjectFactory>("1");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.Result.IsModelSet.Should().BeTrue();
      ctx.Result.Model.Should().Be(SmartEnum_Keyless_ObjectFactory.Item1);
   }

   [Fact]
   public async Task Should_return_error_when_binding_keyless_enum_with_factories_with_invalid_intput()
   {
      var ctx = await BindAsync<SmartEnum_Keyless_ObjectFactory>("A");

      ctx.ModelState.ErrorCount.Should().Be(1);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEquivalentTo([new ModelError("Unknown item 'A'")]);
      ctx.Result.IsModelSet.Should().BeFalse();
   }

   [Fact]
   public async Task Should_bind_simple_value_object_having_custom_factory_name()
   {
      var ctx = await BindAsync<IntBasedReferenceValueObjectWithCustomFactoryNames>("1");

      ctx.ModelState.ErrorCount.Should().Be(0);
      ctx.ModelState[ctx.ModelName]!.Errors.Should().BeEmpty();
      ctx.Result.Model.Should().Be(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1));
   }

   private static T Bind<T>(string value)
   {
      var context = BindAsync<T>(value).GetAwaiter().GetResult();

      if (!context.ModelState.IsValid)
         throw new Exception(context.ModelState["name"]!.Errors[0].ErrorMessage);

      if (!context.Result.IsModelSet)
         throw new Exception("Model is not set");

      return (T)context.Result.Model;
   }

   private static void BindNot<T>(string value)
   {
      // ASP.NET or rather IQueryCollection doesn't distinguish between null and none
      var context = BindAsync<T>(value).GetAwaiter().GetResult();

      if (!context.ModelState.IsValid)
         throw new Exception(context.ModelState["name"]!.Errors[0].ErrorMessage);

      if (context.Result.IsModelSet)
         throw new Exception("Model msut not be set");

      if (context.Result.Model is not null)
         throw new Exception("Model must be null");
   }

   private static async Task<DefaultModelBindingContext> BindAsync<T>(string value)
   {
      var query = new Dictionary<string, StringValues> { { "name", value } };

      var ctx = new DefaultModelBindingContext
                {
                   ModelName = "name",
                   ValueProvider = new QueryStringValueProvider(BindingSource.Query, new QueryCollection(query), CultureInfo.InvariantCulture),
                   ModelState = new ModelStateDictionary(),
                   ModelMetadata = BindingContextHelper.CreateModelMetadata<T>()
                };

      var binder = GetModelBinder<T>();

      binder.GetType().GetGenericTypeDefinition().Should().Be(typeof(ThinktectureModelBinder<,,>));

      await binder.BindModelAsync(ctx);

      return ctx;
   }

   private static IModelBinder GetModelBinder<T>()
   {
      var provider = new ThinktectureModelBinderProvider();
      var contextMock = Substitute.For<ModelBinderProviderContext>();
      contextMock.Metadata.Returns(BindingContextHelper.CreateModelMetadata<T>());
      var serviceProvider = new ServiceCollection().AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance).BuildServiceProvider();
      contextMock.Services.Returns(serviceProvider);
      contextMock.BindingInfo.Returns(new BindingInfo());

      return provider.GetBinder(contextMock);
   }
}
