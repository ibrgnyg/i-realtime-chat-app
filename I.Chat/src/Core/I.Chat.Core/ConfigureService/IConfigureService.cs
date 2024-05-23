using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Core.ConfigureService
{
    public interface IConfigureService
    {
        string GraphQLPath { get; }
        IServiceCollection SetGraphQLInitialize();
        WebApplication BuildRun(WebApplication app);
    }
}
