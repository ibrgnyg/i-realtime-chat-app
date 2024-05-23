using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Busniess.Services.ServiceHelper
{
    public interface IJWTTokenService
    {
        string CreateToken(string userId, int expiredDay = 1);
        bool IsTokenExpired(string? token);
        bool ValidateToken(string? token);
    }
}
