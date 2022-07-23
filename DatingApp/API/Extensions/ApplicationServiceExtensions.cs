using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config){

            // We could use services.AddSingleton but this service will be present until the app stop - we don't need it
            // We could use services.Trasnsient - service expire every time when any method is executed (also not need it)
            // We use services.addScoped - service is creating only to work with particular http requst
            services.AddScoped<ITokenService, TokenService>();    // we could only create class TokenService but for the testing purpose we should create also interface
            
            // Provide connection with database
            services.AddDbContext<DataContext>(options => 
            {
                // Place for connection string (taks the specific section from configuration file: appsettings.Developement.json)
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}