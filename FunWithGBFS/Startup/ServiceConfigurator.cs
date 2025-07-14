using FunWithGBFS.Application.Game;
using FunWithGBFS.Application.Stations;
using FunWithGBFS.Application.Stations.Interfaces;
using FunWithGBFS.Application.Users;
using FunWithGBFS.Application.Users.Interfaces;
using FunWithGBFS.Infrastructure.Gbfs;
using FunWithGBFS.Infrastructure.Http;
using FunWithGBFS.Infrastructure.Http.Interfaces;
using FunWithGBFS.Persistence.Context;
using FunWithGBFS.Persistence.Repository;
using FunWithGBFS.Persistence.Repository.Interfaces;
using FunWithGBFS.Presentation.Console;
using FunWithGBFS.Presentation.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Startup
{
    // Startup/ServiceConfigurator.cs
    public static class ServiceConfigurator
    {
        public static IServiceProvider ConfigureServices(IConfiguration config)
        {
            var services = new ServiceCollection();

            services.AddLogging(builder => builder.AddConsole());

            var gameSettings = AppConfigurator.LoadGameSettings(config);
            var connectionString = AppConfigurator.GetConnectionString(config);

            services.AddSingleton(gameSettings);
            services.AddDbContext<GameDbContext>(options =>
                options.UseSqlite(connectionString));

            // Application + infrastructure
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, DbUserService>();
            services.AddScoped<IHttpJsonFetcher, HttpJsonFetcher>();
            services.AddScoped<IStationDataMapper, GbfsStationDataMapper>();
            services.AddScoped<IStationProvider, GbfsStationProvider>();
            services.AddScoped<IUserInteraction, ConsoleUserInteraction>();
            services.AddScoped<UserSessionService>();
            services.AddScoped<StationFetchService>();
            services.AddHttpClient();

            return services.BuildServiceProvider();
        }
    }
}
