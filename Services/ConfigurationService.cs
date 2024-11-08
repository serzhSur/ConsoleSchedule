using Microsoft.Extensions.Configuration;

namespace VizitConsole.Services
{
    internal class ConfigurationService
    {
        public IConfiguration Configuration { get; }
        public ConfigurationService() 
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
        }
        public string GetConnectionString(string name)
        {
            return Configuration.GetConnectionString(name);
        }
        public string GetSetting(string key)
        {
            return Configuration[key];
        }

    }
}
