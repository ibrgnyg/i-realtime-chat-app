using HotChocolate.Types;
using I.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Api.GraphQL.Types
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Ignore(t => t.Roles);
            descriptor.Ignore(t => t.Claims);
            descriptor.Ignore(t => t.Logins);
            descriptor.Ignore(t => t.Tokens);
            descriptor.Ignore(t => t.SecurityStamp);
            descriptor.Ignore(t => t.PasswordHash);
            descriptor.Ignore(t => t.ConcurrencyStamp);
            descriptor.Ignore(t => t.AccessFailedCount);
            descriptor.Ignore(t => t.LockoutEnabled);
            descriptor.Ignore(t => t.NormalizedEmail);
            descriptor.Ignore(t => t.NormalizedUserName);
            descriptor.Ignore(t => t.CreateDate);
            descriptor.Ignore(t => t.UpdateDate);
            descriptor.Ignore(t => t.PhoneNumberConfirmed);
        }
    }
}
