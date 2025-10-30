using FCG.Authentication.Services;
using FCG.Clients.Data;
using FCG.Clients.Data.Repository;
using FCG.Clients.Services;
using FCG.Games.Data;
using FCG.Games.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace App.FCG.WebApi.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ClientsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Core")));
            builder.Services.AddDbContext<GameContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("Core")));
            // Registrar outros serviços, repositórios e unidades de trabalho aqui
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();

            return builder;
        }
    }
}
