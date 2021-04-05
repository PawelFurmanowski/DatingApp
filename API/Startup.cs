using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Extentions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
            //metoda rozszerzająca (zawiera nasze serwisy)
            services.AddApplicationServices(_config);

            services.AddControllers();
            services.AddSwagger();
            services.AddCors();

            //metoda rozszerzająca (zawiera nasz serwis autentykacji)
            services.AddIdentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // KOLEJNOŚĆ W CONFIGURE MOTHOD JEST KLUCZOWA
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            //     app.UseSwagger();
            //     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            // }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            // Niezbędne aby klient mógł użyć metod http np. do pobrania wszystkich userów 
            // app.UseCors musi znajdować się pomiędzy UseRouting a autentication 
            // najpierw dostarczamy url ścieżkę następnie zezwalamy na metody użycie metod z inngeo orgin'a
            // a następnie autoryzujemy 
            // Zezwalamy na dowolny nagłówek np. authentication header z aplikacji angulara
            // Zezwalamy na dowolne metody np. put request, get request
            // Ustalamy konkretne źródło pochodzenia dla nagłówków oraz metod

            //w skrócie możesz wszystko to => AllowAnyHeader().AllowAnyMethod() TYLKO gdy pochodzi z tego =>  WithOrigins("https://localhost:4200")
            app.UseCors( policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

            //potrzebne do autentykacji musi znajdować się po CORS'ach i przed autoryzacją
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
