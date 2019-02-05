namespace Data.Contracts.Models.Entities
{
    public class PostRatingEntity
    {
        public int Id { get; set; }

        public int? PostId { get; set; }

        public int? UserProfileId { get; set; }

        public byte Score { get; set; }

        public virtual PostEntity Post { get; set; }

        public virtual UserProfileEntity UserProfile { get; set; }
    }
}
