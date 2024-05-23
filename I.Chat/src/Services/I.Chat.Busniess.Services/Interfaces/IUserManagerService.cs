using I.Chat.Configure.Models.DTOs;
using I.Chat.Core.Events;
using I.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Busniess.Services.Services
{
    public interface IUserManagerService
    {
        IStateResult RegisterUser(DTOUserRegister dtoModel);
        IStateResult LoginUser(DTOUserLogin dtoModel);
        List<DTOSearchUser> GetSearchUsers(string username,string userId);
        User GetUser(string id);
        IStateResult UpdateUserName(DTOUserChangeName dtoModel);
        IStateResult UpdatePassword(DTOUserChangePassword dtoModel);
    }
}
