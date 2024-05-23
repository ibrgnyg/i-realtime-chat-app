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
    public class UserConnectionIdMutation
    {
        private readonly IUserConnectionIdService _userConnectionIdService;

        public UserConnectionIdMutation(IUserConnectionIdService userConnectionIdService)
        {
            _userConnectionIdService = userConnectionIdService;
        }

        [Authorize]
        public StateResult UpdateUserConnectionId(DTOUserConnectionId model) =>
              (StateResult)_userConnectionIdService.UpdateMessageConnectionId(model);
    }
}
