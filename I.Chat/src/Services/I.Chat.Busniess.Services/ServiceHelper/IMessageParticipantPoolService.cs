using HotChocolate.Data.Sorting;
using I.Chat.Configure.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Busniess.Services.ServiceHelper
{
    public interface IMessageParticipantPoolService
    {
        List<DTOMessageParticipantPool> GripsMessages { get; }

        bool AnyGripMessages(string messageId, string userId);

        void AddItem(DTOMessageSelcted model);
        void RemoveItem(string connectionId);

    }
}
