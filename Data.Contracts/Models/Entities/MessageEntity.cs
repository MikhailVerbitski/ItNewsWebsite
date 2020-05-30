using System;

namespace Data.Contracts.Models.Entities
{
    public class MessageEntity
    {
        public int Id { get; set; }

        public int? UserProfileId { get; set; }

        public int? ChatRoomId { get; set; }

        public string Content { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; }

        public virtual ChatRoomEntity ChatRoom { get; set; }
    }
}
