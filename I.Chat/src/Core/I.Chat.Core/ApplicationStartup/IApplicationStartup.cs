using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace I.Chat.Core.ApplicationStartup
{
    public interface IApplicationStartup
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
        void Configure(IApplicationBuilder app, IWebHostEnvironment hostBuilder);
        int Order { get; }
    }
}
