using I.Chat.Configure.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Domain.Models
{
    public class MessageContent : BaseEntity
    {
        public string Message { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
    }
}
