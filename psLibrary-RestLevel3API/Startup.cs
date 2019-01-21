using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using psLibrary_RestLevel3API.Entities;
using psLibrary_RestLevel3API.Helpers;
using psLibrary_RestLevel3API.Services;

namespace psLibrary_RestLevel3API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // register the DbContext on the container, getting the connection string from
            // appSettings (note: use this during development; in a production environment,
            // it's better to store the connection string in an environment variable)
            var connectionString = Configuration["connectionStrings:libraryDBConnectionString"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));

            // register the repository
            services.AddScoped<ILibraryRepository, LibraryRepository>();


            // for pagination
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();// UrlHelper use this so must be before; AddSingleton - first tine is requested only
                // AddScoped - once per request
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, LibraryContext libraryContext)
        {

            loggerFactory.AddConsole();
            loggerFactory.AddDebug(LogLevel.Information);
            //loggerFactory.AddProvider(new NLogLoggerProvider());
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
                        }

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });

                app.UseHsts();
            }


            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.Author, Models.AuthorDto>()
                    .ForMember(des => des.Name, opt=> opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                    .ForMember(des => des.Age, opt => opt.MapFrom( src=> src.DateOfBirth.GetCurrentAge()));

                cfg.CreateMap<Entities.Book, Models.BookDto>();

                cfg.CreateMap<Models.AuthorForCreationDto, Entities.Author>();
                cfg.CreateMap<Models.AuthorBooksForCreationDto, Entities.Author>();

                cfg.CreateMap<Models.BookForCreationDto, Entities.Book>();
                cfg.CreateMap<Models.BookForUpdateDto, Entities.Book>();

                cfg.CreateMap<Entities.Book, Models.BookForUpdateDto>();

            });


            app.UseHttpsRedirection();

            libraryContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
