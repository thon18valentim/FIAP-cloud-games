using Microsoft.EntityFrameworkCore;
using FCG.Authentication.Services;
using FCG.Clients.Data.Repository;
using FCG.Clients.Data;
using FCG.Clients.Services;
using FCG.Games.Data;
using FCG.Games.Data.Repository;

namespace App.FCG.WebApi.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ClientContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Core")));

            builder.Services.AddDbContext<GameContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Core")));

            // Repositories
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();

            // Services
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            return builder;
        }
    }
}
