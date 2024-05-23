using I.Chat.Configure.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Configure.Models.DTOs
{
    public class DTOMessageDetailContent : DTOBase
    {
        public string Message { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreateDate { get; set; }
        public bool IsBloked { get; set; } = false;
        public string BlockUserId { get; set; } = string.Empty;
    }
}
