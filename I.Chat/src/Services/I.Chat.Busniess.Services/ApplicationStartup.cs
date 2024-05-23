using I.Chat.Configure.Models;
using I.Chat.Core.ApplicationStartup;
using I.Chat.Core.Events;
using I.Chat.Core.MDBRepository.Identity;
using I.Chat.Core.MDBRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using I.Chat.Busniess.Services.Services;
using I.Chat.Busniess.Services.ServiceHelper;
using I.Chat.Busniess.Services.Interfaces;

namespace I.Chat.Busniess.Services
{
    public class ApplicationStartup : IApplicationStartup
    {
        public int Order => 2;

        public void Configure(IApplicationBuilder app, IWebHostEnvironment hostBuilder)
        {
            app.UseCors("CorsPolicy");
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials();
            }));


            services.AddTransient(
            typeof(Lazy<>),
            typeof(DependencyInjectionResolve<>));

            //Core
            services.AddSingleton<IStateResult, StateResult>();
            services.Configure<DBSettings>(configuration.GetSection(nameof(DBSettings)));
            services.AddSingleton(_ => _.GetRequiredService<IOptions<DBSettings>>().Value);
            services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            services.AddSingleton(typeof(IMongoRepositoryIdentity<>), typeof(MongoRepositoryIdentity<>));

            //Services
            services.AddSingleton<IUserManagerService, UserManagerService>();
            services.AddSingleton<IUserConnectionIdService, UserConnectionIdService>();
            services.AddSingleton<IMessageSendService, MessageSendService>();
            services.AddSingleton<IMessageParticipantPoolService, MessageParticipantPoolService>();

            //Helper Services
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
            services.AddSingleton(_ => _.GetRequiredService<IOptions<AppSettings>>().Value);

            services.AddSingleton<IJWTTokenService, JWTTokenService>();

        }
    }
}