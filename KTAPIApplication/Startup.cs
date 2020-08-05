using DinkToPdf;
using DinkToPdf.Contracts;
using KTAPIApplication.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SystemAPIApplication;

namespace KTAPIApplication
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
            services.AddMvc(option => option.EnableEndpointRouting = false)
                           .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                           .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.Configure<MongoSetting>(Configuration.GetSection("MongoSetting"))
                .Configure<MongoOtherSetting>(Configuration.GetSection("MongoOtherSetting"))
                .Configure<ServiceUrls>(Configuration.GetSection("ServiceUrls"))
                .Configure<ThirdPartyServiceUrls>(Configuration.GetSection("ThirdPartyServiceUrls"))
                .AddControllers();


            services.AddSingleton<IMongoService, MongoService>();
            services.AddSingleton<IDamageAnalysisService, DamageAnalysisService>();

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));//DinkToPdf注入
            services.AddTransient<IPDFService, PDFService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
