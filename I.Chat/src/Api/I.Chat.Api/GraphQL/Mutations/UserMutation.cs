using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using I.Chat.Busniess.Services.Services;
using I.Chat.Configure.Models.DTOs;
using I.Chat.Core.BaseGraphQL;
using I.Chat.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Api.GraphQL.Mutations
{
    [ExtendObjectType(nameof(BaseMutation))]
    public class UserMutation
    {
        private readonly IUserManagerService _userManagerService;

        public UserMutation(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        public StateResult Register(DTOUserRegister dtoModel) =>
              (StateResult)_userManagerService.RegisterUser(dtoModel);

        public StateResult Login(DTOUserLogin dtoModel) =>
              (StateResult)_userManagerService.LoginUser(dtoModel);

        [Authorize]
        public StateResult UpdateUserName(DTOUserChangeName dtoModel) =>
              (StateResult)_userManagerService.UpdateUserName(dtoModel);

        [Authorize]
        public StateResult UpdatePassword(DTOUserChangePassword dtoModel) =>
              (StateResult)_userManagerService.UpdatePassword(dtoModel);
    }
}
