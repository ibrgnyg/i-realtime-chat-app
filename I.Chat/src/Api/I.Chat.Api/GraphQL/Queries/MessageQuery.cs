using HotChocolate.Authorization;
using HotChocolate.Types;
using I.Chat.Busniess.Services.Interfaces;
using I.Chat.Configure.Models.DTOs;
using I.Chat.Core.BaseGraphQL;
using I.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Api.GraphQL.Queries
{
    [ExtendObjectType(nameof(BaseQuery))]
    public class MessageQuery
    {
        private readonly IMessageSendService _messageService;

        public MessageQuery(IMessageSendService messageService)
        {
            _messageService = messageService;
        }

        [Authorize]
        public DTOMessagePagination GetMessage(string id, string userId, int activePage, int pageSize) =>
             _messageService.GetDTOMessagePagination(id, userId, activePage, pageSize);
    }
}
