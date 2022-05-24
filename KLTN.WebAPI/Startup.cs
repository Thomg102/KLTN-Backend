using KLTN.Common.Models;
using KLTN.Core.DepartmentServices.Implementations;
using KLTN.Core.DepartmentServices.Interfaces;
using KLTN.Core.LecturerServicess.Implementations;
using KLTN.Core.LecturerServicess.Interfaces;
using KLTN.Core.MissionServices.Implementations;
using KLTN.Core.MissionServices.Interfaces;
using KLTN.Core.ProductServices.Implementations;
using KLTN.Core.ProductServices.Interfaces;
using KLTN.Core.RequestActiveServices.Implementations;
using KLTN.Core.RequestActiveServices.Interfaces;
using KLTN.Core.ScholarshipServices.Implementations;
using KLTN.Core.ScholarshipServices.Interfaces;
using KLTN.Core.StudentServices.Implementations;
using KLTN.Core.StudentServices.Interfaces;
using KLTN.Core.SubjectServices.Implementations;
using KLTN.Core.SubjectServices.Interfaces;
using KLTN.Core.TuitionServices.Implementations;
using KLTN.Core.TuitionServices.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Filters;

namespace KLTN.WebAPI
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
            services.AddMvc(options =>
            {
                options.Filters.Add(new ExceptionFilter());
            });

            services.Configure<WebAPIAppSettings>(Configuration)
            .AddSingleton(sp => sp.GetRequiredService<IOptions<WebAPIAppSettings>>().Value);

            var configurationSetting = Configuration.Get<WebAPIAppSettings>();
            WebAPIAppSettings.SetValue(configurationSetting);

            services.AddScoped<ITuitionService, TuitionService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IScholarshipService, ScholarshipService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IMissionService, MissionService>();
            services.AddScoped<ILecturerService, LecturerService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IActivateRequestService, ActivateRequestService>();

            services.AddControllers();
            services.AddRouting(option => option.LowercaseUrls = true);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API"));
        }
    }
}
