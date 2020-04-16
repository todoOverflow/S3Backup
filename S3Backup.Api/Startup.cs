using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using S3Backup.Domain.Interfaces;
using S3Backup.Infrastructure.Repository;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;

namespace S3Backup.Api
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
            services.AddAWSService<IAmazonS3>(Configuration.GetAWSOptions());
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IBucketRepository, BucketRepository>();
            services.AddSwaggerGen(
                c => c.SwaggerDoc("v1", new Info { Title = "S3 backup App", Version = "v1" })
            );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseExceptionHandler(
                appBuilder => appBuilder.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exceptions = exceptionHandlerPathFeature?.Error;

                    var result = JsonConvert.SerializeObject(new { error = exceptions.Message });
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(result);
                })
            );

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix=string.Empty;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "S3 Backup APP V1");
            });
            app.UseMvc();
        }
    }
}
