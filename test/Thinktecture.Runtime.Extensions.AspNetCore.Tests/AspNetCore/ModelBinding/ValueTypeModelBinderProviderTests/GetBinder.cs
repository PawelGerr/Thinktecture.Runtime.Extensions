using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueTypes;
using Xunit;

namespace Thinktecture.Runtime.Tests.AspNetCore.ModelBinding.ValueTypeModelBinderProviderTests
{
   public class GetBinder
   {
      [Fact]
      public void Should_return_binder_for_int_based_enum()
      {
         var binder = GetModelBinder<IntegerEnum>();
         binder.Should().BeOfType<ValueTypeModelBinder<IntegerEnum, int>>();
      }

      [Fact]
      public void Should_return_binder_for_string_based_enum()
      {
         GetModelBinder<TestEnum>().Should().BeOfType<ValueTypeModelBinder<TestEnum, string>>();

         GetModelBinder<ExtensibleTestEnum>().Should().BeOfType<ValueTypeModelBinder<ExtensibleTestEnum, string>>();
         GetModelBinder<ExtendedTestEnum>().Should().BeOfType<ValueTypeModelBinder<ExtendedTestEnum, string>>();
         GetModelBinder<DifferentAssemblyExtendedTestEnum>().Should().BeOfType<ValueTypeModelBinder<DifferentAssemblyExtendedTestEnum, string>>();
      }

      [Fact]
      public void Should_return_binder_for_string_based_value_type()
      {
         var binder = GetModelBinder<StringBasedReferenceValueType>();
         binder.Should().BeOfType<ValueTypeModelBinder<StringBasedReferenceValueType, string>>();
      }

      [Fact]
      public void Should_return_null_for_non_enums_and_non_value_types()
      {
         GetModelBinder<GetBinder>().Should().BeNull();
      }

      private static IModelBinder GetModelBinder<T>()
      {
         var provider = new ValueTypeModelBinderProvider();
         var contextMock = new Mock<ModelBinderProviderContext>();
         contextMock.Setup(c => c.Metadata).Returns(BindingContextHelper.CreateModelMetadata<T>());
         var serviceProvider = new ServiceCollection().AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance).BuildServiceProvider();
         contextMock.Setup(c => c.Services).Returns(serviceProvider);

         return provider.GetBinder(contextMock.Object);
      }
   }
}
