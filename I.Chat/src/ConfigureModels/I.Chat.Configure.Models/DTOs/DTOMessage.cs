using I.Chat.Configure.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Configure.Models.DTOs
{
    public class DTOMessage : DTOBase
    {
        public string StartUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public string MessageContent { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
    }
}
