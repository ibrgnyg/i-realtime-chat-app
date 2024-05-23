using I.Chat.Api.GraphQL.Mutations;
using I.Chat.Api.GraphQL.Queries;
using I.Chat.Api.GraphQL.Types;
using I.Chat.Busniess.Services.Services;
using I.Chat.Configure.Models;
using I.Chat.Core.BaseGraphQL;
using I.Chat.Core.Events;
using I.Chat.Core.MDBRepository.Identity;
using I.Chat.Core.MDBRepository;
using I.Chat.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using I.Chat.Busniess.Services.ConfigureIdentity;
using Microsoft.AspNetCore.Cors.Infrastructure;
using I.Chat.Api.Hubs;

namespace I.Chat.Api
{
    public static class ConfigureSevices
    {
        private const string GraphQLPath = "/gql";

        public static IServiceCollection SetGraphQLInitialize(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddSignalR();

            //Services
            var dBSettings = configuration.GetSection(nameof(DBSettings)).Get<DBSettings>();
            services.SetInitializeIdentity<User, UserRole>(dBSettings?.ConnectionString, dBSettings?.DatabaseName);

            var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

            services.SetJwtScheme(appSettings, appSettings?.TokenKey);

            services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<BaseQuery>()
                .AddMutationType<BaseMutation>()
                .AddType<UserConnectionIdMutation>()
                .AddType<UserMutation>()
                .AddType<UserType>()
                .AddType<StateResultType>()
                .AddType<MessageQuery>()
                .AddType<UserQuery>()
                .AddType<EnumStateResultType>()
                .AddMutationConventions();

            return services;
        }

        public static WebApplication BuildRun(this WebApplication app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();            

            app.MapGraphQL(GraphQLPath);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MessageHub>($"{GraphQLPath}/message");
            });

            app.Run();

            return app;
        }
    }
}
