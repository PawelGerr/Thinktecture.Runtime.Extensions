using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Thinktecture.AspNetCore.ModelBinding.TestClasses;
using Xunit;

namespace Thinktecture.AspNetCore.ModelBinding.ValueTypeModelBinderProviderTests
{
   public class GetBinder
   {
      [Fact]
      public void Should_return_binder_for_int_based_enum()
      {
         var binder = GetModelBinder<IntBasedEnum>();
         binder.Should().BeOfType<ValueTypeModelBinder<IntBasedEnum, int>>();
      }

      [Fact]
      public void Should_return_binder_for_string_based_enum()
      {
         var binder = GetModelBinder<StringBasedEnum>();
         binder.Should().BeOfType<ValueTypeModelBinder<StringBasedEnum, string>>();
      }

      [Fact]
      public void Should_return_binder_for_string_based_value_type()
      {
         var binder = GetModelBinder<StringBasedValueType>();
         binder.Should().BeOfType<ValueTypeModelBinder<StringBasedValueType, string>>();
      }

      [Fact]
      public void Should_return_null_for_non_enums_and_non_value_types()
      {
         var binder = GetModelBinder<GetBinder>();
         binder.Should().BeNull();
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
