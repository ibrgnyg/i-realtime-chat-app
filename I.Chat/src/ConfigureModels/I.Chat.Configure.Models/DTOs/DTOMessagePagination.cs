using I.Chat.Configure.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Configure.Models.DTOs
{
    public class DTOMessagePagination : DTOBase
    {
        public int TotalCount { get; set; } = 0;
        public int TotalPageCount { get; set; } = 0;
        public string StartUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public List<DTOMessageDetailContent> Messages { get; set; } = new List<DTOMessageDetailContent>();
    }
}
