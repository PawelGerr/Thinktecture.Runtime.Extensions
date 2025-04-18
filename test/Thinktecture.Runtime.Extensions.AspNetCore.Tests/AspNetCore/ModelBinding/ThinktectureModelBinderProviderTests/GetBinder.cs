using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.AspNetCore.ModelBinding.ThinktectureModelBinderProviderTests;

public class GetBinder
{
   [Fact]
   public void Should_return_binder_for_int_based_enum()
   {
      var binder = GetModelBinder<IntegerEnum>();
      binder.Should().BeOfType<ThinktectureModelBinder<IntegerEnum, int, ValidationError>>();
   }

   [Fact]
   public void Should_return_binder_for_int_based_struct()
   {
      var binder = GetModelBinder<TestSmartEnum_Struct_IntBased_Validatable>();
      binder.Should().BeOfType<ThinktectureModelBinder<TestSmartEnum_Struct_IntBased_Validatable, int, ValidationError>>();
   }

   [Fact]
   public void Should_return_binder_for_int_based_struct_nullable()
   {
      var binder = GetModelBinder<TestSmartEnum_Struct_IntBased_Validatable?>();
      binder.Should().BeOfType<ThinktectureModelBinder<TestSmartEnum_Struct_IntBased_Validatable, int, ValidationError>>();
   }

   [Fact]
   public void Should_return_binder_for_string_based_enum()
   {
      GetModelBinder<TestEnum>().Should().BeOfType<ThinktectureModelBinder<TestEnum, string, ValidationError>>();
   }

   [Fact]
   public void Should_return_binder_for_string_based_value_type()
   {
      var binder = GetModelBinder<StringBasedReferenceValueObject>();
      binder.Should().BeOfType<ThinktectureModelBinder<StringBasedReferenceValueObject, string, ValidationError>>();
   }

   [Fact]
   public void Should_return_string_base_binder_specified_by_ObjectFactoryAttribute_of_value_object_()
   {
      var binder = GetModelBinder<BoundaryWithFactories>();
      binder.Should().BeOfType<ThinktectureModelBinder<BoundaryWithFactories, string, ValidationError>>();
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
      binder.Should().BeOfType<ThinktectureModelBinder<KeylessTestEnumWithFactory, string, ValidationError>>();
   }

   [Fact]
   public void Should_return_string_base_binder_specified_by_ObjectFactoryAttribute_smart_enum()
   {
      var binder = GetModelBinder<EnumWithFactory>();
      binder.Should().BeOfType<ThinktectureModelBinder<EnumWithFactory, string, ValidationError>>();
   }

   [Fact]
   public void Should_return_int_base_binder_for_simple_value_object_with_custom_factory_name()
   {
      var binder = GetModelBinder<IntBasedReferenceValueObjectWithCustomFactoryNames>();
      binder.Should().BeOfType<ThinktectureModelBinder<IntBasedReferenceValueObjectWithCustomFactoryNames, int, ValidationError>>();
   }

   [Fact]
   public void Should_return_null_for_non_enums_and_non_value_types()
   {
      GetModelBinder<GetBinder>().Should().BeNull();
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
