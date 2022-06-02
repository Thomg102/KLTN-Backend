using KLTN.Common.Models;
using KLTN.Common.Models.AppSettingModels;
using KLTN.Core.DepartmentServices.Implementations;
using KLTN.Core.DepartmentServices.Interfaces;
using KLTN.Core.LecturerServicess.Implementations;
using KLTN.Core.LecturerServicess.Interfaces;
using KLTN.Core.MissionServices.Implementations;
using KLTN.Core.MissionServices.Interfaces;
using KLTN.Core.ProductServices.Implementations;
using KLTN.Core.ProductServices.Interfaces;
using KLTN.Core.RequestActivateServices.Implementations;
using KLTN.Core.RequestActivateServices.Interfaces;
using KLTN.Core.ScholarshipServices.Implementations;
using KLTN.Core.ScholarshipServices.Interfaces;
using KLTN.Core.StudentServices.Implementations;
using KLTN.Core.StudentServices.Interfaces;
using KLTN.Core.SubjectServices.Implementations;
using KLTN.Core.SubjectServices.Interfaces;
using KLTN.Core.TuitionServices.Implementations;
using KLTN.Core.TuitionServices.Interfaces;
using KLTN.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KLTN.ManagerPoolListen
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });
            var configuration = Configuration.Get<ListenMangerPoolAppSettings>();
            ListenMangerPoolAppSettings.SetValue(configuration);

            services.Configure<Mongosettings>(options =>
            {
                options.Connection = configuration.ConnectionString;
                options.DatabaseName = configuration.DatabaseName;
            });

            services.AddSingleton<IMongoDbContext, MongoDbContext>();
            services.AddScoped<IMissionService, MissionService>();
            services.AddScoped<ITuitionService, TuitionService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IScholarshipService, ScholarshipService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ILecturerService, LecturerService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IActivateRequestService, ActivateRequestService>();

            services.AddHostedService<Worker>();
            services.AddSignalR();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
