using HotChocolate.Authorization;
using HotChocolate.Types.Pagination;
using I.Chat.Busniess.Services.Interfaces;
using I.Chat.Busniess.Services.ServiceHelper;
using I.Chat.Busniess.Services.Services;
using I.Chat.Configure.Models.DTOs;
using I.Chat.Core.Events;
using I.Chat.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace I.Chat.Api.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        #region Fields
        private readonly IMessageSendService _messageSendService;
        private readonly IUserConnectionIdService _userConnectionIdService;
        private readonly IMessageParticipantPoolService _messageParticipantPoolService;
        #endregion

        #region Ctor
        public MessageHub(IMessageSendService messageSendService,
            IUserConnectionIdService userConnectionIdService,
            IMessageParticipantPoolService messageParticipantPoolService)
        {
            _messageSendService = messageSendService;
            _userConnectionIdService = userConnectionIdService;
            _messageParticipantPoolService = messageParticipantPoolService;
        }
        #endregion


        public void JoinConversation(DTOMessageSelcted model)
        {
            model.ConnectionId = Context.ConnectionId;
            _messageParticipantPoolService.AddItem(model);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            _messageParticipantPoolService.RemoveItem(connectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<StateResult> SendMessage(DTOMessage model)
        {
            var connectionId = _userConnectionIdService.GetMessageConnectionIdFilter(x => x.UserId == model.ToUserId).ConnectionId;

            model.IsRead = _messageParticipantPoolService.AnyGripMessages(model.Id, model.ToUserId);

            var stateResult = _messageSendService.UpdateMessage(model);

            await Clients.Client(connectionId).SendAsync("ReceiveMessage", stateResult);

            //update other client
            var messages = await _messageSendService.GetMessages(model.ToUserId);
            await Clients.Client(connectionId).SendAsync("GetMessages", messages);
            var count = await _messageSendService.GetUnreadMessagesCount(model.ToUserId);
            await Clients.Client(connectionId).SendAsync("GetMessageCount", count);
            return (StateResult)stateResult;
        }

        public async Task<List<DTOMessages>> GetMessageList(string userId)
        {
            var connectionId = _userConnectionIdService.GetMessageConnectionIdFilter(x => x.UserId == userId).ConnectionId;

            var messages = await _messageSendService.GetMessages(userId);

            await Clients.Client(connectionId).SendAsync("GetMessages", messages);

            return messages;
        }

        public async Task UpdateMessageIsRead(DTOMessageSelcted model)
        {
            await _messageSendService.UpdateIsRead(model.Id, model.UserId);

            var count = await _messageSendService.GetUnreadMessagesCount(model.UserId);

            await Clients.Client(Context.ConnectionId).SendAsync("GetMessageCount", count);
        }

        public async Task<int> GetTotalUnreadMessageCount(string userId)
        {
            var connectionId = _userConnectionIdService.GetMessageConnectionIdFilter(x => x.UserId == userId).ConnectionId;

            var count = await _messageSendService.GetUnreadMessagesCount(userId);

            await Clients.Client(connectionId).SendAsync("GetMessageCount", count);

            return count;
        }

        
        public async Task<StateResult> BlockMessage(DTOBlockMessage model)
        {
            //Not working
            /*var connectionId = _userConnectionIdService.GetMessageConnectionIdFilter(x => x.UserId == model.ToUserId).ConnectionId;

            var stateResult = _messageSendService.BlockMessage(model);

            await Clients.Client(connectionId).SendAsync("BlockInput", stateResult);*/

            return new StateResult();
        }
    }
}
