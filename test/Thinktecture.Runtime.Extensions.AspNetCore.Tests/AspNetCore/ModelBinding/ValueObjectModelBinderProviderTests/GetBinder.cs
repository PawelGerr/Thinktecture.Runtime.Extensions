using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.AspNetCore.ModelBinding.ValueObjectModelBinderProviderTests;

public class GetBinder
{
   [Fact]
   public void Should_return_binder_for_int_based_enum()
   {
      var binder = GetModelBinder<IntegerEnum>();
      binder.Should().BeOfType<ValueObjectModelBinder<IntegerEnum, int, ValidationError>>();
   }

   [Fact]
   public void Should_return_binder_for_string_based_enum()
   {
      GetModelBinder<TestEnum>().Should().BeOfType<TrimmingSmartEnumModelBinder<TestEnum, ValidationError>>();
   }

   [Fact]
   public void Should_return_binder_for_string_based_value_type()
   {
      var binder = GetModelBinder<StringBasedReferenceValueObject>();
      binder.Should().BeOfType<ValueObjectModelBinder<StringBasedReferenceValueObject, string, ValidationError>>();
   }

   [Fact]
   public void Should_return_string_base_binder_specified_by_ValueObjectFactoryAttribute_of_value_object_()
   {
      var binder = GetModelBinder<BoundaryWithFactories>();
      binder.Should().BeOfType<ValueObjectModelBinder<BoundaryWithFactories, string, ValidationError>>();
   }

   [Fact]
   public void Should_return_null_for_keyless_enum()
   {
      var binder = GetModelBinder<KeylessTestEnum>();
      binder.Should().BeNull();
   }

   [Fact]
   public void Should_return_binder_for_keyless_enum_with_factory()
   {
      var binder = GetModelBinder<KeylessTestEnumWithFactory>();
      binder.Should().BeOfType<ValueObjectModelBinder<KeylessTestEnumWithFactory, string, ValidationError>>();
   }

   [Fact]
   public void Should_return_string_base_binder_specified_by_ValueObjectFactoryAttribute_smart_enum()
   {
      var binder = GetModelBinder<EnumWithFactory>();
      binder.Should().BeOfType<TrimmingSmartEnumModelBinder<EnumWithFactory, ValidationError>>();
   }

   [Fact]
   public void Should_return_null_for_non_enums_and_non_value_types()
   {
      GetModelBinder<GetBinder>().Should().BeNull();
   }

   private static IModelBinder GetModelBinder<T>()
   {
      var provider = new ValueObjectModelBinderProvider();
      var contextMock = Substitute.For<ModelBinderProviderContext>();
      contextMock.Metadata.Returns(BindingContextHelper.CreateModelMetadata<T>());
      var serviceProvider = new ServiceCollection().AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance).BuildServiceProvider();
      contextMock.Services.Returns(serviceProvider);
      contextMock.BindingInfo.Returns(new BindingInfo());

      return provider.GetBinder(contextMock);
   }
}
