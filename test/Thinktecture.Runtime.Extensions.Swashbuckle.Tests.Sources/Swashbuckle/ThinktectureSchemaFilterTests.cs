using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using NSubstitute;
using Thinktecture.Runtime.Tests.Swashbuckle.Helpers;
using Thinktecture.Swashbuckle;

namespace Thinktecture.Runtime.Tests.Swashbuckle;

public abstract partial class ThinktectureSchemaFilterTests : IAsyncLifetime
{
   private readonly ITestOutputHelper _testOutputHelper;

   private WebApplication? _app;
   private WebApplication App => _app ??= CreateApp();

   private SmartEnumSchemaFilter _smartEnumFilter = SmartEnumSchemaFilter.Default;
   private SmartEnumSchemaExtension _smartEnumExtension = SmartEnumSchemaExtension.None;
   private RequiredMemberEvaluator _requiredMemberEvaluator = RequiredMemberEvaluator.Default;
   private bool _nonNullableReferenceTypesAsRequired;
   private bool _useOneOfForPolymorphism;
   private Type? _controllerType;

   protected ThinktectureSchemaFilterTests(ITestOutputHelper testOutputHelper)
   {
      _testOutputHelper = testOutputHelper;
   }

   private WebApplication CreateApp()
   {
      var appBuilder = WebApplication.CreateBuilder();
      appBuilder.Services
                .AddSingleton(Substitute.For<IHostLifetime>())
                .AddSingleton<IServer, TestServer>()
                .AddLogging(builder => builder.AddXUnit(_testOutputHelper))
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(options =>
                {
                   options.SwaggerDoc("test", new OpenApiInfo { Title = "Test API", Version = "v1" });
                   options.TagActionsBy(_ => ["Tests"]);
                   options.SupportNonNullableReferenceTypes();
                   options.UseAllOfToExtendReferenceSchemas();

                   if (_nonNullableReferenceTypesAsRequired)
                      options.NonNullableReferenceTypesAsRequired();

                   if (_useOneOfForPolymorphism)
                      options.UseOneOfForPolymorphism();
                })
                .AddThinktectureOpenApiFilters(filterOptions =>
                {
                   filterOptions.SmartEnumSchemaFilter = _smartEnumFilter;
                   filterOptions.SmartEnumSchemaExtension = _smartEnumExtension;
                   filterOptions.RequiredMemberEvaluator = _requiredMemberEvaluator;
                });

      if (_controllerType is not null)
      {
         appBuilder.Services.AddMvc(options =>
                   {
                      options.OutputFormatters.RemoveType<StringOutputFormatter>();
                      options.OutputFormatters.RemoveType<StreamOutputFormatter>();

                      var inputMediaTypes = options.InputFormatters.OfType<SystemTextJsonInputFormatter>().Single().SupportedMediaTypes;
                      RemoveUnnecessaryContentTypes(inputMediaTypes);

                      var outputMediaTypes = options.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().Single().SupportedMediaTypes;
                      RemoveUnnecessaryContentTypes(outputMediaTypes);
                   })
                   .ConfigureApplicationPartManager(manager =>
                   {
                      var controllerFeatureProvider = manager.FeatureProviders.Single(p => p is ControllerFeatureProvider);
                      var controllerFeatureProviderIndex = manager.FeatureProviders.IndexOf(controllerFeatureProvider);

                      manager.FeatureProviders[controllerFeatureProviderIndex] = new TestControllerFeatureProvider(_controllerType);

                      manager.ApplicationParts.Add(new TestApplicationPart(_controllerType));
                   });
      }

      var app = appBuilder.Build();
      app.UseSwagger(options => options.OpenApiVersion = Constants.OpenApiSpecVersion);

      if (_controllerType is not null)
         app.MapControllers();

      return app;
   }

   private static void RemoveUnnecessaryContentTypes(MediaTypeCollection supportedMediaTypes)
   {
      var unnecessaryContentTypes = supportedMediaTypes.Where(contentType => contentType != "application/json").ToList();

      foreach (var contentType in unnecessaryContentTypes)
      {
         supportedMediaTypes.Remove(contentType);
      }
   }

   private async Task<string> GetOpenApiJsonAsync()
   {
      App.Start();

      var testServer = (TestServer)App.Services.GetRequiredService<IServer>();
      using var client = testServer.CreateClient();
      using var response = await client.GetAsync("/swagger/test/swagger.json");

      return await response.Content.ReadAsStringAsync();
   }

   public ValueTask InitializeAsync()
   {
      return ValueTask.CompletedTask;
   }

   public async ValueTask DisposeAsync()
   {
      if (_app is null)
         return;

      await _app.StopAsync();
      await _app.DisposeAsync();
   }
}
