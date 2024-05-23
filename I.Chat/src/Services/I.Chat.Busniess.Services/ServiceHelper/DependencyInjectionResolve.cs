using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Busniess.Services.ServiceHelper
{
    public class DependencyInjectionResolve<TEntity> : Lazy<TEntity>
    {
        public DependencyInjectionResolve(IServiceProvider serviceProvider) :
        base(serviceProvider.GetRequiredService<TEntity>)
        {
        }
    }
}
