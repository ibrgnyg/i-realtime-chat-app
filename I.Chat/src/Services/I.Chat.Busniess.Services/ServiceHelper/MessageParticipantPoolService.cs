using I.Chat.Configure.Models.DTOs;
using MongoDB.Driver.Core.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Busniess.Services.ServiceHelper
{
    public class MessageParticipantPoolService : IMessageParticipantPoolService
    {
        private readonly static List<DTOMessageParticipantPool> messagesGrips = new List<DTOMessageParticipantPool>();

        public List<DTOMessageParticipantPool> GripsMessages => messagesGrips;

        public void AddItem(DTOMessageSelcted model)
        {
            if (!GripsMessages.Any(x => x.UserId == model.UserId))
            {
                GripsMessages.Add(new DTOMessageParticipantPool()
                {
                    Id = model.Id,
                    UserId = model.UserId,
                    ConnectionId = model.ConnectionId,
                });
            }
            else
            {
                GripsMessages.FirstOrDefault(x => x.UserId == model.UserId).Id = model.Id;
            }
        }

        public void RemoveItem(string connectionId)
        {
            var itemToRemove = GripsMessages.FirstOrDefault(x => x.ConnectionId == connectionId);
            if (itemToRemove != null)
            {
                GripsMessages.Remove(itemToRemove);
            }
        }

        public bool AnyGripMessages(string messageId, string userId)
        {
            return GripsMessages.Any(x => x.Id == messageId && x.UserId == userId);
        }
    }
}
