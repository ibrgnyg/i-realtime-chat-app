using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using I.Chat.Busniess.Services.Services;
using I.Chat.Configure.Models.DTOs;
using I.Chat.Core.BaseGraphQL;
using I.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Api.GraphQL.Queries
{
    [ExtendObjectType(nameof(BaseQuery))]
    public class UserQuery
    {
        private readonly IUserManagerService _userManagerService;

        public UserQuery(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        [Authorize]
        public User User(string id) =>
            _userManagerService.GetUser(id);

        [Authorize]
        public List<DTOSearchUser> GetSearchUsers(string username,string userId) =>
            _userManagerService.GetSearchUsers(username,userId);

    }
}
