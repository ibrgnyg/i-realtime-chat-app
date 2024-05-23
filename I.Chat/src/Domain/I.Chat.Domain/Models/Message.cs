using I.Chat.Configure.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Domain.Models
{
    public class Message : BaseEntity
    {
        public string StartUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public bool IsBloked { get; set; } = false;
        public string BlockUserId { get; set; } = string.Empty;
        public List<MessageContent> MessageContents { get; set; } = new List<MessageContent>();
    }
}
