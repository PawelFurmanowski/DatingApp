using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
            {
                //UseSqlite potrzebuje usinga microsoft.EntityFrameworkCore
                options.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // KOLEJNOŚĆ W CONFIGURE MOTHOD JEST KLUCZOWA
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Niezbędne aby klient mógł użyć metod http np. do pobrania wszystkich userów 
            // app.UseCors musi znajdować się pomiędzy UseRouting a autentication 
            // najpierw dostarczamy url ścieżkę następnie zezwalamy na metody użycie metod z inngeo orgin'a
            // a następnie autoryzujemy 
            // Zezwalamy na dowolny nagłówek np. authentication header z aplikacji angulara
            // Zezwalamy na dowolne metody np. put request, get request
            // Ustalamy konkretne źródło pochodzenia dla nagłówków oraz metod

            //w skrócie możesz wszystkot to => AllowAnyHeader().AllowAnyMethod() TYLKO gdy pochodzi z tego =>  WithOrigins("http://localhost:4200")
            app.UseCors( policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
