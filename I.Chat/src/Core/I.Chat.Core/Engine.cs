using I.Chat.Core.ApplicationStartup;
using I.Chat.Core.TypeFinders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace I.Chat.Core
{
    public class Engine
    {
        public static WebApplicationBuilder CreateAsBuilder(string[] args)
        {
            var application = WebApplication.CreateBuilder(args);
            return application;
        }

        public static void ConfigureRequestPipeline(IApplicationBuilder application, IWebHostEnvironment webHostEnvironment)
        {
            var typeFinder = new AppTypeFinder();
            var getstartupConfigurations = typeFinder.FindClassesOfType<IApplicationStartup>();

            var instances = getstartupConfigurations
                    .Select(startup => Activator.CreateInstance(startup) as IApplicationStartup)
                   .OrderBy(startup => startup.Order);

            foreach (var instance in instances)
                instance?.Configure(application, webHostEnvironment);
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var typeFinder = new AppTypeFinder();
            var startupConfigurations = typeFinder.FindClassesOfType<IApplicationStartup>();

            var instances = startupConfigurations
                 .Select(startup => Activator.CreateInstance(startup) as IApplicationStartup)
                 .OrderBy(startup => startup.Order);

            foreach (var instance in instances)
                instance?.ConfigureServices(services, configuration);
        }

        public static void ConfigurationFiles(IConfigurationBuilder configurationBuilder)
        {
            string? directoryBasePath = Directory.GetParent(AppContext.BaseDirectory)?.FullName;

            bool alwaysReload = true, alwaysOptionIncluded = true;

            configurationBuilder.SetBasePath(directoryBasePath)
                .AddJsonFile("appsettings.json", alwaysOptionIncluded, alwaysReload)
                .AddJsonFile("appconfigure.business.services.json", alwaysOptionIncluded, alwaysReload)
                .AddJsonFile("appconfigure.core.json", alwaysOptionIncluded, alwaysReload)
                .AddEnvironmentVariables()
                .Build();
        }

    }
}
