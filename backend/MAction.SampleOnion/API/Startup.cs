using MAction.AspNetIdentity.Base;
using MAction.BaseServices.Configure;
using MAction.SampleOnion.Service.Configure;
using MAction.WebHelpers.Middlewares;
using MAction.WebHelpers.ServiceRegistration;
using MAction.WebHelpers.SwaggerHelpers;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.SwaggerUI;
using static MAction.BaseMongoRepository.ServiceRegistration;

namespace MAction.SampleOnion.API
{
    public class Startup
    {
        IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            Repository.ServiceRegistration.AddConfigureService(services, _configuration);
            var connectionBuilder =
                new MongoDB.Driver.MongoUrlBuilder(_configuration.GetConnectionString("MactionMongoDBConnection"));
            var _jwtSettings = _configuration.GetSection("JwtSettings").Get<AspNetIdentity.Base.JwtSettings>();

            Infrastructure.Configure.ServiceRegistration.AddConfigureService(services, _configuration);


            services.Configure<AspNetIdentity.Base.JwtSettings>(_configuration.GetSection("JwtSettings"));

            services.ConfigureLocalization();

            AspNetIdentity.Mongo.ServiceRegistration.AddConfigureService(services, connectionBuilder.ToString(),
                new TempMongoDependencyProvider()
                {
                    DatabaseName = connectionBuilder.DatabaseName
                },
                _jwtSettings, typeof(Infrastructure.EmailService.UserEmailSender));

            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSwaggerGen(swagger =>
            {
                swagger.IgnoreObsoleteActions();
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Mehdi Haghsheans API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://api.test/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "maction",
                        Email = string.Empty,
                        Url = new Uri("https://api.test/maction"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://api.test/license"),
                    },
                });
                //swagger.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference {
                //                Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                //            , Scheme = "oauth2", Name = "Bearer", In = ParameterLocation.Header, }
                //        , new List<string>()
                //    }
                //});
                swagger.EnableAnnotations();

                swagger.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "OAuth2");
                foreach (var fi in new DirectoryInfo(AppContext.BaseDirectory).EnumerateFiles("*.xml"))
                {
                    swagger.IncludeXmlComments(fi.FullName, true);
                }
            });
            services.AddCors(o => o.AddPolicy(name: "CorsPolicy", builder =>
            {
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(origin => true) // allow any origin
                    .AllowCredentials();
            }));
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.DateParseHandling = Newtonsoft.Json.DateParseHandling.None;
            });

            services.AddResponseCompression();

            ServiceRegistration.AddFluentValidation(services.AddMvc());
            ServiceRegistration.AddConfigureService(services, _configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.SetLanguageMiddleWare();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //c.RoutePrefix = string.Empty;
                c.DocExpansion(DocExpansion.List);
                c.DefaultModelsExpandDepth(3);
            });
            //app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseCors("CorsPolicy");
            app.CustomExceptionMidleware();
            AspNetIdentity.Mongo.ServiceRegistration.Configure(app);
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}