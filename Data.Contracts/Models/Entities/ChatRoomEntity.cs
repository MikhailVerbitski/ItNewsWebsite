using System;
using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class ChatRoomEntity
    {
        public int Id { get; set; }

        public virtual DateTime Created { get; set; }

        public string Header { get; set; }

        public virtual IEnumerable<MessageEntity> MessageEntities { get; set; }

        public virtual IEnumerable<UserChatEntity> UserChats { get; set; }
    }
}
