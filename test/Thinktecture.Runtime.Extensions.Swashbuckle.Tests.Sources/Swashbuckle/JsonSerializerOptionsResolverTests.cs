using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Thinktecture.Swashbuckle.Internal;

namespace Thinktecture.Runtime.Tests.Swashbuckle;

public class JsonSerializerOptionsResolverTests
{
   [Fact]
   public void Should_use_http_json_options_when_mvc_is_not_registered()
   {
      var services = CreateServices();
      using var serviceProvider = services.BuildServiceProvider();

      var options = new JsonSerializerOptionsResolver(serviceProvider).JsonSerializerOptions;

      options.Should().BeSameAs(GetHttpJsonOptions(serviceProvider));
   }

   [Fact]
   public void Should_use_mvc_json_options_when_mvc_is_registered()
   {
      var services = CreateServices();
      services.AddSingleton(Substitute.For<IControllerFactory>());
      using var serviceProvider = services.BuildServiceProvider();

      var options = new JsonSerializerOptionsResolver(serviceProvider).JsonSerializerOptions;

      options.Should().BeSameAs(GetMvcJsonOptions(serviceProvider));
   }

   [Fact]
   public void Should_ignore_json_serializer_options_registered_in_di()
   {
      var registeredOptions = new JsonSerializerOptions();
      var services = CreateServices();
      services.AddSingleton(registeredOptions);
      using var serviceProvider = services.BuildServiceProvider();

      var options = new JsonSerializerOptionsResolver(serviceProvider).JsonSerializerOptions;

      options.Should().BeSameAs(GetHttpJsonOptions(serviceProvider));
      options.Should().NotBeSameAs(registeredOptions);
   }

   private static ServiceCollection CreateServices()
   {
      var services = new ServiceCollection();
      services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(_ => { });
      services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(_ => { });

      return services;
   }

   private static JsonSerializerOptions GetHttpJsonOptions(ServiceProvider serviceProvider)
   {
      return serviceProvider.GetRequiredService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>().Value.SerializerOptions;
   }

   private static JsonSerializerOptions GetMvcJsonOptions(ServiceProvider serviceProvider)
   {
      return serviceProvider.GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>().Value.JsonSerializerOptions;
   }
}
