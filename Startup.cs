using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using BottleCapApi.Models;
using BottleCapApi.Middleware;
using BottleCapApi.Slack;

namespace BottleCapApi
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

      services.AddControllers();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "BottleCapApi", Version = "v1" });
      });

      services.AddCors(options =>
         {
           options.AddDefaultPolicy(
              builder =>
              {
                builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
              });
         });

      services.AddSingleton<ResponseFactory>();
      services.AddDbContext<DatabaseContext>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BottleCapApi v1"));

      app.UseHttpsRedirection();

      app.UseRouting();
      app.UseCors();

      app.UseAuthorization();

      app.UseMiddleware<RequestResponseLoggingMiddleware>();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
