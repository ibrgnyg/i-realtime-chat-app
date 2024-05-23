using I.Chat.Configure.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Domain.Models
{
    public class User : BaseEntityIdentity
    {
        private const string DefaultAvatar = "https://www.tenforums.com/attachments/user-accounts-family-safety/322690d1615743307-user-account-image-log-user.png";

        public string Avatar { get; set; } = DefaultAvatar;
    }
}
