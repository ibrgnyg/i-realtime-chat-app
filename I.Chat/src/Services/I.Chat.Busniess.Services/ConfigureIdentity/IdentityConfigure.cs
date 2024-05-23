using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using I.Chat.Configure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Busniess.Services.ConfigureIdentity
{
    public static class IdentityConfigure
    {
        public static void SetInitializeIdentity<TUser, TRole>(this IServiceCollection services, string? connectionString, string? dbName)
            where TUser : MongoUser<string>
            where TRole : MongoRole<string>
        {
            var connectionSTring = $"{connectionString}/{dbName}";

            services.AddIdentityMongoDbProvider<TUser, TRole, string>(identity =>
            {
                identity.Password.RequiredLength = 6;
                identity.Password.RequireDigit = false;
                identity.Password.RequiredLength = 2;
                identity.Password.RequireNonAlphanumeric = false;
                identity.Password.RequireUppercase = false;
                identity.Password.RequireLowercase = false;
                identity.User.RequireUniqueEmail = true;
                identity.User.AllowedUserNameCharacters = "abcçdefghiıjklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789_-";
                identity.Lockout = new LockoutOptions()
                {
                    AllowedForNewUsers = true,
                };
            },
            mongo =>
            {
                mongo.UsersCollection = typeof(TUser).Name;
                mongo.ConnectionString = connectionSTring;

            }).AddDefaultTokenProviders();
        }


        public static void SetJwtScheme(this IServiceCollection services, AppSettings? appSettings, string? secreKey)
        {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = appSettings?.Issuer,
                    ValidAudience = appSettings?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreKey))
                };
            });
        }
    }
}
