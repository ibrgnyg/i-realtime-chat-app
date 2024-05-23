using I.Chat.Configure.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Domain.Models
{
    public class UserConnectionId : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string SelectedMessageId { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }
}
