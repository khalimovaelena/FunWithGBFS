using FunWithGBFS.Application.Game;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Startup
{
    public static class AppConfigurator
    {
        public static IConfiguration LoadConfiguration(string settingsFilePath = "Config/appsettings.json")
        {
            return new ConfigurationBuilder()
                .AddJsonFile(settingsFilePath, optional: false, reloadOnChange: true)
                .Build();
        }

        public static GameSettings LoadGameSettings(IConfiguration config)
        {
            return config.GetSection("GameSettings").Get<GameSettings>()
                   ?? throw new InvalidOperationException("GameSettings section is missing or invalid.");
        }

        public static string GetConnectionString(IConfiguration config)
        {
            return config.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("DefaultConnection string is missing.");
        }
    }
}
