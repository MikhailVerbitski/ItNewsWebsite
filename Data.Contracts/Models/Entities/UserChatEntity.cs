namespace Data.Contracts.Models.Entities
{
    public class UserChatEntity
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? ChatRoomId { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; }

        public virtual ChatRoomEntity ChatRoom { get; set; }
    }
}
