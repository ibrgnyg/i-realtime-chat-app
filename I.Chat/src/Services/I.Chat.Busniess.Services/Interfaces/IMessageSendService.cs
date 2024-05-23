using I.Chat.Configure.Models.DTOs;
using I.Chat.Core.Events;
using I.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace I.Chat.Busniess.Services.Interfaces
{
    public interface IMessageSendService
    {
        IQueryable<Message> Table { get; }
        Message GetMessage(string id);
        DTOMessagePagination GetDTOMessagePagination(string id, string userId, int activePage, int pageSize);
        DTOMessageCheck DTOMessageCheck(string startUserId, string toUserId);
        Task<List<DTOMessages>> GetMessages(string userId);
        Message GetMessageFilter(Expression<Func<Message, bool>> filter);
        long GetCountMessage(Expression<Func<Message, bool>> filter);
        bool ExistMessage(Expression<Func<Message, bool>> filter);
        IStateResult SaveMessage(DTOMessage model);
        IStateResult SaveMessage(Message model);
        IStateResult UpdateMessage(DTOMessage model);
        IStateResult BlockMessage(DTOBlockMessage model);
        Task UpdateIsRead(string id, string userId);
        Task<int> GetUnreadMessagesCount(string userId);
    }
}
