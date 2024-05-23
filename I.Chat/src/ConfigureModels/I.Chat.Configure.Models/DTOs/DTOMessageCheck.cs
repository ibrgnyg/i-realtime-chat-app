using I.Chat.Configure.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Configure.Models.DTOs
{
    public class DTOMessageCheck : DTOBase
    {
        public bool AnyMessage { get; set; } = true;
    }
}
