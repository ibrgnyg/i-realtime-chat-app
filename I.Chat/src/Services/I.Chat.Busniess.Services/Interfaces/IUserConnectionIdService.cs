using I.Chat.Configure.Models.DTOs;
using I.Chat.Core.Events;
using I.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Busniess.Services.Services
{
    public interface IUserConnectionIdService
    {
        IQueryable<UserConnectionId> Table { get; }
        UserConnectionId GetMessageConnectionId(string id);
        UserConnectionId GetMessageConnectionIdFilter(Expression<Func<UserConnectionId, bool>> filter);
        int GetCountMessageConnectionId(Expression<Func<UserConnectionId, bool>> filter);
        bool ExistMessageConnectionId(Expression<Func<UserConnectionId, bool>> filter);
        IStateResult SaveMessageConnectionId(DTOUserConnectionId model);
        IStateResult SaveMessageConnectionId(UserConnectionId model);
        IStateResult UpdateMessageConnectionId(DTOUserConnectionId model);
        IStateResult UpdateOneFieldMessageConnectionId<U>(string id, Expression<Func<UserConnectionId, U>> expression, U value);
    }
}
