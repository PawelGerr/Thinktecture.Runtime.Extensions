using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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
      binder.Should().BeOfType<ValueObjectModelBinder<IntegerEnum, int>>();
   }

   [Fact]
   public void Should_return_binder_for_string_based_enum()
   {
      GetModelBinder<TestEnum>().Should().BeOfType<TrimmingSmartEnumModelBinder<TestEnum>>();

      GetModelBinder<ExtensibleTestEnum>().Should().BeOfType<TrimmingSmartEnumModelBinder<ExtensibleTestEnum>>();
      GetModelBinder<ExtendedTestEnum>().Should().BeOfType<TrimmingSmartEnumModelBinder<ExtendedTestEnum>>();
      GetModelBinder<DifferentAssemblyExtendedTestEnum>().Should().BeOfType<TrimmingSmartEnumModelBinder<DifferentAssemblyExtendedTestEnum>>();
   }

   [Fact]
   public void Should_return_binder_for_string_based_value_type()
   {
      var binder = GetModelBinder<StringBasedReferenceValueObject>();
      binder.Should().BeOfType<ValueObjectModelBinder<StringBasedReferenceValueObject, string>>();
   }

   [Fact]
   public void Should_return_null_for_non_enums_and_non_value_types()
   {
      GetModelBinder<GetBinder>().Should().BeNull();
   }

   private static IModelBinder GetModelBinder<T>()
   {
      var provider = new ValueObjectModelBinderProvider();
      var contextMock = new Mock<ModelBinderProviderContext>();
      contextMock.Setup(c => c.Metadata).Returns(BindingContextHelper.CreateModelMetadata<T>());
      var serviceProvider = new ServiceCollection().AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance).BuildServiceProvider();
      contextMock.Setup(c => c.Services).Returns(serviceProvider);
      contextMock.Setup(c => c.BindingInfo).Returns(new BindingInfo());

      return provider.GetBinder(contextMock.Object);
   }
}
